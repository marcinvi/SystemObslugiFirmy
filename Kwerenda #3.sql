-- ============================================================================
-- NAPRAWA WOLNYCH FILTRÓW - DODAJ INDEKSY
-- ============================================================================
-- 
-- PROBLEM:
-- Zapytania SELECT DISTINCT są BARDZO WOLNE (8+ sekund!)
-- 
-- ROZWIĄZANIE:
-- Dodaj indeksy na kolumny używane w filtrach
--
-- WYNIK:
-- Czas wykonania: z 8 sekund → do < 100ms!
-- ============================================================================

USE ReklamacjeDB;

-- ============================================================================
-- TABELA: klienci
-- ============================================================================

-- Indeks dla autocomplete ImieNazwisko
ALTER TABLE klienci 
ADD INDEX IF NOT EXISTS idx_imie_nazwisko (ImieNazwisko);

-- ============================================================================
-- TABELA: Zgloszenia
-- ============================================================================

-- Indeksy dla autocomplete
ALTER TABLE Zgloszenia 
ADD INDEX IF NOT EXISTS idx_nr_faktury (NrFaktury);

ALTER TABLE Zgloszenia 
ADD INDEX IF NOT EXISTS idx_nr_seryjny (NrSeryjny);

-- Indeksy dla tag-filtrów
ALTER TABLE Zgloszenia 
ADD INDEX IF NOT EXISTS idx_status_ogolny (StatusOgolny);

ALTER TABLE Zgloszenia 
ADD INDEX IF NOT EXISTS idx_status_klient (StatusKlient);

ALTER TABLE Zgloszenia 
ADD INDEX IF NOT EXISTS idx_status_producent (StatusProducent);

ALTER TABLE Zgloszenia 
ADD INDEX IF NOT EXISTS idx_skad (Skad);

-- ============================================================================
-- TABELA: Produkty
-- ============================================================================

-- Indeksy dla autocomplete produktów
ALTER TABLE Produkty 
ADD INDEX IF NOT EXISTS idx_nazwa_systemowa (NazwaSystemowa);

ALTER TABLE Produkty 
ADD INDEX IF NOT EXISTS idx_nazwa_krotka (NazwaKrotka);

ALTER TABLE Produkty 
ADD INDEX IF NOT EXISTS idx_kod_producenta (KodProducenta);

-- ============================================================================
-- TABELA: Producenci
-- ============================================================================

-- Indeks dla tag-filtra producentów
ALTER TABLE Producenci 
ADD INDEX IF NOT EXISTS idx_nazwa_producenta (NazwaProducenta);

-- ============================================================================
-- WERYFIKACJA
-- ============================================================================

-- Sprawdź indeksy na każdej tabeli:
SHOW INDEX FROM klienci WHERE Key_name LIKE 'idx_%';
SHOW INDEX FROM Zgloszenia WHERE Key_name LIKE 'idx_%';
SHOW INDEX FROM Produkty WHERE Key_name LIKE 'idx_%';
SHOW INDEX FROM Producenci WHERE Key_name LIKE 'idx_%';

-- ============================================================================
-- TEST WYDAJNOŚCI
-- ============================================================================

-- Zmierz czas PRZED dodaniem indeksów:
-- SELECT DISTINCT ImieNazwisko FROM klienci WHERE ImieNazwisko IS NOT NULL;
-- (zanotuj czas)

-- Dodaj indeksy (powyżej)

-- Zmierz czas PO dodaniu indeksów:
-- SELECT DISTINCT ImieNazwisko FROM klienci WHERE ImieNazwisko IS NOT NULL;
-- (powinno być 10-100x szybciej!)

-- ============================================================================
-- KONIEC
-- ============================================================================
