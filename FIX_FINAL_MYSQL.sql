-- ============================================================================
-- OSTATECZNY SKRYPT DLA MYSQL/MARIADB
-- Zgodny z rzeczywistą strukturą w kodzie
-- ============================================================================

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ============================================================================
-- TABELA: AllegroAccountOpiekun
-- ============================================================================
CREATE TABLE IF NOT EXISTS `AllegroAccountOpiekun` (
    `AllegroAccountId` INT NOT NULL,
    `OpiekunId` INT NOT NULL,
    PRIMARY KEY (`AllegroAccountId`),
    UNIQUE KEY `uk_account` (`AllegroAccountId`),
    INDEX `idx_opiekun` (`OpiekunId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: AllegroCustomerReturns
-- ============================================================================
CREATE TABLE IF NOT EXISTS `AllegroCustomerReturns` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `AllegroReturnId` VARCHAR(255),
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
    `UwagiMagazyn` TEXT,
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
    UNIQUE KEY `uk_allegro_return` (`AllegroReturnId`),
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
    `UzytkownikId` INT NOT NULL,
    `ZastepcaId` INT NOT NULL,
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
    `DotyczyZwrotuId` INT,
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
    `TypStatusu` VARCHAR(100) NOT NULL,
    PRIMARY KEY (`Id`),
    INDEX `idx_typ` (`TypStatusu`),
    INDEX `idx_nazwa` (`Nazwa`)
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
('Archiwalny', 'StatusWewnetrzny'),
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
('Do dalszej analizy', 'DecyzjaHandlowca'),
('Przekaż do reklamacji', 'DecyzjaHandlowca');

-- ============================================================================
-- TABELA: Wiadomosci
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Wiadomosci` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `NadawcaId` INT NOT NULL,
    `OdbiorcaId` INT NOT NULL,
    `Tresc` TEXT,
    `DataWyslania` DATETIME NOT NULL,
    `CzyPrzeczytana` TINYINT(1) NOT NULL DEFAULT 0,
    `DotyczyZwrotuId` INT,
    `Tytul` VARCHAR(500),
    `ParentMessageId` INT,
    `CzyOdpowiedziano` TINYINT(1) NOT NULL DEFAULT 0,
    PRIMARY KEY (`Id`),
    INDEX `idx_nadawca` (`NadawcaId`),
    INDEX `idx_odbiorca` (`OdbiorcaId`),
    INDEX `idx_data` (`DataWyslania`),
    INDEX `idx_przeczytana` (`CzyPrzeczytana`),
    INDEX `idx_zwrot` (`DotyczyZwrotuId`),
    INDEX `idx_parent` (`ParentMessageId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: ZwrotDzialania
-- ============================================================================
CREATE TABLE IF NOT EXISTS `ZwrotDzialania` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `ZwrotId` INT NOT NULL,
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

SELECT TABLE_NAME AS Tabela, TABLE_ROWS AS Wiersze
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = DATABASE()
AND TABLE_NAME IN ('AllegroAccountOpiekun', 'AllegroCustomerReturns', 'Delegacje', 'MagazynDziennik', 'Statusy', 'Wiadomosci', 'ZwrotDzialania')
ORDER BY TABLE_NAME;

SELECT '========================================' AS '';
SELECT '✅ STATUSY' AS '';
SELECT '========================================' AS '';

SELECT TypStatusu, COUNT(*) AS Ilosc
FROM Statusy
GROUP BY TypStatusu
ORDER BY TypStatusu;

SELECT '========================================' AS '';
SELECT '✅ KLUCZOWE KOLUMNY' AS '';
SELECT '========================================' AS '';

-- Sprawdź nazwę kolumny w AllegroCustomerReturns
SELECT 
    'AllegroCustomerReturns' AS Tabela,
    COLUMN_NAME AS Kolumna,
    DATA_TYPE AS Typ
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
AND TABLE_NAME = 'AllegroCustomerReturns'
AND COLUMN_NAME LIKE '%Uwagi%';

-- Sprawdź nazwę kolumny w Wiadomosci
SELECT 
    'Wiadomosci' AS Tabela,
    COLUMN_NAME AS Kolumna,
    DATA_TYPE AS Typ
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
AND TABLE_NAME = 'Wiadomosci'
AND (COLUMN_NAME LIKE '%Odczytana%' OR COLUMN_NAME LIKE '%Przeczytana%');

SELECT '========================================' AS '';
SELECT '✅✅✅ WSZYSTKO GOTOWE! ✅✅✅' AS '';
SELECT 'Kod używa: UwagiMagazyn, CzyPrzeczytana' AS 'Info';
SELECT '========================================' AS '';
