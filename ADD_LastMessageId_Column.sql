-- ============================================================
-- SQL: Dodanie kolumny LastMessageId do AllegroDisputes
-- ============================================================
-- Kolumna będzie przechowywać ID ostatniej wiadomości z czatu
-- Pozwoli to na szybkie sprawdzanie czy są nowe wiadomości
-- ============================================================

USE reklamacje;

-- Sprawdź czy kolumna LastMessageId istnieje
SET @col_exists = 0;
SELECT COUNT(*) INTO @col_exists 
FROM information_schema.COLUMNS 
WHERE TABLE_SCHEMA = 'reklamacje' 
AND TABLE_NAME = 'AllegroDisputes' 
AND COLUMN_NAME = 'LastMessageId';

SET @query = IF(@col_exists = 0,
    'ALTER TABLE AllegroDisputes ADD COLUMN LastMessageId VARCHAR(100) NULL COMMENT "ID ostatniej wiadomości z czatu (dla optymalizacji synchronizacji)" AFTER LastMessageCount',
    'SELECT "Kolumna LastMessageId już istnieje" AS Info');
PREPARE stmt FROM @query;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Pokaż końcową strukturę
SELECT 
    COLUMN_NAME,
    COLUMN_TYPE,
    IS_NULLABLE,
    COLUMN_COMMENT
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = 'reklamacje'
AND TABLE_NAME = 'AllegroDisputes'
AND COLUMN_NAME IN ('LastMessageCount', 'LastMessageId', 'HasNewMessages')
ORDER BY ORDINAL_POSITION;

SELECT '✅ Kolumna LastMessageId dodana pomyślnie!' AS Status;
