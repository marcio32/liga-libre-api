using LigaLibre.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LigaLibre.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayersController(IPlayerService playerService ) : ControllerBase
{
    [HttpGet]
    [Route("GetPlayers")]
    public async Task<IActionResult> GetByID(int id)
    {
        var players = await playerService.GetPlayerByIdAsync(id);
        return Ok(players);
    }
}
