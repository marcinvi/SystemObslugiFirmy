package com.example.ena;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.ImageButton;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.ena.api.ApiClient;
import com.example.ena.ui.LoginActivity;
import com.example.ena.ui.MessagesActivity;
import com.example.ena.ui.ModulesAdapter;
import com.example.ena.ui.ReturnsListActivity;
import com.example.ena.ui.SettingsActivity;
import com.example.ena.ui.SummaryActivity;

import java.util.List;

public class MainActivity extends AppCompatActivity {

    private TextView txtUserName;
    private RecyclerView recyclerModules;
    private ProgressBar loadingModules;
    private TextView txtError;
    private ImageButton btnLogout;
    private ApiClient apiClient;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        // 1. Sprawdzenie sesji
        if (!UserSession.isLoggedIn(this)) {
            startLogin();
            return;
        }

        setContentView(R.layout.activity_main);

        // 2. Inicjalizacja widoków
        txtUserName = findViewById(R.id.txtUserName);
        recyclerModules = findViewById(R.id.recyclerModules);
        loadingModules = findViewById(R.id.loadingModules);
        txtError = findViewById(R.id.txtError);
        btnLogout = findViewById(R.id.btnLogout);

        apiClient = new ApiClient(this);
        startBackgroundService();

        // 3. Ustawienie nagłówka
        String userDisplay = UserSession.getDisplayName(this);
        if (userDisplay == null || userDisplay.isEmpty()) userDisplay = UserSession.getLogin(this);
        txtUserName.setText(userDisplay);

        // 4. Konfiguracja siatki (2 kolumny - nowoczesny wygląd)
        recyclerModules.setLayoutManager(new GridLayoutManager(this, 2));

        // 5. Obsługa wylogowania
        btnLogout.setOnClickListener(v -> {
            UserSession.clear(this);
            stopBackgroundService();
            startLogin();
        });

        // 6. Pobranie modułów
        fetchModules();
    }

    private void fetchModules() {
        loadingModules.setVisibility(View.VISIBLE);
        txtError.setVisibility(View.GONE);

        apiClient.fetchAssignedModules(new ApiClient.ApiCallback<List<String>>() {
            @Override
            public void onSuccess(List<String> modules) {
                runOnUiThread(() -> {
                    loadingModules.setVisibility(View.GONE);
                    if (modules == null || modules.isEmpty()) {
                        txtError.setText("Brak przypisanych modułów.\nSkontaktuj się z administratorem.");
                        txtError.setVisibility(View.VISIBLE);
                        return;
                    }
                    setupMenu(modules);
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    loadingModules.setVisibility(View.GONE);
                    txtError.setText("Błąd: " + message + "\n(Dotknij aby odświeżyć)");
                    txtError.setVisibility(View.VISIBLE);
                    txtError.setOnClickListener(v -> fetchModules());
                });
            }
        });
    }

    private void setupMenu(List<String> modules) {
        ModulesAdapter adapter = new ModulesAdapter(modules, moduleName -> {
            String key = moduleName.toLowerCase();

            // --- ROUTING MODUŁÓW ---

            if (key.contains("magazyn")) {
                // Moduł Magazyniera: Skanowanie i przyjmowanie
                openReturns("warehouse");
            }
            else if (key.contains("handlowiec") || key.contains("sprzedaż")) {
                // Moduł Handlowca: Decyzje
                openReturns("sales");
            }
            else if (key.contains("zwroty") || key.contains("podsumowanie")) {
                // Moduł Ogólny: Tabela/Przegląd
                startActivity(new Intent(MainActivity.this, SummaryActivity.class));
            }
            else if (key.contains("wiadomości") || key.contains("wiadomosci")) {
                startActivity(new Intent(MainActivity.this, MessagesActivity.class));
            }
            else if (key.contains("ustawienia")) {
                startActivity(new Intent(MainActivity.this, SettingsActivity.class));
            }
            else {
                Toast.makeText(MainActivity.this, "Moduł w przygotowaniu: " + moduleName, Toast.LENGTH_SHORT).show();
            }
        });

        recyclerModules.setAdapter(adapter);
    }

    private void openReturns(String mode) {
        Intent intent = new Intent(this, ReturnsListActivity.class);
        intent.putExtra("mode", mode); // Przekazujemy tryb do ReturnsListActivity
        startActivity(intent);
    }

    private void startLogin() {
        Intent intent = new Intent(this, LoginActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        startActivity(intent);
        finish();
    }

    private void startBackgroundService() {
        Intent serviceIntent = new Intent(this, BackgroundService.class);
        ContextCompat.startForegroundService(this, serviceIntent);
    }

    private void stopBackgroundService() {
        stopService(new Intent(this, BackgroundService.class));
    }
}
