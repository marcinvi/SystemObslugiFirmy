namespace ReklamacjeAPI.DTOs;

// Generic API Response wrapper
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

// Paginated response
public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
}

// Auth DTOs
public class LoginRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class UserDto
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string NazwaWyswietlana { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool Aktywny { get; set; }
}

// Zgloszenie DTOs
public class ZgloszenieDto
{
    public int Id { get; set; }
    public string NrZgloszenia { get; set; } = string.Empty;
    public DateTime DataZgloszenia { get; set; }
    public string StatusOgolny { get; set; } = string.Empty;
    public string? Usterka { get; set; }
    public string? Priorytet { get; set; }
    public DateTime DataModyfikacji { get; set; }
    public DateTime? DataZamkniecia { get; set; }
    public KlientDto Klient { get; set; } = null!;
    public ProduktDto? Produkt { get; set; }
    public UserDto? PrzypisanyDo { get; set; }
}

public class ZgloszenieDetailsDto : ZgloszenieDto
{
    public string? Uwagi { get; set; }
    public List<DzialanieDto> Dzialania { get; set; } = new();
    public List<PlikDto> Pliki { get; set; } = new();
}

public class CreateZgloszenieRequest
{
    public int IdKlienta { get; set; }
    public int? IdProduktu { get; set; }
    public string? Usterka { get; set; }
    public string? Priorytet { get; set; }
    public int? PrzypisanyDo { get; set; }
}

public class UpdateZgloszenieRequest
{
    public string? Usterka { get; set; }
    public string? StatusOgolny { get; set; }
    public string? Priorytet { get; set; }
    public int? PrzypisanyDo { get; set; }
    public string? Uwagi { get; set; }
}

public class UpdateStatusRequest
{
    public string StatusOgolny { get; set; } = string.Empty;
    public string? Komentarz { get; set; }
}

// Klient DTOs
public class KlientDto
{
    public int Id { get; set; }
    public string ImieNazwisko { get; set; } = string.Empty;
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public string? Adres { get; set; }
    public string? Miasto { get; set; }
}

public class CreateKlientRequest
{
    public string ImieNazwisko { get; set; } = string.Empty;
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public string? Adres { get; set; }
    public string? KodPocztowy { get; set; }
    public string? Miasto { get; set; }
    public string? Uwagi { get; set; }
}

// Produkt DTOs
public class ProduktDto
{
    public int Id { get; set; }
    public string Nazwa { get; set; } = string.Empty;
    public string? Producent { get; set; }
    public string? Model { get; set; }
    public string? NumerSeryjny { get; set; }
}

// Dzialanie DTOs
public class DzialanieDto
{
    public int Id { get; set; }
    public string TypDzialania { get; set; } = string.Empty;
    public string? Opis { get; set; }
    public DateTime DataDzialania { get; set; }
    public string? StatusPoprzedni { get; set; }
    public string? StatusNowy { get; set; }
    public UserDto User { get; set; } = null!;
}

public class CreateNotatkaRequest
{
    public string Opis { get; set; } = string.Empty;
}

// Plik DTOs
public class PlikDto
{
    public int Id { get; set; }
    public string NazwaPliku { get; set; } = string.Empty;
    public string? TypPliku { get; set; }
    public long? RozmiarPliku { get; set; }
    public DateTime DataDodania { get; set; }
    public UserDto? DodanyPrzez { get; set; }
}

public class FileUploadResponse
{
    public int Id { get; set; }
    public string NazwaPliku { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
