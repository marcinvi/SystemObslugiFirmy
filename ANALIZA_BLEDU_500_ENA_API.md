# Analiza błędu HTTP 500 w aplikacji ENA (profil / reklamacje / admin)

## Objawy
- Android po wejściu w **Mój profil** pokazuje: `Błąd profilu: Błąd serwera (HTTP 500)`.
- Podobny błąd występuje w modułach **Reklamacje** i **Admin**.

## Główna przyczyna
Problem nie jest po stronie Androida (klient tylko pokazuje kod odpowiedzi), tylko po stronie API i niespójności model ↔ baza.

### 1) Profil: zapytanie do kolumny `Telefon` w tabeli `Uzytkownicy`
Kontroler profilu robi SQL:

```sql
SELECT Login, `Nazwa Wyświetlana`, Email, Telefon FROM Uzytkownicy WHERE Id = @id
```

oraz:

```sql
UPDATE Uzytkownicy SET Email = @email, Telefon = @phone WHERE Id = @id
```

Jeżeli w Twojej produkcyjnej tabeli `Uzytkownicy` nie ma kolumny `Telefon`, API rzuci wyjątek SQL i zwróci HTTP 500.

### 2) Admin: niespójne nazwy kolumn użytkownika w EF
Model `User` mapuje m.in.:
- `PasswordHash` -> kolumna `Hasło`
- `IsActive` -> kolumna `IsActive`

W `schema_mysql_complete.sql` tabela `Uzytkownicy` jest opisana inaczej:
- `Haslo` (bez polskiego znaku)
- `CzyAktywny` (zamiast `IsActive`)

Jeżeli realna baza ma wariant ze schematu (`Haslo`, `CzyAktywny`), endpointy admina (`api/admin/users`) będą dawały 500 przy materializacji encji EF.

### 3) Reklamacje (dashboard): niespójne mapowanie encji `Zgloszenie`
Model `Zgloszenie` mapuje:
- `Id` -> `IdZgloszenia`
- `IdKlienta` -> `IdKlienta`

Natomiast `schema_mysql_complete.sql` definiuje odpowiednio:
- `Id`
- `KlientID`

Endpoint dashboardu reklamacji (`api/dashboard/complaints/processing`) działa na EF (`_context.Zgloszenia...`). Jeśli kolumny są inne niż mapowanie modelu, wynik to HTTP 500.

## Dlaczego Android pokazuje tylko ogólny błąd?
W kliencie Android odpowiedź != 2xx jest mapowana do komunikatu:

`Błąd serwera (HTTP <kod>)`

czyli nie zobaczysz konkretnego wyjątku SQL bez logów API.

## Dodatkowe błędy logiczne wykryte w API
1. W `Program.cs` brakuje kompletnej konfiguracji JWT middleware (`AddAuthentication`, `UseAuthentication`, `UseAuthorization`), mimo że kontrolery używają `[Authorize]`.
2. W kodzie istnieją dwa równoległe modele użytkownika (`User` i `Uzytkownik`) i dwa różne style nazewnictwa kolumn, co zwiększa ryzyko regresji.
3. W repo istnieją sprzeczne skrypty inicjalizacji (`HasloHash`/`Aktywny` vs `Haslo`/`CzyAktywny` vs `Hasło`/`IsActive`).

## Co naprawić (kolejność rekomendowana)
1. **Ujednolicić kontrakt DB** (jeden kanoniczny schemat) i do niego dopasować encje EF oraz raw SQL.
2. **Sprawdzić produkcyjne DDL** komendami:
   - `SHOW CREATE TABLE Uzytkownicy;`
   - `SHOW CREATE TABLE Zgloszenia;`
   - `SHOW CREATE TABLE Klienci;`
3. Jeśli chcesz szybko przywrócić działanie:
   - dodać brakujące kolumny (`Telefon`, ewentualnie alias migracyjny na `CzyAktywny`/`IsActive`),
   - poprawić mapowanie `Zgloszenie` do faktycznych nazw kolumn.
4. Dodać globalny middleware obsługi wyjątków, aby zamiast „gołego 500” zwracać diagnostyczny `ApiResponse`.
5. Dopiąć autoryzację JWT w pipeline ASP.NET Core.

## Minimalna checklista po poprawkach
- Login działa.
- `GET /api/profile` zwraca 200.
- `GET /api/admin/users` zwraca 200.
- `GET /api/dashboard/complaints/processing` zwraca 200.
- Android nie pokazuje już `HTTP 500` dla tych ekranów.
