using Microsoft.EntityFrameworkCore;
using ReklamacjeAPI.Data;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Models;

namespace ReklamacjeAPI.Services;

public interface IDzialanieService
{
    Task<List<DzialanieDto>> GetDzialaniaByZgloszenieAsync(int zgloszenieId);
    Task<DzialanieDto> AddNotatkaAsync(int zgloszenieId, CreateNotatkaRequest request, int userId);
}

public class DzialanieService : IDzialanieService
{
    private readonly ApplicationDbContext _context;

    public DzialanieService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DzialanieDto>> GetDzialaniaByZgloszenieAsync(int zgloszenieId)
    {
        var dzialania = await _context.Dzialania
            .Include(d => d.User)
            .Where(d => d.IdZgloszenia == zgloszenieId)
            .OrderByDescending(d => d.DataDzialania)
            .ToListAsync();

        return dzialania.Select(d => new DzialanieDto
        {
            Id = d.Id,
            TypDzialania = d.TypDzialania,
            Opis = d.Opis,
            StatusPoprzedni = d.StatusPoprzedni,
            StatusNowy = d.StatusNowy,
            DataDzialania = d.DataDzialania,
            User = d.User != null ? new UserDto
            {
                Id = d.User.Id,
                Login = d.User.Login,
                NazwaWyswietlana = d.User.DisplayNameOrLogin,
                Email = d.User.Email,
                Aktywny = d.User.IsActive
            } : null!
        }).ToList();
    }

    public async Task<DzialanieDto> AddNotatkaAsync(
        int zgloszenieId, CreateNotatkaRequest request, int userId)
    {
        var dzialanie = new Dzialanie
        {
            IdZgloszenia = zgloszenieId,
            IdUzytkownika = userId,
            TypDzialania = "Notatka",
            Opis = request.Opis,
            DataDzialania = DateTime.Now
        };

        _context.Dzialania.Add(dzialanie);
        
        // Update modification date on zgloszenie
        var zgloszenie = await _context.Zgloszenia.FindAsync(zgloszenieId);
        if (zgloszenie != null)
        {
            zgloszenie.DataModyfikacji = DateTime.Now;
        }

        await _context.SaveChangesAsync();

        // Reload with user
        dzialanie = await _context.Dzialania
            .Include(d => d.User)
            .FirstAsync(d => d.Id == dzialanie.Id);

        return new DzialanieDto
        {
            Id = dzialanie.Id,
            TypDzialania = dzialanie.TypDzialania,
            Opis = dzialanie.Opis,
            DataDzialania = dzialanie.DataDzialania,
            StatusPoprzedni = dzialanie.StatusPoprzedni,
            StatusNowy = dzialanie.StatusNowy,
            User = new UserDto
            {
                Id = dzialanie.User.Id,
                Login = dzialanie.User.Login,
                NazwaWyswietlana = dzialanie.User.DisplayNameOrLogin,
                Email = dzialanie.User.Email,
                Aktywny = dzialanie.User.IsActive
            }
        };
    }
}
