-- ================================================
-- SKRYPT: Utworzenie pierwszego użytkownika
-- ================================================
-- Ten użytkownik będzie potrzebny do pierwszego logowania do API

-- 1. Utwórz użytkownika z hasłem: admin123
-- (Hash wygenerowany przez BCrypt)

INSERT INTO Uzytkownicy (
    Login,
    Haslo,
    Email,
    NazwaWyswietlana,
    Imie,
    Nazwisko,
    CzyAktywny,
    DataDodania
)
VALUES (
    'admin',
    '$2a$11$ZBXhVQKYx6mYYEJ3N.x5XOZ7Z7Z7Z7Z7Z7Z7Z7Z7Z7Z7Z7Z7Z7Z7',  -- BCrypt hash of "admin123"
    'admin@example.com',
    'Administrator',
    'Jan',
    'Kowalski',
    1,
    NOW()
);

-- ================================================
-- UWAGA: Zmień hasło!
-- ================================================
-- Ten hash to przykład. Wygeneruj własny używając:
--
-- C#:
-- var hash = BCrypt.Net.BCrypt.HashPassword("twoje_haslo");
-- Console.WriteLine(hash);
--
-- Online: https://bcrypt-generator.com/
--
-- Następnie zaktualizuj użytkownika:
-- UPDATE Uzytkownicy SET Haslo = 'twoj_nowy_hash' WHERE Login = 'admin';

-- ================================================
-- Sprawdź czy użytkownik został utworzony
-- ================================================

SELECT 
    IdUzytkownika,
    Login,
    Email,
    NazwaWyswietlana,
    CzyAktywny,
    DataDodania
FROM Uzytkownicy
WHERE Login = 'admin';

-- ================================================
-- Dodatkowi użytkownicy (opcjonalnie)
-- ================================================

-- Użytkownik: pracownik1, hasło: pracownik123
INSERT INTO Uzytkownicy (
    Login,
    Haslo,
    Email,
    NazwaWyswietlana,
    Imie,
    Nazwisko,
    CzyAktywny,
    DataDodania
)
VALUES (
    'pracownik1',
    '$2a$11$EXAMPLE_HASH_CHANGE_THIS',  -- Zmień na własny hash!
    'pracownik1@example.com',
    'Pracownik 1',
    'Anna',
    'Nowak',
    1,
    NOW()
);

-- ================================================
-- Przydatne zapytania testowe
-- ================================================

-- Wszystkie zgłoszenia
SELECT 
    z.NrZgloszenia,
    z.StatusOgolny,
    k.ImieNazwisko AS Klient,
    p.Nazwa AS Produkt,
    u.NazwaWyswietlana AS Przypisany,
    z.DataZgloszenia
FROM Zgloszenia z
LEFT JOIN Klienci k ON z.IdKlienta = k.Id
LEFT JOIN Produkty p ON z.IdProduktu = p.IdProduktu
LEFT JOIN Uzytkownicy u ON z.IdUzytkownikaPrzypisany = u.IdUzytkownika
ORDER BY z.DataZgloszenia DESC
LIMIT 10;

-- Historia działań dla zgłoszenia
SELECT 
    d.DataDzialania,
    d.TypDzialania,
    d.Opis,
    u.NazwaWyswietlana AS Wykonawca
FROM Dzialania d
LEFT JOIN Uzytkownicy u ON d.IdUzytkownika = u.IdUzytkownika
WHERE d.IdZgloszenia = 1
ORDER BY d.DataDzialania DESC;
