using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaLibre.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MatchController(IMatchService matchService) : ControllerBase
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
        var match = await matchService.CreateMatchAsync(matchDto);
        return CreatedAtAction(nameof(GetMatchById), new { id = match.Id }, match);
    }

    [HttpPut]
    [Route("UpdateMatch")]
    public async Task<ActionResult<MatchDto>> UpdateMatch(int id, UpdateMatchDto matchDto)
    {
        var match = await matchService.UpdateMatchAsync(id, matchDto);
        return match != null ? Ok(match) : NotFound();
    }

    [HttpDelete]
    [Route("DeleteMatch")]
    public async Task<ActionResult> DeleteMatch(int id)
    {
        var result = await matchService.DeleteMatchAsync(id);
        return result ? NoContent() : NotFound();
    }
}
