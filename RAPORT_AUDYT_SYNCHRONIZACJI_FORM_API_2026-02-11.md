# Audyt synchronizacji: Windows Forms -> API

Data: 2026-02-11
Zakres: analiza miejsc synchronizacji w aplikacji Windows Forms oraz dostępnych endpointów i mechanizmów synchronizacji po stronie `ReklamacjeAPI`.

## 1) Czy wszystkie formy, które miały synchronizację, są zmienione?

**Wniosek: NIE.**

### Potwierdzone formy/kontrolki nadal synchronizujące dane poza nowym `ApiSyncService`

1. `Form1.cs`
   - ma własne timery (`_googleSheetSyncTimer`, `_allegroSyncTimer`) i własne metody `RunAllegroSync()` + `RunGoogleSheetsSync()`.
   - dla Allegro korzysta bezpośrednio z `AllegroSyncService.SynchronizeDisputesAsync()`.

2. `ReklamacjeControl.cs`
   - ma kilka niezależnych timerów (`_emailSyncTimer`, `_googleSheetSyncTimer`, `_allegroSyncTimer`, `_returnsSyncTimer`, itp.).
   - uruchamia lokalne procesy `RunEmailSync()`, `RunAllegroSync()`, `RunGoogleSheetsSync()`, `RunReturnsSync()`.
   - `RunAllegroSync()` wywołuje bezpośrednio `AllegroSyncService`.
   - część odczytów odbywa się bezpośrednio z DB (np. licznik nowych zwrotów).

3. `FormApiConfig.cs`
   - to jedyne miejsce form, które realnie używa `ApiSyncService.SyncZgloszeniaAsync(...)`.
   - nie przejmuje jednak synchronizacji z `Form1`/`ReklamacjeControl`.

### Konkluzja architektoniczna

Migracja do API jest **częściowa**: nowa ścieżka `ApiSyncService` istnieje, ale dotyczy głównie konfiguracji i ręcznego „Sync now” z `FormApiConfig`. Główne ekrany operacyjne nadal mają osobną, starszą logikę synchronizacji.

## 2) Czy synchronizacja w API działa?

**Wniosek: częściowo / zależnie od obszaru.**

### Co wygląda na działające

- API ma kontrolery i endpointy dla kluczowych domen: `Auth`, `Zgloszenia`, `Klienci`, `Returns`, `Messages`, `Files`.
- API posiada zadania tła (`HostedService`), m.in. odświeżanie tokenów Allegro i synchronizację zwrotów.
- Klient WinForms (`ReklamacjeApiClient` + `ApiSyncService`) implementuje logowanie, pobieranie zgłoszeń, update statusu i dodawanie notatek.

### Co jest jednoznacznie niegotowe

- `WarehouseController` zwraca HTTP 501 (`not implemented`) dla endpointów magazynowych (`search`, `intake`).

### Co ogranicza ocenę „działa”

- W tym środowisku nie było możliwości uruchomienia `dotnet build` (brak `dotnet`), więc nie potwierdzono runtime end-to-end testem.
- Na podstawie kodu: część API jest kompletna, ale są obszary celowo niedokończone (np. warehouse).

## 3) W jaki sposób program Forms ma wiedzieć o zmianach?

### Obecny stan

- Brak globalnego mechanizmu push dla WinForms (brak użycia SignalR/WebSocket w kliencie desktop).
- Aplikacja wykrywa zmiany przez **polling** (timery) oraz ręczne odświeżenie.
- `ApiSyncService` ma cache 5 minut, ale nie posiada eventów typu `OnDataChanged` ani subskrypcji zmian z serwera.

### Co to oznacza praktycznie

- Formularze dowiadują się o zmianach dopiero przy kolejnym cyklu timera lub przy ręcznym odświeżeniu.
- W obecnym modelu mogą występować opóźnienia i niespójności między widokami.

## 4) Najważniejsze ryzyka logiczne wykryte podczas przeglądu

1. **Dwie równoległe architektury synchronizacji**
   - stara (timery + bezpośrednie serwisy/DB) i nowa (`ApiSyncService`) działają obok siebie.
   - ryzyko: duplikacja pracy, różne źródła prawdy, trudniejsze debugowanie.

2. **Pozorny przełącznik „automatyczna synchronizacja co 5 min” w `FormApiConfig`**
   - checkbox zapisuje ustawienie, ale ten formularz nie uruchamia własnego timera auto-sync.
   - ryzyko: użytkownik oczekuje automatyki, której realnie nie ma.

3. **Brak real-time eventów do Forms**
   - bez push-owego kanału (SignalR) UI jest zależne od pollingów i może być „spóźnione”.

4. **Niedokończona warstwa magazynowa w API**
   - endpointy `warehouse` zwracają 501, więc pełna migracja obszaru magazynu do API nie jest gotowa.

## 5) Proponowana kolejność domknięcia migracji

1. Wybrać jedną ścieżkę synchronizacji jako docelową (API-first).
2. Dodać adapter w `Form1` i `ReklamacjeControl`, aby zamiast lokalnych usług korzystały z `ApiSyncService`/`ReklamacjeApiClient`.
3. Dodać mechanizm sygnalizacji zmian:
   - wariant minimum: krótszy polling + centralny event `DataRefreshed`.
   - wariant docelowy: SignalR hub + subskrypcja w desktop.
4. Dokończyć `WarehouseController` i związaną logikę serwisową.
5. Dopisać testy integracyjne API i checklistę E2E dla scenariuszy Forms.

## 6) Odpowiedzi skrócone

- **Czy wszystkie formy z synchronizacją są zmienione?** Nie – tylko fragment (głównie `FormApiConfig`) używa nowego API sync.
- **Czy synchronizacja w API działa?** Częściowo; wiele endpointów jest gotowych, ale np. magazyn (`warehouse`) jest nadal 501.
- **Skąd Form ma wiedzieć o zmianach?** Obecnie z timerów/pollingu i ręcznego odświeżania; brak pełnego mechanizmu push do desktopu.
