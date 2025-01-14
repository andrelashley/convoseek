using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConvoSeekBackend.Data;
using ConvoSeekBackend.Models;
using ConvoSeekBackend.Services;
using ConvoSeekBackend.Repositories;
using ConvoSeekBackend.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace ConvoSeekBackend.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly ConvoSeekBackendContext _context;
        private readonly IMessagesRepository _messagesRepository;
        private readonly IEmbeddingService _embeddingService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(
            ConvoSeekBackendContext context,
            IMessagesRepository messagesRepository,
            IEmbeddingService embeddingService,
            IConfiguration configuration,
            ILogger<MessagesController> logger)
        {
            _context = context;
            _messagesRepository = messagesRepository;
            _embeddingService = embeddingService;
            _configuration = configuration;
            _logger = logger;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            return View(await _context.Messages.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            // return View(message);
            // Retrieve the encryption key from the configuration
            var encryptionKey = _configuration["Encryption:Key"];
            var encryptionHelper = new EncryptionHelper(encryptionKey!);

            // Encrypt the message text
            var decryptedText = encryptionHelper.Decrypt(message.EncryptedText!);

            return Ok(decryptedText);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MessageId,Text")] Message message)
        {
            if (ModelState.IsValid)
            {
                var embeddings = await _embeddingService.GenerateEmbedding(message.Text);
                var embeddingArray = embeddings.ToArray();
                message.Embedding = new Pgvector.Vector(embeddingArray);

                var encryptionKey = _configuration["Encryption:Key"];
                var encryptionHelper = new EncryptionHelper(encryptionKey!);
                message.EncryptedText = encryptionHelper.Encrypt(message.Text);
                message.Text = string.Empty;

                _context.Add(message);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(message);
        }

        [HttpGet("Messages/Search")]
        public async Task<IActionResult> Search([FromQuery] string q = "who is suffering from dementia?")
        {
            try
            {
                var answer = await _messagesRepository.SearchAsync(q);
                return Ok(answer);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Search error: {ex.Message}");
                return RedirectToAction(nameof(Error));
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return RedirectToAction(nameof(Error));
            }
        }

        public IActionResult Error()
        {
            return View();
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("MessageId,Text")] Message message)
        {
            if (id != message.MessageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.MessageId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(Guid id)
        {
            return _context.Messages.Any(e => e.MessageId == id);
        }
    }
}
