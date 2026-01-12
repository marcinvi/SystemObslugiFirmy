-- ============================================================================
-- SUPER KOMPLETNY SKRYPT - WSZYSTKIE BRAKUJĄCE TABELE
-- Data: 2026-01-07
-- Wersja: FINALNA - 100% KOMPLETNA
-- ============================================================================

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ============================================================================
-- TABELA 1: Statusy (statusy zwrotów)
-- ============================================================================
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

-- Wstaw statusy domyślne
INSERT IGNORE INTO `Statusy` (`Nazwa`, `TypStatusu`, `Kolejnosc`, `Kolor`) VALUES
-- Statusy wewnętrzne
('Oczekuje na przyjęcie', 'StatusWewnetrzny', 1, '#FFA500'),
('Przyjęty do magazynu', 'StatusWewnetrzny', 2, '#4169E1'),
('W trakcie weryfikacji', 'StatusWewnetrzny', 3, '#FFD700'),
('Oczekuje na decyzję handlowca', 'StatusWewnetrzny', 4, '#FF6347'),
('Zakończony', 'StatusWewnetrzny', 5, '#32CD32'),
('Anulowany', 'StatusWewnetrzny', 6, '#808080'),
('Archiwalny', 'StatusWewnetrzny', 7, '#696969'),
-- Stany produktu
('Nieprzypisany', 'StanProduktu', 0, '#D3D3D3'),
('Nowy / Nieużywany', 'StanProduktu', 1, '#00FF00'),
('Używany - Stan Dobry', 'StanProduktu', 2, '#90EE90'),
('Używany - Stan Zadowalający', 'StanProduktu', 3, '#FFFF00'),
('Używany - Stan Zły', 'StanProduktu', 4, '#FFA500'),
('Uszkodzony', 'StanProduktu', 5, '#FF0000'),
('Niekompletny', 'StanProduktu', 6, '#FF69B4'),
('Brak produktu w przesyłce', 'StanProduktu', 7, '#8B0000'),
-- Decyzje handlowca
('Nieprzypisany', 'DecyzjaHandlowca', 0, '#D3D3D3'),
('Zwrot pieniędzy - Pełna kwota', 'DecyzjaHandlowca', 1, '#32CD32'),
('Zwrot pieniędzy - Częściowy', 'DecyzjaHandlowca', 2, '#90EE90'),
('Wymiana na nowy produkt', 'DecyzjaHandlowca', 3, '#4169E1'),
('Naprawa gwarancyjna', 'DecyzjaHandlowca', 4, '#FFD700'),
('Odrzucenie zwrotu', 'DecyzjaHandlowca', 5, '#FF0000'),
('Do dalszej analizy', 'DecyzjaHandlowca', 6, '#FFA500'),
('Przekazanie do producenta', 'DecyzjaHandlowca', 7, '#800080'),
('Przekaż do reklamacji', 'DecyzjaHandlowca', 8, '#9370DB');

-- ============================================================================
-- TABELA 2: MagazynDziennik (dziennik akcji w magazynie)
-- ============================================================================
CREATE TABLE IF NOT EXISTS `MagazynDziennik` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Data` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `Uzytkownik` VARCHAR(255) NOT NULL,
    `Akcja` TEXT NOT NULL,
    `DotyczyZwrotuId` INT DEFAULT NULL,
    INDEX `idx_data` (`Data`),
    INDEX `idx_zwrot` (`DotyczyZwrotuId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA 3: Wiadomosci (system wiadomości wewnętrznych)
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Wiadomosci` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `NadawcaId` INT NOT NULL,
    `OdbiorcaId` INT NOT NULL,
    `Tytul` VARCHAR(500),
    `Tresc` TEXT NOT NULL,
    `DataWyslania` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `CzyPrzeczytana` TINYINT(1) DEFAULT 0,
    `CzyOdpowiedziano` TINYINT(1) DEFAULT 0,
    `DotyczyZwrotuId` INT DEFAULT NULL,
    `ParentMessageId` INT DEFAULT NULL COMMENT 'ID wiadomości nadrzędnej (dla wątków)',
    INDEX `idx_nadawca` (`NadawcaId`),
    INDEX `idx_odbiorca` (`OdbiorcaId`),
    INDEX `idx_data` (`DataWyslania`),
    INDEX `idx_przeczytana` (`CzyPrzeczytana`),
    INDEX `idx_zwrot` (`DotyczyZwrotuId`),
    INDEX `idx_parent` (`ParentMessageId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA 4: AllegroAccountOpiekun (przypisanie opiekunów do kont Allegro)
-- ============================================================================
CREATE TABLE IF NOT EXISTS `AllegroAccountOpiekun` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `AllegroAccountId` INT NOT NULL,
    `OpiekunId` INT NOT NULL COMMENT 'ID użytkownika (handlowca)',
    `DataPrzypisania` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `CzyAktywny` TINYINT(1) DEFAULT 1,
    UNIQUE KEY `idx_account_opiekun` (`AllegroAccountId`, `OpiekunId`),
    INDEX `idx_account` (`AllegroAccountId`),
    INDEX `idx_opiekun` (`OpiekunId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA 5: Delegacje (delegacje handlowców na zastępców)
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Delegacje` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `UzytkownikId` INT NOT NULL COMMENT 'ID handlowca, który deleguje',
    `ZastepcaId` INT NOT NULL COMMENT 'ID zastępcy',
    `DataOd` DATE NOT NULL,
    `DataDo` DATE NOT NULL,
    `Powod` VARCHAR(500),
    `CzyAktywna` TINYINT(1) DEFAULT 1,
    `DataUtworzenia` DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX `idx_uzytkownik` (`UzytkownikId`),
    INDEX `idx_zastepca` (`ZastepcaId`),
    INDEX `idx_aktywna` (`CzyAktywna`),
    INDEX `idx_daty` (`DataOd`, `DataDo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA 6: ZwrotDzialania (historia działań na zwrotach)
-- ============================================================================
CREATE TABLE IF NOT EXISTS `ZwrotDzialania` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `ZwrotId` INT NOT NULL,
    `Data` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `Uzytkownik` VARCHAR(255) NOT NULL,
    `Tresc` TEXT NOT NULL,
    INDEX `idx_zwrot` (`ZwrotId`),
    INDEX `idx_data` (`Data`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA 7: AllegroReturnItems (produkty w zwrotach wieloproduktowych)
-- ============================================================================
CREATE TABLE IF NOT EXISTS `AllegroReturnItems` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `ReturnId` INT NOT NULL COMMENT 'FK do AllegroCustomerReturns',
    `OfferId` VARCHAR(100),
    `ProductName` VARCHAR(500),
    `Quantity` INT DEFAULT 1,
    `Price` DECIMAL(10,2),
    `Currency` VARCHAR(10) DEFAULT 'PLN',
    `ReasonType` VARCHAR(100),
    `ReasonComment` TEXT,
    `ProductUrl` VARCHAR(500),
    `JsonDetails` TEXT,
    INDEX `idx_return` (`ReturnId`),
    INDEX `idx_offer` (`OfferId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA 8: AllegroCustomerReturns - DODANIE BRAKUJĄCYCH KOLUMN
-- ============================================================================

-- Sprawdź czy tabela istnieje
SET @table_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                     WHERE TABLE_SCHEMA = DATABASE() 
                     AND TABLE_NAME = 'AllegroCustomerReturns');

-- Jeśli tabela istnieje, dodaj brakujące kolumny
SET @dbname = DATABASE();

-- StatusWewnetrznyId
SET @col_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = @dbname 
                   AND TABLE_NAME = 'AllegroCustomerReturns' 
                   AND COLUMN_NAME = 'StatusWewnetrznyId');
SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN StatusWewnetrznyId INT DEFAULT NULL',
    'SELECT "StatusWewnetrznyId already exists" AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- StanProduktuId
SET @col_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = @dbname 
                   AND TABLE_NAME = 'AllegroCustomerReturns' 
                   AND COLUMN_NAME = 'StanProduktuId');
SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN StanProduktuId INT DEFAULT NULL',
    'SELECT "StanProduktuId already exists" AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- DecyzjaHandlowcaId
SET @col_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = @dbname 
                   AND TABLE_NAME = 'AllegroCustomerReturns' 
                   AND COLUMN_NAME = 'DecyzjaHandlowcaId');
SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN DecyzjaHandlowcaId INT DEFAULT NULL',
    'SELECT "DecyzjaHandlowcaId already exists" AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- UwagiMagazyn
SET @col_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = @dbname 
                   AND TABLE_NAME = 'AllegroCustomerReturns' 
                   AND COLUMN_NAME = 'UwagiMagazyn');
SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN UwagiMagazyn TEXT',
    'SELECT "UwagiMagazyn already exists" AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- UwagiHandlowiec
SET @col_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = @dbname 
                   AND TABLE_NAME = 'AllegroCustomerReturns' 
                   AND COLUMN_NAME = 'UwagiHandlowiec');
SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN UwagiHandlowiec TEXT',
    'SELECT "UwagiHandlowiec already exists" AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ZgloszenieId
SET @col_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = @dbname 
                   AND TABLE_NAME = 'AllegroCustomerReturns' 
                   AND COLUMN_NAME = 'ZgloszenieId');
SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN ZgloszenieId INT DEFAULT NULL',
    'SELECT "ZgloszenieId already exists" AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- DataPrzyjecia
SET @col_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = @dbname 
                   AND TABLE_NAME = 'AllegroCustomerReturns' 
                   AND COLUMN_NAME = 'DataPrzyjecia');
SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN DataPrzyjecia DATETIME',
    'SELECT "DataPrzyjecia already exists" AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- PrzyjetyPrzezId
SET @col_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = @dbname 
                   AND TABLE_NAME = 'AllegroCustomerReturns' 
                   AND COLUMN_NAME = 'PrzyjetyPrzezId');
SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN PrzyjetyPrzezId INT DEFAULT NULL',
    'SELECT "PrzyjetyPrzezId already exists" AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- KomentarzHandlowca
SET @col_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = @dbname 
                   AND TABLE_NAME = 'AllegroCustomerReturns' 
                   AND COLUMN_NAME = 'KomentarzHandlowca');
SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN KomentarzHandlowca TEXT',
    'SELECT "KomentarzHandlowca already exists" AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- DataDecyzji
SET @col_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_SCHEMA = @dbname 
                   AND TABLE_NAME = 'AllegroCustomerReturns' 
                   AND COLUMN_NAME = 'DataDecyzji');
SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN DataDecyzji DATETIME',
    'SELECT "DataDecyzji already exists" AS Info');
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- ============================================================================
-- USTAW DOMYŚLNE STATUSY DLA ISTNIEJĄCYCH ZWROTÓW
-- ============================================================================
UPDATE AllegroCustomerReturns acr
SET StatusWewnetrznyId = (
    SELECT Id FROM Statusy 
    WHERE Nazwa = 'Oczekuje na przyjęcie' 
    AND TypStatusu = 'StatusWewnetrzny' 
    LIMIT 1
)
WHERE StatusWewnetrznyId IS NULL 
AND StatusAllegro = 'DELIVERED';

-- ============================================================================
-- WERYFIKACJA FINALNA
-- ============================================================================
SET FOREIGN_KEY_CHECKS = 1;

SELECT '========================================' AS '';
SELECT '✅ WERYFIKACJA TABEL' AS '';
SELECT '========================================' AS '';

SELECT 
    'Statusy' AS Tabela,
    COUNT(*) AS LiczbaRekordow,
    CASE WHEN COUNT(*) >= 22 THEN '✅ OK' ELSE '⚠️ BRAK DANYCH' END AS Status
FROM Statusy

UNION ALL

SELECT 
    'MagazynDziennik' AS Tabela,
    COUNT(*) AS LiczbaRekordow,
    '✅ OK (może być pusta)' AS Status
FROM MagazynDziennik

UNION ALL

SELECT 
    'Wiadomosci' AS Tabela,
    COUNT(*) AS LiczbaRekordow,
    '✅ OK (może być pusta)' AS Status
FROM Wiadomosci

UNION ALL

SELECT 
    'AllegroAccountOpiekun' AS Tabela,
    COUNT(*) AS LiczbaRekordow,
    '✅ OK (może być pusta)' AS Status
FROM AllegroAccountOpiekun

UNION ALL

SELECT 
    'Delegacje' AS Tabela,
    COUNT(*) AS LiczbaRekordow,
    '✅ OK (może być pusta)' AS Status
FROM Delegacje

UNION ALL

SELECT 
    'ZwrotDzialania' AS Tabela,
    COUNT(*) AS LiczbaRekordow,
    '✅ OK (może być pusta)' AS Status
FROM ZwrotDzialania

UNION ALL

SELECT 
    'AllegroReturnItems' AS Tabela,
    COUNT(*) AS LiczbaRekordow,
    '✅ OK (może być pusta)' AS Status
FROM AllegroReturnItems;

SELECT '========================================' AS '';
SELECT '✅ KOLUMNY W AllegroCustomerReturns' AS '';
SELECT '========================================' AS '';

SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
AND TABLE_NAME = 'AllegroCustomerReturns'
AND COLUMN_NAME IN ('StatusWewnetrznyId', 'StanProduktuId', 'DecyzjaHandlowcaId', 'UwagiMagazyn', 'UwagiHandlowiec', 'DataPrzyjecia', 'PrzyjetyPrzezId', 'KomentarzHandlowca', 'DataDecyzji')
ORDER BY COLUMN_NAME;

SELECT '========================================' AS '';
SELECT '✅✅✅ WSZYSTKO GOTOWE! ✅✅✅' AS '';
SELECT 'Teraz możesz uruchomić aplikację!' AS '';
SELECT '========================================' AS '';
