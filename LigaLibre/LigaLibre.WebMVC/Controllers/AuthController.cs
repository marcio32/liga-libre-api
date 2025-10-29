using LigaLibre.WebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LigaLibre.WebMVC.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetSession([FromBody] JsonElement data)
        {
            var token = data.GetProperty("token").GetString();
            var email = data.GetProperty("email").GetString();
            
            HttpContext.Session.SetString("Token", token);
            HttpContext.Session.SetString("Email", email);
            HttpContext.Session.SetString("IsAuthenticated", "true");
            
            return Ok();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
