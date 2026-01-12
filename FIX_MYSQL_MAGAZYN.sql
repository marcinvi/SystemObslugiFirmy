-- ============================================================================
-- SKRYPT TWORZENIA TABEL DLA MYSQL/MARIADB
-- (Konwersja ze struktury SQLite którą podałeś)
-- ============================================================================

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ============================================================================
-- TABELA: AllegroAccountOpiekun
-- ============================================================================
CREATE TABLE IF NOT EXISTS `AllegroAccountOpiekun` (
    `AllegroAccountId` INT NOT NULL UNIQUE COMMENT 'ID konta z Baza.db',
    `OpiekunId` INT NOT NULL COMMENT 'ID użytkownika (handlowca) z Baza.db',
    PRIMARY KEY (`AllegroAccountId`),
    INDEX `idx_opiekun` (`OpiekunId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: AllegroCustomerReturns
-- ============================================================================
CREATE TABLE IF NOT EXISTS `AllegroCustomerReturns` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `AllegroReturnId` VARCHAR(255) UNIQUE,
    `AllegroAccountId` INT,
    `ReferenceNumber` VARCHAR(255),
    `OrderId` VARCHAR(255),
    `BuyerLogin` VARCHAR(255),
    `CreatedAt` DATETIME,
    `StatusAllegro` VARCHAR(100),
    `Waybill` VARCHAR(255),
    `CarrierName` VARCHAR(255),
    `InvoiceNumber` VARCHAR(255),
    `ManualSenderDetails` TEXT,
    `IsManual` TINYINT(1) NOT NULL DEFAULT 0,
    `JsonDetails` TEXT,
    `StanProduktuId` INT,
    `UwagiMagazynu` TEXT,
    `StatusWewnetrznyId` INT,
    `DecyzjaHandlowcaId` INT,
    `DataPrzyjecia` DATETIME,
    `PrzyjetyPrzezId` INT,
    `ProductName` TEXT,
    `OfferId` VARCHAR(255),
    `Quantity` INT,
    `PaymentType` VARCHAR(100),
    `FulfillmentStatus` VARCHAR(100),
    `Delivery_FirstName` VARCHAR(255),
    `Delivery_LastName` VARCHAR(255),
    `Delivery_Street` VARCHAR(500),
    `Delivery_ZipCode` VARCHAR(20),
    `Delivery_City` VARCHAR(255),
    `Delivery_PhoneNumber` VARCHAR(50),
    `Buyer_FirstName` VARCHAR(255),
    `Buyer_LastName` VARCHAR(255),
    `Buyer_Street` VARCHAR(500),
    `Buyer_ZipCode` VARCHAR(20),
    `Buyer_City` VARCHAR(255),
    `Buyer_PhoneNumber` VARCHAR(50),
    `Invoice_CompanyName` VARCHAR(500),
    `Invoice_TaxId` VARCHAR(50),
    `Invoice_Street` VARCHAR(500),
    `Invoice_ZipCode` VARCHAR(20),
    `Invoice_City` VARCHAR(255),
    `BuyerFullName` VARCHAR(500),
    `KomentarzHandlowca` TEXT,
    `HandlowiecOpiekunId` INT,
    `DataDecyzji` DATETIME,
    PRIMARY KEY (`Id`),
    INDEX `idx_allegro_return` (`AllegroReturnId`),
    INDEX `idx_account` (`AllegroAccountId`),
    INDEX `idx_order` (`OrderId`),
    INDEX `idx_buyer` (`BuyerLogin`),
    INDEX `idx_status` (`StatusAllegro`),
    INDEX `idx_stan` (`StanProduktuId`),
    INDEX `idx_status_wew` (`StatusWewnetrznyId`),
    INDEX `idx_decyzja` (`DecyzjaHandlowcaId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: Delegacje
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Delegacje` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `UzytkownikId` INT NOT NULL COMMENT 'ID handlowca, który deleguje',
    `ZastepcaId` INT NOT NULL COMMENT 'ID handlowca, który jest zastępcą',
    `DataOd` DATETIME NOT NULL,
    `DataDo` DATETIME NOT NULL,
    `CzyAktywna` TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (`Id`),
    INDEX `idx_uzytkownik` (`UzytkownikId`),
    INDEX `idx_zastepca` (`ZastepcaId`),
    INDEX `idx_aktywna` (`CzyAktywna`),
    INDEX `idx_daty` (`DataOd`, `DataDo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: MagazynDziennik
-- ============================================================================
CREATE TABLE IF NOT EXISTS `MagazynDziennik` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Data` DATETIME NOT NULL,
    `Uzytkownik` VARCHAR(255),
    `Akcja` TEXT,
    `DotyczyZwrotuId` INT COMMENT 'Klucz obcy do tabeli AllegroCustomerReturns',
    PRIMARY KEY (`Id`),
    INDEX `idx_data` (`Data`),
    INDEX `idx_zwrot` (`DotyczyZwrotuId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: Statusy
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Statusy` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Nazwa` VARCHAR(255) NOT NULL,
    `TypStatusu` VARCHAR(100) NOT NULL COMMENT 'np. StanProduktu, StatusWewnetrzny, DecyzjaHandlowca',
    PRIMARY KEY (`Id`),
    INDEX `idx_typ` (`TypStatusu`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Wstaw domyślne statusy
INSERT IGNORE INTO `Statusy` (`Nazwa`, `TypStatusu`) VALUES
-- Statusy wewnętrzne
('Oczekuje na przyjęcie', 'StatusWewnetrzny'),
('Przyjęty do magazynu', 'StatusWewnetrzny'),
('W trakcie weryfikacji', 'StatusWewnetrzny'),
('Oczekuje na decyzję handlowca', 'StatusWewnetrzny'),
('Zakończony', 'StatusWewnetrzny'),
('Anulowany', 'StatusWewnetrzny'),
-- Stany produktu
('Nieprzypisany', 'StanProduktu'),
('Nowy / Nieużywany', 'StanProduktu'),
('Używany - Stan Dobry', 'StanProduktu'),
('Używany - Stan Zadowalający', 'StanProduktu'),
('Używany - Stan Zły', 'StanProduktu'),
('Uszkodzony', 'StanProduktu'),
('Niekompletny', 'StanProduktu'),
('Brak produktu w przesyłce', 'StanProduktu'),
-- Decyzje handlowca
('Nieprzypisany', 'DecyzjaHandlowca'),
('Zwrot pieniędzy - Pełna kwota', 'DecyzjaHandlowca'),
('Zwrot pieniędzy - Częściowy', 'DecyzjaHandlowca'),
('Wymiana na nowy produkt', 'DecyzjaHandlowca'),
('Naprawa gwarancyjna', 'DecyzjaHandlowca'),
('Odrzucenie zwrotu', 'DecyzjaHandlowca'),
('Do dalszej analizy', 'DecyzjaHandlowca');

-- ============================================================================
-- TABELA: Wiadomosci
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Wiadomosci` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `NadawcaId` INT NOT NULL COMMENT 'ID użytkownika (z Baza.db)',
    `OdbiorcaId` INT NOT NULL COMMENT 'ID użytkownika (z Baza.db)',
    `Tresc` TEXT,
    `DataWyslania` DATETIME NOT NULL,
    `CzyOdczytana` TINYINT(1) NOT NULL DEFAULT 0,
    `DotyczyZwrotuId` INT COMMENT 'Opcjonalny klucz obcy do tabeli AllegroCustomerReturns',
    `Tytul` VARCHAR(500),
    `ParentMessageId` INT,
    `CzyOdpowiedziano` TINYINT(1) NOT NULL DEFAULT 0,
    PRIMARY KEY (`Id`),
    INDEX `idx_nadawca` (`NadawcaId`),
    INDEX `idx_odbiorca` (`OdbiorcaId`),
    INDEX `idx_data` (`DataWyslania`),
    INDEX `idx_odczytana` (`CzyOdczytana`),
    INDEX `idx_zwrot` (`DotyczyZwrotuId`),
    INDEX `idx_parent` (`ParentMessageId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: ZwrotDzialania
-- ============================================================================
CREATE TABLE IF NOT EXISTS `ZwrotDzialania` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `ZwrotId` INT NOT NULL COMMENT 'Klucz obcy do tabeli AllegroCustomerReturns',
    `Data` DATETIME NOT NULL,
    `Uzytkownik` VARCHAR(255),
    `Tresc` TEXT,
    PRIMARY KEY (`Id`),
    INDEX `idx_zwrot` (`ZwrotId`),
    INDEX `idx_data` (`Data`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- PRZYWRÓCENIE SPRAWDZANIA KLUCZY OBCYCH
-- ============================================================================
SET FOREIGN_KEY_CHECKS = 1;

-- ============================================================================
-- WERYFIKACJA
-- ============================================================================
SELECT '========================================' AS '';
SELECT '✅ WERYFIKACJA TABEL' AS '';
SELECT '========================================' AS '';

SELECT 
    'AllegroAccountOpiekun' AS Tabela,
    COUNT(*) AS LiczbaKolumn
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
AND TABLE_NAME = 'AllegroAccountOpiekun'

UNION ALL

SELECT 
    'AllegroCustomerReturns' AS Tabela,
    COUNT(*) AS LiczbaKolumn
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
AND TABLE_NAME = 'AllegroCustomerReturns'

UNION ALL

SELECT 
    'Delegacje' AS Tabela,
    COUNT(*) AS LiczbaKolumn
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
AND TABLE_NAME = 'Delegacje'

UNION ALL

SELECT 
    'MagazynDziennik' AS Tabela,
    COUNT(*) AS LiczbaKolumn
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
AND TABLE_NAME = 'MagazynDziennik'

UNION ALL

SELECT 
    'Statusy' AS Tabela,
    COUNT(*) AS LiczbaKolumn
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
AND TABLE_NAME = 'Statusy'

UNION ALL

SELECT 
    'Wiadomosci' AS Tabela,
    COUNT(*) AS LiczbaKolumn
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
AND TABLE_NAME = 'Wiadomosci'

UNION ALL

SELECT 
    'ZwrotDzialania' AS Tabela,
    COUNT(*) AS LiczbaKolumn
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
AND TABLE_NAME = 'ZwrotDzialania';

-- Sprawdź statusy
SELECT '========================================' AS '';
SELECT '✅ STATUSY' AS '';
SELECT '========================================' AS '';

SELECT Nazwa, TypStatusu, COUNT(*) OVER(PARTITION BY TypStatusu) AS IloscWTypie
FROM Statusy
ORDER BY TypStatusu, Nazwa;

SELECT '========================================' AS '';
SELECT '✅✅✅ WSZYSTKO GOTOWE! ✅✅✅' AS '';
SELECT 'Teraz możesz uruchomić aplikację!' AS '';
SELECT '========================================' AS '';
