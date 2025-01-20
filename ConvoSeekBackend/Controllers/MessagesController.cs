using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConvoSeekBackend.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        public IActionResult Index() => View();
    }
}
