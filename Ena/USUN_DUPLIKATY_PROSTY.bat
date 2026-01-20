@echo off
setlocal enabledelayedexpansion
chcp 65001 >nul
color 0C

echo.
echo ╔════════════════════════════════════════════════════════╗
echo ║                                                        ║
echo ║          USUWANIE DUPLIKATÓW - FORCE MODE              ║
echo ║                                                        ║
echo ╚════════════════════════════════════════════════════════╝
echo.

set "apiPath=C:\Users\mpaprocki\Documents\GitHub\SystemObslugiFirmy\Ena\app\src\main\java\com\example\ena\api"

if not exist "%apiPath%" (
    echo ❌ BŁĄD: Folder api nie istnieje!
    echo.
    pause
    exit /b 1
)

echo Folder: %apiPath%
echo.
echo ═══════════════════════════════════════════════════════
echo  DUPLIKATY DO USUNIĘCIA:
echo ═══════════════════════════════════════════════════════
echo.

set /a count=0
set /a found=0

if exist "%apiPath%\ReturnWarehouseUpdateRequest.java" (
    echo   ❌ ReturnWarehouseUpdateRequest.java
    set /a found+=1
)

if exist "%apiPath%\ReturnListItem.java" (
    echo   ❌ ReturnListItem.java
    set /a found+=1
)

if exist "%apiPath%\ReturnDetails.java" (
    echo   ❌ ReturnDetails.java
    set /a found+=1
)

if exist "%apiPath%\ReturnSummaryItem.java" (
    echo   ❌ ReturnSummaryItem.java
    set /a found+=1
)

if exist "%apiPath%\ReturnSummaryStats.java" (
    echo   ❌ ReturnSummaryStats.java
    set /a found+=1
)

echo.

if %found%==0 (
    color 0A
    echo ✅ Wszystkie duplikaty już usunięte!
    echo.
    echo Jeśli nadal masz błąd kompilacji:
    echo   1. Zamknij Android Studio
    echo   2. Usuń foldery: build, .gradle, app\build
    echo   3. Otwórz Android Studio
    echo   4. Build → Rebuild Project
    echo.
    pause
    exit /b 0
)

echo Znaleziono %found% duplikatów
echo.
echo ⚠️  UWAGA: Czy na pewno chcesz usunąć te pliki?
echo.
choice /C YN /M "Naciśnij Y (Tak) lub N (Nie)"

if errorlevel 2 (
    echo.
    echo Anulowano.
    pause
    exit /b 0
)

echo.
echo ═══════════════════════════════════════════════════════
echo  USUWANIE...
echo ═══════════════════════════════════════════════════════
echo.

REM Usuń każdy plik
if exist "%apiPath%\ReturnWarehouseUpdateRequest.java" (
    del /f /q "%apiPath%\ReturnWarehouseUpdateRequest.java" 2>nul
    if not exist "%apiPath%\ReturnWarehouseUpdateRequest.java" (
        echo   ✅ Usunięto: ReturnWarehouseUpdateRequest.java
        set /a count+=1
    ) else (
        echo   ❌ Nie można usunąć: ReturnWarehouseUpdateRequest.java
    )
)

if exist "%apiPath%\ReturnListItem.java" (
    del /f /q "%apiPath%\ReturnListItem.java" 2>nul
    if not exist "%apiPath%\ReturnListItem.java" (
        echo   ✅ Usunięto: ReturnListItem.java
        set /a count+=1
    ) else (
        echo   ❌ Nie można usunąć: ReturnListItem.java
    )
)

if exist "%apiPath%\ReturnDetails.java" (
    del /f /q "%apiPath%\ReturnDetails.java" 2>nul
    if not exist "%apiPath%\ReturnDetails.java" (
        echo   ✅ Usunięto: ReturnDetails.java
        set /a count+=1
    ) else (
        echo   ❌ Nie można usunąć: ReturnDetails.java
    )
)

if exist "%apiPath%\ReturnSummaryItem.java" (
    del /f /q "%apiPath%\ReturnSummaryItem.java" 2>nul
    if not exist "%apiPath%\ReturnSummaryItem.java" (
        echo   ✅ Usunięto: ReturnSummaryItem.java
        set /a count+=1
    ) else (
        echo   ❌ Nie można usunąć: ReturnSummaryItem.java
    )
)

if exist "%apiPath%\ReturnSummaryStats.java" (
    del /f /q "%apiPath%\ReturnSummaryStats.java" 2>nul
    if not exist "%apiPath%\ReturnSummaryStats.java" (
        echo   ✅ Usunięto: ReturnSummaryStats.java
        set /a count+=1
    ) else (
        echo   ❌ Nie można usunąć: ReturnSummaryStats.java
    )
)

echo.
echo ═══════════════════════════════════════════════════════

if %count%==%found% (
    color 0A
    echo.
    echo ✅ SUKCES! Usunięto %count% / %found% plików
    echo.
    echo NASTĘPNE KROKI:
    echo   1. Zamknij Android Studio (jeśli otwarty)
    echo   2. Otwórz Android Studio
    echo   3. File → Open → Folder Ena
    echo   4. Build → Clean Project
    echo   5. Build → Rebuild Project
    echo   6. ✅ BUILD SUCCESSFUL!
    echo.
) else (
    color 0E
    echo.
    echo ⚠️  UWAGA! Usunięto tylko %count% / %found% plików
    echo.
    echo Niektóre pliki mogą być zablokowane.
    echo.
    echo ROZWIĄZANIE:
    echo   1. Zamknij Android Studio
    echo   2. Uruchom ten skrypt ponownie
    echo   3. Lub usuń pliki ręcznie w Eksploratorze
    echo.
)

pause
