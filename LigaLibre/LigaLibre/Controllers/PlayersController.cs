using FluentValidation;
using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LigaLibre.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PlayersController(IPlayerService playerService, IValidator<CreatePlayerDto> validator) : ControllerBase
{

    [HttpGet]
    [Route("GetAllPlayers")]
    public async Task<IActionResult> GetAllPlayers()
    {
        throw new Exception("Error de prueba");
        var players = await playerService.GetAllPlayers();
        return Ok(players);
    }


    [HttpGet]
    [Route("GetPlayersByClub")]
    public async Task<IActionResult> GetPlayersByClub(int clubId)
    {
        var players = await playerService.GetPlayersByClubAsync(clubId);
        return Ok(players);
    }


    [HttpGet]
    [Route("GetPlayerById")]
    public async Task<IActionResult> GetByID(int id)
    {
        var players = await playerService.GetPlayerByIdAsync(id);
        return Ok(players);
    }


    [HttpPost]
    [Route("CreatePlayer")]
    public async Task<IActionResult> CreatePlayer(CreatePlayerDto createPlayerDto)
    {
        var validationResult = await validator.ValidateAsync(createPlayerDto);
        return validationResult.IsValid ? StatusCode(201, await playerService.CreatePlayerAsync(createPlayerDto)) : BadRequest(validationResult.Errors);
    }

    [HttpPut]
    [Route("UpdatePlayer")]
    public async Task<IActionResult> UpdatePlayer(int id, CreatePlayerDto createPlayerDto)
    {
        var validationResult = await validator.ValidateAsync(createPlayerDto);
        return validationResult.IsValid ? Ok(await playerService.UpdatePlayerAsync(id, createPlayerDto)) : BadRequest(validationResult.Errors);
    }

    [HttpDelete]
    [Route("DeletePlayer")]
    public async Task<IActionResult> DeletePlayers(int id)
    {
        var player = await playerService.DeletePlayerAsync(id);
        return Ok(player);
    }
}
