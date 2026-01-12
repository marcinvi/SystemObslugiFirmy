Naprawa błędu: 'Nie można załadować pliku projektu ... Reklamacje Dane\Reklamacje Dane.csproj'

Co zrobiłem:
- Zmieniłem wpis w 'Reklamacje Dane.sln' tak, aby wskazywał na 'Reklamacje Dane.csproj' bez podfolderu.
- Spakowałem całość pod nazwą katalogu top-level: 'Reklamacje Dane'.

Jak uruchomić:
1) Rozpakuj archiwum tak, aby pliki znalazły się w folderze:
   C:\...\Reklamacje Dane\
   (w tym samym folderze muszą być zarówno plik .sln, jak i .csproj).
2) Otwórz 'Reklamacje Dane.sln' w Visual Studio.
3) Zrób Restore NuGet Packages i uruchom.

Uwagi:
- Jeśli wcześniej otwierałeś błędną ścieżkę, zamknij VS, usuń ewentualne .vs/suo i otwórz ponownie.
