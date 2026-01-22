-- ============================================
-- Reklamacje API - Database Initialization
-- ============================================
-- Ten skrypt dodaje brakujące elementy do istniejącej bazy ReklamacjeDB
-- Uruchom ten skrypt w MariaDB/MySQL przed pierwszym uruchomieniem API

USE ReklamacjeDB;

-- ============================================
-- 1. Tabela RefreshTokens (dla JWT)
-- ============================================

CREATE TABLE IF NOT EXISTS RefreshTokens (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    Token VARCHAR(500) NOT NULL,
    ExpiryDate DATETIME NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IsRevoked BOOLEAN DEFAULT FALSE,
    
    FOREIGN KEY (UserId) REFERENCES Uzytkownicy(IdUzytkownika) ON DELETE CASCADE,
    INDEX idx_token (Token(255)),
    INDEX idx_user (UserId),
    INDEX idx_expiry (ExpiryDate)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- 2. Dodaj indeksy dla wydajności
-- ============================================

-- Indeksy na tabeli Zgloszenia
ALTER TABLE Zgloszenia 
ADD INDEX IF NOT EXISTS idx_status (StatusOgolny),
ADD INDEX IF NOT EXISTS idx_data_zgloszenia (DataZgloszenia),
ADD INDEX IF NOT EXISTS idx_przypisany_do (PrzypisanyDo),
ADD INDEX IF NOT EXISTS idx_data_modyfikacji (DataModyfikacji);

-- Indeksy na tabeli Klienci
ALTER TABLE Klienci 
ADD INDEX IF NOT EXISTS idx_telefon (Telefon),
ADD INDEX IF NOT EXISTS idx_email (Email),
ADD INDEX IF NOT EXISTS idx_miasto (Miasto);

-- Indeksy na tabeli Dzialania
ALTER TABLE Dzialania 
ADD INDEX IF NOT EXISTS idx_zgloszenie (IdZgloszenia),
ADD INDEX IF NOT EXISTS idx_uzytkownik (IdUzytkownika),
ADD INDEX IF NOT EXISTS idx_data_dzialania (DataDzialania);

-- Indeksy na tabeli Pliki
ALTER TABLE Pliki 
ADD INDEX IF NOT EXISTS idx_zgloszenie_pliki (IdZgloszenia),
ADD INDEX IF NOT EXISTS idx_data_dodania (DataDodania);

-- ============================================
-- 3. Utwórz testowego użytkownika (opcjonalne)
-- ============================================

-- Hasło: test123 (bcrypt hash)
-- Jeśli użytkownik już istnieje, ten INSERT zostanie zignorowany
INSERT IGNORE INTO Uzytkownicy (Login, HasloHash, NazwaWyswietlana, Email, Aktywny, DataDodania)
VALUES (
    'admin',
    '$2a$11$6ZwFqYqKl2.xP9LV8vCqO.K3fWGdZOZ2XJoQQRq2QhZl8CQqNlQfK',
    'Administrator',
    'admin@reklamacje.pl',
    TRUE,
    NOW()
);

-- Dodaj drugiego testowego użytkownika
INSERT IGNORE INTO Uzytkownicy (Login, HasloHash, NazwaWyswietlana, Email, Aktywny, DataDodania)
VALUES (
    'technik',
    '$2a$11$6ZwFqYqKl2.xP9LV8vCqO.K3fWGdZOZ2XJoQQRq2QhZl8CQqNlQfK',
    'Jan Kowalski',
    'technik@reklamacje.pl',
    TRUE,
    NOW()
);

-- ============================================
-- 4. Utwórz testowych klientów (opcjonalne)
-- ============================================

INSERT IGNORE INTO Klienci (Id, ImieNazwisko, Telefon, Email, Adres, KodPocztowy, Miasto, DataDodania)
VALUES 
(1, 'Anna Nowak', '123456789', 'anna@example.com', 'ul. Testowa 1', '00-001', 'Warszawa', NOW()),
(2, 'Piotr Wiśniewski', '987654321', 'piotr@example.com', 'ul. Przykładowa 5', '30-001', 'Kraków', NOW());

-- ============================================
-- 5. Utwórz testowe produkty (opcjonalne)
-- ============================================

INSERT IGNORE INTO Produkty (IdProduktu, Nazwa, Producent, Model, NumerSeryjny, DataDodania)
VALUES 
(1, 'Laptop Dell XPS 15', 'Dell', 'XPS 15', 'SN123456', NOW()),
(2, 'iPhone 13 Pro', 'Apple', 'iPhone 13 Pro', 'SN789012', NOW());

-- ============================================
-- 6. Utwórz testowe zgłoszenie (opcjonalne)
-- ============================================

INSERT IGNORE INTO Zgloszenia (
    IdZgloszenia, NrZgloszenia, DataZgloszenia, IdKlienta, IdProduktu, 
    Usterka, StatusOgolny, PrzypisanyDo, DataModyfikacji
)
VALUES (
    1,
    'R/1/2025',
    NOW(),
    1,
    1,
    'Laptop nie włącza się',
    'Nowe',
    1,
    NOW()
);

-- Dodaj początkowe działanie do zgłoszenia
INSERT IGNORE INTO Dzialania (
    IdDzialania, IdZgloszenia, IdUzytkownika, TypDzialania, 
    Opis, DataDzialania, StatusNowy
)
VALUES (
    1,
    1,
    1,
    'utworzenie',
    'Zgłoszenie utworzone przez system',
    NOW(),
    'Nowe'
);

-- ============================================
-- 7. Weryfikacja
-- ============================================

-- Sprawdź utworzone tabele
SELECT 'Tabele w bazie:' as Info;
SHOW TABLES;

-- Sprawdź użytkowników
SELECT 'Użytkownicy:' as Info;
SELECT IdUzytkownika, Login, NazwaWyswietlana, Email, Aktywny FROM Uzytkownicy;

-- Sprawdź zgłoszenia
SELECT 'Zgłoszenia:' as Info;
SELECT COUNT(*) as LiczbaZgloszen FROM Zgloszenia;

-- ============================================
-- GOTOWE!
-- ============================================

SELECT '✅ Baza danych przygotowana!' as Status;
SELECT 'Dane testowe:' as Info;
SELECT 'Login: admin | Hasło: test123' as DaneLogowania;
SELECT 'Login: technik | Hasło: test123' as DaneLogowania2;
