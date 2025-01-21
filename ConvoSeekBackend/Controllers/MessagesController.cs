using ConvoSeekBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ConvoSeekBackend.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly UserManager<User> _userManager;
        public MessagesController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || !currentUser.IsSubscriptionActive)
            {
                ViewData["ShowSubscriptionAlert"] = true;
            }

            return View();
        }
    }
}
