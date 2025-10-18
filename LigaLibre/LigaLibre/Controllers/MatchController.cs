using FluentValidation;
using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaLibre.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MatchController(IMatchService matchService, IValidator<CreateMatchDto> createValidator, IValidator<UpdateMatchDto> updateValidator) : ControllerBase
{
    [HttpGet]
    [Route("GetAllMatches")]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetAllMatches()
    {
        var matches = await matchService.GetAllMatchesAsync();
        return Ok(matches);
    }

    [HttpGet]
    [Route("GetMatchById")]
    public async Task<ActionResult<MatchDto>> GetMatchById(int id)
    {
        var match = await matchService.GetMatchByIdAsync(id);
        return match != null ? Ok(match) : NotFound();
    }

    [HttpGet]
    [Route("GetMatchesByClub")]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatchesByClub(int clubId)
    {
        var matches = await matchService.GetMatchesByClubAsync(clubId);
        return Ok(matches);
    }

    [HttpGet]
    [Route("GetMatchesByRound")]
    public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatchesByRound(int round)
    {
        var matches = await matchService.GetMatchesByRoundAsync(round);
        return Ok(matches);
    }

    [HttpPost]
    [Route("CreateMatch")]
    public async Task<ActionResult<MatchDto>> CreateMatch(CreateMatchDto matchDto)
    {
        var validationResult = await createValidator.ValidateAsync(matchDto);
        return validationResult.IsValid ? StatusCode(201, await matchService.CreateMatchAsync(matchDto)) : BadRequest(validationResult.Errors);
    }

    [HttpPut]
    [Route("UpdateMatch")]
    public async Task<ActionResult<MatchDto>> UpdateMatch(int id, UpdateMatchDto matchDto)
    {
        var validationResult = await updateValidator.ValidateAsync(matchDto);
        return validationResult.IsValid ? Ok(await matchService.UpdateMatchAsync(id, matchDto)) : BadRequest(validationResult.Errors);
    }

    [HttpDelete]
    [Route("DeleteMatch")]
    public async Task<ActionResult> DeleteMatch(int id)
    {
        var result = await matchService.DeleteMatchAsync(id);
        return result ? NoContent() : NotFound();
    }
}
