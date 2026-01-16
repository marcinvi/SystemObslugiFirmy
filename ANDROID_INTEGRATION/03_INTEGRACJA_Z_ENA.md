# 🔄 INTEGRACJA Z ISTNIEJĄCĄ APLIKACJĄ ENA

## 📱 AKTUALNA SYTUACJA

Masz już działającą aplikację Android **ENA** z ważnymi funkcjami!

### ✅ CO ENA JUŻ ROBI:

```
ENA (Android) - Serwer HTTP na porcie 8080
├── GET /stan              → Status połączenia (dzwoni? jaki numer?)
├── GET /sms               → Lista 50 ostatnich SMS (JSON)
├── GET /wyslij            → Wysłanie SMS (params: numer, tresc)
├── GET /lista_zdjec       → Lista zdjęć z galerii
├── GET /miniaturka?id=X   → Miniaturka zdjęcia
└── GET /pobierz_zdjecie?id=X → Pełne zdjęcie
```

**Komponenty:**
- `MainActivity` - Serwer HTTP (NanoHTTPD:8080)
- `CallReceiver` - Broadcast receiver dla połączeń przychodzących
- Uprawnienia: READ_SMS, SEND_SMS, READ_CALL_LOG, READ_PHONE_STATE, READ_EXTERNAL_STORAGE

---

## 🎯 STRATEGIA INTEGRACJI: HYBRYDOWA ⭐

Zachowaj ENA + Dodaj nowe funkcje dla zgłoszeń!

```
┌────────────────────────────────────────────────────────┐
│              ANDROID APP (Rozszerzona ENA)             │
│                                                        │
│  ┌─────────────────────┐  ┌────────────────────────┐ │
│  │   ENA (istniejące)  │  │  NOWE MODUŁY           │ │
│  │                     │  │                        │ │
│  │  • Serwer HTTP      │  │  • REST API Client     │ │
│  │  • SMS Handler      │  │  • Zgłoszenia UI       │ │
│  │  • Call Receiver    │  │  • Status Update       │ │
│  │  • Photo Gallery    │  │  • Notatki             │ │
│  └─────────────────────┘  └────────────────────────┘ │
└───────────┬────────────────────┬───────────────────────┘
            │                    │
    Windows Form          REST API Server
    (legacy SMS/calls)    (zgłoszenia z bazy)
```

**Dlaczego to najlepsze?**
- ✅ Zachowujesz WSZYSTKIE działające funkcje
- ✅ Zero ryzyka - nie ruszasz sprawdzonego kodu
- ✅ Dodajesz tylko nowe funkcje (zgłoszenia)
- ✅ Windows Form dalej może wysyłać SMS przez ENA
- ✅ Stopniowa migracja - każda funkcja działa osobno

---

## 📁 NOWA STRUKTURA PROJEKTU

```
Ena/app/src/main/java/com/example/ena/
│
├── MainActivity.java           ✅ NIE RUSZAJ (HTTP Server 8080)
├── CallReceiver.java           ✅ NIE RUSZAJ (Połączenia)
│
├── api/                        🆕 DODAJ - REST API
│   ├── ReklamacjeApiClient.java
│   ├── RetrofitInstance.java
│   └── models/
│       ├── Zgloszenie.java
│       ├── ApiResponse.java
│       └── ...
│
├── ui/                         🆕 DODAJ - Nowe ekrany
│   ├── LoginActivity.java
│   ├── ZgloszeniaActivity.java
│   ├── ZgloszenieDetailsActivity.java
│   └── adapters/
│       └── ZgloszeniaAdapter.java
│
└── utils/                      🆕 DODAJ - Narzędzia
    ├── TokenManager.java
    └── Config.java
```

---

## 🚀 PLAN IMPLEMENTACJI (4-6 TYGODNI)

### **TYDZIEŃ 1: Setup + REST API Client**

#### 1.1 Aktualizuj build.gradle

**Ena/app/build.gradle:**

```gradle
dependencies {
    // ✅ Istniejące (nie usuwaj!)
    implementation 'org.nanohttpd:nanohttpd:2.3.1'
    
    // 🆕 DODAJ dla REST API
    implementation 'com.squareup.retrofit2:retrofit:2.9.0'
    implementation 'com.squareup.retrofit2:converter-gson:2.9.0'
    implementation 'com.squareup.okhttp3:okhttp:4.12.0'
    implementation 'com.squareup.okhttp3:logging-interceptor:4.12.0'
    
    // 🆕 DODAJ dla UI
    implementation 'androidx.recyclerview:recyclerview:1.3.2'
    implementation 'androidx.cardview:cardview:1.0.0'
    implementation 'com.google.android.material:material:1.11.0'
}
```

Kliknij **"Sync Now"**

#### 1.2 Stwórz Config.java

**Ena/app/src/main/java/com/example/ena/utils/Config.java:**

```java
package com.example.ena.utils;

public class Config {
    // 🔧 ZMIEŃ NA ADRES SWOJEGO REST API!
    public static final String API_BASE_URL = "https://api.reklamacje.pl/";
    
    // Port lokalnego serwera ENA (nie zmieniaj)
    public static final int ENA_PORT = 8080;
}
```

#### 1.3 Stwórz modele danych

**Ena/app/src/main/java/com/example/ena/api/models/ApiResponse.java:**

```java
package com.example.ena.api.models;

import com.google.gson.annotations.SerializedName;

public class ApiResponse<T> {
    @SerializedName("success")
    private boolean success;
    
    @SerializedName("data")
    private T data;
    
    @SerializedName("message")
    private String message;
    
    @SerializedName("timestamp")
    private String timestamp;
    
    // Getters
    public boolean isSuccess() { return success; }
    public T getData() { return data; }
    public String getMessage() { return message; }
}
```

**Zgloszenie.java:**

```java
package com.example.ena.api.models;

import com.google.gson.annotations.SerializedName;

public class Zgloszenie {
    @SerializedName("id")
    private int id;
    
    @SerializedName("nrZgloszenia")
    private String nrZgloszenia;
    
    @SerializedName("dataZgloszenia")
    private String dataZgloszenia;
    
    @SerializedName("statusOgolny")
    private String statusOgolny;
    
    @SerializedName("usterka")
    private String usterka;
    
    @SerializedName("klient")
    private Klient klient;
    
    @SerializedName("produkt")
    private Produkt produkt;
    
    // Getters & Setters
    public int getId() { return id; }
    public String getNrZgloszenia() { return nrZgloszenia; }
    public String getStatusOgolny() { return statusOgolny; }
    public String getUsterka() { return usterka; }
    public Klient getKlient() { return klient; }
    public Produkt getProdukt() { return produkt; }
    
    public static class Klient {
        @SerializedName("id")
        private int id;
        
        @SerializedName("imieNazwisko")
        private String imieNazwisko;
        
        @SerializedName("telefon")
        private String telefon;
        
        public String getImieNazwisko() { return imieNazwisko; }
        public String getTelefon() { return telefon; }
    }
    
    public static class Produkt {
        @SerializedName("nazwa")
        private String nazwa;
        
        @SerializedName("producent")
        private String producent;
        
        public String getNazwa() { return nazwa; }
        public String getProducent() { return producent; }
    }
}
```

#### 1.4 Stwórz ReklamacjeApiClient

**Ena/app/src/main/java/com/example/ena/api/ReklamacjeApiClient.java:**

```java
package com.example.ena.api;

import com.example.ena.api.models.*;
import retrofit2.Call;
import retrofit2.http.*;
import java.util.List;

public interface ReklamacjeApiClient {
    
    @POST("api/auth/login")
    Call<ApiResponse<LoginResponse>> login(@Body LoginRequest request);
    
    @GET("api/zgloszenia/moje")
    Call<ApiResponse<PaginatedResponse<Zgloszenie>>> getZgloszenia(
        @Header("Authorization") String token,
        @Query("page") int page,
        @Query("pageSize") int pageSize
    );
    
    @GET("api/zgloszenia/{id}")
    Call<ApiResponse<Zgloszenie>> getZgloszenieById(
        @Header("Authorization") String token,
        @Path("id") int id
    );
    
    @PATCH("api/zgloszenia/{id}/status")
    Call<ApiResponse<Zgloszenie>> updateStatus(
        @Header("Authorization") String token,
        @Path("id") int id,
        @Body StatusUpdateRequest request
    );
}
```

#### 1.5 Stwórz RetrofitInstance

**Ena/app/src/main/java/com/example/ena/api/RetrofitInstance.java:**

```java
package com.example.ena.api;

import com.example.ena.utils.Config;
import okhttp3.OkHttpClient;
import okhttp3.logging.HttpLoggingInterceptor;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;
import java.util.concurrent.TimeUnit;

public class RetrofitInstance {
    private static Retrofit retrofit;
    
    public static Retrofit getClient() {
        if (retrofit == null) {
            HttpLoggingInterceptor logging = new HttpLoggingInterceptor();
            logging.setLevel(HttpLoggingInterceptor.Level.BODY);
            
            OkHttpClient client = new OkHttpClient.Builder()
                .addInterceptor(logging)
                .connectTimeout(30, TimeUnit.SECONDS)
                .readTimeout(30, TimeUnit.SECONDS)
                .writeTimeout(30, TimeUnit.SECONDS)
                .build();
            
            retrofit = new Retrofit.Builder()
                .baseUrl(Config.API_BASE_URL)
                .client(client)
                .addConverterFactory(GsonConverterFactory.create())
                .build();
        }
        return retrofit;
    }
    
    public static ReklamacjeApiClient getApiClient() {
        return getClient().create(ReklamacjeApiClient.class);
    }
}
```

---

### **TYDZIEŃ 2: Login + Lista Zgłoszeń**

#### 2.1 TokenManager

**Ena/app/src/main/java/com/example/ena/utils/TokenManager.java:**

```java
package com.example.ena.utils;

import android.content.Context;
import android.content.SharedPreferences;

public class TokenManager {
    private static final String PREFS_NAME = "EnaPrefs";
    private static final String KEY_TOKEN = "jwt_token";
    private static final String KEY_USER_NAME = "user_name";
    
    public static void saveToken(Context context, String token) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        prefs.edit().putString(KEY_TOKEN, token).apply();
    }
    
    public static String getToken(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        return prefs.getString(KEY_TOKEN, null);
    }
    
    public static void saveUserName(Context context, String name) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        prefs.edit().putString(KEY_USER_NAME, name).apply();
    }
    
    public static String getUserName(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        return prefs.getString(KEY_USER_NAME, "");
    }
    
    public static void clearToken(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        prefs.edit().clear().apply();
    }
    
    public static boolean isLoggedIn(Context context) {
        return getToken(context) != null;
    }
}
```

#### 2.2 LoginActivity

**Ena/app/src/main/java/com/example/ena/ui/LoginActivity.java:**

```java
package com.example.ena.ui;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import com.example.ena.R;
import com.example.ena.api.RetrofitInstance;
import com.example.ena.api.models.*;
import com.example.ena.utils.TokenManager;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class LoginActivity extends AppCompatActivity {
    private EditText etLogin, etPassword;
    private Button btnLogin;
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        
        // Sprawdź czy już zalogowany
        if (TokenManager.isLoggedIn(this)) {
            goToZgloszenia();
            return;
        }
        
        setContentView(R.layout.activity_login);
        
        etLogin = findViewById(R.id.etLogin);
        etPassword = findViewById(R.id.etPassword);
        btnLogin = findViewById(R.id.btnLogin);
        
        btnLogin.setOnClickListener(v -> login());
    }
    
    private void login() {
        String login = etLogin.getText().toString();
        String password = etPassword.getText().toString();
        
        if (login.isEmpty() || password.isEmpty()) {
            Toast.makeText(this, "Wypełnij wszystkie pola", Toast.LENGTH_SHORT).show();
            return;
        }
        
        btnLogin.setEnabled(false);
        btnLogin.setText("Logowanie...");
        
        LoginRequest request = new LoginRequest(login, password);
        
        RetrofitInstance.getApiClient().login(request).enqueue(new Callback<ApiResponse<LoginResponse>>() {
            @Override
            public void onResponse(Call<ApiResponse<LoginResponse>> call, Response<ApiResponse<LoginResponse>> response) {
                btnLogin.setEnabled(true);
                btnLogin.setText("Zaloguj");
                
                if (response.isSuccessful() && response.body() != null && response.body().isSuccess()) {
                    LoginResponse data = response.body().getData();
                    TokenManager.saveToken(LoginActivity.this, data.getToken());
                    TokenManager.saveUserName(LoginActivity.this, data.getUser().getNazwaWyswietlana());
                    
                    Toast.makeText(LoginActivity.this, "Zalogowano!", Toast.LENGTH_SHORT).show();
                    goToZgloszenia();
                } else {
                    Toast.makeText(LoginActivity.this, "Błąd logowania", Toast.LENGTH_SHORT).show();
                }
            }
            
            @Override
            public void onFailure(Call<ApiResponse<LoginResponse>> call, Throwable t) {
                btnLogin.setEnabled(true);
                btnLogin.setText("Zaloguj");
                Toast.makeText(LoginActivity.this, "Błąd: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
    
    private void goToZgloszenia() {
        startActivity(new Intent(this, ZgloszeniaActivity.class));
        finish();
    }
}
```

#### 2.3 Layout dla LoginActivity

**Ena/app/src/main/res/layout/activity_login.xml:**

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:orientation="vertical"
    android:padding="24dp"
    android:gravity="center">
    
    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="ENA - Logowanie"
        android:textSize="24sp"
        android:textStyle="bold"
        android:layout_marginBottom="32dp"/>
    
    <EditText
        android:id="@+id/etLogin"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:hint="Login"
        android:inputType="text"
        android:layout_marginBottom="16dp"/>
    
    <EditText
        android:id="@+id/etPassword"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:hint="Hasło"
        android:inputType="textPassword"
        android:layout_marginBottom="24dp"/>
    
    <Button
        android:id="@+id/btnLogin"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:text="Zaloguj"
        android:textSize="16sp"/>
</LinearLayout>
```

#### 2.4 Aktualizuj MainActivity (dodaj przycisk)

**MainActivity.java (TYLKO DODAJ na końcu onCreate):**

```java
// Na końcu onCreate() dodaj:

Button btnZgloszenia = new Button(this);
btnZgloszenia.setText("📋 ZGŁOSZENIA");
btnZgloszenia.setTextSize(18);
btnZgloszenia.setPadding(40, 40, 40, 40);
btnZgloszenia.setOnClickListener(v -> {
    startActivity(new Intent(this, com.example.ena.ui.LoginActivity.class));
});

// Dodaj button do layoutu (na końcu ScrollView)
((LinearLayout) scroll.getChildAt(0)).addView(btnZgloszenia);
```

#### 2.5 Aktualizuj AndroidManifest.xml

**Ena/app/src/main/AndroidManifest.xml (DODAJ):**

```xml
<!-- DODAJ te activity do <application> -->
<activity
    android:name=".ui.LoginActivity"
    android:exported="false"/>

<activity
    android:name=".ui.ZgloszeniaActivity"
    android:exported="false"/>

<activity
    android:name=".ui.ZgloszenieDetailsActivity"
    android:exported="false"/>
```

---

### **TYDZIEŃ 3: Lista Zgłoszeń**

#### 3.1 ZgloszeniaAdapter

**Ena/app/src/main/java/com/example/ena/ui/adapters/ZgloszeniaAdapter.java:**

```java
package com.example.ena.ui.adapters;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.R;
import com.example.ena.api.models.Zgloszenie;
import java.util.List;

public class ZgloszeniaAdapter extends RecyclerView.Adapter<ZgloszeniaAdapter.ViewHolder> {
    private List<Zgloszenie> zgloszenia;
    private OnItemClickListener listener;
    
    public interface OnItemClickListener {
        void onItemClick(Zgloszenie zgloszenie);
    }
    
    public ZgloszeniaAdapter(List<Zgloszenie> zgloszenia, OnItemClickListener listener) {
        this.zgloszenia = zgloszenia;
        this.listener = listener;
    }
    
    @NonNull
    @Override
    public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.item_zgloszenie, parent, false);
        return new ViewHolder(view);
    }
    
    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
        Zgloszenie z = zgloszenia.get(position);
        holder.tvNumer.setText(z.getNrZgloszenia());
        holder.tvKlient.setText(z.getKlient().getImieNazwisko());
        holder.tvProdukt.setText(z.getProdukt().getNazwa());
        holder.tvStatus.setText(z.getStatusOgolny());
        
        holder.itemView.setOnClickListener(v -> {
            if (listener != null) listener.onItemClick(z);
        });
    }
    
    @Override
    public int getItemCount() {
        return zgloszenia.size();
    }
    
    static class ViewHolder extends RecyclerView.ViewHolder {
        TextView tvNumer, tvKlient, tvProdukt, tvStatus;
        
        ViewHolder(View view) {
            super(view);
            tvNumer = view.findViewById(R.id.tvNumer);
            tvKlient = view.findViewById(R.id.tvKlient);
            tvProdukt = view.findViewById(R.id.tvProdukt);
            tvStatus = view.findViewById(R.id.tvStatus);
        }
    }
}
```

#### 3.2 Layout item_zgloszenie.xml

**Ena/app/src/main/res/layout/item_zgloszenie.xml:**

```xml
<?xml version="1.0" encoding="utf-8"?>
<androidx.cardview.widget.CardView xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="wrap_content"
    android:layout_margin="8dp"
    app:cardCornerRadius="8dp"
    app:cardElevation="4dp">
    
    <LinearLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:orientation="vertical"
        android:padding="16dp">
        
        <TextView
            android:id="@+id/tvNumer"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="R/123/2025"
            android:textSize="18sp"
            android:textStyle="bold"/>
        
        <TextView
            android:id="@+id/tvKlient"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Jan Kowalski"
            android:textSize="14sp"
            android:layout_marginTop="4dp"/>
        
        <TextView
            android:id="@+id/tvProdukt"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Laptop Dell XPS 15"
            android:textSize="14sp"
            android:layout_marginTop="4dp"/>
        
        <TextView
            android:id="@+id/tvStatus"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="W realizacji"
            android:textSize="14sp"
            android:textColor="#FF9800"
            android:textStyle="bold"
            android:layout_marginTop="8dp"/>
    </LinearLayout>
</androidx.cardview.widget.CardView>
```

---

## ⏱️ TIMELINE KOMPLETNY

```
TYDZIEŃ 1: Setup + REST API Client ✅
TYDZIEŃ 2: Login + TokenManager ✅
TYDZIEŃ 3: Lista zgłoszeń (RecyclerView)
TYDZIEŃ 4: Szczegóły + Zmiana statusu
TYDZIEŃ 5: Notatki + Integracja SMS (ENA)
TYDZIEŃ 6: Upload zdjęć + Polish

TOTAL: 6 TYGODNI
```

---

## 📊 CO ZOSTANIE ZACHOWANE vs CO NOWE

| Funkcja | ENA (zachowane) | Nowe (dodane) |
|---------|-----------------|---------------|
| **Serwer HTTP 8080** | ✅ Bez zmian | - |
| **SMS odczyt/wysyłka** | ✅ Działa jak było | 🆕 + Wysyłka z poziomu zgłoszenia |
| **Połączenia** | ✅ CallReceiver działa | - |
| **Zdjęcia galeria** | ✅ Bez zmian | 🆕 + Upload do zgłoszenia |
| **Zgłoszenia** | - | 🆕 Lista, szczegóły, statusy |
| **Login JWT** | - | 🆕 Autentykacja z REST API |
| **Synchronizacja** | - | 🆕 Z bazą MariaDB |

---

## 🎉 REZULTAT

Po implementacji będziesz miał:

1. ✅ **ENA działa dalej** - SMS, połączenia, zdjęcia (port 8080)
2. 🆕 **Nowe funkcje** - zgłoszenia z bazy, statusy, notatki (REST API)
3. 🔗 **Integracja** - możesz wysłać SMS z poziomu zgłoszenia
4. 💪 **Zero ryzyka** - stary kod nietknięty
5. 🚀 **Gotowe do rozbudowy** - łatwo dodać więcej funkcji

---

**Następny krok:** Implementuj tydzień po tygodniu według tego dokumentu!

**Data:** 2025-01-16  
**Wersja:** 1.0 - Integracja Hybrydowa
