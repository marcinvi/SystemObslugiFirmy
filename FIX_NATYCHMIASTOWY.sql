-- ============================================================================
-- SUPER SZYBKA NAPRAWA - TYLKO BRAKUJĄCE TABELE
-- Wykonaj to TERAZ jeśli masz błąd "Table doesn't exist"
-- ============================================================================

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- Tabela MagazynDziennik (dziennik akcji w magazynie)
CREATE TABLE IF NOT EXISTS `MagazynDziennik` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Data` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `Uzytkownik` VARCHAR(255) NOT NULL,
    `Akcja` TEXT NOT NULL,
    `DotyczyZwrotuId` INT DEFAULT NULL,
    INDEX `idx_data` (`Data`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabela Statusy (statusy zwrotów)
CREATE TABLE IF NOT EXISTS `Statusy` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Nazwa` VARCHAR(100) NOT NULL,
    `TypStatusu` VARCHAR(50) NOT NULL,
    `Kolejnosc` INT DEFAULT 0,
    `Kolor` VARCHAR(20) DEFAULT NULL,
    `CzyAktywny` TINYINT(1) DEFAULT 1,
    `DataUtworzenia` DATETIME DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY `idx_nazwa_typ` (`Nazwa`, `TypStatusu`),
    INDEX `idx_typ` (`TypStatusu`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Wstaw statusy TYLKO jeśli tabela jest pusta
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
('Przekazanie do producenta', 'DecyzjaHandlowca', 7, '#800080');

-- Dodaj kolumny do AllegroCustomerReturns jeśli nie istnieją
SET @dbname = DATABASE();
SET @tablename = 'AllegroCustomerReturns';

-- StatusWewnetrznyId
SET @colexists = (SELECT COUNT(*) 
                  FROM INFORMATION_SCHEMA.COLUMNS 
                  WHERE TABLE_SCHEMA = @dbname 
                  AND TABLE_NAME = @tablename 
                  AND COLUMN_NAME = 'StatusWewnetrznyId');

SET @query = IF(@colexists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN StatusWewnetrznyId INT DEFAULT NULL',
    'SELECT "StatusWewnetrznyId already exists" AS Info');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- StanProduktuId
SET @colexists = (SELECT COUNT(*) 
                  FROM INFORMATION_SCHEMA.COLUMNS 
                  WHERE TABLE_SCHEMA = @dbname 
                  AND TABLE_NAME = @tablename 
                  AND COLUMN_NAME = 'StanProduktuId');

SET @query = IF(@colexists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN StanProduktuId INT DEFAULT NULL',
    'SELECT "StanProduktuId already exists" AS Info');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- DecyzjaHandlowcaId
SET @colexists = (SELECT COUNT(*) 
                  FROM INFORMATION_SCHEMA.COLUMNS 
                  WHERE TABLE_SCHEMA = @dbname 
                  AND TABLE_NAME = @tablename 
                  AND COLUMN_NAME = 'DecyzjaHandlowcaId');

SET @query = IF(@colexists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN DecyzjaHandlowcaId INT DEFAULT NULL',
    'SELECT "DecyzjaHandlowcaId already exists" AS Info');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Ustaw domyślne statusy dla istniejących zwrotów
UPDATE AllegroCustomerReturns acr
SET StatusWewnetrznyId = (
    SELECT Id FROM Statusy 
    WHERE Nazwa = 'Oczekuje na przyjęcie' 
    AND TypStatusu = 'StatusWewnetrzny' 
    LIMIT 1
)
WHERE StatusWewnetrznyId IS NULL 
AND StatusAllegro = 'DELIVERED';

SET FOREIGN_KEY_CHECKS = 1;

-- Sprawdź czy wszystko działa
SELECT 'Tabela MagazynDziennik' AS Tabela, 
       CASE WHEN COUNT(*) > 0 THEN 'OK ✓' ELSE 'BŁĄD ✗' END AS Status
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = DATABASE() 
AND TABLE_NAME = 'MagazynDziennik'

UNION ALL

SELECT 'Tabela Statusy' AS Tabela,
       CASE WHEN COUNT(*) > 0 THEN 'OK ✓' ELSE 'BŁĄD ✗' END AS Status
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = DATABASE() 
AND TABLE_NAME = 'Statusy'

UNION ALL

SELECT 'Liczba statusów' AS Tabela,
       CONCAT(COUNT(*), ' statusów') AS Status
FROM Statusy;

-- KONIEC
SELECT '✅ GOTOWE! Teraz uruchom aplikację.' AS Komunikat;
