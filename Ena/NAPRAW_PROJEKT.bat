@echo off
chcp 65001 >nul
color 0A

echo ========================================
echo   NAPRAWA PROJEKTU ANDROID ENA v2.0
echo ========================================
echo.

set "projectPath=C:\Users\mpaprocki\Documents\GitHub\SystemObslugiFirmy\Ena"

if not exist "%projectPath%" (
    echo ❌ BŁĄD: Nie znaleziono projektu!
    pause
    exit /b 1
)

echo ✅ Znaleziono projekt: %projectPath%
echo.

echo ========================================
echo KROK 1: Usuwam problematyczne pliki
echo ========================================
echo.

REM Usuń stare pliki Groovy (.gradle)
set deleted=0

if exist "%projectPath%\build.gradle" (
    del /f "%projectPath%\build.gradle" 2>nul
    if not exist "%projectPath%\build.gradle" (
        echo   ✅ Usunięto: build.gradle
        set /a deleted+=1
    )
) else (
    echo   ⏭️  Pominięto: build.gradle
)

if exist "%projectPath%\settings.gradle" (
    del /f "%projectPath%\settings.gradle" 2>nul
    if not exist "%projectPath%\settings.gradle" (
        echo   ✅ Usunięto: settings.gradle
        set /a deleted+=1
    )
) else (
    echo   ⏭️  Pominięto: settings.gradle
)

if exist "%projectPath%\app\build.gradle" (
    del /f "%projectPath%\app\build.gradle" 2>nul
    if not exist "%projectPath%\app\build.gradle" (
        echo   ✅ Usunięto: app\build.gradle
        set /a deleted+=1
    )
) else (
    echo   ⏭️  Pominięto: app\build.gradle
)

echo.
echo ========================================
echo KROK 2: Czyszczenie cache i build
echo ========================================
echo.

REM Usuń foldery cache
if exist "%projectPath%\.gradle" (
    echo   🗑️  Usuwam .gradle...
    rmdir /s /q "%projectPath%\.gradle" 2>nul
    echo   ✅ Usunięto: .gradle
)

if exist "%projectPath%\.idea" (
    echo   🗑️  Usuwam .idea...
    rmdir /s /q "%projectPath%\.idea" 2>nul
    echo   ✅ Usunięto: .idea
)

if exist "%projectPath%\build" (
    echo   🗑️  Usuwam build...
    rmdir /s /q "%projectPath%\build" 2>nul
    echo   ✅ Usunięto: build
)

if exist "%projectPath%\app\build" (
    echo   🗑️  Usuwam app\build...
    rmdir /s /q "%projectPath%\app\build" 2>nul
    echo   ✅ Usunięto: app\build
)

REM Usuń local.properties (może być przestarzały)
if exist "%projectPath%\local.properties" (
    echo   🗑️  Usuwam local.properties...
    del /f "%projectPath%\local.properties" 2>nul
    echo   ✅ Usunięto: local.properties
)

echo.
echo ========================================
echo KROK 3: Weryfikacja plików
echo ========================================
echo.

set errors=0

if exist "%projectPath%\build.gradle.kts" (
    echo   ✅ build.gradle.kts - OK
) else (
    echo   ❌ BRAK: build.gradle.kts
    set /a errors+=1
)

if exist "%projectPath%\settings.gradle.kts" (
    echo   ✅ settings.gradle.kts - OK
) else (
    echo   ❌ BRAK: settings.gradle.kts
    set /a errors+=1
)

if exist "%projectPath%\app\build.gradle.kts" (
    echo   ✅ app\build.gradle.kts - OK
) else (
    echo   ❌ BRAK: app\build.gradle.kts
    set /a errors+=1
)

if exist "%projectPath%\gradle\wrapper\gradle-wrapper.properties" (
    echo   ✅ gradle-wrapper.properties - OK
) else (
    echo   ❌ BRAK: gradle-wrapper.properties
    set /a errors+=1
)

echo.
echo ========================================
echo PODSUMOWANIE
echo ========================================
echo.

if %errors%==0 (
    echo ✅ PROJEKT GOTOWY!
    echo.
    echo 📋 NASTĘPNE KROKI:
    echo.
    echo 1. Otwórz Android Studio
    echo 2. File → Close Project (jeśli coś otwarte)
    echo 3. File → Open
    echo 4. Wybierz folder: %projectPath%
    echo 5. Kliknij OK
    echo 6. Poczekaj na Gradle sync (2-5 minut przy pierwszym razie)
    echo.
    echo 💡 WSKAZÓWKI:
    echo    - Jeśli sync fails, sprawdź Internet
    echo    - Jeśli brakuje SDK: Tools → SDK Manager → Android 14.0
    echo    - Jeśli Gradle JDK error: Settings → Gradle → Embedded JDK (jbr)
    echo.
) else (
    echo ❌ ZNALEZIONO BŁĘDY!
    echo.
    echo Brakuje %errors% plik(ów).
    echo Skontaktuj się z developerem.
    echo.
)

echo ========================================
echo.
pause
