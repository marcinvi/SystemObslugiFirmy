using Microsoft.EntityFrameworkCore;
using ReklamacjeAPI.Data;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Models;

namespace ReklamacjeAPI.Services;

public interface IZgloszeniaService
{
    Task<PaginatedResponse<ZgloszenieDto>> GetZgloszeniaAsync(int page, int pageSize, string? status, int? userId);
    Task<ZgloszenieDetailsDto?> GetZgloszenieByIdAsync(int id);
    Task<ZgloszenieDto> CreateZgloszenieAsync(CreateZgloszenieRequest request, int userId);
    Task<ZgloszenieDto?> UpdateZgloszenieAsync(int id, UpdateZgloszenieRequest request, int userId);
    Task<ZgloszenieDto?> UpdateStatusAsync(int id, UpdateStatusRequest request, int userId);
    Task<bool> DeleteZgloszenieAsync(int id);
}

public class ZgloszeniaService : IZgloszeniaService
{
    private readonly ApplicationDbContext _context;

    public ZgloszeniaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<ZgloszenieDto>> GetZgloszeniaAsync(
        int page, int pageSize, string? status, int? userId)
    {
        var query = _context.Zgloszenia
            .Include(z => z.Klient)
            .Include(z => z.Produkt)
            .Include(z => z.AssignedUser)
            .AsQueryable();

        // Filter by status
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(z => z.StatusOgolny == status);
        }

        // Filter by assigned user
        if (userId.HasValue)
        {
            query = query.Where(z => z.PrzypisanyDo == userId.Value);
        }

        // Get total count
        var totalItems = await query.CountAsync();

        // Get paginated results
        var items = await query
            .OrderByDescending(z => z.DataZgloszenia)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(z => MapToDto(z))
            .ToListAsync();

        return new PaginatedResponse<ZgloszenieDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<ZgloszenieDetailsDto?> GetZgloszenieByIdAsync(int id)
    {
        var zgloszenie = await _context.Zgloszenia
            .Include(z => z.Klient)
            .Include(z => z.Produkt)
            .Include(z => z.AssignedUser)
            .Include(z => z.Dzialania).ThenInclude(d => d.User)
            .Include(z => z.Pliki).ThenInclude(p => p.UploadedBy)
            .FirstOrDefaultAsync(z => z.Id == id);

        if (zgloszenie == null)
            return null;

        return MapToDetailsDto(zgloszenie);
    }

    public async Task<ZgloszenieDto> CreateZgloszenieAsync(CreateZgloszenieRequest request, int userId)
    {
        // Generate unique zgłoszenie number
        var lastZgloszenie = await _context.Zgloszenia
            .OrderByDescending(z => z.Id)
            .FirstOrDefaultAsync();

        var nextNumber = lastZgloszenie != null ? lastZgloszenie.Id + 1 : 1;
        var nrZgloszenia = $"R/{nextNumber}/{DateTime.UtcNow.Year}";

        var zgloszenie = new Zgloszenie
        {
            NrZgloszenia = nrZgloszenia,
            IdKlienta = request.IdKlienta,
            IdProduktu = request.IdProduktu,
            Usterka = request.Usterka,
            Priorytet = request.Priorytet,
            PrzypisanyDo = request.PrzypisanyDo,
            StatusOgolny = "Nowe",
            DataZgloszenia = DateTime.UtcNow,
            DataModyfikacji = DateTime.UtcNow
        };

        _context.Zgloszenia.Add(zgloszenie);

        // Add initial action
        var dzialanie = new Dzialanie
        {
            IdZgloszenia = zgloszenie.Id,
            IdUzytkownika = userId,
            TypDzialania = "utworzenie",
            Opis = "Zgłoszenie utworzone",
            StatusNowy = "Nowe",
            DataDzialania = DateTime.UtcNow
        };
        _context.Dzialania.Add(dzialanie);

        await _context.SaveChangesAsync();

        // Reload with navigation properties
        await _context.Entry(zgloszenie).Reference(z => z.Klient).LoadAsync();
        await _context.Entry(zgloszenie).Reference(z => z.Produkt).LoadAsync();
        await _context.Entry(zgloszenie).Reference(z => z.AssignedUser).LoadAsync();

        return MapToDto(zgloszenie);
    }

    public async Task<ZgloszenieDto?> UpdateZgloszenieAsync(
        int id, UpdateZgloszenieRequest request, int userId)
    {
        var zgloszenie = await _context.Zgloszenia
            .Include(z => z.Klient)
            .Include(z => z.Produkt)
            .Include(z => z.AssignedUser)
            .FirstOrDefaultAsync(z => z.Id == id);

        if (zgloszenie == null)
            return null;

        var statusPoprzedni = zgloszenie.StatusOgolny;

        // Update fields
        if (request.Usterka != null)
            zgloszenie.Usterka = request.Usterka;

        if (request.StatusOgolny != null)
            zgloszenie.StatusOgolny = request.StatusOgolny;

        if (request.Priorytet != null)
            zgloszenie.Priorytet = request.Priorytet;

        if (request.PrzypisanyDo.HasValue)
            zgloszenie.PrzypisanyDo = request.PrzypisanyDo;

        if (request.Uwagi != null)
            zgloszenie.Uwagi = request.Uwagi;

        zgloszenie.DataModyfikacji = DateTime.UtcNow;

        // Log action
        var dzialanie = new Dzialanie
        {
            IdZgloszenia = id,
            IdUzytkownika = userId,
            TypDzialania = "modyfikacja",
            Opis = "Zgłoszenie zmodyfikowane",
            StatusPoprzedni = statusPoprzedni,
            StatusNowy = zgloszenie.StatusOgolny,
            DataDzialania = DateTime.UtcNow
        };
        _context.Dzialania.Add(dzialanie);

        await _context.SaveChangesAsync();

        return MapToDto(zgloszenie);
    }

    public async Task<ZgloszenieDto?> UpdateStatusAsync(
        int id, UpdateStatusRequest request, int userId)
    {
        var zgloszenie = await _context.Zgloszenia
            .Include(z => z.Klient)
            .Include(z => z.Produkt)
            .Include(z => z.AssignedUser)
            .FirstOrDefaultAsync(z => z.Id == id);

        if (zgloszenie == null)
            return null;

        var statusPoprzedni = zgloszenie.StatusOgolny;
        zgloszenie.StatusOgolny = request.StatusOgolny;
        zgloszenie.DataModyfikacji = DateTime.UtcNow;

        if (request.StatusOgolny == "Zamknięte")
        {
            zgloszenie.DataZamkniecia = DateTime.UtcNow;
        }

        // Log status change
        var dzialanie = new Dzialanie
        {
            IdZgloszenia = id,
            IdUzytkownika = userId,
            TypDzialania = "zmiana_statusu",
            Opis = request.Komentarz ?? $"Status zmieniony: {statusPoprzedni} → {request.StatusOgolny}",
            StatusPoprzedni = statusPoprzedni,
            StatusNowy = request.StatusOgolny,
            DataDzialania = DateTime.UtcNow
        };
        _context.Dzialania.Add(dzialanie);

        await _context.SaveChangesAsync();

        return MapToDto(zgloszenie);
    }

    public async Task<bool> DeleteZgloszenieAsync(int id)
    {
        var zgloszenie = await _context.Zgloszenia.FindAsync(id);
        if (zgloszenie == null)
            return false;

        _context.Zgloszenia.Remove(zgloszenie);
        await _context.SaveChangesAsync();
        return true;
    }

    private static ZgloszenieDto MapToDto(Zgloszenie z)
    {
        return new ZgloszenieDto
        {
            Id = z.Id,
            NrZgloszenia = z.NrZgloszenia,
            DataZgloszenia = z.DataZgloszenia,
            StatusOgolny = z.StatusOgolny,
            Usterka = z.Usterka,
            Priorytet = z.Priorytet,
            DataModyfikacji = z.DataModyfikacji,
            DataZamkniecia = z.DataZamkniecia,
            Klient = new KlientDto
            {
                Id = z.Klient.Id,
                ImieNazwisko = z.Klient.ImieNazwisko,
                Telefon = z.Klient.Telefon,
                Email = z.Klient.Email,
                Adres = z.Klient.Adres,
                Miasto = z.Klient.Miasto
            },
            Produkt = z.Produkt != null ? new ProduktDto
            {
                Id = z.Produkt.Id,
                Nazwa = z.Produkt.Nazwa,
                Producent = z.Produkt.Producent,
                Model = z.Produkt.Model,
                NumerSeryjny = z.Produkt.NumerSeryjny
            } : null,
            PrzypisanyDo = z.AssignedUser != null ? new UserDto
            {
                Id = z.AssignedUser.Id,
                Login = z.AssignedUser.Login,
                NazwaWyswietlana = z.AssignedUser.DisplayNameOrLogin,
                Email = z.AssignedUser.Email,
                Aktywny = z.AssignedUser.IsActive
            } : null
        };
    }

    private static ZgloszenieDetailsDto MapToDetailsDto(Zgloszenie z)
    {
        var dto = new ZgloszenieDetailsDto
        {
            Id = z.Id,
            NrZgloszenia = z.NrZgloszenia,
            DataZgloszenia = z.DataZgloszenia,
            StatusOgolny = z.StatusOgolny,
            Usterka = z.Usterka,
            Priorytet = z.Priorytet,
            DataModyfikacji = z.DataModyfikacji,
            DataZamkniecia = z.DataZamkniecia,
            Uwagi = z.Uwagi,
            Klient = new KlientDto
            {
                Id = z.Klient.Id,
                ImieNazwisko = z.Klient.ImieNazwisko,
                Telefon = z.Klient.Telefon,
                Email = z.Klient.Email,
                Adres = z.Klient.Adres,
                Miasto = z.Klient.Miasto
            },
            Produkt = z.Produkt != null ? new ProduktDto
            {
                Id = z.Produkt.Id,
                Nazwa = z.Produkt.Nazwa,
                Producent = z.Produkt.Producent,
                Model = z.Produkt.Model,
                NumerSeryjny = z.Produkt.NumerSeryjny
            } : null,
            PrzypisanyDo = z.AssignedUser != null ? new UserDto
            {
                Id = z.AssignedUser.Id,
                Login = z.AssignedUser.Login,
                NazwaWyswietlana = z.AssignedUser.DisplayNameOrLogin,
                Email = z.AssignedUser.Email,
                Aktywny = z.AssignedUser.IsActive
            } : null,
            Dzialania = z.Dzialania.OrderByDescending(d => d.DataDzialania).Select(d => new DzialanieDto
            {
                Id = d.Id,
                TypDzialania = d.TypDzialania,
                Opis = d.Opis,
                DataDzialania = d.DataDzialania,
                StatusPoprzedni = d.StatusPoprzedni,
                StatusNowy = d.StatusNowy,
                User = new UserDto
                {
                    Id = d.User.Id,
                    Login = d.User.Login,
                    NazwaWyswietlana = d.User.DisplayNameOrLogin,
                    Email = d.User.Email,
                    Aktywny = d.User.IsActive
                }
            }).ToList(),
            Pliki = z.Pliki.OrderByDescending(p => p.DataDodania).Select(p => new PlikDto
            {
                Id = p.Id,
                NazwaPliku = p.NazwaPliku,
                TypPliku = p.TypPliku,
                RozmiarPliku = p.RozmiarPliku,
                DataDodania = p.DataDodania,
                DodanyPrzez = p.UploadedBy != null ? new UserDto
                {
                    Id = p.UploadedBy.Id,
                    Login = p.UploadedBy.Login,
                    NazwaWyswietlana = p.UploadedBy.DisplayNameOrLogin,
                    Email = p.UploadedBy.Email,
                    Aktywny = p.UploadedBy.IsActive
                } : null
            }).ToList()
        };

        return dto;
    }
}
