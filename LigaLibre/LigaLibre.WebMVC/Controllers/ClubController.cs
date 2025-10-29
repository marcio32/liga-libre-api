using LigaLibre.WebMVC.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LigaLibre.WebMVC.Controllers
{
    [AuthorizeSession]
    public class ClubController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
