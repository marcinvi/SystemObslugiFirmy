-- ========================================
-- Tabela: AllegroReturnItems
-- Opis: Przechowuje szczegóły produktów w zwrotach Allegro
--       (gdy zwrot zawiera więcej niż 1 produkt)
-- Data: 2026-01-07
-- ========================================

CREATE TABLE IF NOT EXISTS `AllegroReturnItems` (
    `Id` INT(11) NOT NULL AUTO_INCREMENT,
    `ReturnId` INT(11) NOT NULL COMMENT 'FK do AllegroCustomerReturns.Id',
    `OfferId` VARCHAR(100) NULL DEFAULT NULL COMMENT 'ID oferty Allegro',
    `ProductName` VARCHAR(500) NULL DEFAULT NULL COMMENT 'Nazwa produktu',
    `Quantity` INT(11) NULL DEFAULT NULL COMMENT 'Ilość sztuk',
    `Price` DECIMAL(10,2) NULL DEFAULT NULL COMMENT 'Cena jednostkowa (jeśli dostępna)',
    `Currency` VARCHAR(10) NULL DEFAULT 'PLN' COMMENT 'Waluta',
    `ReasonType` VARCHAR(100) NULL DEFAULT NULL COMMENT 'Typ powodu zwrotu',
    `ReasonComment` TEXT NULL DEFAULT NULL COMMENT 'Komentarz kupującego dot. zwrotu',
    `ProductUrl` VARCHAR(500) NULL DEFAULT NULL COMMENT 'URL do produktu (jeśli dostępny)',
    `JsonDetails` TEXT NULL DEFAULT NULL COMMENT 'Pełne dane JSON produktu z API',
    `CreatedAt` DATETIME NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Data utworzenia rekordu',
    
    PRIMARY KEY (`Id`) USING BTREE,
    INDEX `idx_return_id` (`ReturnId`) USING BTREE,
    INDEX `idx_offer_id` (`OfferId`) USING BTREE,
    
    CONSTRAINT `fk_return_items_return` 
        FOREIGN KEY (`ReturnId`) 
        REFERENCES `AllegroCustomerReturns` (`Id`) 
        ON DELETE CASCADE 
        ON UPDATE CASCADE
) 
COMMENT='Szczegóły produktów w zwrotach Allegro (dla zwrotów wieloproduktowych)'
COLLATE='utf8mb4_unicode_ci'
ENGINE=InnoDB;

-- ========================================
-- KONIEC SKRYPTU
-- ========================================
