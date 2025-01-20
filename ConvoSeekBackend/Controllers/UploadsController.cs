using ConvoSeekBackend.Data;
using ConvoSeekBackend.Dtos;
using ConvoSeekBackend.Helpers;
using ConvoSeekBackend.Models;
using ConvoSeekBackend.Repositories;
using ConvoSeekBackend.Services;
using ConvoSeekBackend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ConvoSeekBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly ConvoSeekBackendContext _context;
        private readonly IMessagesRepository _messagesRepository;
        private readonly IEmbeddingService _embeddingService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UploadsController> _logger;

        public UploadsController(
            ConvoSeekBackendContext context,
            ILogger<UploadsController> logger,
            IEmbeddingService embeddingService,
            IMessagesRepository messagesRepository,
            IConfiguration configuration)
        {
            _context = context;
            _messagesRepository = messagesRepository;
            _embeddingService = embeddingService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromQuery] string source, [FromBody] List<MessageDto> messages)
        {
            Stopwatch sw = Stopwatch.StartNew();

            if (string.IsNullOrWhiteSpace(source))
            {
                return BadRequest("Data source is required (e.g., 'whatsapp' or 'facebook').");
            }

            if (messages == null || messages.Count == 0)
            {
                return BadRequest("No messages provided.");
            }

            List<MessageViewModel> messagesViewModels = new();

            foreach (var message in messages)
            {
                var vm = new MessageViewModel
                {
                    Sender = message.Author,
                    Text = message.Message
                };

                if (message.Date.HasValue)
                {
                    var dateTime = message.Date.Value;

                    if (dateTime.Kind == DateTimeKind.Utc)
                    {
                        vm.Date = new DateTimeOffset(dateTime, TimeSpan.Zero); // Already UTC
                    }
                    else if (dateTime.Kind == DateTimeKind.Local)
                    {
                        vm.Date = new DateTimeOffset(dateTime, TimeZoneInfo.Local.GetUtcOffset(dateTime)).ToUniversalTime();
                    }
                    else
                    {
                        // Unspecified - Assume local time and convert to UTC
                        vm.Date = new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Local), TimeZoneInfo.Local.GetUtcOffset(dateTime)).ToUniversalTime();
                    }
                }

                messagesViewModels.Add(vm);
            }

            try
            {
                List<Message> messageEntities = new();

                var encryptionKey = _configuration["Encryption:Key"];
                var encryptionHelper = new EncryptionHelper(encryptionKey!);

                if (string.IsNullOrWhiteSpace(encryptionKey))
                {
                    _logger.LogError("Encryption key is not configured.");
                    return StatusCode(500, "Encryption key is not configured.");
                }

                foreach (var vm in messagesViewModels)
                {
                    if (string.IsNullOrWhiteSpace(vm.Text))
                    {
                        continue;
                    }

                    Message message = new();
                   
                    message.EncryptedText = encryptionHelper.Encrypt(vm.Text);
                    message.Text = vm.Text;

                    messageEntities.Add(message);
                }

                var texts = messagesViewModels.Select(vm => vm.Text).ToList();
                var embeddingsList = await _embeddingService.GenerateEmbeddings(texts!);

                if (embeddingsList.Count != messagesViewModels.Count)
                {
                    _logger.LogError("Mismatch between input messages and embeddings. Input: {InputCount}, Embeddings: {EmbeddingCount}",
                        messagesViewModels.Count, embeddingsList.Count);
                    return StatusCode(500, "Embedding generation failed for some messages.");
                }

                for (int i = 0; i < messagesViewModels.Count; i++)
                {
                    messageEntities[i].Embedding = new Pgvector.Vector(embeddingsList[i].ToArray());
                    messageEntities[i].Text = string.Empty;
                }

                _context.AddRange(messageEntities);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing messages. Message: {Message}", ex.Message);
                return StatusCode(500, "An error occurred while processing the messages.");
            }

            sw.Stop();
            _logger.LogInformation("Processing completed in {ElapsedTime} seconds", sw.Elapsed.TotalSeconds);

            return Ok();
        }
    }
}
