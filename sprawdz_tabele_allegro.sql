-- ========================================
-- Skrypt sprawdzający wszystkie tabele Allegro
-- Data: 2026-01-07
-- ========================================

-- 1. SPRAWDŹ WSZYSTKIE TABELE ALLEGRO
SELECT 
    TABLE_NAME as 'Tabela',
    TABLE_ROWS as 'Liczba wierszy (przybliżona)',
    ROUND(((DATA_LENGTH + INDEX_LENGTH) / 1024 / 1024), 2) as 'Rozmiar (MB)',
    CREATE_TIME as 'Data utworzenia'
FROM information_schema.TABLES 
WHERE TABLE_SCHEMA = 'reklamacjedb' 
AND TABLE_NAME LIKE 'Allegro%'
ORDER BY TABLE_NAME;

-- 2. SPRAWDŹ CZY WSZYSTKIE WYMAGANE TABELE ISTNIEJĄ
SELECT 
    'AllegroAccounts' as RequiredTable,
    CASE WHEN EXISTS (
        SELECT 1 FROM information_schema.TABLES 
        WHERE TABLE_SCHEMA = 'reklamacjedb' AND TABLE_NAME = 'AllegroAccounts'
    ) THEN '✅ ISTNIEJE' ELSE '❌ BRAK' END as Status
UNION ALL
SELECT 'AllegroCustomerReturns',
    CASE WHEN EXISTS (
        SELECT 1 FROM information_schema.TABLES 
        WHERE TABLE_SCHEMA = 'reklamacjedb' AND TABLE_NAME = 'AllegroCustomerReturns'
    ) THEN '✅ ISTNIEJE' ELSE '❌ BRAK' END
UNION ALL
SELECT 'AllegroReturnItems',
    CASE WHEN EXISTS (
        SELECT 1 FROM information_schema.TABLES 
        WHERE TABLE_SCHEMA = 'reklamacjedb' AND TABLE_NAME = 'AllegroReturnItems'
    ) THEN '✅ ISTNIEJE' ELSE '❌ BRAK' END
UNION ALL
SELECT 'AllegroDisputes',
    CASE WHEN EXISTS (
        SELECT 1 FROM information_schema.TABLES 
        WHERE TABLE_SCHEMA = 'reklamacjedb' AND TABLE_NAME = 'AllegroDisputes'
    ) THEN '✅ ISTNIEJE' ELSE '❌ BRAK' END
UNION ALL
SELECT 'AllegroChatMessages',
    CASE WHEN EXISTS (
        SELECT 1 FROM information_schema.TABLES 
        WHERE TABLE_SCHEMA = 'reklamacjedb' AND TABLE_NAME = 'AllegroChatMessages'
    ) THEN '✅ ISTNIEJE' ELSE '❌ BRAK' END
UNION ALL
SELECT 'AllegroChatAttachments',
    CASE WHEN EXISTS (
        SELECT 1 FROM information_schema.TABLES 
        WHERE TABLE_SCHEMA = 'reklamacjedb' AND TABLE_NAME = 'AllegroChatAttachments'
    ) THEN '✅ ISTNIEJE' ELSE '❌ BRAK' END
UNION ALL
SELECT 'AllegroSyncLog',
    CASE WHEN EXISTS (
        SELECT 1 FROM information_schema.TABLES 
        WHERE TABLE_SCHEMA = 'reklamacjedb' AND TABLE_NAME = 'AllegroSyncLog'
    ) THEN '✅ ISTNIEJE' ELSE '❌ BRAK' END;

-- 3. SPRAWDŹ SZCZEGÓŁY AllegroReturnItems (JEŚLI ISTNIEJE)
SELECT 
    COLUMN_NAME as 'Kolumna',
    COLUMN_TYPE as 'Typ',
    IS_NULLABLE as 'NULL?',
    COLUMN_KEY as 'Klucz',
    COLUMN_DEFAULT as 'Domyślna',
    COLUMN_COMMENT as 'Opis'
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = 'reklamacjedb'
AND TABLE_NAME = 'AllegroReturnItems'
ORDER BY ORDINAL_POSITION;

-- 4. SPRAWDŹ KLUCZE OBCE AllegroReturnItems
SELECT 
    CONSTRAINT_NAME as 'Nazwa FK',
    COLUMN_NAME as 'Kolumna',
    REFERENCED_TABLE_NAME as 'Tabela nadrzędna',
    REFERENCED_COLUMN_NAME as 'Kolumna nadrzędna'
FROM information_schema.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = 'reklamacjedb'
AND TABLE_NAME = 'AllegroReturnItems'
AND REFERENCED_TABLE_NAME IS NOT NULL;

-- 5. SPRAWDŹ INDEKSY AllegroReturnItems
SELECT 
    INDEX_NAME as 'Indeks',
    COLUMN_NAME as 'Kolumna',
    NON_UNIQUE as 'Czy nieunikalny',
    INDEX_TYPE as 'Typ indeksu'
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA = 'reklamacjedb'
AND TABLE_NAME = 'AllegroReturnItems'
ORDER BY INDEX_NAME, SEQ_IN_INDEX;

-- ========================================
-- KONIEC SKRYPTU
-- ========================================
