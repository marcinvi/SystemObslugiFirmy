-- ============================================
-- FIX: Dodanie brakujących kolumn do AllegroDisputes
-- Data: 2026-01-08
-- Błąd: Unknown column 'OfferId' in 'SET'
-- ============================================

USE reklamacje;

-- Sprawdź czy kolumna OfferId istnieje (jeśli nie - dodaj)
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM information_schema.COLUMNS 
WHERE TABLE_SCHEMA = 'reklamacje' 
AND TABLE_NAME = 'AllegroDisputes' 
AND COLUMN_NAME = 'OfferId';

SET @query = IF(@col_exists = 0,
    'ALTER TABLE AllegroDisputes ADD COLUMN OfferId VARCHAR(50) NULL AFTER ProductId',
    'SELECT "Kolumna OfferId już istnieje" AS Info');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Sprawdź czy kolumna ProductEAN istnieje (jeśli nie - dodaj)
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM information_schema.COLUMNS 
WHERE TABLE_SCHEMA = 'reklamacje' 
AND TABLE_NAME = 'AllegroDisputes' 
AND COLUMN_NAME = 'ProductEAN';

SET @query = IF(@col_exists = 0,
    'ALTER TABLE AllegroDisputes ADD COLUMN ProductEAN VARCHAR(50) NULL AFTER ProductName',
    'SELECT "Kolumna ProductEAN już istnieje" AS Info');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Sprawdź czy kolumna ProductSKU istnieje (jeśli nie - dodaj)
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM information_schema.COLUMNS 
WHERE TABLE_SCHEMA = 'reklamacje' 
AND TABLE_NAME = 'AllegroDisputes' 
AND COLUMN_NAME = 'ProductSKU';

SET @query = IF(@col_exists = 0,
    'ALTER TABLE AllegroDisputes ADD COLUMN ProductSKU VARCHAR(100) NULL AFTER ProductEAN',
    'SELECT "Kolumna ProductSKU już istnieje" AS Info');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Sprawdź czy kolumna InvoiceNumber istnieje (jeśli nie - dodaj)
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM information_schema.COLUMNS 
WHERE TABLE_SCHEMA = 'reklamacje' 
AND TABLE_NAME = 'AllegroDisputes' 
AND COLUMN_NAME = 'InvoiceNumber';

SET @query = IF(@col_exists = 0,
    'ALTER TABLE AllegroDisputes ADD COLUMN InvoiceNumber VARCHAR(50) NULL AFTER ProductSKU',
    'SELECT "Kolumna InvoiceNumber już istnieje" AS Info');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Sprawdź czy kolumna BuyerEmail istnieje (jeśli nie - dodaj)
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM information_schema.COLUMNS 
WHERE TABLE_SCHEMA = 'reklamacje' 
AND TABLE_NAME = 'AllegroDisputes' 
AND COLUMN_NAME = 'BuyerEmail';

SET @query = IF(@col_exists = 0,
    'ALTER TABLE AllegroDisputes ADD COLUMN BuyerEmail VARCHAR(255) NULL AFTER BuyerLogin',
    'SELECT "Kolumna BuyerEmail już istnieje" AS Info');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Sprawdź czy kolumna ClosedAt istnieje (jeśli nie - dodaj)
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM information_schema.COLUMNS 
WHERE TABLE_SCHEMA = 'reklamacje' 
AND TABLE_NAME = 'AllegroDisputes' 
AND COLUMN_NAME = 'ClosedAt';

SET @query = IF(@col_exists = 0,
    'ALTER TABLE AllegroDisputes ADD COLUMN ClosedAt DATETIME NULL AFTER DecisionDueDate',
    'SELECT "Kolumna ClosedAt już istnieje" AS Info');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Pokaż końcową strukturę tabeli
SELECT 
    COLUMN_NAME,
    COLUMN_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = 'reklamacje'
AND TABLE_NAME = 'AllegroDisputes'
ORDER BY ORDINAL_POSITION;
