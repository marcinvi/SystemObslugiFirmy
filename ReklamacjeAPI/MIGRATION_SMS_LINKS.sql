-- ============================================================================
-- MIGRACJA: Nowe tabele dla linków SMS (reklamacyjnych) wysyłanych po rozmowie
-- Data: 2026-02-09
-- Uruchom po MIGRATION_PHONE_API.sql (jeśli tabele phone_events itp. już istnieją)
-- ============================================================================

-- Linki reklamacyjne do wysyłki SMS po zakończeniu rozmowy
CREATE TABLE IF NOT EXISTS phone_sms_links (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(200) NOT NULL COMMENT 'Nazwa wyświetlana na telefonie, np. "Formularz reklamacji"',
    Url VARCHAR(500) NOT NULL COMMENT 'Pełny URL linku wysyłanego w SMS',
    SmsTemplate VARCHAR(500) DEFAULT NULL COMMENT 'Szablon SMS. Użyj {url} jako placeholder. Jeśli null: wysyła sam URL',
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    SortOrder INT NOT NULL DEFAULT 0 COMMENT 'Kolejność wyświetlania (rosnąco)',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_phone_sms_links_active (IsActive, SortOrder)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Log wysłanych linków (historia)
CREATE TABLE IF NOT EXISTS phone_sms_links_log (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserLogin VARCHAR(100) NOT NULL,
    LinkId INT NOT NULL,
    PhoneNumber VARCHAR(50) NOT NULL,
    SentAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Status VARCHAR(20) NOT NULL DEFAULT 'SENT' COMMENT 'SENT, FAILED',
    INDEX idx_sms_links_log_user (UserLogin),
    INDEX idx_sms_links_log_date (SentAt),
    FOREIGN KEY (LinkId) REFERENCES phone_sms_links(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================================================
-- Przykładowe linki (dostosuj URL-e do swoich)
-- ============================================================================
INSERT INTO phone_sms_links (Name, Url, SmsTemplate, IsActive, SortOrder) VALUES
('Formularz reklamacji', 'https://twojafirma.pl/reklamacja', 'Dzień dobry, przesyłamy link do zgłoszenia reklamacji: {url}', 1, 1),
('Formularz zwrotu', 'https://twojafirma.pl/zwrot', 'Dzień dobry, przesyłamy link do zgłoszenia zwrotu: {url}', 1, 2),
('Status zamówienia', 'https://twojafirma.pl/status', 'Dzień dobry, status zamówienia można sprawdzić tutaj: {url}', 1, 3),
('Kontakt e-mail', 'https://twojafirma.pl/kontakt', 'Dzień dobry, zachęcamy do kontaktu mailowego: {url}', 1, 4);

-- ============================================================================
-- Czyszczenie starych logów (opcjonalne - cron/event)
-- ============================================================================
-- DELETE FROM phone_sms_links_log WHERE SentAt < DATE_SUB(NOW(), INTERVAL 90 DAY);
