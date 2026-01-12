# 🚀 SZYBKA NAPRAWA - 3 minuty

## ❌ Błąd
```
Table 'reklamacjedb.allegroreturnitems' doesn't exist
```

## ✅ Rozwiązanie (3 kroki)

### 1️⃣ Otwórz swoją bazę danych MySQL
Użyj HeidiSQL, MySQL Workbench, phpMyAdmin lub innego klienta

### 2️⃣ Wykonaj ten SQL
```sql
USE reklamacjedb;

CREATE TABLE IF NOT EXISTS `AllegroReturnItems` (
    `Id` INT(11) NOT NULL AUTO_INCREMENT,
    `ReturnId` INT(11) NOT NULL,
    `OfferId` VARCHAR(100) NULL DEFAULT NULL,
    `ProductName` VARCHAR(500) NULL DEFAULT NULL,
    `Quantity` INT(11) NULL DEFAULT NULL,
    `Price` DECIMAL(10,2) NULL DEFAULT NULL,
    `Currency` VARCHAR(10) NULL DEFAULT 'PLN',
    `ReasonType` VARCHAR(100) NULL DEFAULT NULL,
    `ReasonComment` TEXT NULL DEFAULT NULL,
    `ProductUrl` VARCHAR(500) NULL DEFAULT NULL,
    `JsonDetails` TEXT NULL DEFAULT NULL,
    `CreatedAt` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
    
    PRIMARY KEY (`Id`),
    INDEX `idx_return_id` (`ReturnId`),
    INDEX `idx_offer_id` (`OfferId`),
    
    CONSTRAINT `fk_return_items_return` 
        FOREIGN KEY (`ReturnId`) 
        REFERENCES `AllegroCustomerReturns` (`Id`) 
        ON DELETE CASCADE 
        ON UPDATE CASCADE
) ENGINE=InnoDB COLLATE='utf8mb4_unicode_ci';
```

### 3️⃣ Zweryfikuj
```sql
SHOW TABLES LIKE 'AllegroReturnItems';
```

Powinieneś zobaczyć: ✅ `AllegroReturnItems`

---

## 🎯 Gotowe!

Teraz uruchom ponownie synchronizację zwrotów Allegro.

---

## 📄 Więcej informacji

- **Szczegóły:** `NAPRAWA_BRAKUJACEJ_TABELI.md`
- **Sprawdzenie tabel:** Wykonaj `sprawdz_tabele_allegro.sql`
- **Pełny skrypt:** `create_allegro_return_items_table.sql`

---

**Status:** ⏳ Wykonaj SQL → ✅ Gotowe!
