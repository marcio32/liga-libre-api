using LigaLibre.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LigaLibre.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController(IStatisticsService statisticsService) : ControllerBase
    {
        [HttpGet]
        [Route("League")]
        public async Task<IActionResult> GetLeagueStatistics()
        {
            var statistics = await statisticsService.GetLeaguesStatisticsDtoAsync();
            return Ok(statistics);
        }

        [HttpGet]
        [Route("Matches")]
        public async Task<IActionResult> GetMatchStatistics()
        {
            var statistics = await statisticsService.GetMatchesStatisticsDtoAsync();
            return Ok(statistics);
        }
        
        [HttpGet]
        [Route("Players")]
        public async Task<IActionResult> GetPlayersStatistics()
        {
            var statistics = await statisticsService.GetPlayersStatisticsDtoAsync();
            return Ok(statistics);
        }
    }
}
