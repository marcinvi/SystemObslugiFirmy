-- =============================================
-- INDEKSY DLA WYSZUKIWARKI ZGŁOSZEŃ
-- Uruchom RAZ w HeidiSQL / phpMyAdmin / DBeaver
-- =============================================

-- 1. Główny JOIN: Zgloszenia → klienci (po KlientID)
CREATE INDEX IF NOT EXISTS idx_zgloszenia_klientid 
    ON Zgloszenia(KlientID);

-- 2. Główny JOIN: Zgloszenia → Produkty (po ProduktID)
CREATE INDEX IF NOT EXISTS idx_zgloszenia_produktid 
    ON Zgloszenia(ProduktID);

-- 3. JOIN: Produkty → Producenci (po nazwie producenta)
CREATE INDEX IF NOT EXISTS idx_produkty_producent 
    ON Produkty(Producent);

-- 4. Subquery GROUP_CONCAT: dzialania grupowane po NrZgloszenia
CREATE INDEX IF NOT EXISTS idx_dzialania_nrzgloszenia 
    ON dzialania(NrZgloszenia);

-- 5. Sortowanie wyników po dacie
CREATE INDEX IF NOT EXISTS idx_zgloszenia_datazgloszenia 
    ON Zgloszenia(DataZgloszenia DESC);

-- 6. Filtr w dzialania: WHERE Tresc IS NOT NULL AND Tresc != ''
CREATE INDEX IF NOT EXISTS idx_dzialania_nrzgloszenia_tresc 
    ON dzialania(NrZgloszenia, Tresc(1));

-- Sprawdź aktualne indeksy:
-- SHOW INDEX FROM Zgloszenia;
-- SHOW INDEX FROM dzialania;
-- SHOW INDEX FROM Produkty;
-- SHOW INDEX FROM klienci;
