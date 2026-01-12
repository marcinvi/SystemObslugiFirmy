-- === MIGRACJE: przypomnienia, synchronizacja, indeksy (2025-08-31) ===

BEGIN TRANSACTION;

-- 1) Tabela dziennika synchronizacji
CREATE TABLE IF NOT EXISTS SyncRuns (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    source TEXT NOT NULL,                     -- 'ALLEGRO', 'DPD', 'GOOGLE', 'LOCAL'
    started_at DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    finished_at DATETIME,
    ok INTEGER NOT NULL DEFAULT 0,            -- 0/1
    rows_fetched INTEGER DEFAULT 0,
    rows_written INTEGER DEFAULT 0,
    details TEXT,
    error_message TEXT
);
CREATE INDEX IF NOT EXISTS idx_syncruns_source_started ON SyncRuns(source, started_at DESC);

-- 2) Rozszerzenia tabeli Przypomnienia
-- Jeśli tabela nie istnieje, tworzymy ją kompletną.
CREATE TABLE IF NOT EXISTS Przypomnienia (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    ticket_id TEXT,                    -- numer zgłoszenia / klucz sprawy
    source TEXT NOT NULL,              -- 'AUTO','DPD','MANUAL'
    category TEXT NOT NULL,            -- 'decision','courier','manual'
    title TEXT NOT NULL,
    message TEXT,
    due_at DATETIME,                   -- termin docelowy (SLA, dostawa, itp.)
    next_run_at DATETIME,              -- kiedy ponownie przypomnieć
    snoozed_until DATETIME,            -- snooze; nie generuj i nie pinguj przed tą datą
    repeat_interval_days INTEGER,      -- dla cyklicznych
    status TEXT NOT NULL DEFAULT 'pending',   -- 'pending','done','cancelled','snoozed','error'
    dedupe_key TEXT,                   -- unikalny klucz „co to reprezentuje” (np. 'DPD|{waybill}')
    created_at DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    updated_at DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);

-- Dla istniejącej tabeli dokładamy brakujące kolumny (bez błędu jeśli są).
ALTER TABLE Przypomnienia ADD COLUMN source TEXT DEFAULT 'AUTO';
ALTER TABLE Przypomnienia ADD COLUMN category TEXT DEFAULT 'manual';
ALTER TABLE Przypomnienia ADD COLUMN due_at DATETIME;
ALTER TABLE Przypomnienia ADD COLUMN next_run_at DATETIME;
ALTER TABLE Przypomnienia ADD COLUMN snoozed_until DATETIME;
ALTER TABLE Przypomnienia ADD COLUMN repeat_interval_days INTEGER;
ALTER TABLE Przypomnienia ADD COLUMN status TEXT DEFAULT 'pending';
ALTER TABLE Przypomnienia ADD COLUMN dedupe_key TEXT;
ALTER TABLE Przypomnienia ADD COLUMN created_at DATETIME DEFAULT (CURRENT_TIMESTAMP);
ALTER TABLE Przypomnienia ADD COLUMN updated_at DATETIME DEFAULT (CURRENT_TIMESTAMP);

-- 3) Triggery aktualizacji updated_at
DROP TRIGGER IF EXISTS trg_przypomnienia_updated;
CREATE TRIGGER IF NOT EXISTS trg_przypomnienia_updated
AFTER UPDATE ON Przypomnienia
FOR EACH ROW BEGIN
    UPDATE Przypomnienia SET updated_at=CURRENT_TIMESTAMP WHERE id=OLD.id;
END;

-- 4) Indeksy
CREATE INDEX IF NOT EXISTS idx_przypomnienia_status ON Przypomnienia(status);
CREATE INDEX IF NOT EXISTS idx_przypomnienia_due ON Przypomnienia(due_at);
CREATE INDEX IF NOT EXISTS idx_przypomnienia_next_run ON Przypomnienia(next_run_at);
CREATE INDEX IF NOT EXISTS idx_przypomnienia_ticket ON Przypomnienia(ticket_id);
CREATE INDEX IF NOT EXISTS idx_przypomnienia_dedupe ON Przypomnienia(dedupe_key);

-- 5) Unikalność aktywnych przypomnień po dedupe_key (nie duplikuj tego samego „stanu”)
-- W SQLite można użyć indeksu częściowego:
CREATE UNIQUE INDEX IF NOT EXISTS uidx_przypomnienia_active_dedupe
ON Przypomnienia(dedupe_key)
WHERE status='pending';

COMMIT;
