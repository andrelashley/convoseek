using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using Microsoft.AspNetCore.Authorization;
using ConvoSeekBackend.ViewModels;
using ConvoSeekBackend.Models;
using Microsoft.AspNetCore.Identity;

namespace ConvoSeekBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public PaymentController(
            IConfiguration configuration, 
            UserManager<User> userManager
            )
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        [HttpPost("create-checkout-session")]
        public IActionResult CreateCheckoutSession()
        {
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "subscription",
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = _configuration["Stripe:ProductId"],
                    Quantity = 1
                }
            },
                SuccessUrl = $"{baseUrl}/Billing/Success?sessionId={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{baseUrl}/Billing/Cancel",
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Ok(new { sessionId = session.Id });
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] Event stripeEvent)
        {
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            try
            {
                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;

                    var subscriptionId = session.SubscriptionId;
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
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Webhook error: {ex.Message}");
                return BadRequest();
            }
        }
        [HttpPost("cancel-subscription")]
        [Authorize]
        public async Task<IActionResult> CancelSubscription([FromBody] CancelSubscriptionRequest request)
        {
            try
            {
                StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

                var service = new SubscriptionService();
                var subscription = service.Get(request.SubscriptionId);

                // Validate the user owns the subscription
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null || currentUser.SubscriptionId != request.SubscriptionId)
                {
                    return Unauthorized(new { Error = "You are not authorized to cancel this subscription." });
                }

                // Cancel the subscription
                var options = new SubscriptionCancelOptions
                {
                    InvoiceNow = true, // Finalize the current invoice immediately
                    Prorate = true     // Prorate charges for unused time
                };
                var canceledSubscription = service.Cancel(subscription.Id, options);

                // Update user subscription details in the database
                currentUser.SubscriptionId = null; // Clear subscription ID
                currentUser.IsSubscriptionActive = false;
                await _userManager.UpdateAsync(currentUser); // Persist changes

                return Ok(new { Message = "Subscription canceled successfully." });
            }
            catch (StripeException ex)
            {
                // Handle Stripe API errors
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle other unexpected errors
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { Error = "An unexpected error occurred. Please try again later." });
            }
        }

    }
}
