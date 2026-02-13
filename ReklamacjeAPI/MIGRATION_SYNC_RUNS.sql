-- ============================================
-- MIGRATION: SyncRuns table
-- Tabela przechowuje historię uruchomień synchronizacji
-- API zapisuje wynik po każdym cyklu BackgroundService
-- WinForms może czytać status bez łączenia się z API
-- ============================================

CREATE TABLE IF NOT EXISTS SyncRuns (
    id            BIGINT AUTO_INCREMENT PRIMARY KEY,
    source        VARCHAR(32)  NOT NULL COMMENT 'ALLEGRO, GOOGLE, DPD, EMAIL',
    started_at    DATETIME     NOT NULL,
    finished_at   DATETIME     NULL,
    ok            TINYINT(1)   NOT NULL DEFAULT 0,
    rows_written  INT          NOT NULL DEFAULT 0,
    error_message TEXT         NULL,
    details       TEXT         NULL COMMENT 'JSON z dodatkowymi danymi, np. UnregisteredCount',
    INDEX idx_source_started (source, started_at DESC)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
