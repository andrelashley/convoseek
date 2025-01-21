using Microsoft.AspNetCore.Mvc;

namespace ConvoSeekBackend.Controllers
{
    public class DocumentationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
