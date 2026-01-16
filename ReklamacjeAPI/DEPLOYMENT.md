# 🚀 DEPLOYMENT GUIDE

Przewodnik po wdrożeniu REST API na produkcję.

---

## 📋 PRE-DEPLOYMENT CHECKLIST

### ✅ Przed deploym upewnij się że:

- [ ] Connection string zaktualizowany (production DB)
- [ ] JWT Secret zmieniony (min 64 znaki losowe)
- [ ] CORS origins skonfigurowane
- [ ] HTTPS wymuszony
- [ ] appsettings.Production.json utworzony
- [ ] Folder uploads/ ma odpowiednie uprawnienia
- [ ] Backup strategy zdefiniowana
- [ ] Monitoring skonfigurowany
- [ ] Logi skonfigurowane

---

## 🔧 KONFIGURACJA PRODUCTION

### 1. Utwórz `appsettings.Production.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=production-server.com;Port=3306;Database=ReklamacjeDB;User=prod_user;Password=STRONG_PASSWORD_HERE;CharSet=utf8mb4;SslMode=Required;"
  },
  "JwtSettings": {
    "Secret": "GENERATE_RANDOM_64_CHAR_SECRET_HERE_USE_OPENSSL_RAND_BASE64_64",
    "Issuer": "ReklamacjeAPI",
    "Audience": "ReklamacjeApp",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "Cors": {
    "AllowedOrigins": [
      "https://your-domain.com",
      "https://app.your-domain.com"
    ]
  }
}
```

### 2. Generuj silny JWT Secret

**Linux/Mac:**
```bash
openssl rand -base64 64
```

**Windows (PowerShell):**
```powershell
$bytes = New-Object byte[] 64
[Security.Cryptography.RNGCryptoServiceProvider]::Create().GetBytes($bytes)
[Convert]::ToBase64String($bytes)
```

### 3. Zaktualizuj Program.cs dla Production

```csharp
// HTTPS Redirect
app.UseHttpsRedirection();

// JWT - Require HTTPS
options.RequireHttpsMetadata = true; // Change from false!

// CORS - Specific origins
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins(allowedOrigins!)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

app.UseCors("Production");
```

---

## 🪟 DEPLOYMENT: WINDOWS SERVER (IIS)

### 1. Przygotuj aplikację

```bash
# Publish projekt
dotnet publish -c Release -o ./publish

# Struktura publish/:
# - ReklamacjeAPI.dll
# - appsettings.json
# - appsettings.Production.json
# - web.config (auto-generated)
# - uploads/ (create manually)
```

### 2. Zainstaluj wymagania

1. **Windows Server 2019/2022**
2. **IIS** (Internet Information Services)
3. **.NET 8 Hosting Bundle**
   - Download: https://dotnet.microsoft.com/download/dotnet/8.0
   - Wybierz: "Hosting Bundle" for IIS

### 3. Konfiguruj IIS

**A. Utwórz Application Pool:**
1. Otwórz IIS Manager
2. Application Pools → Add Application Pool
3. Name: `ReklamacjeAPI`
4. .NET CLR version: `No Managed Code`
5. Managed pipeline mode: `Integrated`
6. Start immediately: ✓

**B. Utwórz Website:**
1. Sites → Add Website
2. Site name: `ReklamacjeAPI`
3. Application pool: `ReklamacjeAPI`
4. Physical path: `C:\inetpub\wwwroot\ReklamacjeAPI`
5. Binding:
   - Type: `https`
   - Port: `443`
   - Hostname: `api.your-domain.com`
   - SSL certificate: (choose your cert)

**C. Kopiuj pliki:**
```powershell
# Copy published files
xcopy /E /I .\publish C:\inetpub\wwwroot\ReklamacjeAPI

# Create uploads folder
mkdir C:\inetpub\wwwroot\ReklamacjeAPI\uploads

# Set permissions
icacls C:\inetpub\wwwroot\ReklamacjeAPI\uploads /grant "IIS AppPool\ReklamacjeAPI:(OI)(CI)M"
```

**D. Restart IIS:**
```powershell
iisreset
```

### 4. Sprawdź

Otwórz: `https://api.your-domain.com/health`

---

## 🐧 DEPLOYMENT: LINUX (Ubuntu 22.04)

### 1. Przygotuj serwer

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install .NET 8
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

sudo apt update
sudo apt install -y dotnet-sdk-8.0 aspnetcore-runtime-8.0

# Install Nginx (reverse proxy)
sudo apt install -y nginx

# Install MariaDB (if not on separate server)
sudo apt install -y mariadb-server
sudo mysql_secure_installation
```

### 2. Publish aplikację

```bash
# Na swoim komputerze
dotnet publish -c Release -o ./publish

# Pakuj
tar -czf reklamacje-api.tar.gz -C ./publish .

# Skopiuj na serwer
scp reklamacje-api.tar.gz user@your-server.com:/tmp/
```

### 3. Deploy na serwer

```bash
# SSH do serwera
ssh user@your-server.com

# Utwórz katalog
sudo mkdir -p /var/www/reklamacje-api
cd /var/www/reklamacje-api

# Rozpakuj
sudo tar -xzf /tmp/reklamacje-api.tar.gz -C /var/www/reklamacje-api

# Uprawnienia
sudo chown -R www-data:www-data /var/www/reklamacje-api
sudo chmod -R 755 /var/www/reklamacje-api

# Utwórz folder uploads
sudo mkdir -p /var/www/reklamacje-api/uploads
sudo chown -R www-data:www-data /var/www/reklamacje-api/uploads
```

### 4. Konfiguruj systemd service

```bash
sudo nano /etc/systemd/system/reklamacje-api.service
```

```ini
[Unit]
Description=Reklamacje REST API
After=network.target

[Service]
Type=notify
WorkingDirectory=/var/www/reklamacje-api
ExecStart=/usr/bin/dotnet /var/www/reklamacje-api/ReklamacjeAPI.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=reklamacje-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

```bash
# Włącz i uruchom
sudo systemctl enable reklamacje-api
sudo systemctl start reklamacje-api
sudo systemctl status reklamacje-api

# Logi
sudo journalctl -u reklamacje-api -f
```

### 5. Konfiguruj Nginx (Reverse Proxy)

```bash
sudo nano /etc/nginx/sites-available/reklamacje-api
```

```nginx
server {
    listen 80;
    server_name api.your-domain.com;
    
    # Redirect HTTP to HTTPS
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name api.your-domain.com;

    # SSL Certificate (Let's Encrypt)
    ssl_certificate /etc/letsencrypt/live/api.your-domain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/api.your-domain.com/privkey.pem;
    
    # SSL Configuration
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_prefer_server_ciphers on;
    ssl_ciphers ECDHE-RSA-AES256-GCM-SHA512:DHE-RSA-AES256-GCM-SHA512:ECDHE-RSA-AES256-GCM-SHA384:DHE-RSA-AES256-GCM-SHA384;

    # Proxy to Kestrel
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        
        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }
    
    # File uploads limit
    client_max_body_size 10M;
}
```

```bash
# Enable site
sudo ln -s /etc/nginx/sites-available/reklamacje-api /etc/nginx/sites-enabled/

# Test configuration
sudo nginx -t

# Restart Nginx
sudo systemctl restart nginx
```

### 6. SSL Certificate (Let's Encrypt)

```bash
# Install Certbot
sudo apt install -y certbot python3-certbot-nginx

# Get certificate
sudo certbot --nginx -d api.your-domain.com

# Auto-renewal (cron already set up by certbot)
sudo certbot renew --dry-run
```

---

## 🐳 DEPLOYMENT: DOCKER

### 1. Utwórz Dockerfile

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ReklamacjeAPI.csproj", "./"]
RUN dotnet restore "ReklamacjeAPI.csproj"

COPY . .
RUN dotnet build "ReklamacjeAPI.csproj" -c Release -o /app/build
RUN dotnet publish "ReklamacjeAPI.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Create uploads directory
RUN mkdir -p /app/uploads && chmod 755 /app/uploads

# Expose port
EXPOSE 80
EXPOSE 443

# Run app
ENTRYPOINT ["dotnet", "ReklamacjeAPI.dll"]
```

### 2. Utwórz docker-compose.yml

```yaml
version: '3.8'

services:
  api:
    build: .
    ports:
      - "5000:80"
      - "5001:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80;https://+:443
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/cert.pfx
      - ASPNETCORE_Kestrel__Certificates__Default__Password=YOUR_CERT_PASSWORD
    volumes:
      - ./uploads:/app/uploads
      - ./https:/https:ro
    depends_on:
      - db
    restart: unless-stopped

  db:
    image: mariadb:11
    environment:
      MYSQL_ROOT_PASSWORD: ROOT_PASSWORD
      MYSQL_DATABASE: ReklamacjeDB
      MYSQL_USER: api_user
      MYSQL_PASSWORD: API_USER_PASSWORD
    volumes:
      - mariadb-data:/var/lib/mysql
    ports:
      - "3306:3306"
    restart: unless-stopped

volumes:
  mariadb-data:
```

### 3. Build i Run

```bash
# Build
docker-compose build

# Run
docker-compose up -d

# Logs
docker-compose logs -f api

# Stop
docker-compose down
```

---

## 🔍 MONITORING & LOGGING

### Application Insights (Azure)

**Install package:**
```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

**Configure in Program.cs:**
```csharp
builder.Services.AddApplicationInsightsTelemetry(
    builder.Configuration["ApplicationInsights:ConnectionString"]);
```

### Serilog (Structured Logging)

**Install packages:**
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Console
```

**Configure in Program.cs:**
```csharp
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/reklamacje-api-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
```

---

## 🔄 UPDATE STRATEGY

### Zero-Downtime Deployment

**Option 1: Blue-Green Deployment**
1. Deploy new version to "green" environment
2. Test green environment
3. Switch traffic to green
4. Keep blue as backup

**Option 2: Rolling Update (Kubernetes)**
```yaml
strategy:
  type: RollingUpdate
  rollingUpdate:
    maxSurge: 1
    maxUnavailable: 0
```

### Database Migrations

```bash
# Generate migration
dotnet ef migrations add MigrationName

# Apply to production
dotnet ef database update --connection "Server=prod;..."
```

---

## 📊 HEALTH CHECKS

### Extended Health Check

**Install package:**
```bash
dotnet add package AspNetCore.HealthChecks.MariaDB
```

**Configure:**
```csharp
builder.Services.AddHealthChecks()
    .AddMySql(connectionString, name: "database");

app.MapHealthChecks("/health");
```

---

## 🔒 SECURITY BEST PRACTICES

### Production Checklist:

- [ ] HTTPS enforced (no HTTP)
- [ ] Strong JWT Secret (64+ chars)
- [ ] Database credentials secured
- [ ] CORS properly configured
- [ ] Rate limiting enabled
- [ ] Input validation everywhere
- [ ] Logs don't contain sensitive data
- [ ] File upload limits enforced
- [ ] SQL injection prevented (EF Core)
- [ ] XSS prevention (API returns JSON only)
- [ ] Regular security updates

---

## 📞 TROUBLESHOOTING

### API nie odpowiada

```bash
# Check if running
sudo systemctl status reklamacje-api

# Check logs
sudo journalctl -u reklamacje-api -n 100

# Check port
sudo netstat -tulpn | grep :5000
```

### Database connection error

```bash
# Test connection
mysql -h localhost -u user -p ReklamacjeDB

# Check firewall
sudo ufw status
sudo ufw allow 3306/tcp
```

### Permission errors (uploads/)

```bash
# Fix permissions
sudo chown -R www-data:www-data /var/www/reklamacje-api/uploads
sudo chmod -R 755 /var/www/reklamacje-api/uploads
```

---

**Date:** 2025-01-16  
**Version:** 1.0  
**Framework:** ASP.NET Core 8.0
