-- SPRAWDZENIE STANU BAZY DANYCH
-- Wykonaj te zapytania w MySQL Workbench lub innym kliencie

-- 1. Ile issues jest w bazie?
SELECT 
    AllegroAccountId,
    COUNT(*) as IssuesCount,
    MAX(LastCheckedAt) as LastSync
FROM allegrodisputes
GROUP BY AllegroAccountId;

-- 2. Czy kolumna LastMessageId istnieje?
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = 'reklamacjedb' 
  AND TABLE_NAME = 'allegrodisputes' 
  AND COLUMN_NAME = 'LastMessageId';

-- 3. Ostatnie 10 issues w bazie
SELECT 
    DisputeId,
    Type,
    StatusAllegro,
    Subject,
    LastCheckedAt
FROM allegrodisputes
ORDER BY LastCheckedAt DESC
LIMIT 10;

-- 4. Logi synchronizacji
SELECT 
    AllegroAccountId,
    SyncType,
    Status,
    StartedAt,
    CompletedAt,
    TIMESTAMPDIFF(SECOND, StartedAt, CompletedAt) as DurationSeconds,
    ItemsProcessed,
    ItemsNew,
    ErrorMessage
FROM AllegroSyncLog
ORDER BY StartedAt DESC
LIMIT 20;
