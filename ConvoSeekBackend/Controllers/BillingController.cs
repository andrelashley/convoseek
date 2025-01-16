using ConvoSeekBackend.Models;
using ConvoSeekBackend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace ConvoSeekBackend.Controllers
{
    [Authorize]
    public class BillingController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public BillingController(
            IConfiguration configuration,
            UserManager<User> userManager
            )
        {
            _configuration = configuration;
            _userManager = userManager;
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

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);

            var model = new SubscriptionViewModel
            {
                SubscriptionId = currentUser.SubscriptionId ?? string.Empty,
                IsSubscriptionActive = currentUser.IsSubscriptionActive,
                PublishableKey = _configuration["Stripe:PublishableKey"]
            };

            return View(model);
        }

        public IActionResult Cancel() => View();

        [HttpGet("Billing/Success")]
        public async Task<IActionResult> Success(string sessionId)
        {
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            var service = new SessionService();
            var session = service.Get(sessionId);

            var subscriptionId = session.SubscriptionId.ToString();
            var customerId = session.CustomerId;

            var currentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
            currentUser!.SubscriptionId = subscriptionId;
            currentUser!.IsSubscriptionActive = true;

            if (string.IsNullOrEmpty(currentUser!.CustomerId))
            {
                currentUser.CustomerId = customerId;
                currentUser.CustomerCreatedAt = DateTime.UtcNow; // for auditing
            }

            await _userManager.UpdateAsync(currentUser);

            return View();
        }

    }
}
