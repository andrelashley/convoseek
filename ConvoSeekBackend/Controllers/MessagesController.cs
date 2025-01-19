using Microsoft.AspNetCore.Mvc;

namespace ConvoSeekBackend.Controllers
{
    public class MessagesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
