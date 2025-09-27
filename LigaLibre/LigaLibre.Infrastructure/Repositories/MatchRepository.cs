using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Enums;
using LigaLibre.Domain.Interfaces;
using LigaLibre.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LigaLibre.Infrastructure.Repositories;

public class MatchRepository(ApplicationDbContext context) : IMatchRepository
{
    public async Task<IEnumerable<Match>> GetAllAsync()
    {
        return await context.Match
            .Include(m => m.HomeClub)
            .Include(m => m.AwayClub)
            .Include(m => m.Referee)
            .OrderBy(m => m.MatchDate)
            .ToListAsync();
    }

    public async Task<Match?> GetByIdAsync(int id)
    {
        return await context.Match
            .Include(m => m.HomeClub)
            .Include(m => m.AwayClub)
            .Include(m => m.Referee)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Match>> GetByClubAsync(int clubId)
    {
        return await context.Match
            .Include(m => m.HomeClub)
            .Include(m => m.AwayClub)
            .Include(m => m.Referee)
            .Where(m => m.HomeClubId == clubId || m.AwayClubId == clubId)
            .OrderBy(m => m.MatchDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Match>> GetByRoundAsync(int round)
    {
        return await context.Match
           .Include(m => m.HomeClub)
           .Include(m => m.AwayClub)
           .Include(m => m.Referee)
           .Where(m => m.Round == round)
           .OrderBy(m => m.MatchDate)
           .ToListAsync();
    }

    public async Task<IEnumerable<Match>> GetByStatusAsync(MatchStatusEnum status)
    {
        return await context.Match
          .Include(m => m.HomeClub)
          .Include(m => m.AwayClub)
          .Include(m => m.Referee)
          .Where(m => m.Status == status)
          .OrderBy(m => m.MatchDate)
          .ToListAsync();
    }

    public async Task<Match> CreateAsync(Match match)
    {
        context.Match.Add(match);
        await context.SaveChangesAsync();
        return match;
    }

    public async Task<Match?> UpdateAsync(Match match)
    {
        match.UpdatedAt = DateTime.UtcNow;
        context.Match.Update(match);
        await context.SaveChangesAsync();
        return match;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var match = await context.Match.FindAsync(id);
        if (match == null) return false;

        context.Match.Remove(match);
        return await context.SaveChangesAsync() > 0;
    }
}

