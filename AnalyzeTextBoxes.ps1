# ========================================
# Skrypt do analizy TextBoxów w projekcie
# ========================================
# Znajduje wszystkie TextBoxy i RichTextBoxy w formularzach
# i generuje raport z możliwością dodania sprawdzania pisowni

param(
    [string]$ProjectPath = ".",
    [switch]$AddSpellCheck = $false
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Analiza TextBoxów w projekcie" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$csFiles = Get-ChildItem -Path $ProjectPath -Filter "*.Designer.cs" -Recurse

$stats = @{
    TotalForms = 0
    TotalTextBoxes = 0
    TotalRichTextBoxes = 0
    FormsWithTextBoxes = @()
}

Write-Host "Skanowanie plików Designer.cs..." -ForegroundColor Yellow
Write-Host ""

foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Raw
    
    # Pomiń pliki, które już mają sprawdzanie pisowni
    if ($content -match "SpellCheckRichTextBox|SpellCheckTextBox") {
        continue
    }
    
    $textBoxCount = ([regex]::Matches($content, "new System\.Windows\.Forms\.TextBox\(\)")).Count
    $richTextBoxCount = ([regex]::Matches($content, "new System\.Windows\.Forms\.RichTextBox\(\)")).Count
    
    if ($textBoxCount -gt 0 -or $richTextBoxCount -gt 0) {
        $stats.TotalForms++
        $stats.TotalTextBoxes += $textBoxCount
        $stats.TotalRichTextBoxes += $richTextBoxCount
        
        $formName = [System.IO.Path]::GetFileNameWithoutExtension($file.Name).Replace(".Designer", "")
        
        $formInfo = [PSCustomObject]@{
            FormName = $formName
            FilePath = $file.FullName
            TextBoxes = $textBoxCount
            RichTextBoxes = $richTextBoxCount
            Total = $textBoxCount + $richTextBoxCount
        }
        
        $stats.FormsWithTextBoxes += $formInfo
        
        Write-Host "✓ $formName" -ForegroundColor Green
        Write-Host "  TextBox: $textBoxCount, RichTextBox: $richTextBoxCount" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  PODSUMOWANIE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Formularze z kontrolkami tekstowymi: $($stats.TotalForms)" -ForegroundColor White
Write-Host "Łączna liczba TextBox: $($stats.TotalTextBoxes)" -ForegroundColor White
Write-Host "Łączna liczba RichTextBox: $($stats.TotalRichTextBoxes)" -ForegroundColor White
Write-Host "Suma wszystkich kontrolek: $($stats.TotalTextBoxes + $stats.TotalRichTextBoxes)" -ForegroundColor White
Write-Host ""

# Sortuj formularze według liczby kontrolek
$topForms = $stats.FormsWithTextBoxes | Sort-Object -Property Total -Descending | Select-Object -First 10

if ($topForms) {
    Write-Host "Top 10 formularzy z największą liczbą kontrolek tekstowych:" -ForegroundColor Yellow
    Write-Host ""
    
    $i = 1
    foreach ($form in $topForms) {
        Write-Host "  $i. $($form.FormName) - $($form.Total) kontrolek" -ForegroundColor White
        Write-Host "     (TextBox: $($form.TextBoxes), RichTextBox: $($form.RichTextBoxes))" -ForegroundColor Gray
        $i++
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  REKOMENDACJE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if ($stats.TotalForms -eq 0) {
    Write-Host "✓ Nie znaleziono formularzy z kontrolkami tekstowymi." -ForegroundColor Green
    Write-Host "  Projekt nie wymaga dodania sprawdzania pisowni." -ForegroundColor Gray
} else {
    Write-Host "Aby dodać sprawdzanie pisowni do wszystkich formularzy:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "METODA 1 - Automatyczna (zalecana):" -ForegroundColor White
    Write-Host "  1. Uruchom aplikację" -ForegroundColor Gray
    Write-Host "  2. Otwórz formularz FormSpellCheckTest" -ForegroundColor Gray
    Write-Host "  3. Kliknij 'Dodaj sprawdzanie pisowni do wszystkich formularzy'" -ForegroundColor Gray
    Write-Host "  4. Przebuduj projekt" -ForegroundColor Gray
    Write-Host ""
    Write-Host "METODA 2 - Parametr uruchomieniowy:" -ForegroundColor White
    Write-Host "  YourApp.exe --setup-spellcheck" -ForegroundColor Gray
    Write-Host ""
    Write-Host "METODA 3 - Ręczna modyfikacja:" -ForegroundColor White
    Write-Host "  Przeczytaj plik SPELLCHECK_README.md" -ForegroundColor Gray
}

# Generuj raport do pliku
$reportPath = Join-Path $ProjectPath "TextBox_Analysis_Report.txt"
$reportContent = @"
========================================
RAPORT ANALIZY TEXTBOXÓW
Data: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
========================================

STATYSTYKI:
-----------
Formularze z kontrolkami tekstowymi: $($stats.TotalForms)
Łączna liczba TextBox: $($stats.TotalTextBoxes)
Łączna liczba RichTextBox: $($stats.TotalRichTextBoxes)
Suma wszystkich kontrolek: $($stats.TotalTextBoxes + $stats.TotalRichTextBoxes)

SZCZEGÓŁOWA LISTA FORMULARZY:
------------------------------
"@

foreach ($form in ($stats.FormsWithTextBoxes | Sort-Object -Property FormName)) {
    $reportContent += "`r`n$($form.FormName)"
    $reportContent += "`r`n  Ścieżka: $($form.FilePath)"
    $reportContent += "`r`n  TextBox: $($form.TextBoxes)"
    $reportContent += "`r`n  RichTextBox: $($form.RichTextBoxes)"
    $reportContent += "`r`n  Razem: $($form.Total)`r`n"
}

$reportContent | Out-File -FilePath $reportPath -Encoding UTF8

Write-Host ""
Write-Host "Raport zapisano do pliku:" -ForegroundColor Green
Write-Host "  $reportPath" -ForegroundColor White
Write-Host ""

# Przykładowe użycie
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  UŻYCIE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Skanuj projekt:" -ForegroundColor White
Write-Host "  .\AnalyzeTextBoxes.ps1" -ForegroundColor Gray
Write-Host ""
Write-Host "Skanuj określoną ścieżkę:" -ForegroundColor White
Write-Host "  .\AnalyzeTextBoxes.ps1 -ProjectPath 'C:\Projects\MyApp'" -ForegroundColor Gray
Write-Host ""
