# INSTRUKCJA MIGRACJI DO MYSQL/MARIADB

## Przegląd
Ten dokument opisuje proces migracji systemu obsługi reklamacji do bazy danych MySQL/MariaDB.

## 1. WYMAGANIA WSTĘPNE

### Oprogramowanie
- MySQL 8.0+ lub MariaDB 10.5+
- HeidiSQL, MySQL Workbench lub phpMyAdmin
- Dostęp administratora do serwera baz danych

### Konfiguracja serwera MySQL/MariaDB
```sql
-- Zalecane ustawienia w my.ini lub my.cnf
[mysqld]
character-set-server=utf8mb4
collation-server=utf8mb4_unicode_ci
max_connections=200
innodb_buffer_pool_size=256M
max_allowed_packet=64M
```

## 2. KROK PO KROKU - INSTALACJA

### 2.1. Utworzenie bazy danych
```sql
CREATE DATABASE IF NOT EXISTS ReklamacjeDB 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;
```

### 2.2. Utworzenie użytkownika (opcjonalnie)
```sql
-- Utwórz dedykowanego użytkownika (zamiast używać root)
CREATE USER 'reklamacje_user'@'localhost' IDENTIFIED BY 'TwojeSilneHaslo';
GRANT ALL PRIVILEGES ON ReklamacjeDB.* TO 'reklamacje_user'@'localhost';
FLUSH PRIVILEGES;
```

### 2.3. Import schematu
```sql
-- W MySQL Command Line lub HeidiSQL:
USE ReklamacjeDB;
SOURCE C:/Users/mpaprocki/Desktop/dosql/schema_mysql_complete.sql;
```

Lub za pomocą HeidiSQL:
1. Połącz się z serwerem MySQL/MariaDB
2. Wybierz bazę `ReklamacjeDB`
3. Przejdź do Narzędzia → Load SQL file
4. Wybierz plik `schema_mysql_complete.sql`
5. Kliknij Execute

## 3. AKTUALIZACJA KONFIGURACJI W KODZIE

### 3.1. Plik DbConfig.cs
Upewnij się, że plik `DbConfig.cs` ma poprawne dane połączenia:

```csharp
private const string Server = "localhost";        // Adres serwera
private const string Database = "ReklamacjeDB";   // Nazwa bazy
private const string User = "root";               // Użytkownik (lub 'reklamacje_user')
private const string Password = "TwojeHaslo";     // ZMIEŃ NA SWOJE HASŁO!
```

### 3.2. Weryfikacja połączenia
Po aktualizacji konfiguracji, uruchom aplikację i sprawdź, czy połączenie działa:

```csharp
// Test połączenia
using (var connection = Database.GetNewOpenConnection())
{
    Console.WriteLine("Połączenie udane!");
}
```

## 4. MIGRACJA DANYCH (jeśli przechodzisz z SQLite)

### 4.1. Export danych z SQLite
```bash
# Użyj narzędzia sqlite3 lub DB Browser for SQLite
sqlite3 stara_baza.db .dump > export.sql
```

### 4.2. Konwersja składni SQLite → MySQL
Musisz dostosować składnię:

```sql
-- SQLite:
AUTOINCREMENT
-- MySQL:
AUTO_INCREMENT

-- SQLite:
INTEGER PRIMARY KEY AUTOINCREMENT
-- MySQL:
INT AUTO_INCREMENT PRIMARY KEY

-- SQLite:
DATETIME DEFAULT (CURRENT_TIMESTAMP)
-- MySQL:
DATETIME DEFAULT CURRENT_TIMESTAMP
```

### 4.3. Import danych do MySQL
```sql
-- W MySQL:
USE ReklamacjeDB;
SOURCE export_converted.sql;
```

## 5. WERYFIKACJA PO MIGRACJI

### 5.1. Sprawdzenie tabel
```sql
-- Pokaż wszystkie tabele
SHOW TABLES;

-- Sprawdź strukturę kluczowej tabeli
DESCRIBE Zgloszenia;

-- Policz rekordy
SELECT COUNT(*) FROM Zgloszenia;
```

### 5.2. Sprawdzenie indeksów
```sql
-- Zobacz indeksy dla tabeli
SHOW INDEX FROM Zgloszenia;
```

### 5.3. Sprawdzenie kluczy obcych
```sql
-- Zobacz relacje
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    CONSTRAINT_NAME,
    REFERENCED_TABLE_NAME,
    REFERENCED_COLUMN_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = 'ReklamacjeDB'
AND REFERENCED_TABLE_NAME IS NOT NULL;
```

## 6. OPTYMALIZACJA

### 6.1. Dodaj brakujące indeksy (jeśli potrzeba)
```sql
-- Przykłady dodatkowych indeksów dla wydajności:
CREATE INDEX idx_zgloszenia_data_status ON Zgloszenia(DataZgloszenia, StatusOgolny);
CREATE INDEX idx_dzialania_composite ON Dzialania(ZgloszenieID, DataDzialania);
```

### 6.2. Analiza zapytań
```sql
-- Włącz slow query log w my.ini:
slow_query_log = 1
slow_query_log_file = /var/log/mysql/slow-query.log
long_query_time = 2
```

## 7. BACKUP I BEZPIECZEŃSTWO

### 7.1. Automatyczne kopie zapasowe
```bash
# Skrypt backup (Windows .bat)
@echo off
set BACKUP_PATH=C:\Backups\MySQL
set DATE=%date:~-4%%date:~3,2%%date:~0,2%
set TIME=%time:~0,2%%time:~3,2%
mysqldump -u root -p ReklamacjeDB > %BACKUP_PATH%\backup_%DATE%_%TIME%.sql
```

### 7.2. Uprawnienia użytkowników
```sql
-- Minimalne uprawnienia dla aplikacji
GRANT SELECT, INSERT, UPDATE, DELETE ON ReklamacjeDB.* TO 'app_user'@'localhost';

-- Oddzielny użytkownik tylko do odczytu (raporty)
CREATE USER 'readonly_user'@'localhost' IDENTIFIED BY 'HasloReadOnly';
GRANT SELECT ON ReklamacjeDB.* TO 'readonly_user'@'localhost';
```

## 8. ROZWIĄZYWANIE PROBLEMÓW

### Problem: Błąd połączenia "Access denied"
**Rozwiązanie:**
```sql
-- Sprawdź użytkowników
SELECT user, host FROM mysql.user;

-- Jeśli potrzeba, zresetuj hasło
ALTER USER 'root'@'localhost' IDENTIFIED BY 'NoweHaslo';
FLUSH PRIVILEGES;
```

### Problem: Błąd "max_connections"
**Rozwiązanie:**
```sql
-- Zwiększ limit w my.ini/my.cnf:
max_connections=300

-- Lub tymczasowo:
SET GLOBAL max_connections=300;
```

### Problem: Wolne zapytania
**Rozwiązanie:**
```sql
-- Analizuj query plan:
EXPLAIN SELECT * FROM Zgloszenia WHERE StatusOgolny='Nowe';

-- Dodaj brakujące indeksy
CREATE INDEX idx_status ON Zgloszenia(StatusOgolny);
```

### Problem: Błędy kodowania znaków
**Rozwiązanie:**
```sql
-- Sprawdź kodowanie
SHOW VARIABLES LIKE 'character_set%';

-- Ustaw dla bazy:
ALTER DATABASE ReklamacjeDB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Ustaw dla tabeli:
ALTER TABLE Zgloszenia CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

## 9. TESTOWANIE APLIKACJI

### 9.1. Lista kontrolna testów
- [ ] Logowanie do systemu
- [ ] Tworzenie nowego zgłoszenia
- [ ] Edycja zgłoszenia
- [ ] Dodawanie działań
- [ ] Wysyłanie emaili
- [ ] Synchronizacja Allegro
- [ ] Śledzenie DPD
- [ ] Wyszukiwanie zgłoszeń
- [ ] Generowanie raportów
- [ ] Eksport danych

### 9.2. Testy wydajności
```sql
-- Test szybkości zapytań:
SELECT SQL_NO_CACHE COUNT(*) FROM Zgloszenia; -- powinno < 100ms

-- Test joinów:
SELECT z.NrZgloszenia, k.ImieNazwisko, p.Nazwa
FROM Zgloszenia z
JOIN Klienci k ON z.KlientID = k.Id
JOIN Produkty p ON z.ProduktID = p.Id
WHERE z.DataZgloszenia > DATE_SUB(NOW(), INTERVAL 30 DAY);
```

## 10. MAINTENANCE

### 10.1. Regularne zadania
```sql
-- Co tydzień - optymalizuj tabele
OPTIMIZE TABLE Zgloszenia, Dzialania, EmailLog;

-- Co miesiąc - sprawdź i napraw
CHECK TABLE Zgloszenia;
REPAIR TABLE Zgloszenia;

-- Czyszczenie starych logów (starszych niż rok)
DELETE FROM LogAktywnosci WHERE DataCzas < DATE_SUB(NOW(), INTERVAL 1 YEAR);
```

### 10.2. Monitoring
```sql
-- Rozmiar bazy danych
SELECT 
    table_schema AS 'Database',
    ROUND(SUM(data_length + index_length) / 1024 / 1024, 2) AS 'Size (MB)'
FROM information_schema.tables
WHERE table_schema = 'ReklamacjeDB'
GROUP BY table_schema;

-- Rozmiar poszczególnych tabel
SELECT 
    table_name AS 'Table',
    ROUND(((data_length + index_length) / 1024 / 1024), 2) AS 'Size (MB)'
FROM information_schema.tables
WHERE table_schema = 'ReklamacjeDB'
ORDER BY (data_length + index_length) DESC;
```

## 11. BEZPIECZEŃSTWO

### 11.1. Szyfrowanie połączenia (SSL)
```sql
-- W my.ini:
[mysqld]
require_secure_transport=ON
ssl-ca=/path/to/ca.pem
ssl-cert=/path/to/server-cert.pem
ssl-key=/path/to/server-key.pem
```

### 11.2. Szyfrowanie haseł w aplikacji
Upewnij się, że hasła są szyfrowane przed zapisem do bazy:
```csharp
// Użyj EncryptionHelper.cs
string encryptedPassword = SecurityHelper.Encrypt(plainPassword);
```

## 12. KONTAKT I WSPARCIE

W razie problemów:
1. Sprawdź logi MySQL: `C:\ProgramData\MySQL\MySQL Server 8.0\Data\*.err`
2. Sprawdź logi aplikacji
3. Użyj narzędzi diagnostycznych MySQL:
   - `mysqlcheck`
   - `mysqladmin`
   - Performance Schema

---

**Ostatnia aktualizacja:** 2025-01-07
**Wersja dokumentu:** 1.0
