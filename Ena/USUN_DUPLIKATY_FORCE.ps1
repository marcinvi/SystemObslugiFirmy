# KRYTYCZNA NAPRAWA - Usuń duplikaty RĘCZNIE
# PowerShell Script

Write-Host "========================================" -ForegroundColor Red
Write-Host "  USUWANIE DUPLIKATÓW - FORCE DELETE" -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Red
Write-Host ""

$apiPath = "C:\Users\mpaprocki\Documents\GitHub\SystemObslugiFirmy\Ena\app\src\main\java\com\example\ena\api"

# Lista plików do WYMUSZENIA USUNIĘCIA
$filesToDelete = @(
    "$apiPath\ReturnWarehouseUpdateRequest.java",
    "$apiPath\ReturnListItem.java",
    "$apiPath\ReturnDetails.java",
    "$apiPath\ReturnSummaryItem.java",
    "$apiPath\ReturnSummaryStats.java"
)

Write-Host "Wymuszam usunięcie duplikatów..." -ForegroundColor Yellow
Write-Host ""

$deleted = 0
foreach ($file in $filesToDelete) {
    if (Test-Path $file) {
        try {
            # Usuń atrybut read-only
            Set-ItemProperty -Path $file -Name IsReadOnly -Value $false -ErrorAction SilentlyContinue
            
            # Wymuś usunięcie
            Remove-Item -Path $file -Force -ErrorAction Stop
            
            Write-Host "  ✅ USUNIĘTO: $(Split-Path $file -Leaf)" -ForegroundColor Green
            $deleted++
        }
        catch {
            Write-Host "  ❌ BŁĄD: $(Split-Path $file -Leaf) - $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    else {
        Write-Host "  ℹ️  Nie istnieje: $(Split-Path $file -Leaf)" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Usunięto: $deleted / $($filesToDelete.Count) plików"
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if ($deleted -gt 0) {
    Write-Host "TERAZ:" -ForegroundColor Green
    Write-Host "1. Zamknij Android Studio" -ForegroundColor Yellow
    Write-Host "2. Usuń foldery:" -ForegroundColor Yellow
    Write-Host "   - Ena\app\build" -ForegroundColor Yellow
    Write-Host "   - Ena\build" -ForegroundColor Yellow
    Write-Host "   - Ena\.gradle" -ForegroundColor Yellow
    Write-Host "3. Otwórz Android Studio" -ForegroundColor Yellow
    Write-Host "4. Build → Rebuild Project" -ForegroundColor Yellow
}
else {
    Write-Host "⚠️  Żaden plik nie został usunięty!" -ForegroundColor Red
    Write-Host "   Usuń pliki RĘCZNIE w Eksploratorze Windows!" -ForegroundColor Red
}

Write-Host ""
Write-Host "Naciśnij dowolny klawisz..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
