# 📦 INSTRUKCJA INSTALACJI - WINDOWS

## ⚡ SZYBKI START (15 minut)

### KROK 1: Zainstaluj pakiet NuGet (5 min)

1. **Otwórz projekt w Visual Studio**
   - Otwórz `Reklamacje Dane.sln`

2. **Otwórz NuGet Package Manager**
   - Kliknij prawym na projekt → **Manage NuGet Packages**
   - LUB użyj: `Tools` → `NuGet Package Manager` → `Manage NuGet Packages for Solution`

3. **Zainstaluj QRCoder**
   - Kliknij zakładkę **Browse**
   - Wyszukaj: `QRCoder`
   - Wybierz pakiet **QRCoder** (autor: Raffael Herrmann)
   - Wybierz wersję **1.4.3** lub nowszą
   - Kliknij **Install**
   - Zaakceptuj licencję (MIT)

**Alternatywnie - Package Manager Console:**
```
Install-Package QRCoder -Version 1.4.3
```

### KROK 2: Dodaj pliki do projektu (3 min)

1. **Skopiuj pliki:**
   - Skopiuj `FormQrSync.cs` do głównego katalogu projektu
   - Skopiuj `QrCodeGenerator.cs` do głównego katalogu projektu

2. **Dodaj do projektu:**
   - W Visual Studio, kliknij prawym na projekt
   - **Add** → **Existing Item**
   - Wybierz oba pliki:
     - `FormQrSync.cs`
     - `QrCodeGenerator.cs`
   - Kliknij **Add**

### KROK 3: Dodaj przycisk w UI (5 min)

**Opcja A - Dodaj do głównego menu (Form1.cs):**

```csharp
// W Form1.cs (lub główny formularz):

// 1. Dodaj przycisk w Designer:
private System.Windows.Forms.Button btnQrSync;

// 2. W InitializeComponent():
this.btnQrSync = new System.Windows.Forms.Button();
this.btnQrSync.Location = new System.Drawing.Point(20, 400);
this.btnQrSync.Size = new System.Drawing.Size(200, 40);
this.btnQrSync.Text = "📱 Paruj przez QR Code";
this.btnQrSync.Click += new System.EventHandler(this.BtnQrSync_Click);
this.Controls.Add(this.btnQrSync);

// 3. Dodaj obsługę kliknięcia:
private void BtnQrSync_Click(object sender, EventArgs e)
{
    var qrForm = new FormQrSync();
    qrForm.ShowDialog();
}
```

**Opcja B - Dodaj do menu Settings (FormUstawienia.cs):**

```csharp
// W FormUstawienia.cs:

// Znajdź istniejący przycisk "Paruj telefon" i dodaj nowy obok:
private void btnParujQr_Click(object sender, EventArgs e)
{
    var qrForm = new FormQrSync();
    qrForm.ShowDialog();
}
```

**Opcja C - Dodaj do istniejącego FormParujTelefon.cs:**

```csharp
// W FormParujTelefon.cs, dodaj nowy przycisk:

Button btnQrMethod = new Button
{
    Location = new Point(20, 280),
    Size = new Size(460, 40),
    Text = "🎯 LUB UŻYJ QR CODE (szybciej!)",
    Font = new Font(this.Font.FontFamily, 10F, FontStyle.Bold),
    BackColor = Color.DodgerBlue,
    ForeColor = Color.White,
    FlatStyle = FlatStyle.Flat
};
btnQrMethod.Click += (s, e) => {
    var qrForm = new FormQrSync();
    if (qrForm.ShowDialog() == DialogResult.OK)
    {
        this.DialogResult = DialogResult.OK;
        this.Close();
    }
};
this.Controls.Add(btnQrMethod);
```

### KROK 4: Build & Test (2 min)

1. **Build Solution:**
   - Kliknij `Build` → `Rebuild Solution`
   - Sprawdź czy nie ma błędów

2. **Uruchom aplikację:**
   - Naciśnij `F5` lub kliknij **Start**

3. **Testuj funkcjonalność:**
   - Znajdź przycisk "Paruj przez QR Code"
   - Kliknij go
   - Powinno otworzyć się okno z QR Code
   - Sprawdź czy QR Code się wyświetla

---

## 🔧 ROZWIĄZYWANIE PROBLEMÓW

### Problem: "QRCoder not found"

**Rozwiązanie:**
1. Sprawdź czy pakiet jest zainstalowany:
   - `Tools` → `NuGet Package Manager` → `Manage NuGet Packages`
   - Zakładka **Installed**
   - Znajdź `QRCoder`

2. Jeśli nie ma, zainstaluj ponownie:
   ```
   Install-Package QRCoder -Version 1.4.3
   ```

### Problem: "NetworkAutoDiscovery nie istnieje"

**Rozwiązanie:**
- NetworkAutoDiscovery.cs powinien już istnieć w projekcie
- Jeśli nie ma go, skopiuj z głównego katalogu projektu

### Problem: "SessionManager.CurrentUser nie istnieje"

**Rozwiązanie:**
- W FormQrSync.cs zmień linię:
```csharp
// PRZED:
string userName = SessionManager.CurrentUser?.Login ?? "Użytkownik";

// PO:
string userName = System.Environment.UserName; // Nazwa użytkownika Windows
// LUB
string userName = "Administrator"; // Stała wartość
```

### Problem: QR Code się nie wyświetla

**Rozwiązanie:**
1. Sprawdź czy REST API jest uruchomione
2. Sprawdź czy jesteś w sieci lokalnej
3. Sprawdź Output Window w Visual Studio dla błędów
4. Dodaj breakpoint w `GenerateQrCode()` i debuguj

### Problem: "API URL not found"

**Rozwiązanie:**
- W FormQrSync.cs możesz ręcznie ustawić URL:
```csharp
// W GenerateQrCode(), przed var config = ...:
string apiUrl = "http://localhost:5000"; // Twój URL API
string phoneIp = NetworkAutoDiscovery.GetLocalIPAddress();

var config = QrCodeGenerator.GenerateConfig(apiUrl, phoneIp, userName);
```

---

## 📋 CHECKLIST INSTALACJI

- [ ] Zainstalowano pakiet QRCoder (NuGet)
- [ ] Skopiowano FormQrSync.cs do projektu
- [ ] Skopiowano QrCodeGenerator.cs do projektu
- [ ] Dodano pliki do projektu (Add Existing Item)
- [ ] Dodano przycisk w UI
- [ ] Build Solution - bez błędów
- [ ] Uruchomiono aplikację
- [ ] Kliknięto przycisk "Paruj przez QR Code"
- [ ] QR Code się wyświetla
- [ ] Timer działa (pasek postępu)

---

## ⚙️ KONFIGURACJA OPCJONALNA

### Zmiana czasu ważności QR Code

W `FormQrSync.cs`, zmień czas wygaśnięcia:

```csharp
// PRZED (5 minut):
qrCodeExpiryTime = config.Timestamp.AddMinutes(5);
progressBar.Maximum = 300; // 300 sekund = 5 minut

// PO (10 minut):
qrCodeExpiryTime = config.Timestamp.AddMinutes(10);
progressBar.Maximum = 600; // 600 sekund = 10 minut
```

### Zmiana rozmiaru QR Code

W `QrCodeGenerator.cs`:

```csharp
// PRZED:
private const int QR_CODE_SIZE = 20;
private const int QR_IMAGE_SIZE = 300;

// PO (większy QR):
private const int QR_CODE_SIZE = 25; // Większe moduły
private const int QR_IMAGE_SIZE = 400; // Większy obraz
```

### Dodanie logo do QR Code

W `QrCodeGenerator.cs`, w metodzie `GenerateQrCodeImage`:

```csharp
// Wczytaj logo
Bitmap logo = new Bitmap("logo.png");

Bitmap qrCodeImage = qrCode.GetGraphic(
    QR_CODE_SIZE,
    Color.Black,
    Color.White,
    logo, // Dodaj logo
    10    // Procent rozmiaru logo
);
```

---

## 🎯 NASTĘPNE KROKI

Po instalacji Windows:
1. Przejdź do `../ANDROID/INSTRUKCJA_INSTALACJI.md`
2. Zainstaluj część Android
3. Testuj połączenie między aplikacjami

---

## 📞 WSPARCIE

Jeśli masz problemy:
1. Sprawdź sekcję "Rozwiązywanie problemów" powyżej
2. Sprawdź Output Window w Visual Studio
3. Dodaj breakpointy i debuguj kod
4. Sprawdź czy wszystkie zależności są zainstalowane

---

**Status:** ✅ Gotowe do instalacji  
**Czas:** ~15 minut  
**Poziom:** Średni
