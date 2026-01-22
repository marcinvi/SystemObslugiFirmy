-- ============================================
-- FIX: Dodanie brakujących kolumn do Klienci
-- Data: 2026-01-22
-- Błąd: Unknown column 'Adres' in 'INSERT INTO'
-- ============================================

USE ReklamacjeDB;

-- Helper: add column only if missing
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = 'Klienci'
  AND COLUMN_NAME = 'Adres';

SET @query = IF(@col_exists = 0,
    'ALTER TABLE Klienci ADD COLUMN Adres VARCHAR(200) NULL AFTER Email',
    'SELECT "Kolumna Adres już istnieje" AS Info');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = 'Klienci'
  AND COLUMN_NAME = 'KodPocztowy';

SET @query = IF(@col_exists = 0,
    'ALTER TABLE Klienci ADD COLUMN KodPocztowy VARCHAR(10) NULL AFTER Adres',
    'SELECT "Kolumna KodPocztowy już istnieje" AS Info');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = 'Klienci'
  AND COLUMN_NAME = 'Miasto';

SET @query = IF(@col_exists = 0,
    'ALTER TABLE Klienci ADD COLUMN Miasto VARCHAR(100) NULL AFTER KodPocztowy',
    'SELECT "Kolumna Miasto już istnieje" AS Info');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = 'Klienci'
  AND COLUMN_NAME = 'DataDodania';

SET @query = IF(@col_exists = 0,
    'ALTER TABLE Klienci ADD COLUMN DataDodania DATETIME NULL DEFAULT CURRENT_TIMESTAMP AFTER Miasto',
    'SELECT "Kolumna DataDodania już istnieje" AS Info');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = 'Klienci'
  AND COLUMN_NAME = 'Uwagi';

SET @query = IF(@col_exists = 0,
    'ALTER TABLE Klienci ADD COLUMN Uwagi TEXT NULL AFTER DataDodania',
    'SELECT "Kolumna Uwagi już istnieje" AS Info');
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
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = 'Klienci'
ORDER BY ORDINAL_POSITION;
