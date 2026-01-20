# ═══════════════════════════════════════════════════════════════
#  OSTATECZNA NAPRAWA - Android Ena Project
#  
#  Ten skrypt MUSI być uruchomiony jako Administrator!
#  Kliknij prawym → "Uruchom jako administrator"
# ═══════════════════════════════════════════════════════════════

# Wymaga uprawnień administratora
if (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Warning "Ten skrypt wymaga uprawnień administratora!"
    Write-Host "Kliknij prawym → 'Uruchom jako administrator'" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Naciśnij dowolny klawisz aby zamknąć..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    Exit
}

$ErrorActionPreference = "Continue"

Write-Host "╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║                                                           ║" -ForegroundColor Cyan
Write-Host "║       OSTATECZNA NAPRAWA - Android Ena                    ║" -ForegroundColor Cyan
Write-Host "║                                                           ║" -ForegroundColor Cyan
Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

$projectPath = "C:\Users\mpaprocki\Documents\GitHub\SystemObslugiFirmy\Ena"
$apiPath = "$projectPath\app\src\main\java\com\example\ena\api"

# ═══════════════════════════════════════════════════════════════
# KROK 1: Zamknij Android Studio
# ═══════════════════════════════════════════════════════════════

Write-Host "KROK 1: Zamykanie Android Studio..." -ForegroundColor Yellow
Write-Host ""

$androidStudioProcesses = Get-Process -Name "studio64" -ErrorAction SilentlyContinue
if ($androidStudioProcesses) {
    Write-Host "  ⚠️  Znaleziono $($ androidStudioProcesses.Count) proces(ów) Android Studio" -ForegroundColor Yellow
    Write-Host "  ⏸️  Zamykam Android Studio..." -ForegroundColor Yellow
    
    $androidStudioProcesses | ForEach-Object {
        try {
            $_.CloseMainWindow() | Out-Null
            Start-Sleep -Seconds 2
            if (!$_.HasExited) {
                $_ | Stop-Process -Force
            }
            Write-Host "  ✅ Zamknięto Android Studio" -ForegroundColor Green
        }
        catch {
            Write-Host "  ⚠️  Nie można zamknąć: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    
    Start-Sleep -Seconds 3
}
else {
    Write-Host "  ✅ Android Studio nie działa" -ForegroundColor Green
}

Write-Host ""

# ═══════════════════════════════════════════════════════════════
# KROK 2: Usuń duplikaty Java
# ═══════════════════════════════════════════════════════════════

Write-Host "KROK 2: Usuwanie duplikatów Java..." -ForegroundColor Yellow
Write-Host ""

$filesToDelete = @(
    "$apiPath\ReturnWarehouseUpdateRequest.java",
    "$apiPath\ReturnListItem.java",
    "$apiPath\ReturnDetails.java",
    "$apiPath\ReturnSummaryItem.java",
    "$apiPath\ReturnSummaryStats.java"
)

$deleted = 0
foreach ($file in $filesToDelete) {
    if (Test-Path $file) {
        try {
            # Usuń atrybut read-only
            if (Get-Item $file | Select-Object -ExpandProperty IsReadOnly) {
                Set-ItemProperty -Path $file -Name IsReadOnly -Value $false
            }
            
            # Wymuś usunięcie
            Remove-Item -Path $file -Force
            
            # Sprawdź czy naprawdę usunięto
            if (!(Test-Path $file)) {
                Write-Host "  ✅ USUNIĘTO: $(Split-Path $file -Leaf)" -ForegroundColor Green
                $deleted++
            }
            else {
                Write-Host "  ❌ BŁĄD: Plik nadal istnieje: $(Split-Path $file -Leaf)" -ForegroundColor Red
            }
        }
        catch {
            Write-Host "  ❌ BŁĄD: $(Split-Path $file -Leaf) - $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    else {
        Write-Host "  ℹ️  Już usunięty: $(Split-Path $file -Leaf)" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "  Usunięto: $deleted / $($filesToDelete.Count) plików" -ForegroundColor Cyan
Write-Host ""

# ═══════════════════════════════════════════════════════════════
# KROK 3: Wymuś usunięcie cache
# ═══════════════════════════════════════════════════════════════

Write-Host "KROK 3: Usuwanie cache..." -ForegroundColor Yellow
Write-Host ""

$foldersToDelete = @(
    "$projectPath\app\build",
    "$projectPath\build",
    "$projectPath\.gradle",
    "$projectPath\.idea\caches"
)

foreach ($folder in $foldersToDelete) {
    if (Test-Path $folder) {
        Write-Host "  🗑️  Usuwam: $(Split-Path $folder -Leaf)..." -ForegroundColor Yellow
        try {
            Remove-Item -Path $folder -Recurse -Force -ErrorAction Stop
            Write-Host "  ✅ Usunięto: $(Split-Path $folder -Leaf)" -ForegroundColor Green
        }
        catch {
            Write-Host "  ⚠️  Częściowo usunięto: $(Split-Path $folder -Leaf)" -ForegroundColor Yellow
        }
    }
}

Write-Host ""

# ═══════════════════════════════════════════════════════════════
# KROK 4: Weryfikacja
# ═══════════════════════════════════════════════════════════════

Write-Host "KROK 4: Weryfikacja..." -ForegroundColor Yellow
Write-Host ""

$errors = 0
foreach ($file in $filesToDelete) {
    if (Test-Path $file) {
        Write-Host "  ❌ PLIK NADAL ISTNIEJE: $(Split-Path $file -Leaf)" -ForegroundColor Red
        $errors++
    }
}

if ($errors -eq 0) {
    Write-Host "  ✅ Wszystkie duplikaty usunięte!" -ForegroundColor Green
}
else {
    Write-Host "  ❌ $errors plik(ów) nie zostało usuniętych!" -ForegroundColor Red
}

Write-Host ""

# ═══════════════════════════════════════════════════════════════
# PODSUMOWANIE
# ═══════════════════════════════════════════════════════════════

Write-Host "╔═══════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║                                                           ║" -ForegroundColor Green
Write-Host "║                    GOTOWE!                                ║" -ForegroundColor Green
Write-Host "║                                                           ║" -ForegroundColor Green
Write-Host "╚═══════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""

if ($errors -eq 0 -and $deleted -gt 0) {
    Write-Host "✅ Naprawa zakończona pomyślnie!" -ForegroundColor Green
    Write-Host ""
    Write-Host "NASTĘPNE KROKI:" -ForegroundColor Cyan
    Write-Host "  1. Otwórz Android Studio" -ForegroundColor White
    Write-Host "  2. File → Open → Wybierz folder Ena" -ForegroundColor White
    Write-Host "  3. Poczekaj na Gradle sync (2-5 min)" -ForegroundColor White
    Write-Host "  4. Build → Rebuild Project" -ForegroundColor White
    Write-Host "  5. ✅ BUILD SUCCESSFUL!" -ForegroundColor Green
}
elseif ($errors -gt 0) {
    Write-Host "⚠️  UWAGA: Niektóre pliki nie zostały usunięte!" -ForegroundColor Red
    Write-Host ""
    Write-Host "USUŃ JE RĘCZNIE:" -ForegroundColor Yellow
    Write-Host "  1. Otwórz folder w Eksploratorze:" -ForegroundColor White
    Write-Host "     $apiPath" -ForegroundColor Gray
    Write-Host "  2. Usuń pliki:" -ForegroundColor White
    foreach ($file in $filesToDelete) {
        if (Test-Path $file) {
            Write-Host "     - $(Split-Path $file -Leaf)" -ForegroundColor Red
        }
    }
    Write-Host "  3. Uruchom ten skrypt ponownie" -ForegroundColor White
}
else {
    Write-Host "ℹ️  Wszystkie pliki już były usunięte" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "JEŚLI NADAL MASZ BŁĄD KOMPILACJI:" -ForegroundColor Yellow
    Write-Host "  1. Otwórz Android Studio" -ForegroundColor White
    Write-Host "  2. Build → Clean Project" -ForegroundColor White
    Write-Host "  3. Build → Rebuild Project" -ForegroundColor White
}

Write-Host ""
Write-Host "Naciśnij dowolny klawisz aby zamknąć..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
