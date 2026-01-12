-- ============================================================================
-- NAPRAWA TABEL STATUSY I ZWROTÓW ALLEGRO - WERSJA KOMPLETNA
-- Data: 2026-01-07
-- Autor: System naprawczy
-- ============================================================================

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ============================================================================
-- CZĘŚĆ 1: TABELA STATUSY (dla zwrotów Allegro)
-- ============================================================================

-- Usuń starą tabelę jeśli istnieje (tylko jeśli jest pusta!)
-- DROP TABLE IF EXISTS Statusy;

-- Utwórz tabelę Statusy z poprawną strukturą
CREATE TABLE IF NOT EXISTS `Statusy` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Nazwa` VARCHAR(100) NOT NULL,
    `TypStatusu` VARCHAR(50) NOT NULL COMMENT 'StatusWewnetrzny, StanProduktu, DecyzjaHandlowca',
    `Kolejnosc` INT DEFAULT 0,
    `Kolor` VARCHAR(20) DEFAULT NULL,
    `CzyAktywny` TINYINT(1) DEFAULT 1,
    `DataUtworzenia` DATETIME DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY `idx_nazwa_typ` (`Nazwa`, `TypStatusu`),
    INDEX `idx_typ` (`TypStatusu`),
    INDEX `idx_aktywny` (`CzyAktywny`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- CZĘŚĆ 2: WSTAW STATUSY DOMYŚLNE
-- ============================================================================

-- Statusy wewnętrzne (cykl życia zwrotu w magazynie)
INSERT IGNORE INTO `Statusy` (`Nazwa`, `TypStatusu`, `Kolejnosc`, `Kolor`) VALUES
('Oczekuje na przyjęcie', 'StatusWewnetrzny', 1, '#FFA500'),
('Przyjęty do magazynu', 'StatusWewnetrzny', 2, '#4169E1'),
('W trakcie weryfikacji', 'StatusWewnetrzny', 3, '#FFD700'),
('Oczekuje na decyzję handlowca', 'StatusWewnetrzny', 4, '#FF6347'),
('Zakończony', 'StatusWewnetrzny', 5, '#32CD32'),
('Anulowany', 'StatusWewnetrzny', 6, '#808080'),
('Archiwalny', 'StatusWewnetrzny', 7, '#696969');

-- Stany produktu (fizyczny stan zwróconego towaru)
INSERT IGNORE INTO `Statusy` (`Nazwa`, `TypStatusu`, `Kolejnosc`, `Kolor`) VALUES
('Nieprzypisany', 'StanProduktu', 0, '#D3D3D3'),
('Nowy / Nieużywany', 'StanProduktu', 1, '#00FF00'),
('Używany - Stan Dobry', 'StanProduktu', 2, '#90EE90'),
('Używany - Stan Zadowalający', 'StanProduktu', 3, '#FFFF00'),
('Używany - Stan Zły', 'StanProduktu', 4, '#FFA500'),
('Uszkodzony', 'StanProduktu', 5, '#FF0000'),
('Niekompletny', 'StanProduktu', 6, '#FF69B4'),
('Brak produktu w przesyłce', 'StanProduktu', 7, '#8B0000');

-- Decyzje handlowca (co zrobić ze zwrotem)
INSERT IGNORE INTO `Statusy` (`Nazwa`, `TypStatusu`, `Kolejnosc`, `Kolor`) VALUES
('Nieprzypisany', 'DecyzjaHandlowca', 0, '#D3D3D3'),
('Zwrot pieniędzy - Pełna kwota', 'DecyzjaHandlowca', 1, '#32CD32'),
('Zwrot pieniędzy - Częściowy', 'DecyzjaHandlowca', 2, '#90EE90'),
('Wymiana na nowy produkt', 'DecyzjaHandlowca', 3, '#4169E1'),
('Naprawa gwarancyjna', 'DecyzjaHandlowca', 4, '#FFD700'),
('Odrzucenie zwrotu', 'DecyzjaHandlowca', 5, '#FF0000'),
('Do dalszej analizy', 'DecyzjaHandlowca', 6, '#FFA500'),
('Przekazanie do producenta', 'DecyzjaHandlowca', 7, '#800080');

-- ============================================================================
-- CZĘŚĆ 3: TABELA ALLEGROCUSTOMERRETURNS - KOMPLETNA STRUKTURA
-- ============================================================================

CREATE TABLE IF NOT EXISTS `AllegroCustomerReturns` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    
    -- Dane podstawowe z Allegro
    `AllegroReturnId` VARCHAR(100) NOT NULL UNIQUE COMMENT 'ID zwrotu w Allegro',
    `AllegroAccountId` INT NOT NULL COMMENT 'Konto Allegro',
    `ReferenceNumber` VARCHAR(50) COMMENT 'Numer referencyjny zwrotu',
    `OrderId` VARCHAR(100) COMMENT 'ID zamówienia',
    `BuyerLogin` VARCHAR(100) COMMENT 'Login kupującego',
    `BuyerEmail` VARCHAR(255) COMMENT 'Email kupującego',
    `CreatedAt` DATETIME COMMENT 'Data utworzenia zwrotu',
    
    -- Status Allegro
    `StatusAllegro` VARCHAR(50) COMMENT 'Status w Allegro: CREATED, IN_TRANSIT, DELIVERED, etc.',
    
    -- Dane przesyłki
    `Waybill` VARCHAR(100) COMMENT 'Numer listu przewozowego',
    `TransportingWaybill` VARCHAR(100) COMMENT 'Numer listu zwrotnego',
    `CarrierName` VARCHAR(100) COMMENT 'Nazwa przewoźnika',
    `TransportingCarrierId` VARCHAR(100) COMMENT 'ID przewoźnika zwrotnego',
    `SenderPhoneNumber` VARCHAR(50) COMMENT 'Telefon nadawcy',
    
    -- Dane produktu
    `ProductName` VARCHAR(500) COMMENT 'Nazwa produktu',
    `OfferId` VARCHAR(100) COMMENT 'ID oferty Allegro',
    `Quantity` INT DEFAULT 1 COMMENT 'Ilość sztuk',
    `ProductPrice` DECIMAL(10,2) COMMENT 'Cena produktu',
    `ProductPriceCurrency` VARCHAR(10) DEFAULT 'PLN',
    
    -- Powód zwrotu
    `ReturnReasonType` VARCHAR(100) COMMENT 'Typ powodu zwrotu',
    `ReturnReasonComment` TEXT COMMENT 'Komentarz kupującego',
    
    -- Dane płatności (z zamówienia)
    `PaymentType` VARCHAR(50),
    `PaymentProvider` VARCHAR(50),
    `PaymentFinishedAt` DATETIME,
    `PaidAmount` DECIMAL(10,2),
    `FulfillmentStatus` VARCHAR(50),
    
    -- Adres dostawy
    `Delivery_FirstName` VARCHAR(100),
    `Delivery_LastName` VARCHAR(100),
    `Delivery_Street` VARCHAR(255),
    `Delivery_ZipCode` VARCHAR(20),
    `Delivery_City` VARCHAR(100),
    `Delivery_PhoneNumber` VARCHAR(50),
    `Delivery_CountryCode` VARCHAR(10) DEFAULT 'PL',
    `Delivery_CompanyName` VARCHAR(255),
    `DeliveryMethod` VARCHAR(100),
    `DeliveryMethodId` VARCHAR(100),
    
    -- Adres kupującego
    `Buyer_FirstName` VARCHAR(100),
    `Buyer_LastName` VARCHAR(100),
    `Buyer_Street` VARCHAR(255),
    `Buyer_ZipCode` VARCHAR(20),
    `Buyer_City` VARCHAR(100),
    `Buyer_PhoneNumber` VARCHAR(50),
    `Buyer_CountryCode` VARCHAR(10) DEFAULT 'PL',
    `Buyer_CompanyName` VARCHAR(255),
    
    -- Dane faktury
    `Invoice_CompanyName` VARCHAR(255),
    `Invoice_TaxId` VARCHAR(50),
    `Invoice_Street` VARCHAR(255),
    `Invoice_ZipCode` VARCHAR(20),
    `Invoice_City` VARCHAR(100),
    `Invoice_CountryCode` VARCHAR(10) DEFAULT 'PL',
    `InvoiceRequired` TINYINT(1) DEFAULT 0,
    `InvoiceNumber` VARCHAR(100),
    
    -- Statusy wewnętrzne (NOWE KOLUMNY!)
    `StatusWewnetrznyId` INT DEFAULT NULL COMMENT 'FK do Statusy (TypStatusu=StatusWewnetrzny)',
    `StanProduktuId` INT DEFAULT NULL COMMENT 'FK do Statusy (TypStatusu=StanProduktu)',
    `DecyzjaHandlowcaId` INT DEFAULT NULL COMMENT 'FK do Statusy (TypStatusu=DecyzjaHandlowca)',
    
    -- Notatki i uwagi
    `UwagiMagazyn` TEXT COMMENT 'Uwagi pracownika magazynu',
    `UwagiHandlowiec` TEXT COMMENT 'Uwagi handlowca',
    
    -- Powiązanie z systemem zgłoszeń
    `ZgloszenieId` INT DEFAULT NULL COMMENT 'Powiązane zgłoszenie w systemie',
    
    -- Metadata
    `MarketplaceId` VARCHAR(50) DEFAULT 'allegro-pl',
    `BoughtAt` DATETIME COMMENT 'Data zakupu',
    `JsonDetails` LONGTEXT COMMENT 'Pełne dane JSON z Allegro (zwrot)',
    `OrderJsonDetails` LONGTEXT COMMENT 'Pełne dane JSON z Allegro (zamówienie)',
    `LastSyncAt` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `CreatedInSystemAt` DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    -- Klucze obce
    FOREIGN KEY (`AllegroAccountId`) REFERENCES `AllegroAccounts`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`StatusWewnetrznyId`) REFERENCES `Statusy`(`Id`) ON DELETE SET NULL,
    FOREIGN KEY (`StanProduktuId`) REFERENCES `Statusy`(`Id`) ON DELETE SET NULL,
    FOREIGN KEY (`DecyzjaHandlowcaId`) REFERENCES `Statusy`(`Id`) ON DELETE SET NULL,
    FOREIGN KEY (`ZgloszenieId`) REFERENCES `Zgloszenia`(`Id`) ON DELETE SET NULL,
    
    -- Indeksy
    INDEX `idx_allegro_return_id` (`AllegroReturnId`),
    INDEX `idx_allegro_account` (`AllegroAccountId`),
    INDEX `idx_reference_number` (`ReferenceNumber`),
    INDEX `idx_order_id` (`OrderId`),
    INDEX `idx_buyer_login` (`BuyerLogin`),
    INDEX `idx_waybill` (`Waybill`),
    INDEX `idx_status_allegro` (`StatusAllegro`),
    INDEX `idx_status_wewnetrzny` (`StatusWewnetrznyId`),
    INDEX `idx_stan_produktu` (`StanProduktuId`),
    INDEX `idx_decyzja_handlowca` (`DecyzjaHandlowcaId`),
    INDEX `idx_created_at` (`CreatedAt`),
    INDEX `idx_buyer_email` (`BuyerEmail`),
    INDEX `idx_zgloszenie` (`ZgloszenieId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- CZĘŚĆ 4: MIGRACJA DANYCH (jeśli tabela już istniała)
-- ============================================================================

-- Dodaj brakujące kolumny jeśli tabela już istnieje
ALTER TABLE `AllegroCustomerReturns` 
ADD COLUMN IF NOT EXISTS `StatusWewnetrznyId` INT DEFAULT NULL AFTER `InvoiceNumber`,
ADD COLUMN IF NOT EXISTS `StanProduktuId` INT DEFAULT NULL AFTER `StatusWewnetrznyId`,
ADD COLUMN IF NOT EXISTS `DecyzjaHandlowcaId` INT DEFAULT NULL AFTER `StanProduktuId`,
ADD COLUMN IF NOT EXISTS `UwagiMagazyn` TEXT AFTER `DecyzjaHandlowcaId`,
ADD COLUMN IF NOT EXISTS `UwagiHandlowiec` TEXT AFTER `UwagiMagazyn`,
ADD COLUMN IF NOT EXISTS `ZgloszenieId` INT DEFAULT NULL AFTER `UwagiHandlowiec`;

-- Dodaj klucze obce jeśli nie istnieją
-- ALTER TABLE `AllegroCustomerReturns`
-- ADD CONSTRAINT `fk_status_wewnetrzny` FOREIGN KEY (`StatusWewnetrznyId`) REFERENCES `Statusy`(`Id`) ON DELETE SET NULL,
-- ADD CONSTRAINT `fk_stan_produktu` FOREIGN KEY (`StanProduktuId`) REFERENCES `Statusy`(`Id`) ON DELETE SET NULL,
-- ADD CONSTRAINT `fk_decyzja_handlowca` FOREIGN KEY (`DecyzjaHandlowcaId`) REFERENCES `Statusy`(`Id`) ON DELETE SET NULL;

-- Ustaw domyślne statusy dla istniejących zwrotów (jeśli są)
UPDATE `AllegroCustomerReturns` acr
SET `StatusWewnetrznyId` = (
    SELECT Id FROM `Statusy` 
    WHERE Nazwa = 'Oczekuje na przyjęcie' 
    AND TypStatusu = 'StatusWewnetrzny' 
    LIMIT 1
)
WHERE `StatusWewnetrznyId` IS NULL 
AND `StatusAllegro` = 'DELIVERED';

UPDATE `AllegroCustomerReturns` acr
SET `StatusWewnetrznyId` = (
    SELECT Id FROM `Statusy` 
    WHERE Nazwa = 'Oczekuje na przyjęcie' 
    AND TypStatusu = 'StatusWewnetrzny' 
    LIMIT 1
)
WHERE `StatusWewnetrznyId` IS NULL;

-- ============================================================================
-- CZĘŚĆ 5: TABELA MAGAZYNDZIENNIK
-- ============================================================================

CREATE TABLE IF NOT EXISTS `MagazynDziennik` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Data` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `Uzytkownik` VARCHAR(255) NOT NULL,
    `Akcja` TEXT NOT NULL,
    `DotyczyZwrotuId` INT DEFAULT NULL,
    FOREIGN KEY (`DotyczyZwrotuId`) REFERENCES `AllegroCustomerReturns`(`Id`) ON DELETE SET NULL,
    INDEX `idx_data` (`Data`),
    INDEX `idx_zwrot` (`DotyczyZwrotuId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- CZĘŚĆ 6: TABELA ALLEGRORETORNITEMS (dla zwrotów wieloproduktowych)
-- ============================================================================

CREATE TABLE IF NOT EXISTS `AllegroReturnItems` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `ReturnId` INT NOT NULL COMMENT 'FK do AllegroCustomerReturns',
    `OfferId` VARCHAR(100) COMMENT 'ID oferty Allegro',
    `ProductName` VARCHAR(500) COMMENT 'Nazwa produktu',
    `Quantity` INT DEFAULT 1,
    `Price` DECIMAL(10,2),
    `Currency` VARCHAR(10) DEFAULT 'PLN',
    `ReasonType` VARCHAR(100) COMMENT 'Powód zwrotu tego produktu',
    `ReasonComment` TEXT,
    `ProductUrl` VARCHAR(500),
    `JsonDetails` TEXT,
    FOREIGN KEY (`ReturnId`) REFERENCES `AllegroCustomerReturns`(`Id`) ON DELETE CASCADE,
    INDEX `idx_return` (`ReturnId`),
    INDEX `idx_offer` (`OfferId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- CZĘŚĆ 7: WERYFIKACJA
-- ============================================================================

-- Sprawdź strukturę tabel
SELECT 'Tabela Statusy utworzona' AS Status;
SELECT COUNT(*) AS 'Liczba statusów' FROM `Statusy`;

SELECT 'Tabela AllegroCustomerReturns zaktualizowana' AS Status;
SHOW COLUMNS FROM `AllegroCustomerReturns` LIKE '%Status%';

SET FOREIGN_KEY_CHECKS = 1;

-- ============================================================================
-- KONIEC SKRYPTU
-- ============================================================================
