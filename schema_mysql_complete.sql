-- ============================================================================
-- KOMPLETNY SCHEMAT BAZY DANYCH MYSQL/MARIADB
-- System Obsługi Reklamacji
-- Data utworzenia: 2025-01-07
-- ============================================================================

-- Ustawienia początkowe
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ============================================================================
-- TABELA: Uzytkownicy
-- Opis: Użytkownicy systemu
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Uzytkownicy` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Login` VARCHAR(100) NOT NULL UNIQUE,
    `Haslo` VARCHAR(255) NOT NULL,
    `Nazwa Wyświetlana` VARCHAR(255) NOT NULL,
    `Email` VARCHAR(255),
    `Rola` VARCHAR(50) DEFAULT 'User',
    `CzyAktywny` TINYINT(1) DEFAULT 1,
    `DataUtworzenia` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `OstatnieLogowanie` DATETIME,
    INDEX `idx_login` (`Login`),
    INDEX `idx_aktywny` (`CzyAktywny`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: Klienci
-- Opis: Dane klientów (osób prywatnych i firm)
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Klienci` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `ImieNazwisko` VARCHAR(255),
    `NazwaFirmy` VARCHAR(255),
    `NIP` VARCHAR(20),
    `Ulica` VARCHAR(255),
    `KodPocztowy` VARCHAR(10),
    `Miejscowosc` VARCHAR(255),
    `Email` VARCHAR(255),
    `Telefon` VARCHAR(50),
    `DataDodania` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `Uwagi` TEXT,
    INDEX `idx_email` (`Email`),
    INDEX `idx_telefon` (`Telefon`),
    INDEX `idx_nip` (`NIP`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: Produkty
-- Opis: Katalog produktów
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Produkty` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Nazwa` VARCHAR(255) NOT NULL,
    `Producent` VARCHAR(255),
    `Model` VARCHAR(255),
    `KodEnova` VARCHAR(100),
    `KodProducenta` VARCHAR(100),
    `Kategoria` VARCHAR(100),
    `Opis` TEXT,
    `DataDodania` DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX `idx_nazwa` (`Nazwa`),
    INDEX `idx_producent` (`Producent`),
    INDEX `idx_kod_enova` (`KodEnova`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: StatusyOgolne
-- Opis: Słownik statusów ogólnych zgłoszeń
-- ============================================================================
CREATE TABLE IF NOT EXISTS `StatusyOgolne` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `NazwaStatusu` VARCHAR(100) NOT NULL UNIQUE,
    `Kolejnosc` INT DEFAULT 0,
    `CzyAktywny` TINYINT(1) DEFAULT 1,
    INDEX `idx_kolejnosc` (`Kolejnosc`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: Zgloszenia
-- Opis: Główna tabela zgłoszeń reklamacyjnych
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Zgloszenia` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `NrZgloszenia` VARCHAR(50) NOT NULL UNIQUE,
    `KlientID` INT,
    `ProduktID` INT,
    `DataZgloszenia` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `DataZakupu` DATE,
    `NrFaktury` VARCHAR(100),
    `NrSeryjny` VARCHAR(100),
    `Usterka` TEXT,
    `OpisUsterki` TEXT,
    `StatusOgolny` VARCHAR(100) DEFAULT 'Nowe',
    `StatusDpd` VARCHAR(100),
    `PrzypisanyDo` INT,
    `DataZamkniecia` DATETIME,
    `Uwagi` TEXT,
    `allegroDisputeId` VARCHAR(100),
    `allegroAccountId` INT,
    `allegroOrderId` VARCHAR(100),
    `allegroBuyerLogin` VARCHAR(100),
    `DataModyfikacji` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (`KlientID`) REFERENCES `Klienci`(`Id`) ON DELETE SET NULL,
    FOREIGN KEY (`ProduktID`) REFERENCES `Produkty`(`Id`) ON DELETE SET NULL,
    FOREIGN KEY (`PrzypisanyDo`) REFERENCES `Uzytkownicy`(`Id`) ON DELETE SET NULL,
    INDEX `idx_nr_zgloszenia` (`NrZgloszenia`),
    INDEX `idx_klient` (`KlientID`),
    INDEX `idx_produkt` (`ProduktID`),
    INDEX `idx_status` (`StatusOgolny`),
    INDEX `idx_data_zgloszenia` (`DataZgloszenia`),
    INDEX `idx_allegro_dispute` (`allegroDisputeId`),
    INDEX `idx_allegro_account` (`allegroAccountId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: Dzialania
-- Opis: Historia działań podejmowanych w ramach zgłoszeń
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Dzialania` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `ZgloszenieID` INT NOT NULL,
    `TypDzialania` VARCHAR(100),
    `Opis` TEXT,
    `DataDzialania` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `UzytkownikID` INT,
    `StatusPrzed` VARCHAR(100),
    `StatusPo` VARCHAR(100),
    `CzyWyslanoPowiadomienie` TINYINT(1) DEFAULT 0,
    FOREIGN KEY (`ZgloszenieID`) REFERENCES `Zgloszenia`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`UzytkownikID`) REFERENCES `Uzytkownicy`(`Id`) ON DELETE SET NULL,
    INDEX `idx_zgloszenie` (`ZgloszenieID`),
    INDEX `idx_data` (`DataDzialania`),
    INDEX `idx_typ` (`TypDzialania`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: KontaPocztowe
-- Opis: Konfiguracja kont pocztowych dla komunikacji z klientami
-- ============================================================================
CREATE TABLE IF NOT EXISTS `KontaPocztowe` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `NazwaWyswietlana` VARCHAR(255),
    `AdresEmail` VARCHAR(255) NOT NULL,
    `Login` VARCHAR(255),
    `Haslo` VARCHAR(500) NOT NULL,
    `Protokol` VARCHAR(50) DEFAULT 'POP3',
    `Pop3Host` VARCHAR(255),
    `Pop3Port` INT DEFAULT 110,
    `Pop3Ssl` TINYINT(1) DEFAULT 0,
    `ImapHost` VARCHAR(255),
    `ImapPort` INT DEFAULT 143,
    `ImapSsl` TINYINT(1) DEFAULT 0,
    `SmtpHost` VARCHAR(255),
    `SmtpPort` INT DEFAULT 587,
    `SmtpSsl` TINYINT(1) DEFAULT 1,
    `PodpisHtml` LONGTEXT,
    `CzyDomyslne` TINYINT(1) DEFAULT 0,
    `CzyAktywne` TINYINT(1) DEFAULT 1,
    INDEX `idx_email` (`AdresEmail`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: EmailLog
-- Opis: Historia wysłanych i odebranych emaili
-- ============================================================================
CREATE TABLE IF NOT EXISTS `EmailLog` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `ZgloszenieID` INT,
    `KlientID` INT,
    `MessageId` VARCHAR(255),
    `Nadawca` VARCHAR(255),
    `Odbiorca` VARCHAR(255),
    `Temat` VARCHAR(500),
    `Tresc` LONGTEXT,
    `TrescHtml` LONGTEXT,
    `Data` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `Kierunek` VARCHAR(10), -- 'IN' lub 'OUT'
    `KontoPoczntoweID` INT,
    `CzyPrzeczytane` TINYINT(1) DEFAULT 0,
    `Zalaczniki` TEXT,
    FOREIGN KEY (`ZgloszenieID`) REFERENCES `Zgloszenia`(`Id`) ON DELETE SET NULL,
    FOREIGN KEY (`KlientID`) REFERENCES `Klienci`(`Id`) ON DELETE SET NULL,
    FOREIGN KEY (`KontoPoczntoweID`) REFERENCES `KontaPocztowe`(`Id`) ON DELETE SET NULL,
    INDEX `idx_zgloszenie` (`ZgloszenieID`),
    INDEX `idx_klient` (`KlientID`),
    INDEX `idx_data` (`Data`),
    INDEX `idx_kierunek` (`Kierunek`),
    INDEX `idx_message_id` (`MessageId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: SmsLog
-- Opis: Historia wysłanych i odebranych SMS-ów
-- ============================================================================
CREATE TABLE IF NOT EXISTS `SmsLog` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `ZgloszenieID` INT,
    `KlientID` INT,
    `NumerTelefonu` VARCHAR(50),
    `Tresc` TEXT,
    `Data` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `Kierunek` VARCHAR(10), -- 'IN' lub 'OUT'
    `Status` VARCHAR(50),
    `MessageId` VARCHAR(255),
    FOREIGN KEY (`ZgloszenieID`) REFERENCES `Zgloszenia`(`Id`) ON DELETE SET NULL,
    FOREIGN KEY (`KlientID`) REFERENCES `Klienci`(`Id`) ON DELETE SET NULL,
    INDEX `idx_zgloszenie` (`ZgloszenieID`),
    INDEX `idx_klient` (`KlientID`),
    INDEX `idx_numer` (`NumerTelefonu`),
    INDEX `idx_data` (`Data`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: AllegroAccounts
-- Opis: Konfiguracja kont Allegro
-- ============================================================================
CREATE TABLE IF NOT EXISTS `AllegroAccounts` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `AccountName` VARCHAR(255) NOT NULL,
    `ClientId` VARCHAR(255) NOT NULL,
    `ClientSecretEncrypted` TEXT NOT NULL,
    `AccessTokenEncrypted` TEXT,
    `RefreshTokenEncrypted` TEXT,
    `TokenExpirationDate` VARCHAR(50),
    `IsAuthorized` TINYINT(1) DEFAULT 0,
    `LastSyncDate` DATETIME,
    `DataDodania` DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX `idx_account_name` (`AccountName`),
    INDEX `idx_authorized` (`IsAuthorized`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: AllegroDisputes
-- Opis: Zgłoszenia reklamacyjne z Allegro
-- ============================================================================
CREATE TABLE IF NOT EXISTS `AllegroDisputes` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `DisputeId` VARCHAR(100) NOT NULL UNIQUE,
    `AccountId` INT NOT NULL,
    `ReferenceNumber` VARCHAR(100),
    `Type` VARCHAR(50),
    `Status` VARCHAR(50),
    `Subject` VARCHAR(500),
    `BuyerLogin` VARCHAR(255),
    `ProductName` VARCHAR(500),
    `OrderId` VARCHAR(100),
    `OpenedDate` DATETIME,
    `DecisionDueDate` DATETIME,
    `BoughtAt` DATETIME,
    `Description` TEXT,
    `ReasonType` VARCHAR(100),
    `ReasonDescription` TEXT,
    `ExpectedRefundAmount` VARCHAR(50),
    `ExpectedRefundCurrency` VARCHAR(10),
    `LastMessageCount` INT DEFAULT 0,
    `HasNewMessages` TINYINT(1) DEFAULT 0,
    `IsRegistered` TINYINT(1) DEFAULT 0,
    `ZgloszenieId` INT,
    `LastSyncDate` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (`AccountId`) REFERENCES `AllegroAccounts`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`ZgloszenieId`) REFERENCES `Zgloszenia`(`Id`) ON DELETE SET NULL,
    INDEX `idx_dispute_id` (`DisputeId`),
    INDEX `idx_account` (`AccountId`),
    INDEX `idx_status` (`Status`),
    INDEX `idx_registered` (`IsRegistered`),
    INDEX `idx_new_messages` (`HasNewMessages`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: AllegroChatMessages
-- Opis: Wiadomości czatu z Allegro
-- ============================================================================
CREATE TABLE IF NOT EXISTS `AllegroChatMessages` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `MessageId` VARCHAR(100) NOT NULL,
    `DisputeId` VARCHAR(100) NOT NULL,
    `AuthorLogin` VARCHAR(255),
    `AuthorRole` VARCHAR(50),
    `MessageText` TEXT,
    `CreatedAt` DATETIME,
    `HasAttachments` TINYINT(1) DEFAULT 0,
    UNIQUE KEY `unique_message` (`MessageId`, `DisputeId`),
    INDEX `idx_dispute` (`DisputeId`),
    INDEX `idx_created` (`CreatedAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: Przypomnienia
-- Opis: System przypomnień i powiadomień
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Przypomnienia` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `ticket_id` VARCHAR(100),
    `source` VARCHAR(50) NOT NULL DEFAULT 'MANUAL', -- 'AUTO','DPD','MANUAL'
    `category` VARCHAR(50) NOT NULL DEFAULT 'manual', -- 'decision','courier','manual'
    `title` VARCHAR(500) NOT NULL,
    `message` TEXT,
    `due_at` DATETIME,
    `next_run_at` DATETIME,
    `snoozed_until` DATETIME,
    `repeat_interval_days` INT,
    `status` VARCHAR(50) NOT NULL DEFAULT 'pending', -- 'pending','done','cancelled','snoozed','error'
    `dedupe_key` VARCHAR(255),
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX `idx_status` (`status`),
    INDEX `idx_due` (`due_at`),
    INDEX `idx_next_run` (`next_run_at`),
    INDEX `idx_ticket` (`ticket_id`),
    INDEX `idx_dedupe` (`dedupe_key`),
    UNIQUE KEY `uidx_active_dedupe` (`dedupe_key`, `status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: SyncRuns
-- Opis: Dziennik operacji synchronizacji
-- ============================================================================
CREATE TABLE IF NOT EXISTS `SyncRuns` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `source` VARCHAR(50) NOT NULL, -- 'ALLEGRO', 'DPD', 'GOOGLE', 'LOCAL'
    `started_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `finished_at` DATETIME,
    `ok` TINYINT(1) DEFAULT 0,
    `rows_fetched` INT DEFAULT 0,
    `rows_written` INT DEFAULT 0,
    `details` TEXT,
    `error_message` TEXT,
    INDEX `idx_source_started` (`source`, `started_at` DESC)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: Wiadomosci
-- Opis: Wiadomości wewnętrzne między użytkownikami
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Wiadomosci` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `NadawcaId` INT NOT NULL,
    `OdbiorcaId` INT NOT NULL,
    `Tytul` VARCHAR(500),
    `Tresc` TEXT,
    `DataWyslania` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `CzyPrzeczytana` TINYINT(1) DEFAULT 0,
    `CzyOdpowiedziano` TINYINT(1) DEFAULT 0,
    `DotyczyZwrotuId` INT,
    `ParentMessageId` INT,
    FOREIGN KEY (`NadawcaId`) REFERENCES `Uzytkownicy`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`OdbiorcaId`) REFERENCES `Uzytkownicy`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`ParentMessageId`) REFERENCES `Wiadomosci`(`Id`) ON DELETE SET NULL,
    INDEX `idx_nadawca` (`NadawcaId`),
    INDEX `idx_odbiorca` (`OdbiorcaId`),
    INDEX `idx_data` (`DataWyslania`),
    INDEX `idx_przeczytana` (`CzyPrzeczytana`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: SzablonyEmail
-- Opis: Szablony wiadomości email
-- ============================================================================
CREATE TABLE IF NOT EXISTS `SzablonyEmail` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Nazwa` VARCHAR(255) NOT NULL UNIQUE,
    `Temat` VARCHAR(500),
    `TrescHtml` LONGTEXT,
    `CzyAktywny` TINYINT(1) DEFAULT 1,
    `DataUtworzenia` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `DataModyfikacji` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX `idx_nazwa` (`Nazwa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: SzablonyDzialan
-- Opis: Szablony akcji/działań
-- ============================================================================
CREATE TABLE IF NOT EXISTS `SzablonyDzialan` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Nazwa` VARCHAR(255) NOT NULL UNIQUE,
    `TypDzialania` VARCHAR(100),
    `Opis` TEXT,
    `CzyAktywny` TINYINT(1) DEFAULT 1,
    `DataUtworzenia` DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX `idx_nazwa` (`Nazwa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: Magazyn
-- Opis: Stan magazynowy części zamiennych
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Magazyn` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `NazwaCzesci` VARCHAR(255) NOT NULL,
    `KodCzesci` VARCHAR(100),
    `ProducentID` INT,
    `Ilosc` INT DEFAULT 0,
    `JednostkaMiary` VARCHAR(50) DEFAULT 'szt',
    `MinimalnyStan` INT DEFAULT 0,
    `Lokalizacja` VARCHAR(255),
    `Uwagi` TEXT,
    `DataOstatniejZmiany` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX `idx_nazwa` (`NazwaCzesci`),
    INDEX `idx_kod` (`KodCzesci`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: RuchyMagazynowe
-- Opis: Historia ruchów magazynowych
-- ============================================================================
CREATE TABLE IF NOT EXISTS `RuchyMagazynowe` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `MagazynId` INT NOT NULL,
    `TypRuchu` VARCHAR(50), -- 'PRZYJĘCIE', 'WYDANIE', 'KOREKTA'
    `Ilosc` INT NOT NULL,
    `ZgloszenieId` INT,
    `UzytkownikId` INT,
    `Data` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `Opis` TEXT,
    FOREIGN KEY (`MagazynId`) REFERENCES `Magazyn`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`ZgloszenieId`) REFERENCES `Zgloszenia`(`Id`) ON DELETE SET NULL,
    FOREIGN KEY (`UzytkownikId`) REFERENCES `Uzytkownicy`(`Id`) ON DELETE SET NULL,
    INDEX `idx_magazyn` (`MagazynId`),
    INDEX `idx_data` (`Data`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: DpdPrzesylki
-- Opis: Śledzenie przesyłek DPD
-- ============================================================================
CREATE TABLE IF NOT EXISTS `DpdPrzesylki` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `ZgloszenieId` INT,
    `NumerPrzesylki` VARCHAR(100) NOT NULL UNIQUE,
    `Status` VARCHAR(100),
    `OpisStatusu` TEXT,
    `DataNadania` DATE,
    `DataOstatniegoSledzenia` DATETIME,
    `OczekiwanaDataDostawy` DATE,
    `DataDostarczenia` DATETIME,
    `Odbiorca` VARCHAR(255),
    `StatusSzczegoly` TEXT,
    FOREIGN KEY (`ZgloszenieId`) REFERENCES `Zgloszenia`(`Id`) ON DELETE SET NULL,
    INDEX `idx_numer` (`NumerPrzesylki`),
    INDEX `idx_zgloszenie` (`ZgloszenieId`),
    INDEX `idx_status` (`Status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: Zalaczniki
-- Opis: Pliki załączone do zgłoszeń
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Zalaczniki` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `ZgloszenieId` INT,
    `NazwaPliku` VARCHAR(500),
    `SciezkaPliku` VARCHAR(1000),
    `TypPliku` VARCHAR(100),
    `RozmiarPliku` BIGINT,
    `DataDodania` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `DodanePrzez` INT,
    FOREIGN KEY (`ZgloszenieId`) REFERENCES `Zgloszenia`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`DodanePrzez`) REFERENCES `Uzytkownicy`(`Id`) ON DELETE SET NULL,
    INDEX `idx_zgloszenie` (`ZgloszenieId`),
    INDEX `idx_data` (`DataDodania`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: LogAktywnosci
-- Opis: Dziennik aktywności użytkowników w systemie
-- ============================================================================
CREATE TABLE IF NOT EXISTS `LogAktywnosci` (
    `Id` BIGINT AUTO_INCREMENT PRIMARY KEY,
    `UzytkownikId` INT,
    `Akcja` VARCHAR(255),
    `Opis` TEXT,
    `IpAddress` VARCHAR(50),
    `DataCzas` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `ZgloszenieId` INT,
    FOREIGN KEY (`UzytkownikId`) REFERENCES `Uzytkownicy`(`Id`) ON DELETE SET NULL,
    FOREIGN KEY (`ZgloszenieId`) REFERENCES `Zgloszenia`(`Id`) ON DELETE SET NULL,
    INDEX `idx_uzytkownik` (`UzytkownikId`),
    INDEX `idx_data` (`DataCzas`),
    INDEX `idx_akcja` (`Akcja`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- TABELA: Ustawienia
-- Opis: Konfiguracja systemu
-- ============================================================================
CREATE TABLE IF NOT EXISTS `Ustawienia` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Klucz` VARCHAR(255) NOT NULL UNIQUE,
    `Wartosc` TEXT,
    `Opis` VARCHAR(500),
    `DataModyfikacji` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX `idx_klucz` (`Klucz`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================================
-- WIDOKI
-- ============================================================================

-- Widok: Zgłoszenia z pełnymi danymi
CREATE OR REPLACE VIEW `v_Zgloszenia_Pelne` AS
SELECT 
    z.`Id`,
    z.`NrZgloszenia`,
    z.`DataZgloszenia`,
    z.`StatusOgolny`,
    z.`StatusDpd`,
    k.`ImieNazwisko`,
    k.`NazwaFirmy`,
    k.`Email`,
    k.`Telefon`,
    p.`Nazwa` AS `NazwaProduktu`,
    p.`Producent`,
    u.`Nazwa Wyświetlana` AS `PrzypisanyDo`,
    z.`allegroDisputeId`,
    COALESCE(k.`NazwaFirmy`, k.`ImieNazwisko`) AS `KlientPelnyNazwa`
FROM `Zgloszenia` z
LEFT JOIN `Klienci` k ON z.`KlientID` = k.`Id`
LEFT JOIN `Produkty` p ON z.`ProduktID` = p.`Id`
LEFT JOIN `Uzytkownicy` u ON z.`PrzypisanyDo` = u.`Id`;

-- Widok: Statystyki zgłoszeń
CREATE OR REPLACE VIEW `v_Statystyki_Zgloszen` AS
SELECT 
    `StatusOgolny`,
    COUNT(*) AS `Ilosc`,
    COUNT(CASE WHEN YEAR(`DataZgloszenia`) = YEAR(CURDATE()) THEN 1 END) AS `IloscBiezacyRok`,
    COUNT(CASE WHEN MONTH(`DataZgloszenia`) = MONTH(CURDATE()) AND YEAR(`DataZgloszenia`) = YEAR(CURDATE()) THEN 1 END) AS `IloscBiezacyMiesiac`
FROM `Zgloszenia`
GROUP BY `StatusOgolny`;

-- ============================================================================
-- DANE STARTOWE
-- ============================================================================

-- Statusy ogólne
INSERT IGNORE INTO `StatusyOgolne` (`NazwaStatusu`, `Kolejnosc`) VALUES
('Nowe', 1),
('W trakcie weryfikacji', 2),
('Oczekuje na części', 3),
('W naprawie', 4),
('Do wysyłki', 5),
('Wysłane', 6),
('Zamknięte', 7),
('Odrzucone', 8);

-- Ustawienia domyślne
INSERT IGNORE INTO `Ustawienia` (`Klucz`, `Wartosc`, `Opis`) VALUES
('app_version', '1.0.0', 'Wersja aplikacji'),
('email_notifications', 'true', 'Czy wysyłać powiadomienia email'),
('sms_notifications', 'true', 'Czy wysyłać powiadomienia SMS'),
('allegro_sync_interval', '30', 'Interwał synchronizacji Allegro (sekundy)'),
('dpd_sync_interval', '60', 'Interwał synchronizacji DPD (sekundy)');

-- Przywrócenie sprawdzania kluczy obcych
SET FOREIGN_KEY_CHECKS = 1;

-- ============================================================================
-- KONIEC SCHEMATU
-- ============================================================================
