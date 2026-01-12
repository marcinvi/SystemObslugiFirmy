@echo off
REM ============================================================================
REM Skrypt do uruchamiania MySQL/MariaDB
REM Uruchom jako ADMINISTRATOR!
REM ============================================================================

echo ========================================
echo  URUCHAMIANIE MySQL/MariaDB
echo ========================================
echo.

REM Sprawdź czy jesteś administratorem
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo [BŁĄD] Musisz uruchomić ten skrypt jako ADMINISTRATOR!
    echo.
    echo Kliknij prawym na plik i wybierz "Uruchom jako administrator"
    echo.
    pause
    exit /b 1
)

echo [1/4] Sprawdzam usługi MySQL/MariaDB...
echo.

REM Lista możliwych nazw usług
set SERVICE_FOUND=0

REM Sprawdź MySQL
sc query MySQL >nul 2>&1
if %errorLevel% == 0 (
    echo [OK] Znaleziono usługę: MySQL
    set SERVICE_NAME=MySQL
    set SERVICE_FOUND=1
    goto :start_service
)

REM Sprawdź MySQL80
sc query MySQL80 >nul 2>&1
if %errorLevel% == 0 (
    echo [OK] Znaleziono usługę: MySQL80
    set SERVICE_NAME=MySQL80
    set SERVICE_FOUND=1
    goto :start_service
)

REM Sprawdź MariaDB
sc query MariaDB >nul 2>&1
if %errorLevel% == 0 (
    echo [OK] Znaleziono usługę: MariaDB
    set SERVICE_NAME=MariaDB
    set SERVICE_FOUND=1
    goto :start_service
)

REM Sprawdź wampmysqld
sc query wampmysqld >nul 2>&1
if %errorLevel% == 0 (
    echo [OK] Znaleziono usługę: wampmysqld (WAMP)
    set SERVICE_NAME=wampmysqld
    set SERVICE_FOUND=1
    goto :start_service
)

REM Nie znaleziono usługi
if %SERVICE_FOUND% == 0 (
    echo [BŁĄD] Nie znaleziono żadnej usługi MySQL/MariaDB!
    echo.
    echo Możliwe rozwiązania:
    echo 1. Zainstaluj MySQL lub MariaDB
    echo 2. Użyj XAMPP Control Panel
    echo 3. Sprawdź czy nazwa usługi jest inna (services.msc)
    echo.
    pause
    exit /b 1
)

:start_service
echo.
echo [2/4] Sprawdzam status usługi %SERVICE_NAME%...
echo.

REM Sprawdź status
sc query %SERVICE_NAME% | find "RUNNING" >nul
if %errorLevel% == 0 (
    echo [OK] Usługa %SERVICE_NAME% JUŻ DZIAŁA!
    echo.
    goto :test_connection
) else (
    echo [INFO] Usługa %SERVICE_NAME% nie działa
)

echo.
echo [3/4] Uruchamiam usługę %SERVICE_NAME%...
echo.

net start %SERVICE_NAME%
if %errorLevel% == 0 (
    echo [OK] Usługa %SERVICE_NAME% została uruchomiona!
    echo.
) else (
    echo [BŁĄD] Nie można uruchomić usługi %SERVICE_NAME%!
    echo.
    echo Możliwe przyczyny:
    echo 1. Port 3306 jest zajęty przez inny proces
    echo 2. Błąd w konfiguracji MySQL
    echo 3. Brak uprawnień
    echo.
    echo Sprawdź logi w: C:\ProgramData\MySQL\MySQL Server X.X\data\*.err
    echo.
    pause
    exit /b 1
)

:test_connection
echo [4/4] Testuję połączenie...
echo.

REM Sprawdź czy port 3306 nasłuchuje
netstat -an | find "3306" | find "LISTENING" >nul
if %errorLevel% == 0 (
    echo [OK] MySQL nasłuchuje na porcie 3306
    echo.
    echo ========================================
    echo  SUKCES! MySQL działa poprawnie!
    echo ========================================
    echo.
    echo Możesz teraz uruchomić aplikację.
    echo.
) else (
    echo [OSTRZEŻENIE] Port 3306 nie nasłuchuje
    echo MySQL może nie działać poprawnie.
    echo.
)

pause
