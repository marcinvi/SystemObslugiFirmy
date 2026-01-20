# NAPRAW PROJEKT ENA - AUTOMATYCZNY SKRYPT
# Uruchom ten skrypt w PowerShell jako Administrator

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  NAPRAWA PROJEKTU ANDROID ENA" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = "C:\Users\mpaprocki\Documents\GitHub\SystemObslugiFirmy\Ena"

# Sprawdź czy ścieżka istnieje
if (-not (Test-Path $projectPath)) {
    Write-Host "BŁĄD: Nie znaleziono projektu w: $projectPath" -ForegroundColor Red
    Write-Host "Naciśnij dowolny klawisz aby zakończyć..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

Write-Host "Znaleziono projekt w: $projectPath" -ForegroundColor Green
Write-Host ""

# KROK 1: Usuń stare pliki Groovy
Write-Host "KROK 1: Usuwam stare pliki Groovy (.gradle)..." -ForegroundColor Yellow

$filesToDelete = @(
    "$projectPath\build.gradle",
    "$projectPath\settings.gradle",
    "$projectPath\app\build.gradle"
)

foreach ($file in $filesToDelete) {
    if (Test-Path $file) {
        Remove-Item $file -Force
        Write-Host "  ✅ Usunięto: $file" -ForegroundColor Green
    } else {
        Write-Host "  ⏭️  Pominięto (nie istnieje): $file" -ForegroundColor Gray
    }
}

Write-Host ""

# KROK 2: Usuń foldery cache
Write-Host "KROK 2: Usuwam foldery cache..." -ForegroundColor Yellow

$foldersToDelete = @(
    "$projectPath\.gradle",
    "$projectPath\.idea",
    "$projectPath\build",
    "$projectPath\app\build"
)

foreach ($folder in $foldersToDelete) {
    if (Test-Path $folder) {
        Remove-Item $folder -Recurse -Force
        Write-Host "  ✅ Usunięto: $folder" -ForegroundColor Green
    } else {
        Write-Host "  ⏭️  Pominięto (nie istnieje): $folder" -ForegroundColor Gray
    }
}

Write-Host ""

# KROK 3: Sprawdź pliki Kotlin DSL
Write-Host "KROK 3: Sprawdzam pliki Kotlin DSL (.kts)..." -ForegroundColor Yellow

$requiredFiles = @(
    "$projectPath\build.gradle.kts",
    "$projectPath\settings.gradle.kts",
    "$projectPath\app\build.gradle.kts"
)

$allFilesExist = $true
foreach ($file in $requiredFiles) {
    if (Test-Path $file) {
        Write-Host "  ✅ Znaleziono: $file" -ForegroundColor Green
    } else {
        Write-Host "  ❌ BRAK: $file" -ForegroundColor Red
        $allFilesExist = $false
    }
}

Write-Host ""

# PODSUMOWANIE
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  PODSUMOWANIE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if ($allFilesExist) {
    Write-Host "✅ Projekt gotowy do otwarcia w Android Studio!" -ForegroundColor Green
    Write-Host ""
    Write-Host "NASTĘPNE KROKI:" -ForegroundColor Yellow
    Write-Host "1. Otwórz Android Studio"
    Write-Host "2. File → Open"
    Write-Host "3. Wybierz folder: $projectPath"
    Write-Host "4. W Settings → Gradle ustaw Gradle JDK: Embedded JDK (jbr)"
    Write-Host "5. Poczekaj na Gradle sync (2-3 minuty)"
    Write-Host ""
} else {
    Write-Host "⚠️  UWAGA: Brakuje niektórych plików!" -ForegroundColor Red
    Write-Host "Skontaktuj się z developerem."
    Write-Host ""
}

Write-Host "Naciśnij dowolny klawisz aby zakończyć..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
