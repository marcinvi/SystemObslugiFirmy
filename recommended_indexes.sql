-- =====================================================================
-- ZALECANE INDEKSY DLA OPTYMALIZACJI WYDAJNOŚCI
-- System obsługi reklamacji - 50+ użytkowników
-- =====================================================================
-- UWAGA: Uruchom ten skrypt w HeidiSQL lub MySQL Workbench
-- Przed uruchomieniem sprawdź, które indeksy już istnieją!
-- =====================================================================

USE ReklamacjeDB;

-- =====================================================================
-- 1. SPRAWDZENIE ISTNIEJĄCYCH INDEKSÓW
-- =====================================================================
-- Uruchom najpierw te zapytania, żeby zobaczyć co już istnieje:

SELECT
    TABLE_NAME,
    INDEX_NAME,
    COLUMN_NAME,
    NON_UNIQUE,
    SEQ_IN_INDEX
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA = 'ReklamacjeDB'
ORDER BY TABLE_NAME, INDEX_NAME, SEQ_IN_INDEX;

-- =====================================================================
-- 2. INDEKSY DLA TABELI Zgloszenia
-- =====================================================================

-- Primary key (zazwyczaj już istnieje)
-- ALTER TABLE Zgloszenia ADD PRIMARY KEY (Id);

-- NrZgloszenia - BARDZO WAŻNE (często wyszukiwane)
CREATE INDEX IF NOT EXISTS idx_zgloszenia_nrzgloszenia
    ON Zgloszenia(NrZgloszenia);

-- KlientID - dla JOIN z tabelą Klienci
CREATE INDEX IF NOT EXISTS idx_zgloszenia_klientid
    ON Zgloszenia(KlientID);

-- ProduktID - dla JOIN z tabelą Produkty
CREATE INDEX IF NOT EXISTS idx_zgloszenia_produktid
    ON Zgloszenia(ProduktID);

-- StatusOgolny - często filtrowane po statusie
CREATE INDEX IF NOT EXISTS idx_zgloszenia_statusogolny
    ON Zgloszenia(StatusOgolny);

-- DataZgloszenia - dla sortowania i filtrowania po dacie
CREATE INDEX IF NOT EXISTS idx_zgloszenia_datazgloszenia
    ON Zgloszenia(DataZgloszenia);

-- Złożony indeks dla najczęstszego zapytania: "pokaż zgłoszenia klienta o statusie X"
CREATE INDEX IF NOT EXISTS idx_zgloszenia_klient_status
    ON Zgloszenia(KlientID, StatusOgolny);

-- allegroAccountId - dla integracji z Allegro
CREATE INDEX IF NOT EXISTS idx_zgloszenia_allegroaccountid
    ON Zgloszenia(allegroAccountId);

-- allegroDisputeId - dla integracji z Allegro
CREATE INDEX IF NOT EXISTS idx_zgloszenia_allegrodisputeid
    ON Zgloszenia(allegroDisputeId);

-- =====================================================================
-- 3. INDEKSY DLA TABELI Klienci
-- =====================================================================

-- Primary key
-- ALTER TABLE Klienci ADD PRIMARY KEY (Id);

-- Telefon - BARDZO WAŻNE (wyszukiwanie po numerze telefonu)
CREATE INDEX IF NOT EXISTS idx_klienci_telefon
    ON Klienci(Telefon);

-- Email - dla wyszukiwania i wysyłki emaili
CREATE INDEX IF NOT EXISTS idx_klienci_email
    ON Klienci(Email);

-- ImieNazwisko - dla wyszukiwania po nazwisku
CREATE INDEX IF NOT EXISTS idx_klienci_imienazwisko
    ON Klienci(ImieNazwisko);

-- NazwaFirmy - dla wyszukiwania firm
CREATE INDEX IF NOT EXISTS idx_klienci_nazwafirmy
    ON Klienci(NazwaFirmy);

-- =====================================================================
-- 4. INDEKSY DLA TABELI CentrumKontaktu
-- =====================================================================

-- Primary key
-- ALTER TABLE CentrumKontaktu ADD PRIMARY KEY (Id);

-- KlientID - dla wyświetlania historii kontaktu z klientem
CREATE INDEX IF NOT EXISTS idx_centrumkontaktu_klientid
    ON CentrumKontaktu(KlientID);

-- ZgloszenieID - dla wyświetlania komunikacji w ramach zgłoszenia
CREATE INDEX IF NOT EXISTS idx_centrumkontaktu_zgloszenid
    ON CentrumKontaktu(ZgloszenieID);

-- DataWyslania - dla sortowania chronologicznego
CREATE INDEX IF NOT EXISTS idx_centrumkontaktu_datawyslania
    ON CentrumKontaktu(DataWyslania);

-- Typ - dla filtrowania po typie kontaktu (Email, SMS, itp.)
CREATE INDEX IF NOT EXISTS idx_centrumkontaktu_typ
    ON CentrumKontaktu(Typ);

-- Złożony indeks dla "pokaż kontakty klienta, posortowane po dacie"
CREATE INDEX IF NOT EXISTS idx_centrumkontaktu_klient_data
    ON CentrumKontaktu(KlientID, DataWyslania DESC);

-- =====================================================================
-- 5. INDEKSY DLA TABELI Produkty
-- =====================================================================

-- Primary key
-- ALTER TABLE Produkty ADD PRIMARY KEY (Id);

-- NazwaSystemowa - dla wyszukiwania produktów
CREATE INDEX IF NOT EXISTS idx_produkty_nazwasystemowa
    ON Produkty(NazwaSystemowa);

-- Model - jeśli używane do wyszukiwania
CREATE INDEX IF NOT EXISTS idx_produkty_model
    ON Produkty(Model);

-- =====================================================================
-- 6. INDEKSY DLA TABELI Dzialania
-- =====================================================================

-- Primary key
-- ALTER TABLE Dzialania ADD PRIMARY KEY (Id);

-- ZgloszenieID - dla wyświetlania działań w ramach zgłoszenia
CREATE INDEX IF NOT EXISTS idx_dzialania_zgloszenid
    ON Dzialania(ZgloszenieID);

-- DataDzialania - dla sortowania chronologicznego
CREATE INDEX IF NOT EXISTS idx_dzialania_datadzialania
    ON Dzialania(DataDzialania);

-- Uzytkownik - dla raportów per użytkownik
CREATE INDEX IF NOT EXISTS idx_dzialania_uzytkownik
    ON Dzialania(Uzytkownik);

-- =====================================================================
-- 7. INDEKSY DLA TABELI Magazyn (jeśli istnieje)
-- =====================================================================

-- Sprawdź czy tabela istnieje
-- SHOW TABLES LIKE 'Magazyn';

-- CREATE INDEX IF NOT EXISTS idx_magazyn_produktid
--     ON Magazyn(ProduktID);

-- CREATE INDEX IF NOT EXISTS idx_magazyn_zgloszenid
--     ON Magazyn(ZgloszenieID);

-- =====================================================================
-- 8. INDEKSY DLA TABELI AllegroAccounts
-- =====================================================================

-- Primary key
-- ALTER TABLE AllegroAccounts ADD PRIMARY KEY (Id);

-- accountName - dla wyszukiwania kont
CREATE INDEX IF NOT EXISTS idx_allegroaccounts_accountname
    ON AllegroAccounts(accountName);

-- =====================================================================
-- 9. WERYFIKACJA UTWORZONYCH INDEKSÓW
-- =====================================================================

-- Sprawdź, które indeksy zostały utworzone
SELECT
    TABLE_NAME,
    INDEX_NAME,
    COLUMN_NAME,
    CARDINALITY,
    INDEX_TYPE
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA = 'ReklamacjeDB'
    AND INDEX_NAME LIKE 'idx_%'
ORDER BY TABLE_NAME, INDEX_NAME;

-- =====================================================================
-- 10. ANALIZA WYKORZYSTANIA INDEKSÓW
-- =====================================================================

-- Sprawdź, które indeksy są używane (wymaga włączenia performance_schema)
SELECT
    OBJECT_SCHEMA,
    OBJECT_NAME,
    INDEX_NAME,
    COUNT_READ,
    COUNT_FETCH
FROM performance_schema.table_io_waits_summary_by_index_usage
WHERE OBJECT_SCHEMA = 'ReklamacjeDB'
ORDER BY COUNT_READ DESC;

-- =====================================================================
-- 11. ZAPYTANIA TESTOWE
-- =====================================================================

-- Test 1: Wyszukiwanie zgłoszenia po numerze (powinno użyć idx_zgloszenia_nrzgloszenia)
EXPLAIN SELECT * FROM Zgloszenia WHERE NrZgloszenia = 'RK-2025-00001';

-- Test 2: Zgłoszenia klienta o statusie (powinno użyć idx_zgloszenia_klient_status)
EXPLAIN SELECT * FROM Zgloszenia
WHERE KlientID = 123 AND StatusOgolny = 'W trakcie';

-- Test 3: Wyszukiwanie klienta po telefonie (powinno użyć idx_klienci_telefon)
EXPLAIN SELECT * FROM Klienci WHERE Telefon LIKE '%123456789%';

-- Test 4: Historia kontaktu z klientem (powinno użyć idx_centrumkontaktu_klient_data)
EXPLAIN SELECT * FROM CentrumKontaktu
WHERE KlientID = 123
ORDER BY DataWyslania DESC
LIMIT 50;

-- =====================================================================
-- 12. OPTYMALIZACJA PO UTWORZENIU INDEKSÓW
-- =====================================================================

-- Zaktualizuj statystyki tabel (pomaga optymalizatorowi)
ANALYZE TABLE Zgloszenia;
ANALYZE TABLE Klienci;
ANALYZE TABLE CentrumKontaktu;
ANALYZE TABLE Produkty;
ANALYZE TABLE Dzialania;

-- Zoptymalizuj tabele (defragmentacja)
OPTIMIZE TABLE Zgloszenia;
OPTIMIZE TABLE Klienci;
OPTIMIZE TABLE CentrumKontaktu;
OPTIMIZE TABLE Produkty;
OPTIMIZE TABLE Dzialania;

-- =====================================================================
-- UWAGI KOŃCOWE
-- =====================================================================

/*
WAŻNE:
1. Tworzenie indeksów na dużych tabelach może zająć kilka minut
2. Po utworzeniu indeksów uruchom ANALYZE TABLE dla każdej tabeli
3. Regularnie monitoruj wykorzystanie indeksów (co miesiąc)
4. Usuń nieużywane indeksy (spowalniają INSERT/UPDATE)
5. Nie twórz zbyt wielu indeksów - każdy spowalnia zapisy!

MONITORING:
- Używaj EXPLAIN przed każdym wolnym zapytaniem
- Sprawdzaj performance_schema regularnie
- Włącz slow query log do debugowania

BACKUP:
- Zrób backup bazy PRZED uruchomieniem tego skryptu!
- mysqldump -u root -p ReklamacjeDB > backup_before_indexes.sql
*/

-- Data utworzenia: 2026-01-07
-- Wersja: 1.0
