# Skrypt PowerShell do znalezienia wszystkich wystąpień "Nazwa Wyświetlana" w cudzysłowach
# Data: 2026-01-08

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "SZUKAM BŁĘDNYCH ZAPYTAŃ SQL" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = "C:\Users\mpaprocki\Desktop\dosql"
$pattern = '\"Nazwa Wyświetlana\"'

Write-Host "Ścieżka projektu: $projectPath" -ForegroundColor Yellow
Write-Host "Szukam wzorca: $pattern" -ForegroundColor Yellow
Write-Host ""

$files = Get-ChildItem -Path $projectPath -Filter "*.cs" -Recurse -ErrorAction SilentlyContinue

$foundFiles = @()

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
    
    if ($content -match $pattern) {
        $foundFiles += $file
        
        Write-Host "ZNALEZIONO: $($file.Name)" -ForegroundColor Red
        
        # Pokaż linie z błędem
        $lines = Get-Content $file.FullName
        for ($i = 0; $i -lt $lines.Length; $i++) {
            if ($lines[$i] -match $pattern) {
                Write-Host "  Linia $($i+1): $($lines[$i].Trim())" -ForegroundColor Gray
            }
        }
        Write-Host ""
    }
}

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "PODSUMOWANIE" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

if ($foundFiles.Count -eq 0) {
    Write-Host "✅ NIE ZNALEZIONO BŁĘDNYCH ZAPYTAŃ!" -ForegroundColor Green
    Write-Host "Wszystkie zapytania używają poprawnej składni!" -ForegroundColor Green
} else {
    Write-Host "❌ Znaleziono $($foundFiles.Count) plików z błędnymi zapytaniami!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Pliki do naprawienia:" -ForegroundColor Yellow
    foreach ($file in $foundFiles) {
        Write-Host "  - $($file.Name)" -ForegroundColor Yellow
    }
    Write-Host ""
    Write-Host "NAPRAWA:" -ForegroundColor Cyan
    Write-Host "Zamień wszystkie wystąpienia:" -ForegroundColor White
    Write-Host '  \"Nazwa Wyświetlana\"  →  `Nazwa Wyświetlana`' -ForegroundColor White
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "INNE KOLUMNY Z SPACJAMI:" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Sprawdź również czy w projekcie nie ma innych kolumn" -ForegroundColor Yellow
Write-Host "z spacjami w nazwach. Uruchom w MySQL:" -ForegroundColor Yellow
Write-Host ""
Write-Host "SELECT TABLE_NAME, COLUMN_NAME" -ForegroundColor White
Write-Host "FROM INFORMATION_SCHEMA.COLUMNS" -ForegroundColor White
Write-Host "WHERE TABLE_SCHEMA = DATABASE()" -ForegroundColor White
Write-Host "AND COLUMN_NAME LIKE '% %';" -ForegroundColor White
Write-Host ""

Write-Host "Naciśnij Enter aby zakończyć..."
Read-Host
