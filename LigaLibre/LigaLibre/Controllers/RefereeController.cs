using FluentValidation;
using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaLibre.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RefereeController(IRefereeService refereeService, IValidator<CreateRefereeDto> createValidator, IValidator<UpdateRefereeDto> updateValidator) : ControllerBase
{

    [HttpGet]
    [Route("GetAllReferees")]
    public async Task<IActionResult> GetAllReferees()
    {
        var referees = await refereeService.GetAllRefereesAsync();
        return Ok(referees);
    }

    [HttpGet]
    [Route("GetActiveReferees")]
    public async Task<IActionResult> GetActiveReferees()
    {
        var referees = await refereeService.GetActiveRefereesAsync();
        return Ok(referees);
    }

    [HttpGet]
    [Route("GetRefereesById")]
    public async Task<IActionResult> GetRefereesById(int id)
    {
        var referees = await refereeService.GetRefereeByIdAsync(id);
        return Ok(referees);
    }

    [HttpPost]
    [Route("CreateReferee")]
    public async Task<IActionResult> CreateReferee([FromBody] CreateRefereeDto createRefereeDto)
    {
        var validationResult = await createValidator.ValidateAsync(createRefereeDto);
        return validationResult.IsValid ? StatusCode(201, await refereeService.CreateRefereeAsync(createRefereeDto)) : BadRequest(validationResult.Errors);
    }

    [HttpPut]
    [Route("UpdateReferee")]
    public async Task<IActionResult> UpdateReferee([FromBody] UpdateRefereeDto updateRefereeDto)
    {
        var validationResult = await updateValidator.ValidateAsync(updateRefereeDto);
        return validationResult.IsValid ? Ok(await refereeService.UpdateRefereeAsync(updateRefereeDto)) : BadRequest(validationResult.Errors);
    }

    [HttpDelete]
    [Route("DeleteReferee")]
    public async Task<IActionResult> DeleteReferee(int id)
    {
        var deleted = await refereeService.DeleteRefereeAsync(id);
        if (!deleted) return NotFound($"Arbitro con ID {id} no encontrado");
        return NoContent();
    }
}

