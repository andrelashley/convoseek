using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConvoSeekBackend.Data;
using ConvoSeekBackend.Services;
using ConvoSeekBackend.Repositories;
using ConvoSeekBackend.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace ConvoSeekBackend.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly ConvoSeekBackendContext _context;
        private readonly IMessagesRepository _messagesRepository;
        private readonly IEmbeddingService _embeddingService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SearchController> _logger;

        public SearchController(
            ConvoSeekBackendContext context,
            IMessagesRepository messagesRepository,
            IEmbeddingService embeddingService,
            IConfiguration configuration,
            ILogger<SearchController> logger)
        {
            _context = context;
            _messagesRepository = messagesRepository;
            _embeddingService = embeddingService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("Index")]
        public ActionResult Index() => View();

        [HttpGet("Search/Query")]
        public async Task<IActionResult> Query([FromQuery] string q = "")
        {
            try
            {
                var answer = await _messagesRepository.SearchAsync(q);
                return Ok(answer);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Search error: {ex.Message}");
                return StatusCode(500);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return StatusCode(500);
            }
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
