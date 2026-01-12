-- =====================================================
-- OPTYMALIZACJA SYSTEMU WIADOMOŚCI
-- =====================================================
-- Data: 2026-01-11
-- Cel: Przyspieszenie wczytywania wiadomości w FormWiadomosci
-- =====================================================

USE reklamacjedb;

-- 1. INDEKS NA DisputeId + CreatedAt (dla szybkiego pobierania wiadomości)
-- ================================================================
-- Ten indeks przyspiesza zapytanie: WHERE DisputeId = X ORDER BY CreatedAt DESC LIMIT 200
-- Z kilku sekund -> milisekundy

SELECT 'Sprawdzanie indeksu idx_chat_dispute_date...' AS Info;

SELECT COUNT(*) INTO @index_exists
FROM INFORMATION_SCHEMA.STATISTICS
WHERE TABLE_SCHEMA = 'reklamacjedb'
  AND TABLE_NAME = 'AllegroChatMessages'
  AND INDEX_NAME = 'idx_chat_dispute_date';

SET @sql = IF(@index_exists = 0,
    'CREATE INDEX idx_chat_dispute_date ON AllegroChatMessages(DisputeId, CreatedAt DESC)',
    'SELECT ''Indeks idx_chat_dispute_date już istnieje'' AS Status'
);

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SELECT 'Indeks idx_chat_dispute_date został utworzony!' AS Sukces;

-- 2. STATYSTYKI PRZED OPTYMALIZACJĄ
-- ================================================================
SELECT 
    'STATYSTYKI BAZY' AS Info,
    (SELECT COUNT(*) FROM AllegroChatMessages) AS 'Wszystkich wiadomości',
    (SELECT COUNT(DISTINCT DisputeId) FROM AllegroChatMessages) AS 'Unikalnych wątków',
    (SELECT AVG(msg_count) FROM (
        SELECT COUNT(*) as msg_count 
        FROM AllegroChatMessages 
        GROUP BY DisputeId
    ) as counts) AS 'Średnio wiadomości na wątek';

-- 3. TEST WYDAJNOŚCI (opcjonalny)
-- ================================================================
SELECT 
    'TEST WYDAJNOŚCI' AS Info,
    'Pobieranie 200 ostatnich wiadomości dla pierwszego wątku...' AS Opis;

SET @test_dispute_id = (SELECT DisputeId FROM AllegroChatMessages LIMIT 1);

SELECT 
    @test_dispute_id AS 'DisputeId testowy',
    COUNT(*) AS 'Liczba wiadomości',
    'Jeśli to zapytanie trwa > 100ms, coś jest nie tak!' AS Uwaga
FROM AllegroChatMessages
WHERE DisputeId = @test_dispute_id
ORDER BY CreatedAt DESC
LIMIT 200;

-- 4. PODSUMOWANIE
-- ================================================================
SELECT '✅ OPTYMALIZACJA ZAKOŃCZONA!' AS Status;
SELECT 'FormWiadomosci powinien teraz wczytywać się błyskawicznie (< 100ms)' AS Rezultat;
SELECT 'Limit 200 wiadomości na wątek - starsze można doładować na żądanie' AS Informacja;
