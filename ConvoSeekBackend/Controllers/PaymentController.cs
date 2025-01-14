using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using Microsoft.AspNetCore.Authorization;

namespace ConvoSeekBackend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IConfiguration _configuration;

        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
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
                SuccessUrl = $"{baseUrl}/Billing/Success",
                CancelUrl = $"{baseUrl}/Billing/Cancel",
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Ok(new { sessionId = session.Id });
        }
    }
}
