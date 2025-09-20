using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Interfaces;
using LigaLibre.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LigaLibre.Infrastructure.Repositories;

public class PlayerRepository(ApplicationDbContext context) : IPlayerRepository
{
    public async Task<IEnumerable<Player>> GetAllAsync()
    {
        return await context.Player.Include(p => p.Club).ToListAsync();
    }

    public async Task<Player?> GetByIdAsync(int id)
    {
        return await context.Player.Include(p => p.Club).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Player>> GetByClubIdAsync(int clubId)
    {
        return await context.Player.Where(p => p.ClubId == clubId).Include(p => p.Club).ToListAsync();
    }

    public async Task<Player> CreateAsync(Player player)
    {
        context.Player.Add(player);
        await context.SaveChangesAsync();
        return player;
    }
    public async Task<Player> UpdateAsync(Player player)
    {
        context.Player.Update(player);
        await context.SaveChangesAsync();
        return player;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var player = await context.Player.FindAsync(id);
        if (player != null)
        {
            player.IsActive = false;
            context.Player.Update(player);
            return await context.SaveChangesAsync() > 0;
        }
        return false;
    }
}
