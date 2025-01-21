using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConvoSeekBackend.Data;
using ConvoSeekBackend.Services;
using ConvoSeekBackend.Repositories;
using ConvoSeekBackend.Helpers;
using Microsoft.AspNetCore.Authorization;
using ConvoSeekBackend.Models;
using Microsoft.AspNetCore.Identity;

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
        private readonly UserManager<User> _userManager;

        public SearchController(
            ConvoSeekBackendContext context,
            IMessagesRepository messagesRepository,
            IEmbeddingService embeddingService,
            IConfiguration configuration,
            ILogger<SearchController> logger,
            UserManager<User> userManager)
        {
            _context = context;
            _messagesRepository = messagesRepository;
            _embeddingService = embeddingService;
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("Search")]
        [Route("Search/Index")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || !currentUser.IsSubscriptionActive)
            {
                ViewData["ShowSubscriptionAlert"] = true;
            }

            return View();
        }

        [HttpGet("Search/Query")]
        public async Task<IActionResult> Query([FromQuery] string q = "")
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var answer = await _messagesRepository.SearchAsync(q, currentUser!);
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
