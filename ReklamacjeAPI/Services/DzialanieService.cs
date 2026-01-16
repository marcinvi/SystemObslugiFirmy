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
    private readonly ReklamacjeDbContext _context;

    public DzialanieService(ReklamacjeDbContext context)
    {
        _context = context;
    }

    public async Task<List<DzialanieDto>> GetDzialaniaByZgloszenieAsync(int zgloszenieId)
    {
        var dzialania = await _context.Dzialania
            .Include(d => d.Uzytkownik)
            .Where(d => d.IdZgloszenia == zgloszenieId)
            .OrderByDescending(d => d.DataDzialania)
            .ToListAsync();

        return dzialania.Select(d => new DzialanieDto
        {
            IdDzialania = d.IdDzialania,
            IdZgloszenia = d.IdZgloszenia,
            TypDzialania = d.TypDzialania,
            Opis = d.Opis,
            StaryStatus = d.StaryStatus,
            NowyStatus = d.NowyStatus,
            DataDzialania = d.DataDzialania,
            Uzytkownik = d.Uzytkownik != null ? new UserMinimalDto
            {
                IdUzytkownika = d.Uzytkownik.IdUzytkownika,
                NazwaWyswietlana = d.Uzytkownik.NazwaWyswietlana
            } : null
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
            .Include(d => d.Uzytkownik)
            .FirstAsync(d => d.IdDzialania == dzialanie.IdDzialania);

        return new DzialanieDto
        {
            IdDzialania = dzialanie.IdDzialania,
            IdZgloszenia = dzialanie.IdZgloszenia,
            TypDzialania = dzialanie.TypDzialania,
            Opis = dzialanie.Opis,
            DataDzialania = dzialanie.DataDzialania,
            Uzytkownik = new UserMinimalDto
            {
                IdUzytkownika = dzialanie.Uzytkownik!.IdUzytkownika,
                NazwaWyswietlana = dzialanie.Uzytkownik.NazwaWyswietlana
            }
        };
    }
}
