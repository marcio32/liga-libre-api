using Microsoft.AspNetCore.Mvc;
using LigaLibre.WebMVC.Filters;
using LigaLibre.WebMVC.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LigaLibre.WebMVC.Controllers
{
    [AuthorizeSession]
    public class MatchController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public MatchController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]!);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("/api/statistics/League");
            
            if (!response.IsSuccessStatusCode)
                return View(new StatisticsViewModel());

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<StatisticsViewModel>(json, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            return View(data ?? new StatisticsViewModel());
        }
    }
}
