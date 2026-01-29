CREATE TABLE IF NOT EXISTS ZwrotPliki (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ZwrotId INT NOT NULL,
    NazwaPliku VARCHAR(255) NOT NULL,
    SciezkaPliku VARCHAR(500) NOT NULL,
    TypPliku VARCHAR(50) NULL,
    RozmiarPliku BIGINT NULL,
    DataDodania DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DodanyPrzez INT NULL,
    DodanyPrzezNazwa VARCHAR(255) NULL,
    INDEX idx_ZwrotPliki_ZwrotId (ZwrotId)
);
