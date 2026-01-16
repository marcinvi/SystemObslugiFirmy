# 🏗️ ARCHITEKTURA REST API

## 📊 PRZEGLĄD

Projekt zbudowany w architekturze **3-warstwowej** (3-tier architecture):

```
┌─────────────────────────────────────────┐
│         CONTROLLERS                     │  ← API Endpoints (HTTP)
│  • AuthController                       │
│  • ZgloszeniaController                 │
│  • FilesController                      │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│         SERVICES                        │  ← Business Logic
│  • AuthService (JWT)                    │
│  • ZgloszeniaService                    │
│  • DzialanieService                     │
│  • FileService                          │
└──────────────┬──────────────────────────┘
               │
               ▼
┌─────────────────────────────────────────┐
│         DATA ACCESS                     │  ← Database
│  • ReklamacjeDbContext (EF Core)        │
│  • Models (Entities)                    │
└──────────────┬──────────────────────────┘
               │
               ▼
        ┌──────────────┐
        │   MARIADB    │
        │ ReklamacjeDB │
        └──────────────┘
```

---

## 📁 STRUKTURA PROJEKTU

### **Controllers/** - API Endpoints

Odpowiedzialne za:
- Przyjmowanie HTTP requests
- Walidację danych wejściowych
- Wywoływanie serwisów
- Zwracanie HTTP responses

**Pattern:** RESTful API
- `GET` - Pobieranie danych
- `POST` - Tworzenie
- `PUT` - Pełna aktualizacja
- `PATCH` - Częściowa aktualizacja
- `DELETE` - Usuwanie

### **Services/** - Logika Biznesowa

Odpowiedzialne za:
- Implementację logiki biznesowej
- Przetwarzanie danych
- Wywołania do bazy danych (przez DbContext)
- Generowanie numerów zgłoszeń
- Zarządzanie tokenami JWT

**Pattern:** Service Layer Pattern

### **Models/** - Entity Models

Odpowiedzialne za:
- Mapowanie tabel bazy danych
- Definicję relacji między encjami
- Navigation properties (EF Core)

**Pattern:** Domain Models (anemic)

### **DTOs/** - Data Transfer Objects

Odpowiedzialne za:
- Transfer danych między warstwami
- Oddzielenie modeli domeny od API contracts
- Walidację danych wejściowych

**Pattern:** DTO Pattern

### **Data/** - Database Access

Odpowiedzialne za:
- Konfigurację Entity Framework Core
- DbSets (dostęp do tabel)
- Fluent API configuration
- Relacje między tabelami

**Pattern:** Repository Pattern (implicit przez EF Core)

---

## 🔄 FLOW REQUESTA

### Przykład: GET /api/zgloszenia/moje

```
1. HTTP Request
   └─> ZgloszeniaController.GetMojeZgloszenia()
       │
       ├─ Sprawdź JWT token (Middleware)
       ├─ Pobierz userId z tokena
       │
       └─> 2. ZgloszeniaService.GetZgloszeniaAsync(userId)
           │
           ├─ Query do bazy (EF Core)
           ├─ Include relations (Klient, Produkt, Uzytkownik)
           ├─ Filtruj po userId
           ├─ Paginacja
           │
           └─> 3. DbContext.Zgloszenia
               │
               └─> 4. MariaDB Query
                   │
                   └─> 5. Zwróć dane
                       │
                       └─> 6. Map to DTOs
                           │
                           └─> 7. ApiResponse<PaginatedResponse<ZgloszenieListDto>>
                               │
                               └─> 8. HTTP Response (JSON)
```

---

## 🔐 AUTENTYKACJA & AUTORYZACJA

### JWT Flow

```
1. Login Request
   └─> AuthController.Login()
       └─> AuthService.LoginAsync()
           ├─ Sprawdź login/hasło (BCrypt)
           ├─ Generuj JWT token
           │   ├─ Claims: UserId, Login, Email
           │   ├─ Expiry: 60 minut
           │   └─ Secret: z appsettings.json
           ├─ Generuj Refresh Token
           └─> Zwróć token + user data

2. Authenticated Request
   └─> Header: Authorization: Bearer {token}
       └─> JWT Middleware
           ├─ Waliduj signature
           ├─ Sprawdź expiry
           ├─ Ekstraktuj Claims
           └─> User.Identity populated
               └─> Controller może użyć User.FindFirst()
```

### Refresh Token Flow

```
Token wygasł (401)
└─> POST /api/auth/refresh
    └─> AuthService.RefreshTokenAsync()
        ├─ Sprawdź Refresh Token w bazie
        ├─ Sprawdź expiry (7 dni)
        ├─ Generuj nowy JWT token
        ├─ Generuj nowy Refresh Token
        └─> Zwróć nowy token
```

---

## 💾 DATABASE ACCESS

### Entity Framework Core

**DbContext Configuration:**

```csharp
public class ReklamacjeDbContext : DbContext
{
    // DbSets = Tables
    public DbSet<Uzytkownik> Uzytkownicy { get; set; }
    public DbSet<Zgloszenie> Zgloszenia { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API - Relacje, constrainty, indeksy
        modelBuilder.Entity<Zgloszenie>()
            .HasOne(z => z.Klient)
            .WithMany(k => k.Zgloszenia)
            .HasForeignKey(z => z.IdKlienta)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
```

**Query Patterns:**

```csharp
// Simple query
var user = await _context.Uzytkownicy
    .FirstOrDefaultAsync(u => u.Login == "admin");

// Query with includes (JOIN)
var zgloszenie = await _context.Zgloszenia
    .Include(z => z.Klient)
    .Include(z => z.Produkt)
    .Include(z => z.UzytkownikPrzypisany)
    .FirstOrDefaultAsync(z => z.IdZgloszenia == id);

// Pagination
var items = await query
    .OrderByDescending(z => z.DataZgloszenia)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

---

## 📤 API RESPONSE FORMAT

### Standardowy format odpowiedzi

```json
{
  "success": true,
  "data": { ... },
  "message": "Operacja zakończona sukcesem",
  "timestamp": "2025-01-16T10:30:00Z"
}
```

### Paginacja

```json
{
  "success": true,
  "data": {
    "items": [ ... ],
    "page": 1,
    "pageSize": 20,
    "totalItems": 150,
    "totalPages": 8
  }
}
```

### Error Response

```json
{
  "success": false,
  "message": "Zgłoszenie nie znalezione",
  "timestamp": "2025-01-16T10:30:00Z"
}
```

---

## 🔄 DEPENDENCY INJECTION

### Registered Services (Program.cs)

```csharp
// Database
builder.Services.AddDbContext<ReklamacjeDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { ... });

// Business Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IZgloszeniaService, ZgloszeniaService>();
builder.Services.AddScoped<IDzialanieService, DzialanieService>();
builder.Services.AddScoped<IFileService, FileService>();
```

**Lifetime:**
- `AddScoped` - Nowa instancja per HTTP request
- `AddSingleton` - Jedna instancja dla całej aplikacji
- `AddTransient` - Nowa instancja za każdym razem

---

## 🎯 DESIGN PATTERNS

### 1. **Repository Pattern** (Implicit)
Entity Framework DbContext działa jako Repository

### 2. **Service Layer Pattern**
Logika biznesowa w Services, Controllers są "cienkie"

### 3. **DTO Pattern**
Oddzielenie API contracts od Domain Models

### 4. **Dependency Injection**
Wszystkie zależności przez constructor injection

### 5. **Unit of Work**
DbContext.SaveChangesAsync() = transakcje

---

## 🔒 SECURITY FEATURES

### Implemented:

1. **JWT Authentication**
   - Bearer tokens
   - Claims-based authorization
   - Refresh tokens

2. **Password Security**
   - BCrypt hashing
   - Salt automatically generated

3. **SQL Injection Prevention**
   - Parametrized queries (EF Core)
   - Input validation

4. **CORS Configuration**
   - Configurable origins
   - Method/Header restrictions

5. **File Upload Security**
   - Type validation
   - Size limits (10MB)
   - Unique filenames (GUID)

### TODO (Production):

- [ ] Rate limiting
- [ ] Request logging
- [ ] API versioning
- [ ] Health checks
- [ ] Metrics & monitoring

---

## 📈 SCALABILITY

### Current Architecture supports:

✅ **Horizontal Scaling**
- Stateless API (JWT)
- Multiple instances possible

✅ **Database Scaling**
- Read replicas (MariaDB)
- Connection pooling

✅ **Caching** (Future)
- Redis for JWT blacklist
- Response caching

✅ **Load Balancing**
- No session state
- Any instance can handle any request

---

## 🧪 TESTING STRATEGY

### Recommended Layers:

1. **Unit Tests**
   - Services (business logic)
   - DTOs mapping

2. **Integration Tests**
   - Controllers (with TestServer)
   - Database access

3. **E2E Tests**
   - Full API flows
   - Authentication

---

## 📊 PERFORMANCE

### Optimizations:

1. **Async/Await** - All database operations
2. **Select Projections** - Only needed fields
3. **Include** - Eager loading (avoid N+1)
4. **Pagination** - Limited result sets
5. **Connection Pooling** - EF Core default

### Monitoring Points:

- Query execution time
- Memory usage
- Connection pool status
- Request latency

---

## 🎉 BEST PRACTICES FOLLOWED

✅ Single Responsibility Principle  
✅ Dependency Inversion  
✅ Interface Segregation  
✅ RESTful conventions  
✅ Async programming  
✅ Proper error handling  
✅ DTOs for API contracts  
✅ Repository pattern (via EF)

---

**Data:** 2025-01-16  
**Framework:** ASP.NET Core 8.0  
**ORM:** Entity Framework Core 8.0  
**Database:** MariaDB
