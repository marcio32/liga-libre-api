using Microsoft.AspNetCore.Mvc;

namespace LigaLibre.WebMVC.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
