-- ============================================================================
-- SKRYPT DIAGNOSTYCZNY - Kolumny z spacjami w MySQL
-- Data: 2026-01-08
-- ============================================================================

SELECT '========================================' AS '';
SELECT 'KOLUMNY Z SPACJAMI W NAZWACH' AS '';
SELECT '========================================' AS '';

-- Pokaż wszystkie kolumny z spacjami
SELECT 
    TABLE_NAME AS 'Tabela',
    COLUMN_NAME AS 'Kolumna',
    DATA_TYPE AS 'Typ',
    CONCAT('`', COLUMN_NAME, '`') AS 'Użyj w SQL'
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
AND COLUMN_NAME LIKE '% %'
ORDER BY TABLE_NAME, ORDINAL_POSITION;

SELECT '========================================' AS '';
SELECT 'INSTRUKCJA' AS '';
SELECT '========================================' AS '';

SELECT 'W kodzie C# ZAWSZE używaj backticks:' AS 'Info';
SELECT 'DOBRZE: SELECT `Nazwa Wyświetlana` FROM ...' AS 'Przykład 1';
SELECT 'ŹLE:    SELECT "Nazwa Wyświetlana" FROM ...' AS 'Przykład 2';

SELECT '========================================' AS '';
SELECT 'KOLUMNY WYMAGAJĄCE UWAGI' AS '';
SELECT '========================================' AS '';

-- Pokaż tylko te kolumny które są używane w aplikacji
SELECT 
    'Uzytkownicy' AS Tabela,
    '`Nazwa Wyświetlana`' AS Kolumna,
    'W zapytaniach SELECT/WHERE/ORDER BY' AS Użycie
WHERE EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = 'Uzytkownicy'
    AND COLUMN_NAME = 'Nazwa Wyświetlana'
);

SELECT '========================================' AS '';
SELECT 'TEST ZAPYTANIA' AS '';
SELECT '========================================' AS '';

-- Test czy backticks działają
SELECT 
    Id,
    `Nazwa Wyświetlana` AS NazwaPoprawna
FROM Uzytkownicy
LIMIT 3;

SELECT '========================================' AS '';
SELECT '✅ JEŚLI WIDZISZ IMIONA POWYŻEJ = OK!' AS '';
SELECT '❌ JEŚLI WIDZISZ "Nazwa Wyświetlana" = BŁĄD!' AS '';
SELECT '========================================' AS '';
