-- Sprawdzenie wszystkich indeksów w bazie
USE ReklamacjeDB;

SELECT
    TABLE_NAME AS 'Tabela',
    INDEX_NAME AS 'Nazwa indeksu',
    GROUP_CONCAT(COLUMN_NAME ORDER BY SEQ_IN_INDEX) AS 'Kolumny',
    NON_UNIQUE AS 'Nie-unikalny',
    INDEX_TYPE AS 'Typ'
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA = 'ReklamacjeDB'
GROUP BY TABLE_NAME, INDEX_NAME, NON_UNIQUE, INDEX_TYPE
ORDER BY TABLE_NAME, INDEX_NAME;
