@echo off
REM Test połączenia - Sprawdza czy API jest dostępne

echo ================================================
echo TEST POLACZENIA - ReklamacjeAPI
echo ================================================
echo.

REM Krok 1: Sprawdź czy API działa lokalnie
echo [1/4] Sprawdzam czy API działa na localhost:50875...
curl -s -o nul -w "%%{http_code}" http://localhost:50875/health > temp.txt
set /p HTTP_CODE=<temp.txt
del temp.txt

if "%HTTP_CODE%"=="200" (
    echo [OK] API dziala na localhost:50875
) else (
    echo [BLAD] API nie odpowiada na localhost:50875
    echo        Upewnij sie, ze ReklamacjeAPI jest uruchomione
    pause
    exit /b 1
)
echo.

REM Krok 2: Pobierz lokalne IP
echo [2/4] Pobieram lokalne IP komputera...
for /f "tokens=2 delims=:" %%a in ('ipconfig ^| findstr /c:"IPv4"') do (
    set LOCAL_IP=%%a
    goto :found_ip
)
:found_ip
set LOCAL_IP=%LOCAL_IP: =%
echo [OK] Lokalne IP: %LOCAL_IP%
echo.

REM Krok 3: Sprawdź czy API jest dostępne przez lokalne IP
echo [3/4] Sprawdzam czy API jest dostepne przez lokalne IP...
curl -s -o nul -w "%%{http_code}" http://%LOCAL_IP%:50875/health > temp.txt
set /p HTTP_CODE_IP=<temp.txt
del temp.txt

if "%HTTP_CODE_IP%"=="200" (
    echo [OK] API jest dostepne przez %LOCAL_IP%:50875
) else (
    echo [OSTRZEZENIE] API nie odpowiada na %LOCAL_IP%:50875
    echo                Mozliwe ze firewall blokuje polaczenia z sieci
    echo                Telefon moze miec problem z polaczeniem
)
echo.

REM Krok 4: Podsumowanie
echo [4/4] PODSUMOWANIE
echo ================================================
echo.
echo Adres API dla telefonu: http://%LOCAL_IP%:50875
echo.
echo Krok nastepny:
echo 1. Upewnij sie, ze telefon jest w tej samej sieci WiFi
echo 2. W aplikacji Desktop: Ustawienia - Paruj telefon
echo 3. Uzyj adresu: %LOCAL_IP%
echo.
echo ALBO uzyj opcji "PARUJ PRZEZ QR" (zalecane)
echo.
echo ================================================
pause
