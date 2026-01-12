-- ============================================================================
-- DANE TESTOWE DLA SYSTEMU OBSŁUGI REKLAMACJI
-- ============================================================================

USE ReklamacjeDB;

-- Wyłącz sprawdzanie kluczy obcych na czas wstawiania danych
SET FOREIGN_KEY_CHECKS = 0;

-- ============================================================================
-- 1. UŻYTKOWNICY TESTOWI
-- ============================================================================

-- Hasło dla wszystkich: "test123" (w produkcji należy użyć hashowania!)
INSERT INTO `Uzytkownicy` (`Login`, `Haslo`, `Nazwa Wyświetlana`, `Email`, `Rola`, `CzyAktywny`) VALUES
('admin', 'test123', 'Administrator Systemu', 'admin@firma.pl', 'Admin', 1),
('jan.kowalski', 'test123', 'Jan Kowalski', 'jan.kowalski@firma.pl', 'User', 1),
('anna.nowak', 'test123', 'Anna Nowak', 'anna.nowak@firma.pl', 'User', 1),
('piotr.wisniewski', 'test123', 'Piotr Wiśniewski', 'piotr.wisniewski@firma.pl', 'Manager', 1);

-- ============================================================================
-- 2. KLIENCI TESTOWI
-- ============================================================================

-- Klienci prywatni
INSERT INTO `Klienci` (`ImieNazwisko`, `Ulica`, `KodPocztowy`, `Miejscowosc`, `Email`, `Telefon`) VALUES
('Jan Nowak', 'ul. Kwiatowa 15', '00-001', 'Warszawa', 'jan.nowak@example.com', '123456789'),
('Maria Kowalska', 'ul. Słoneczna 23', '30-002', 'Kraków', 'maria.kowalska@example.com', '234567890'),
('Piotr Zieliński', 'ul. Leśna 8', '80-003', 'Gdańsk', 'piotr.zielinski@example.com', '345678901');

-- Klienci firmowi
INSERT INTO `Klienci` (`ImieNazwisko`, `NazwaFirmy`, `NIP`, `Ulica`, `KodPocztowy`, `Miejscowosc`, `Email`, `Telefon`) VALUES
('Jan Malinowski', 'Tech Solutions Sp. z o.o.', '1234567890', 'ul. Biznesowa 100', '02-004', 'Warszawa', 'biuro@techsolutions.pl', '221234567'),
('Anna Wiśniewska', 'Elektro-Market S.A.', '9876543210', 'ul. Handlowa 45', '40-005', 'Katowice', 'kontakt@elektromarket.pl', '323456789');

-- ============================================================================
-- 3. PRODUKTY TESTOWE
-- ============================================================================

INSERT INTO `Produkty` (`Nazwa`, `Producent`, `Model`, `KodEnova`, `KodProducenta`, `Kategoria`) VALUES
('Laptop Dell Latitude 5420', 'Dell', 'Latitude 5420', 'DELL-LAT-5420', 'LAT5420-I5-16GB', 'Laptopy'),
('Monitor Samsung 27"', 'Samsung', 'S27A600U', 'SAM-MON-27A600', 'LS27A600UUUXEN', 'Monitory'),
('Drukarka HP LaserJet Pro', 'HP', 'M404dn', 'HP-LJ-M404', 'W1A53A', 'Drukarki'),
('Mysz Logitech MX Master 3', 'Logitech', 'MX Master 3', 'LOG-MX3-BLK', '910-005694', 'Akcesoria'),
('Klawiatura Corsair K70', 'Corsair', 'K70 RGB', 'COR-K70-RGB', 'CH-9109010-NA', 'Akcesoria'),
('Dysk SSD Samsung 1TB', 'Samsung', '970 EVO Plus', 'SAM-SSD-970', 'MZ-V7S1T0BW', 'Dyski'),
('Router TP-Link Archer', 'TP-Link', 'Archer AX50', 'TPL-AX50', 'Archer AX50', 'Sieć'),
('Słuchawki Sony WH-1000XM4', 'Sony', 'WH-1000XM4', 'SON-WH1000XM4', 'WH1000XM4/B', 'Audio');

-- ============================================================================
-- 4. STATUSY OGÓLNE (jeśli nie istnieją)
-- ============================================================================

INSERT IGNORE INTO `StatusyOgolne` (`NazwaStatusu`, `Kolejnosc`, `CzyAktywny`) VALUES
('Nowe', 1, 1),
('W trakcie weryfikacji', 2, 1),
('Oczekuje na części', 3, 1),
('W naprawie', 4, 1),
('Gotowe do wysyłki', 5, 1),
('Wysłane do klienta', 6, 1),
('Zamknięte - naprawione', 7, 1),
('Zamknięte - zwrot pieniędzy', 8, 1),
('Odrzucone', 9, 1);

-- ============================================================================
-- 5. ZGŁOSZENIA TESTOWE
-- ============================================================================

-- Zgłoszenie 1: Laptop - Nowe
INSERT INTO `Zgloszenia` (`NrZgloszenia`, `KlientID`, `ProduktID`, `DataZgloszenia`, `DataZakupu`, `NrFaktury`, `NrSeryjny`, `Usterka`, `StatusOgolny`, `PrzypisanyDo`) VALUES
('R/1/2025', 1, 1, '2025-01-05 10:30:00', '2024-12-15', 'FV/2024/12345', 'SN123456789', 'Laptop nie uruchamia się po naciśnięciu przycisku power. Dioda ładowania świeci się poprawnie.', 'Nowe', 2);

-- Zgłoszenie 2: Monitor - W trakcie weryfikacji
INSERT INTO `Zgloszenia` (`NrZgloszenia`, `KlientID`, `ProduktID`, `DataZgloszenia`, `DataZakupu`, `NrFaktury`, `NrSeryjny`, `Usterka`, `StatusOgolny`, `PrzypisanyDo`) VALUES
('R/2/2025', 2, 2, '2025-01-04 14:20:00', '2024-11-20', 'FV/2024/11234', 'SN987654321', 'Monitor ma pionową linię martwych pikseli po lewej stronie ekranu.', 'W trakcie weryfikacji', 2);

-- Zgłoszenie 3: Drukarka - W naprawie
INSERT INTO `Zgloszenia` (`NrZgloszenia`, `KlientID`, `ProduktID`, `DataZgloszenia`, `DataZakupu`, `NrFaktury`, `NrSeryjny`, `Usterka`, `StatusOgolny`, `PrzypisanyDo`) VALUES
('R/3/2025', 3, 3, '2025-01-03 09:15:00', '2024-10-10', 'FV/2024/10123', 'SN555666777', 'Drukarka zacina papier i wyświetla błąd 50.2. Problem występuje przy każdym wydruku.', 'W naprawie', 3);

-- Zgłoszenie 4: Mysz - Oczekuje na części
INSERT INTO `Zgloszenia` (`NrZgloszenia`, `KlientID`, `ProduktID`, `DataZgloszenia`, `DataZakupu`, `NrFaktury`, `NrSeryjny`, `Usterka`, `StatusOgolny`, `PrzypisanyDo`) VALUES
('R/4/2025', 4, 4, '2025-01-02 16:45:00', '2024-09-05', 'FV/2024/09456', 'SN111222333', 'Środkowy przycisk myszy (scroll) nie działa poprawnie - klik nie jest rejestrowany.', 'Oczekuje na części', 3);

-- Zgłoszenie 5: Klawiatura - Zamknięte
INSERT INTO `Zgloszenia` (`NrZgloszenia`, `KlientID`, `ProduktID`, `DataZgloszenia`, `DataZakupu`, `NrFaktury`, `NrSeryjny`, `Usterka`, `StatusOgolny`, `PrzypisanyDo`, `DataZamkniecia`) VALUES
('R/5/2024', 5, 5, '2024-12-20 11:00:00', '2024-08-15', 'FV/2024/08789', 'SN444555666', 'Kilka klawiszy (W, A, D) reaguje z opóźnieniem podczas grania.', 'Zamknięte - naprawione', 2, '2024-12-28 15:00:00');

-- ============================================================================
-- 6. DZIAŁANIA DLA ZGŁOSZEŃ
-- ============================================================================

-- Działania dla zgłoszenia R/1/2025
INSERT INTO `Dzialania` (`ZgloszenieID`, `TypDzialania`, `Opis`, `DataDzialania`, `UzytkownikID`, `StatusPrzed`, `StatusPo`) VALUES
(1, 'Rejestracja zgłoszenia', 'Zgłoszenie zarejestrowane w systemie. Nadano numer R/1/2025.', '2025-01-05 10:30:00', 2, NULL, 'Nowe'),
(1, 'Kontakt z klientem', 'Wysłano email z potwierdzeniem przyjęcia zgłoszenia.', '2025-01-05 10:35:00', 2, 'Nowe', 'Nowe');

-- Działania dla zgłoszenia R/2/2025
INSERT INTO `Dzialania` (`ZgloszenieID`, `TypDzialania`, `Opis`, `DataDzialania`, `UzytkownikID`, `StatusPrzed`, `StatusPo`) VALUES
(2, 'Rejestracja zgłoszenia', 'Zgłoszenie zarejestrowane w systemie.', '2025-01-04 14:20:00', 2, NULL, 'Nowe'),
(2, 'Zmiana statusu', 'Rozpoczęto weryfikację zgłoszenia. Sprawdzono warunki gwarancji.', '2025-01-05 09:00:00', 2, 'Nowe', 'W trakcie weryfikacji'),
(2, 'Test urządzenia', 'Potwierdzono usterkę - pionowa linia martwych pikseli 2cm od lewej krawędzi.', '2025-01-05 11:30:00', 2, 'W trakcie weryfikacji', 'W trakcie weryfikacji');

-- Działania dla zgłoszenia R/3/2025
INSERT INTO `Dzialania` (`ZgloszenieID`, `TypDzialania`, `Opis`, `DataDzialania`, `UzytkownikID`, `StatusPrzed`, `StatusPo`) VALUES
(3, 'Rejestracja zgłoszenia', 'Zgłoszenie zarejestrowane w systemie.', '2025-01-03 09:15:00', 3, NULL, 'Nowe'),
(3, 'Diagnostyka', 'Zdiagnozowano problem z podajnikiem papieru. Wymagana wymiana rolki.', '2025-01-03 14:00:00', 3, 'Nowe', 'W trakcie weryfikacji'),
(3, 'Rozpoczęcie naprawy', 'Zamówiono część zamienna (rolka podajnika). Rozpoczęto naprawę.', '2025-01-04 10:00:00', 3, 'W trakcie weryfikacji', 'W naprawie');

-- ============================================================================
-- 7. LOGI EMAIL
-- ============================================================================

INSERT INTO `EmailLog` (`ZgloszenieID`, `KlientID`, `Nadawca`, `Odbiorca`, `Temat`, `Tresc`, `Kierunek`, `Data`) VALUES
(1, 1, 'serwis@firma.pl', 'jan.nowak@example.com', 'Potwierdzenie przyjęcia zgłoszenia R/1/2025', 
'Dzień dobry,

Dziękujemy za zgłoszenie reklamacyjne. Nadaliśmy mu numer R/1/2025.

Produkt: Laptop Dell Latitude 5420
Usterka: Laptop nie uruchamia się

Zgłoszenie zostało przypisane do naszego technika, który skontaktuje się z Panem w ciągu 24 godzin.

Pozdrawiamy,
Dział Serwisu', 'OUT', '2025-01-05 10:35:00'),

(2, 2, 'maria.kowalska@example.com', 'serwis@firma.pl', 'Pytanie o zgłoszenie R/2/2025', 
'Dzień dobry,

Chciałabym zapytać, kiedy mogę spodziewać się informacji o wyniku weryfikacji mojego monitora?

Pozdrawiam,
Maria Kowalska', 'IN', '2025-01-05 13:00:00'),

(2, 2, 'serwis@firma.pl', 'maria.kowalska@example.com', 'RE: Pytanie o zgłoszenie R/2/2025', 
'Dzień dobry,

Weryfikacja została już przeprowadzona. Potwierdziliśmy usterkę i zgłoszenie zostało zaakceptowane.
Produkt zostanie wymieniony na nowy. Wysyłkę zamiennika przewidujemy w ciągu 3-5 dni roboczych.

Pozdrawiamy,
Dział Serwisu', 'OUT', '2025-01-05 14:30:00');

-- ============================================================================
-- 8. LOGI SMS
-- ============================================================================

INSERT INTO `SmsLog` (`ZgloszenieID`, `KlientID`, `NumerTelefonu`, `Tresc`, `Kierunek`, `Status`, `Data`) VALUES
(1, 1, '123456789', 'Twoje zgłoszenie R/1/2025 zostało zarejestrowane. Skontaktujemy się w ciągu 24h.', 'OUT', 'SENT', '2025-01-05 10:36:00'),
(3, 3, '345678901', 'Twoje urządzenie (R/3/2025) jest w naprawie. Poinformujemy o zakończeniu.', 'OUT', 'SENT', '2025-01-04 10:05:00');

-- ============================================================================
-- 9. KONTA POCZTOWE
-- ============================================================================

INSERT INTO `KontaPocztowe` (`NazwaWyswietlana`, `AdresEmail`, `Login`, `Haslo`, `Protokol`, 
    `Pop3Host`, `Pop3Port`, `Pop3Ssl`, 
    `SmtpHost`, `SmtpPort`, `SmtpSsl`, 
    `CzyDomyslne`) VALUES
('Serwis - Konto główne', 'serwis@firma.pl', 'serwis@firma.pl', 'haslo123', 'POP3',
    'mail.firma.pl', 110, 0,
    'smtp.firma.pl', 587, 1,
    1);

-- ============================================================================
-- 10. SZABLONY EMAIL
-- ============================================================================

INSERT INTO `SzablonyEmail` (`Nazwa`, `Temat`, `TrescHtml`) VALUES
('Potwierdzenie zgłoszenia', 'Potwierdzenie przyjęcia zgłoszenia {{NUMER_ZGLOSZENIA}}', 
'<html><body>
<h2>Dzień dobry {{IMIE_NAZWISKO}},</h2>
<p>Dziękujemy za zgłoszenie reklamacyjne. Nadaliśmy mu numer <strong>{{NUMER_ZGLOSZENIA}}</strong>.</p>
<p><strong>Produkt:</strong> {{NAZWA_PRODUKTU}}<br>
<strong>Opis usterki:</strong> {{OPIS_USTERKI}}</p>
<p>Zgłoszenie zostało przekazane do realizacji. Skontaktujemy się z Państwem w ciągu 24 godzin roboczych.</p>
<p>Pozdrawiamy,<br>Dział Serwisu</p>
</body></html>'),

('Prośba o uzupełnienie danych', 'Prośba o uzupełnienie danych - zgłoszenie {{NUMER_ZGLOSZENIA}}',
'<html><body>
<h2>Dzień dobry {{IMIE_NAZWISKO}},</h2>
<p>W związku ze zgłoszeniem <strong>{{NUMER_ZGLOSZENIA}}</strong> prosimy o uzupełnienie następujących informacji:</p>
<ul>
<li>{{LISTA_BRAKUJACYCH_DANYCH}}</li>
</ul>
<p>Informacje można przesłać odpowiadając na tego maila.</p>
<p>Pozdrawiamy,<br>Dział Serwisu</p>
</body></html>');

-- ============================================================================
-- 11. MAGAZYN
-- ============================================================================

INSERT INTO `Magazyn` (`NazwaCzesci`, `KodCzesci`, `Ilosc`, `MinimalnyStan`, `Lokalizacja`) VALUES
('Ekran LCD 15.6" Full HD', 'LCD-156-FHD', 5, 2, 'Regał A1'),
('Bateria do Dell Latitude', 'BAT-DELL-LAT', 8, 3, 'Regał A2'),
('Klawiatura Dell Latitude', 'KB-DELL-LAT', 12, 5, 'Regał A3'),
('Zasilacz 65W Dell', 'PSU-DELL-65W', 15, 5, 'Regał B1'),
('Dysk SSD 256GB', 'SSD-256GB', 20, 10, 'Regał B2'),
('Rolka podajnika HP', 'HP-ROLLER-M404', 3, 1, 'Regał C1');

-- ============================================================================
-- 12. PRZYPOMNIENIA
-- ============================================================================

INSERT INTO `Przypomnienia` (`ticket_id`, `source`, `category`, `title`, `message`, `due_at`, `status`) VALUES
('R/1/2025', 'MANUAL', 'decision', 'Weryfikacja zgłoszenia R/1/2025', 
'Należy zweryfikować zgłoszenie i podjąć decyzję o naprawie/wymianie.', 
'2025-01-07 12:00:00', 'pending'),

('R/2/2025', 'MANUAL', 'courier', 'Wysyłka zamiennika dla R/2/2025', 
'Przygotować i wysłać zamiennik dla klienta Maria Kowalska.', 
'2025-01-08 10:00:00', 'pending'),

('R/3/2025', 'AUTO', 'decision', 'Kontakt z klientem - R/3/2025', 
'Poinformować klienta o postępie naprawy.', 
'2025-01-06 14:00:00', 'pending');

-- ============================================================================
-- 13. LOG SYNCHRONIZACJI
-- ============================================================================

INSERT INTO `SyncRuns` (`source`, `started_at`, `finished_at`, `ok`, `rows_fetched`, `rows_written`, `details`) VALUES
('ALLEGRO', '2025-01-05 08:00:00', '2025-01-05 08:02:15', 1, 15, 3, 'Synchronizacja zakończona sukcesem. Znaleziono 3 nowe zgłoszenia.'),
('DPD', '2025-01-05 09:00:00', '2025-01-05 09:00:45', 1, 8, 2, 'Zaktualizowano status 2 przesyłek.'),
('GOOGLE', '2025-01-05 10:00:00', '2025-01-05 10:01:30', 1, 5, 1, 'Zsynchronizowano 1 nowe zgłoszenie z Google Sheets.');

-- ============================================================================
-- 14. LOG AKTYWNOŚCI
-- ============================================================================

INSERT INTO `LogAktywnosci` (`UzytkownikId`, `Akcja`, `Opis`, `IpAddress`, `DataCzas`, `ZgloszenieId`) VALUES
(2, 'LOGIN', 'Użytkownik zalogował się do systemu', '192.168.1.100', '2025-01-05 08:30:00', NULL),
(2, 'DODAJ_ZGLOSZENIE', 'Utworzono nowe zgłoszenie R/1/2025', '192.168.1.100', '2025-01-05 10:30:00', 1),
(2, 'WYSLIJ_EMAIL', 'Wysłano email potwierdzający dla R/1/2025', '192.168.1.100', '2025-01-05 10:35:00', 1),
(3, 'LOGIN', 'Użytkownik zalogował się do systemu', '192.168.1.101', '2025-01-05 09:00:00', NULL),
(3, 'ZMIEN_STATUS', 'Zmieniono status zgłoszenia R/3/2025 na "W naprawie"', '192.168.1.101', '2025-01-04 10:00:00', 3);

-- Przywróć sprawdzanie kluczy obcych
SET FOREIGN_KEY_CHECKS = 1;

-- ============================================================================
-- PODSUMOWANIE
-- ============================================================================

SELECT 'Dane testowe zostały wstawione pomyślnie!' AS Status;

SELECT 
    'Użytkownicy' AS Tabela, COUNT(*) AS Liczba FROM Uzytkownicy
UNION ALL SELECT 'Klienci', COUNT(*) FROM Klienci
UNION ALL SELECT 'Produkty', COUNT(*) FROM Produkty
UNION ALL SELECT 'Zgłoszenia', COUNT(*) FROM Zgloszenia
UNION ALL SELECT 'Działania', COUNT(*) FROM Dzialania
UNION ALL SELECT 'Email Log', COUNT(*) FROM EmailLog
UNION ALL SELECT 'SMS Log', COUNT(*) FROM SmsLog
UNION ALL SELECT 'Magazyn', COUNT(*) FROM Magazyn
UNION ALL SELECT 'Przypomnienia', COUNT(*) FROM Przypomnienia;

-- ============================================================================
-- KONIEC SKRYPTU
-- ============================================================================
