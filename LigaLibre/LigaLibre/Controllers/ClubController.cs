using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaLibre.API.Controllers;


[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class ClubController : ControllerBase
{
    private readonly IClubService _clubService;

    public ClubController(IClubService clubService)
    {
        _clubService = clubService;
    }

    [HttpGet]
    [Route("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var clubs = await _clubService.GetAllClubsAsync();
        return Ok(clubs);
    }

    [HttpGet]
    [Route("GetById/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var club = await _clubService.GetClubByIdAsync(id);
        return club == null ? NotFound() : Ok(club);
    }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> Create(CreateClubDto createClubDto)
    {
        var club = await _clubService.CreateClubAsync(createClubDto);
        return CreatedAtAction(nameof(GetById), new { id = club.Id }, club);
    }

    [HttpPut]
    [Route("Update/{id}")]
    public async Task<IActionResult> Update(int id, CreateClubDto createClubDto)
    {
        var club = await _clubService.UpdateClubAsync(id, createClubDto);
        return Ok(club);
    }

    [HttpDelete]
    [Route("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _clubService.DeleteClubAsync(id);
        return Ok(result);
    }
}