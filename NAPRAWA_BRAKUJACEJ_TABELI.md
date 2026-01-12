# 🚨 PILNA NAPRAWA - Brakująca tabela AllegroReturnItems

**Błąd:** `Table 'reklamacjedb.allegroreturnitems' doesn't exist`  
**Data:** 2026-01-07 23:30 CET  
**Priorytet:** 🔴 KRYTYCZNY  

---

## 📋 Problem

Kod próbuje zapisać produkty zwrotu do tabeli `AllegroReturnItems`, ale tabela nie istnieje w bazie danych.

**Kiedy występuje:**
- Gdy zwrot Allegro zawiera **więcej niż 1 produkt**
- Kod w `SaveReturnItemsAsync()` próbuje zapisać każdy produkt osobno

---

## ✅ SZYBKIE ROZWIĄZANIE (2 minuty)

### Krok 1: Otwórz MySQL/MariaDB
Użyj swojego klienta MySQL (np. HeidiSQL, MySQL Workbench, phpMyAdmin)

### Krok 2: Wybierz bazę danych
```sql
USE reklamacjedb;
```

### Krok 3: Wykonaj skrypt
Otwórz i wykonaj plik: **`create_allegro_return_items_table.sql`**

LUB skopiuj i wykonaj poniższy kod:

```sql
CREATE TABLE IF NOT EXISTS `AllegroReturnItems` (
    `Id` INT(11) NOT NULL AUTO_INCREMENT,
    `ReturnId` INT(11) NOT NULL COMMENT 'FK do AllegroCustomerReturns.Id',
    `OfferId` VARCHAR(100) NULL DEFAULT NULL COMMENT 'ID oferty Allegro',
    `ProductName` VARCHAR(500) NULL DEFAULT NULL COMMENT 'Nazwa produktu',
    `Quantity` INT(11) NULL DEFAULT NULL COMMENT 'Ilość sztuk',
    `Price` DECIMAL(10,2) NULL DEFAULT NULL COMMENT 'Cena jednostkowa',
    `Currency` VARCHAR(10) NULL DEFAULT 'PLN' COMMENT 'Waluta',
    `ReasonType` VARCHAR(100) NULL DEFAULT NULL COMMENT 'Typ powodu zwrotu',
    `ReasonComment` TEXT NULL DEFAULT NULL COMMENT 'Komentarz kupującego',
    `ProductUrl` VARCHAR(500) NULL DEFAULT NULL COMMENT 'URL do produktu',
    `JsonDetails` TEXT NULL DEFAULT NULL COMMENT 'Pełne dane JSON z API',
    `CreatedAt` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
    
    PRIMARY KEY (`Id`) USING BTREE,
    INDEX `idx_return_id` (`ReturnId`) USING BTREE,
    INDEX `idx_offer_id` (`OfferId`) USING BTREE,
    
    CONSTRAINT `fk_return_items_return` 
        FOREIGN KEY (`ReturnId`) 
        REFERENCES `AllegroCustomerReturns` (`Id`) 
        ON DELETE CASCADE 
        ON UPDATE CASCADE
) 
COMMENT='Szczegóły produktów w zwrotach Allegro'
COLLATE='utf8mb4_unicode_ci'
ENGINE=InnoDB;
```

### Krok 4: Zweryfikuj
```sql
SHOW TABLES LIKE 'AllegroReturnItems';
DESCRIBE AllegroReturnItems;
```

Powinieneś zobaczyć:
```
✅ Table: AllegroReturnItems
✅ 12 kolumn (Id, ReturnId, OfferId, ...)
```

---

## 🔍 Co robi ta tabela?

### Cel:
Przechowuje **szczegóły każdego produktu** w zwrocie Allegro

### Kiedy jest używana:
- Gdy zwrot zawiera **więcej niż 1 produkt**
- Przykład: Klient kupił 3 różne produkty i zwraca wszystkie 3

### Struktura danych:
```
AllegroCustomerReturns (1)  ←→  (N) AllegroReturnItems
     Główny zwrot                    Poszczególne produkty
```

**Przykład:**
```
Zwrot ID: 5fa10eda-df90-4ce2-ba1b-cad8d9ac1ab9
└─ Produkt 1: Laptop Dell (Quantity: 1, Reason: "Uszkodzony")
└─ Produkt 2: Mysz Logitech (Quantity: 2, Reason: "Zmiana zdania")
└─ Produkt 3: Klawiatura (Quantity: 1, Reason: "Nie działa")
```

---

## 📊 Weryfikacja po utworzeniu

### Test 1: Sprawdź czy tabela istnieje
```sql
SELECT COUNT(*) as TablesCount 
FROM information_schema.TABLES 
WHERE TABLE_SCHEMA = 'reklamacjedb' 
AND TABLE_NAME = 'AllegroReturnItems';
```
**Oczekiwany wynik:** `TablesCount = 1` ✅

### Test 2: Sprawdź strukturę
```sql
DESCRIBE AllegroReturnItems;
```
**Oczekiwany wynik:** 12 kolumn ✅

### Test 3: Sprawdź klucz obcy
```sql
SELECT 
    CONSTRAINT_NAME,
    TABLE_NAME,
    COLUMN_NAME,
    REFERENCED_TABLE_NAME,
    REFERENCED_COLUMN_NAME
FROM information_schema.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = 'reklamacjedb'
AND TABLE_NAME = 'AllegroReturnItems'
AND REFERENCED_TABLE_NAME IS NOT NULL;
```
**Oczekiwany wynik:** FK do `AllegroCustomerReturns(Id)` ✅

---

## 🎯 Po utworzeniu tabeli

1. ✅ **Uruchom ponownie synchronizację** zwrotów Allegro
2. ✅ **Sprawdź logi** - nie powinno być błędów o brakującej tabeli
3. ✅ **Zweryfikuj dane**:
   ```sql
   SELECT COUNT(*) FROM AllegroReturnItems;
   SELECT * FROM AllegroReturnItems LIMIT 5;
   ```

---

## 🔧 Checklist wszystkich tabel Allegro

Sprawdź czy masz wszystkie potrzebne tabele:

```sql
SELECT TABLE_NAME 
FROM information_schema.TABLES 
WHERE TABLE_SCHEMA = 'reklamacjedb' 
AND TABLE_NAME LIKE 'Allegro%'
ORDER BY TABLE_NAME;
```

**Wymagane tabele:**
- ✅ `AllegroAccounts` - Konta Allegro
- ✅ `AllegroCustomerReturns` - Główna tabela zwrotów
- ✅ `AllegroReturnItems` - **← NOWA TABELA**
- ✅ `AllegroDisputes` - Dyskusje i reklamacje
- ✅ `AllegroChatMessages` - Wiadomości czatu
- ✅ `AllegroChatAttachments` - Załączniki czatu
- ✅ `AllegroSyncLog` - Logi synchronizacji

---

## ❓ Troubleshooting

### Problem: "Cannot add foreign key constraint"
**Przyczyna:** Tabela `AllegroCustomerReturns` nie ma klucza głównego lub ma inną strukturę

**Rozwiązanie:**
```sql
-- Sprawdź strukturę nadrzędnej tabeli
DESCRIBE AllegroCustomerReturns;

-- Jeśli jest OK, usuń constraint i spróbuj ponownie bez niego
ALTER TABLE AllegroReturnItems DROP FOREIGN KEY fk_return_items_return;
```

### Problem: "Table already exists"
**Rozwiązanie:**
```sql
-- Usuń starą tabelę (UWAGA: straci dane!)
DROP TABLE IF EXISTS AllegroReturnItems;

-- Lub użyj IF NOT EXISTS (już jest w skrypcie)
CREATE TABLE IF NOT EXISTS ...
```

---

## 📝 Dodatkowe informacje

### Dlaczego ta tabela jest potrzebna?
- API Allegro może zwracać zwroty z **wieloma produktami**
- Główna tabela `AllegroCustomerReturns` przechowuje tylko **pierwszy produkt**
- Tabela `AllegroReturnItems` przechowuje **wszystkie produkty**

### Kiedy NIE jest używana?
- Gdy zwrot zawiera tylko 1 produkt → zapisywany bezpośrednio w `AllegroCustomerReturns`

---

## 🎉 Gotowe!

Po utworzeniu tabeli:
1. ✅ Tabela `AllegroReturnItems` istnieje
2. ✅ Synchronizacja zwrotów działa
3. ✅ Zwrot `5fa10eda-df90-4ce2-ba1b-cad8d9ac1ab9` powinien się zsynchronizować

**Plik SQL:** `create_allegro_return_items_table.sql`

---

**Status:** ⏳ Oczekuje na wykonanie skryptu SQL
