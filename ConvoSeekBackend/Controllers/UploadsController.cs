using ConvoSeekBackend.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ConvoSeekBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private readonly ILogger<UploadsController> _logger;

        public UploadsController(ILogger<UploadsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] List<MessageViewModel> messages) // take in a DTO and convert to View Model
        {
            if (messages == null || messages.Count == 0)
            {
                return BadRequest("No messages provided.");
            }

            foreach (var message in messages)
            {
                if (message.Date.HasValue)
                {
                    var dateTime = message.Date.Value;

                    if (dateTime.Kind == DateTimeKind.Utc)
                    {
                        // message.Timestamp = new DateTimeOffset(dateTime, TimeSpan.Zero); // Already UTC
                    }
                    else if (dateTime.Kind == DateTimeKind.Local)
                    {
                        // message.Timestamp = new DateTimeOffset(dateTime, TimeZoneInfo.Local.GetUtcOffset(dateTime)).ToUniversalTime();
                    }
                    else
                    {
                        // Unspecified - Assume local time and convert to UTC
                        // message.Timestamp = new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Local), TimeZoneInfo.Local.GetUtcOffset(dateTime)).ToUniversalTime();
                    }
                }
            }

            // TODO: Add database storage or further processing

            return Ok();
        }


        /*
         * 
         *  [HttpPost("upload")]
    public async Task<IActionResult> UploadMessages([FromBody] List<MessageViewModel> messages)
    {
        if (messages == null || messages.Count == 0)
        {
            return BadRequest("No messages provided.");
        }

        try
        {
            // Generate embeddings and store in the database
            foreach (var message in messages)
            {
                // Generate embedding
                var embedding = await _embeddingService.GenerateEmbeddingAsync(message.Message);

                // Store in the database
                await _databaseService.StoreMessageAsync(new StoredMessage
                {
                    Author = message.Author,
                    Text = message.Message,
                    Timestamp = message.Date,
                    Embedding = embedding
                });
            }

            return Ok(new { Message = "Messages uploaded and processed successfully." });
        }
        catch (Exception ex)
        {
            // Log and return error response
            Console.WriteLine(ex);
            return StatusCode(500, "An error occurred while processing the messages.");
        }
    }
}*/
    }
}
