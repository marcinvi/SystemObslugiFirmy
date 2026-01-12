# Quick Start - Optymalizacja dla 50 użytkowników

## ⚡ Szybka konfiguracja (15 minut)

### ✅ Krok 1: Konfiguracja MySQL/MariaDB (5 min)

1. **Otwórz plik konfiguracyjny MySQL:**
   - Windows: `C:\ProgramData\MySQL\MySQL Server X.X\my.ini`
   - Linux: `/etc/mysql/my.cnf`

2. **Dodaj do sekcji `[mysqld]`:**

```ini
[mysqld]
# Podstawowe ustawienia dla 50 użytkowników
max_connections = 200
innodb_buffer_pool_size = 4G
query_cache_size = 256M
thread_cache_size = 50
innodb_flush_log_at_trx_commit = 2
```

3. **Restart MySQL:**
   ```bash
   # Windows
   net stop MySQL && net start MySQL

   # Linux
   sudo systemctl restart mysql
   ```

---

### ✅ Krok 2: Dodaj indeksy bazodanowe (5 min)

1. **Otwórz HeidiSQL lub MySQL Workbench**

2. **Uruchom te najważniejsze indeksy:**

```sql
USE ReklamacjeDB;

-- Najważniejsze indeksy
CREATE INDEX idx_zgloszenia_nrzgloszenia ON Zgloszenia(NrZgloszenia);
CREATE INDEX idx_zgloszenia_klientid ON Zgloszenia(KlientID);
CREATE INDEX idx_zgloszenia_statusogolny ON Zgloszenia(StatusOgolny);
CREATE INDEX idx_klienci_telefon ON Klienci(Telefon);
CREATE INDEX idx_centrumkontaktu_zgloszenid ON CentrumKontaktu(ZgloszenieID);

-- Aktualizuj statystyki
ANALYZE TABLE Zgloszenia;
ANALYZE TABLE Klienci;
ANALYZE TABLE CentrumKontaktu;
```

*Pełna lista indeksów: `recommended_indexes.sql`*

---

### ✅ Krok 3: Zbuduj aplikację (2 min)

Zmiany w kodzie są już zrobione! Wystarczy przebudować projekt:

1. **Visual Studio:** Build → Rebuild Solution
2. **Lub ręcznie:** `msbuild "Reklamacje Dane.sln" /t:Rebuild`

---

### ✅ Krok 4: Testowanie (3 min)

1. **Uruchom aplikację**

2. **Sprawdź połączenie:**
   - Zaloguj się jako użytkownik
   - Otwórz kilka zgłoszeń
   - Sprawdź czy działa szybko

3. **Włącz logowanie wydajności (opcjonalnie):**
   - Otwórz `App.config`
   - Zmień: `<add key="EnablePerformanceLogging" value="true" />`
   - Uruchom ponownie aplikację
   - Sprawdź folder `Logs/performance_YYYYMMDD.log`

---

## 🔍 Weryfikacja

### Sprawdź konfigurację MySQL:

```sql
SHOW VARIABLES LIKE 'max_connections';
-- Oczekiwane: 200

SHOW VARIABLES LIKE 'innodb_buffer_pool_size';
-- Oczekiwane: 4294967296 (4GB)

SHOW STATUS LIKE 'Threads_connected';
-- Powinno być < 100
```

### Sprawdź indeksy:

```sql
SELECT COUNT(*) as liczba_indeksow
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA = 'ReklamacjeDB'
  AND INDEX_NAME LIKE 'idx_%';
-- Oczekiwane: przynajmniej 5
```

---

## 📊 Monitoring

### Sprawdź połączenia MySQL:

```sql
SHOW PROCESSLIST;
```

### Sprawdź logi wydajności aplikacji:

```powershell
# Windows PowerShell
Get-Content "Logs\performance_*.log" -Tail 20
```

### Zobacz statystyki:

```csharp
// Dodaj do kodu w panelu admina:
var stats = PerformanceLogger.Instance.GetTodayStats();
MessageBox.Show($"Wolne zapytania: {stats.SlowQueries}\n" +
                $"Błędy: {stats.FailedQueries}");
```

---

## ⚠️ Najczęstsze problemy

### Problem: "Too many connections"

**Rozwiązanie:**
```sql
SET GLOBAL max_connections = 300;
-- Następnie dodaj do my.cnf i zrestartuj MySQL
```

### Problem: Wolne zapytania

**Rozwiązanie:**
```sql
-- Sprawdź plan zapytania
EXPLAIN SELECT * FROM Zgloszenia WHERE ...;

-- Dodaj brakujący indeks jeśli potrzeba
CREATE INDEX idx_nazwa ON tabela(kolumna);
```

### Problem: Aplikacja nie działa po zmianach

**Rozwiązanie:**
1. Sprawdź czy plik `PerformanceLogger.cs` został dodany do projektu
2. Rebuild całego solution
3. Sprawdź `App.config` - czy jest sekcja `<appSettings>`

---

## 📚 Więcej informacji

- **Pełna dokumentacja:** `OPTYMALIZACJA_DLA_50_UZYTKOWNIKOW.md`
- **Wszystkie indeksy:** `recommended_indexes.sql`
- **Konfiguracja MySQL:** `mysql_optimization_config.cnf`

---

## ✅ Checklist implementacji

- [ ] Zmiany w `DbConfig.cs` (connection pooling) ✅ GOTOWE
- [ ] Zmiany w `DatabaseService.cs` (retry + timeout) ✅ GOTOWE
- [ ] Dodanie `PerformanceLogger.cs` ✅ GOTOWE
- [ ] Konfiguracja MySQL (`my.ini`)
- [ ] Restart MySQL
- [ ] Dodanie indeksów (`recommended_indexes.sql`)
- [ ] Rebuild aplikacji
- [ ] Test funkcjonalności
- [ ] Monitoring przez tydzień

---

**Czas implementacji:** ~15 minut
**Oczekiwany rezultat:** System obsługuje 50+ użytkowników jednocześnie
**Wsparcie:** Zobacz pełną dokumentację w `OPTYMALIZACJA_DLA_50_UZYTKOWNIKOW.md`
