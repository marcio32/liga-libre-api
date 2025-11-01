using LigaLibre.WebMVC.Filters;
using LigaLibre.WebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LigaLibre.WebMVC.Controllers
{
    [AuthorizeSession]
    public class PlayerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public PlayerController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/Players/GetAllPlayers");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }

            return StatusCode((int)response.StatusCode);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/Players/GetPlayerById?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }

            return StatusCode((int)response.StatusCode);
        }

        [HttpGet]
        public async Task<IActionResult> GetByClub(int clubId)
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/Players/GetPlayersByClub?clubId={clubId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }

            return StatusCode((int)response.StatusCode);
        }

        [HttpGet]
        public async Task<IActionResult> GetClubs()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/Club/GetAll");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }

            return StatusCode((int)response.StatusCode);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlayerViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var createDto = new
            {
                firstName = model.FirstName,
                lastName = model.LastName,
                position = model.Position,
                nationality = model.Nationality,
                age = model.Age,
                jerseyNumber = model.JerseyNumber,
                height = model.Height,
                weight = model.Weight,
                clubId = model.ClubId,
                dateOfBirth = model.DateOfBirth
            };

            var json = JsonSerializer.Serialize(createDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/Players/CreatePlayer", content);

            if (response.IsSuccessStatusCode)
                return Ok(await response.Content.ReadAsStringAsync());

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PlayerViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var updateDto = new
            {
                id = model.Id,
                firstName = model.FirstName,
                lastName = model.LastName,
                position = model.Position,
                nationality = model.Nationality,
                age = model.Age,
                jerseyNumber = model.JerseyNumber,
                height = model.Height,
                weight = model.Weight,
                clubId = model.ClubId,
                dateOfBirth = model.DateOfBirth
            };

            var json = JsonSerializer.Serialize(updateDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/Players/UpdatePlayer", content);

            if (response.IsSuccessStatusCode)
                return Ok(await response.Content.ReadAsStringAsync());

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/Players/DeletePlayer?id={id}");

            if (response.IsSuccessStatusCode)
                return Ok();

            return StatusCode((int)response.StatusCode);
        }
    }
}
