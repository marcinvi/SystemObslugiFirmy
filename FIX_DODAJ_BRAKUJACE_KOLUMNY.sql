-- ============================================================================
-- OSTATECZNA NAPRAWA - DODAJE BRAKUJĄCE KOLUMNY
-- Data: 2026-01-08
-- Ten skrypt sprawdza i dodaje KAŻDĄ brakującą kolumnę!
-- ============================================================================

SET NAMES utf8mb4;

-- ============================================================================
-- KROK 1: NAPRAW AllegroCustomerReturns
-- ============================================================================

-- Sprawdź i dodaj IsManual
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME = 'AllegroCustomerReturns' 
  AND COLUMN_NAME = 'IsManual';

SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN IsManual TINYINT(1) NOT NULL DEFAULT 0',
    'SELECT "Kolumna IsManual już istnieje" AS Info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Sprawdź i dodaj ManualSenderDetails
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME = 'AllegroCustomerReturns' 
  AND COLUMN_NAME = 'ManualSenderDetails';

SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN ManualSenderDetails TEXT',
    'SELECT "Kolumna ManualSenderDetails już istnieje" AS Info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Sprawdź i dodaj HandlowiecOpiekunId
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME = 'AllegroCustomerReturns' 
  AND COLUMN_NAME = 'HandlowiecOpiekunId';

SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN HandlowiecOpiekunId INT',
    'SELECT "Kolumna HandlowiecOpiekunId już istnieje" AS Info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Sprawdź i dodaj DataDecyzji
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME = 'AllegroCustomerReturns' 
  AND COLUMN_NAME = 'DataDecyzji';

SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN DataDecyzji DATETIME',
    'SELECT "Kolumna DataDecyzji już istnieje" AS Info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Sprawdź i dodaj KomentarzHandlowca
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME = 'AllegroCustomerReturns' 
  AND COLUMN_NAME = 'KomentarzHandlowca';

SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN KomentarzHandlowca TEXT',
    'SELECT "Kolumna KomentarzHandlowca już istnieje" AS Info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Sprawdź i dodaj BuyerFullName
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME = 'AllegroCustomerReturns' 
  AND COLUMN_NAME = 'BuyerFullName';

SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN BuyerFullName VARCHAR(500)',
    'SELECT "Kolumna BuyerFullName już istnieje" AS Info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Sprawdź i dodaj InvoiceNumber
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = DATABASE() 
  AND TABLE_NAME = 'AllegroCustomerReturns' 
  AND COLUMN_NAME = 'InvoiceNumber';

SET @sql = IF(@col_exists = 0,
    'ALTER TABLE AllegroCustomerReturns ADD COLUMN InvoiceNumber VARCHAR(100)',
    'SELECT "Kolumna InvoiceNumber już istnieje" AS Info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- ============================================================================
-- WERYFIKACJA
-- ============================================================================

SELECT '========================================' AS '';
SELECT '✅ SPRAWDZAM KOLUMNY' AS '';
SELECT '========================================' AS '';

SELECT 
    COLUMN_NAME AS 'Kolumna',
    DATA_TYPE AS 'Typ',
    IS_NULLABLE AS 'Nullable',
    COLUMN_DEFAULT AS 'Domyślna'
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
AND TABLE_NAME = 'AllegroCustomerReturns'
AND COLUMN_NAME IN (
    'IsManual', 
    'ManualSenderDetails', 
    'HandlowiecOpiekunId', 
    'DataDecyzji', 
    'KomentarzHandlowca',
    'BuyerFullName',
    'InvoiceNumber'
)
ORDER BY COLUMN_NAME;

SELECT '========================================' AS '';

-- Pokaż ile kolumn ma tabela
SELECT 
    COUNT(*) AS 'Liczba kolumn w AllegroCustomerReturns'
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
AND TABLE_NAME = 'AllegroCustomerReturns';

SELECT '========================================' AS '';
SELECT '✅✅✅ GOTOWE! ✅✅✅' AS '';
SELECT '========================================' AS '';
