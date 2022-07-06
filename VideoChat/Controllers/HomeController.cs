
using Microsoft.AspNetCore.Mvc;

namespace webrtc_dotnetcore.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("WebRTC");
        }

        public IActionResult WebRTC()
        {
            return View();
        }
    }
}
