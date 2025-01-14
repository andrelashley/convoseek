using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConvoSeekBackend.Controllers
{
    [Authorize]
    public class BillingController : Controller
    {
        private readonly IConfiguration _configuration;

        public BillingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /*
         *  public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User!.Identity!.Name);
            if (user.IsSubscriptionActive == true) return RedirectToAction(nameof(AlreadySubscribed));

            ViewBag.PriceId = _config["StripePriceId"];

            return View();
        }
         * 
         */

        public IActionResult Index()
        {
            ViewData["PublishableKey"] = _configuration["Stripe:PublishableKey"];
            return View();
        }

        public IActionResult Cancel() => View();

        public IActionResult Success() => View();
    }
}
