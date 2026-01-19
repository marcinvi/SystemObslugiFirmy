# ================================================
# Quick Start Script - Windows PowerShell
# ================================================

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  Reklamacje API - Quick Start" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET is installed
Write-Host "Checking .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ .NET SDK $dotnetVersion found" -ForegroundColor Green
} else {
    Write-Host "✗ .NET SDK not found!" -ForegroundColor Red
    Write-Host "  Download from: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

# Restore packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Failed to restore packages" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Packages restored" -ForegroundColor Green

Write-Host ""

# Build project
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Build failed" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Build successful" -ForegroundColor Green

Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  Configuration Checklist" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Check appsettings.json
if (Test-Path "appsettings.json") {
    Write-Host "✓ appsettings.json found" -ForegroundColor Green
    
    $config = Get-Content "appsettings.json" -Raw | ConvertFrom-Json
    $connString = $config.ConnectionStrings.DefaultConnection
    
    if ($connString -match "Password=your_password_here") {
        Write-Host "⚠ WARNING: Update connection string password!" -ForegroundColor Red
        Write-Host "  Edit: appsettings.Development.json" -ForegroundColor Yellow
    } else {
        Write-Host "✓ Connection string configured" -ForegroundColor Green
    }
    
    if ($config.JwtSettings.Secret -match "ChangeThis") {
        Write-Host "⚠ WARNING: Change JWT Secret!" -ForegroundColor Red
        Write-Host "  Edit: appsettings.json -> JwtSettings:Secret" -ForegroundColor Yellow
    } else {
        Write-Host "✓ JWT Secret configured" -ForegroundColor Green
    }
} else {
    Write-Host "✗ appsettings.json not found" -ForegroundColor Red
}

Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  Database Setup" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Make sure you have:" -ForegroundColor Yellow
Write-Host "1. MariaDB/MySQL running" -ForegroundColor White
Write-Host "2. ReklamacjeDB database created" -ForegroundColor White
Write-Host "3. First user created (see init_user.sql)" -ForegroundColor White

Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  Starting API..." -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

$serverIp = "<IP_SERWERA>"
try {
    $ipCandidates = Get-NetIPAddress -AddressFamily IPv4 | Where-Object {
        $_.IPAddress -and $_.IPAddress -ne "127.0.0.1" -and $_.PrefixOrigin -ne "WellKnown"
    }
    if ($ipCandidates) {
        $serverIp = $ipCandidates[0].IPAddress
    }
} catch {
    $serverIp = "<IP_SERWERA>"
}

Write-Host "API will be available at:" -ForegroundColor Yellow
Write-Host "  - HTTP:  http://localhost:5000" -ForegroundColor White
Write-Host "  - HTTPS: https://localhost:5001" -ForegroundColor White
Write-Host "  - Swagger: http://localhost:5000" -ForegroundColor White
Write-Host ""
Write-Host "For other devices on the network use:" -ForegroundColor Yellow
Write-Host "  - HTTP:  http://$serverIp`:5000" -ForegroundColor White
Write-Host "  - HTTPS: https://$serverIp`:5001" -ForegroundColor White
Write-Host ""
Write-Host "Tip: If other devices cannot connect, set:" -ForegroundColor Yellow
Write-Host "  `$env:ASPNETCORE_URLS = ""http://0.0.0.0:5000;https://0.0.0.0:5001""" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop" -ForegroundColor Gray
Write-Host ""

# Run the API
dotnet run
