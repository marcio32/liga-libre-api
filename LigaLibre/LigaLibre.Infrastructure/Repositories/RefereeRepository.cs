using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Interfaces;
using LigaLibre.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LigaLibre.Infrastructure.Repositories;

public class RefereeRepository(ApplicationDbContext context) : IRefereeRepository
{
    public async Task<IEnumerable<Referee>> GetAllAsync()
    {
        return await context.Referee.OrderBy(r => r.LastName).ThenBy(r => r.FirstName).ToListAsync();
    }

    public async Task<Referee?> GetByIdAsync(int id)
    {
        return await context.Referee.Include(r => r.Matches).FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Referee?> GetByLicenseNumberAsync(string licenseNumber)
    {
        return await context.Referee
            .FirstOrDefaultAsync(r => r.LicenseNumber == licenseNumber);
    }

    public async Task<IEnumerable<Referee>> GetActivesAsync()
    {
        return await context.Referee
            .Where(r => r.IsActive)
            .OrderBy(r => r.LastName)
            .ThenBy(r => r.FirstName)
            .ToListAsync();
    }

    public async Task<Referee> CreateAsync(Referee referee)
    {
        context.Referee.Add(referee);
        await context.SaveChangesAsync();
        return referee;
    }

    public async Task<Referee?> UpdateAsync(Referee referee)
    {
        referee.UpdatedAt = DateTime.UtcNow;
        context.Referee.Update(referee);
        await context.SaveChangesAsync();
        return referee;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var referee = await context.Referee.FindAsync(id);
        if (referee == null) return false;

        context.Referee.Remove(referee);
        return await context.SaveChangesAsync() > 0;
       
    }
}
