using Microsoft.EntityFrameworkCore;
using ReklamacjeAPI.Data;
using ReklamacjeAPI.DTOs;
using ReklamacjeAPI.Models;

namespace ReklamacjeAPI.Services;

public interface IFileService
{
    Task<FileUploadResponse> UploadFileAsync(IFormFile file, int? zgloszenieId, int userId);
    Task<Plik?> GetFileByIdAsync(int id);
    Task<List<PlikDto>> GetFilesByZgloszenieAsync(int zgloszenieId);
    Task<bool> DeleteFileAsync(int id, int userId);
}

public class FileService : IFileService
{
    private readonly ReklamacjeDbContext _context;
    private readonly string _uploadPath;

    public FileService(ReklamacjeDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _uploadPath = Path.Combine(environment.ContentRootPath, "uploads");
        
        // Create uploads directory if it doesn't exist
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<FileUploadResponse> UploadFileAsync(
        IFormFile file, int? zgloszenieId, int userId)
    {
        // Validate file
        if (file.Length == 0)
            throw new Exception("Plik jest pusty");

        if (file.Length > 10 * 1024 * 1024) // 10MB limit
            throw new Exception("Plik jest za duży (max 10MB)");

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf" };
        if (!allowedTypes.Contains(file.ContentType))
            throw new Exception("Nieobsługiwany typ pliku");

        // Generate unique filename
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_uploadPath, fileName);

        // Save file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Save to database
        var plik = new Plik
        {
            IdZgloszenia = zgloszenieId,
            NazwaPliku = fileName,
            NazwaOryginalnaPliku = file.FileName,
            SciezkaPliku = filePath,
            TypPliku = file.ContentType,
            RozmiarPliku = file.Length,
            IdUzytkownikaDodal = userId,
            DataDodania = DateTime.Now
        };

        _context.Pliki.Add(plik);
        await _context.SaveChangesAsync();

        return new FileUploadResponse
        {
            IdPliku = plik.IdPliku,
            NazwaPliku = plik.NazwaPliku,
            Url = $"/api/files/{plik.IdPliku}"
        };
    }

    public async Task<Plik?> GetFileByIdAsync(int id)
    {
        return await _context.Pliki.FindAsync(id);
    }

    public async Task<List<PlikDto>> GetFilesByZgloszenieAsync(int zgloszenieId)
    {
        var pliki = await _context.Pliki
            .Include(p => p.UzytkownikDodal)
            .Where(p => p.IdZgloszenia == zgloszenieId)
            .OrderByDescending(p => p.DataDodania)
            .ToListAsync();

        return pliki.Select(p => new PlikDto
        {
            IdPliku = p.IdPliku,
            IdZgloszenia = p.IdZgloszenia,
            NazwaPliku = p.NazwaPliku,
            NazwaOryginalnaPliku = p.NazwaOryginalnaPliku,
            TypPliku = p.TypPliku,
            RozmiarPliku = p.RozmiarPliku,
            DataDodania = p.DataDodania,
            UzytkownikDodal = p.UzytkownikDodal != null ? new UserMinimalDto
            {
                IdUzytkownika = p.UzytkownikDodal.IdUzytkownika,
                NazwaWyswietlana = p.UzytkownikDodal.NazwaWyswietlana
            } : null
        }).ToList();
    }

    public async Task<bool> DeleteFileAsync(int id, int userId)
    {
        var plik = await _context.Pliki.FindAsync(id);
        if (plik == null)
            return false;

        // Delete file from disk
        if (File.Exists(plik.SciezkaPliku))
        {
            File.Delete(plik.SciezkaPliku);
        }

        // Delete from database
        _context.Pliki.Remove(plik);
        await _context.SaveChangesAsync();

        return true;
    }
}
