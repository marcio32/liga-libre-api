using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaLibre.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RefereeController(IRefereeService refereeService) : ControllerBase
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
        var referee = await refereeService.CreateRefereeAsync(createRefereeDto);
        return CreatedAtAction(nameof(GetRefereesById), new { id = referee.Id }, referee);
    }

    [HttpPut]
    [Route("UpdateReferee")]
    public async Task<IActionResult> UpdateReferee(int id, [FromBody] UpdateRefereeDto updateRefereeDto)
    {
        var referee = await refereeService.UpdateRefereeAsync(id, updateRefereeDto);
        return Ok(referee);
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

