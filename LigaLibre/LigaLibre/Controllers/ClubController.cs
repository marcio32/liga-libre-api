using FluentValidation;
using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaLibre.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClubController(IClubService clubService, IValidator<CreateClubDto> validator) : ControllerBase
{

    [HttpGet]
    [Route("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var clubs = await clubService.GetAllClubsAsync();
        return Ok(clubs);
    }

    [HttpGet]
    [Route("GetById")]
    public async Task<IActionResult> GetById(int id)
    {
        var club = await clubService.GetClubByIdAsync(id);
        return club == null ? NotFound() : Ok(club);
    }

    [HttpPost]
    [Route("CreateClub")]
    public async Task<IActionResult> CreateClub(CreateClubDto createClubDto)
    {
        var validationResult = await validator.ValidateAsync(createClubDto);
        return validationResult.IsValid ? StatusCode(201, await clubService.CreateClubAsync(createClubDto)) : BadRequest(validationResult.Errors);
    }

    [HttpPut]
    [Route("UpdateClub")]
    public async Task<IActionResult> UpdateClub(int id, CreateClubDto createClubDto)
    {
        var validationResult = await validator.ValidateAsync(createClubDto);
        return validationResult.IsValid ? Ok(await clubService.UpdateClubAsync(id, createClubDto)) : BadRequest(validationResult.Errors);
    }

    [HttpDelete]
    [Route("Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await clubService.DeleteClubAsync(id);
        return Ok(result);
    }
}