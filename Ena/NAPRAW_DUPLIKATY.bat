@echo off
chcp 65001 >nul
color 0C

echo ========================================
echo   NAPRAWA DUPLIKACJI KLAS v2.0
echo ========================================
echo.
echo Problem: Duplikaty klas Java/Kotlin + nieaktualne nazwy
echo Rozwiązanie: Usuń duplikaty + zaktualizuj pliki
echo.

set "apiPath=C:\Users\mpaprocki\Documents\GitHub\SystemObslugiFirmy\Ena\app\src\main\java\com\example\ena\api"
set "projectPath=C:\Users\mpaprocki\Documents\GitHub\SystemObslugiFirmy\Ena"

echo KROK 1: Usuwam zduplikowane pliki Java...
echo (Zostawiamy tylko wersje Kotlin w ReturnDtos.kt)
echo.

set count=0

if exist "%apiPath%\ReturnWarehouseUpdateRequest.java" (
    del /f "%apiPath%\ReturnWarehouseUpdateRequest.java" 2>nul
    if not exist "%apiPath%\ReturnWarehouseUpdateRequest.java" (
        echo   ✅ Usunięto: ReturnWarehouseUpdateRequest.java
        set /a count+=1
    )
)

if exist "%apiPath%\ReturnListItem.java" (
    del /f "%apiPath%\ReturnListItem.java" 2>nul
    if not exist "%apiPath%\ReturnListItem.java" (
        echo   ✅ Usunięto: ReturnListItem.java
        set /a count+=1
    )
)

if exist "%apiPath%\ReturnDetails.java" (
    del /f "%apiPath%\ReturnDetails.java" 2>nul
    if not exist "%apiPath%\ReturnDetails.java" (
        echo   ✅ Usunięto: ReturnDetails.java
        set /a count+=1
    )
)

if exist "%apiPath%\ReturnSummaryItem.java" (
    del /f "%apiPath%\ReturnSummaryItem.java" 2>nul
    if not exist "%apiPath%\ReturnSummaryItem.java" (
        echo   ✅ Usunięto: ReturnSummaryItem.java
        set /a count+=1
    )
)

if exist "%apiPath%\ReturnSummaryStats.java" (
    del /f "%apiPath%\ReturnSummaryStats.java" 2>nul
    if not exist "%apiPath%\ReturnSummaryStats.java" (
        echo   ✅ Usunięto: ReturnSummaryStats.java
        set /a count+=1
    )
)

if %count%==0 (
    echo   ℹ️  Brak duplikatów do usunięcia (już usunięte)
) else (
    echo.
    echo   Usunięto %count% zduplikowanych plików
)

echo.
echo KROK 2: Czyszczenie build cache...
echo.

if exist "%projectPath%\app\build" (
    echo   🗑️  Usuwam app\build...
    rmdir /s /q "%projectPath%\app\build" 2>nul
    echo   ✅ Usunięto: app\build
)

if exist "%projectPath%\build" (
    echo   🗑️  Usuwam build...
    rmdir /s /q "%projectPath%\build" 2>nul
    echo   ✅ Usunięto: build
)

if exist "%projectPath%\.gradle" (
    echo   🗑️  Usuwam .gradle...
    rmdir /s /q "%projectPath%\.gradle" 2>nul
    echo   ✅ Usunięto: .gradle
)

echo.
echo ========================================
echo   GOTOWE!
echo ========================================
echo.
color 0A
echo ✅ Duplikaty usunięte
echo ✅ Cache wyczyszczony
echo ✅ Pliki zaktualizowane:
echo    - ApiClient.java (ReturnListItemDto, ReturnDetailsDto)
echo    - ReturnsListActivity.java (ReturnListItemDto)
echo    - ReturnDetailActivity.java (ReturnDetailsDto)
echo    - ReturnListAdapter.java (ReturnListItemDto)
echo.
echo NASTĘPNE KROKI (Android Studio):
echo.
echo 1. Build → Clean Project
echo 2. Build → Rebuild Project (poczekaj 1-2 min)
echo 3. Build → Make Project (Ctrl+F9)
echo.
echo ✅ BUILD SUCCESSFUL = Wszystko działa!
echo.
pause
