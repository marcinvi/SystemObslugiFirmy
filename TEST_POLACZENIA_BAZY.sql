-- ============================================================================
-- SKRYPT DIAGNOSTYCZNY BAZY DANYCH
-- ============================================================================
-- Ten skrypt sprawdzi i naprawi podstawowe problemy z bazą danych
-- Uruchom go w MySQL Workbench lub phpMyAdmin
-- ============================================================================

-- KROK 1: Sprawdź czy jesteś połączony
SELECT 'Połączenie OK!' AS status, VERSION() AS wersja_mysql;

-- KROK 2: Utwórz bazę danych (jeśli nie istnieje)
CREATE DATABASE IF NOT EXISTS ReklamacjeDB 
    CHARACTER SET utf8mb4 
    COLLATE utf8mb4_unicode_ci;

-- KROK 3: Użyj bazy danych
USE ReklamacjeDB;

-- KROK 4: Sprawdź istniejące tabele
SELECT 'Sprawdzam tabele...' AS krok;
SHOW TABLES;

-- KROK 5: Sprawdź czy istnieją kluczowe tabele
SELECT 
    'AllegroAccounts' AS tabela,
    CASE 
        WHEN COUNT(*) > 0 THEN '✅ Istnieje'
        ELSE '❌ BRAK - wykonaj migrację!'
    END AS status
FROM information_schema.tables
WHERE table_schema = 'ReklamacjeDB' 
AND table_name = 'AllegroAccounts'

UNION ALL

SELECT 
    'AllegroCustomerReturns' AS tabela,
    CASE 
        WHEN COUNT(*) > 0 THEN '✅ Istnieje'
        ELSE '❌ BRAK - wykonaj migrację!'
    END AS status
FROM information_schema.tables
WHERE table_schema = 'ReklamacjeDB' 
AND table_name = 'AllegroCustomerReturns'

UNION ALL

SELECT 
    'AllegroDisputes' AS tabela,
    CASE 
        WHEN COUNT(*) > 0 THEN '✅ Istnieje'
        ELSE '❌ BRAK - wykonaj migrację!'
    END AS status
FROM information_schema.tables
WHERE table_schema = 'ReklamacjeDB' 
AND table_name = 'AllegroDisputes'

UNION ALL

SELECT 
    'AllegroReturnItems' AS tabela,
    CASE 
        WHEN COUNT(*) > 0 THEN '✅ Istnieje'
        ELSE '❌ BRAK - wykonaj migrację!'
    END AS status
FROM information_schema.tables
WHERE table_schema = 'ReklamacjeDB' 
AND table_name = 'AllegroReturnItems'

UNION ALL

SELECT 
    'AllegroSyncLog' AS tabela,
    CASE 
        WHEN COUNT(*) > 0 THEN '✅ Istnieje'
        ELSE '❌ BRAK - wykonaj migrację!'
    END AS status
FROM information_schema.tables
WHERE table_schema = 'ReklamacjeDB' 
AND table_name = 'AllegroSyncLog';

-- KROK 6: Jeśli tabele nie istnieją, wyświetl instrukcję
SELECT 
    CASE 
        WHEN (SELECT COUNT(*) FROM information_schema.tables 
              WHERE table_schema = 'ReklamacjeDB' 
              AND table_name IN ('AllegroAccounts', 'AllegroCustomerReturns', 'AllegroDisputes')) < 3
        THEN 'UWAGA: Brakuje tabel! Wykonaj MIGRACJA_BAZY_ALLEGRO_V2.sql'
        ELSE 'OK: Wszystkie kluczowe tabele istnieją'
    END AS instrukcja;

-- ============================================================================
-- JEŚLI WSZYSTKO OK, ZOBACZYSZ:
-- ============================================================================
-- ✅ Połączenie OK!
-- ✅ Baza ReklamacjeDB istnieje
-- ✅ Wszystkie kluczowe tabele istnieją
-- ============================================================================

-- ============================================================================
-- JEŚLI WIDZISZ BŁĘDY:
-- ============================================================================
-- 1. "Access denied" → Sprawdź user/password w DbConfig.cs
-- 2. "Unknown database" → Wykonaj: CREATE DATABASE ReklamacjeDB;
-- 3. "❌ BRAK" przy tabelach → Wykonaj MIGRACJA_BAZY_ALLEGRO_V2.sql
-- ============================================================================
