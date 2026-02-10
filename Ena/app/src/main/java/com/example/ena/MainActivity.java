package com.example.ena;

import android.Manifest;
import android.app.AlertDialog;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.ImageButton;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
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
import com.example.ena.ui.UserProfileActivity;

import java.util.ArrayList;
import java.util.List;

public class MainActivity extends AppCompatActivity {

    private TextView txtUserName;
    private RecyclerView recyclerModules;
    private ProgressBar loadingModules;
    private TextView txtError;
    private ImageButton btnLogout;
    private ApiClient apiClient;
    private static final int PERMISSION_REQUEST_CODE = 100;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        if (!UserSession.isLoggedIn(this)) {
            startLogin();
            return;
        }

        setContentView(R.layout.activity_main);

        txtUserName = findViewById(R.id.txtUserName);
        recyclerModules = findViewById(R.id.recyclerModules);
        loadingModules = findViewById(R.id.loadingModules);
        txtError = findViewById(R.id.txtError);
        btnLogout = findViewById(R.id.btnLogout);

        apiClient = new ApiClient(this);
        startBackgroundService();
        requestPhonePermissions();

        String userDisplay = UserSession.getDisplayName(this);
        if (userDisplay == null || userDisplay.isEmpty()) userDisplay = UserSession.getLogin(this);
        txtUserName.setText(userDisplay);

        recyclerModules.setLayoutManager(new GridLayoutManager(this, 2));

        // Kliknięcie w nazwę użytkownika → Ustawienia
        txtUserName.setOnClickListener(v -> {
            // Otwieramy profil, w którym będzie opcja wysyłania linku
            startActivity(new Intent(MainActivity.this, UserProfileActivity.class));
        });

        btnLogout.setOnClickListener(v -> {
            UserSession.clear(this);
            stopBackgroundService();
            startLogin();
        });

        fetchModules();
    }

    // ========================================================================
    // Uprawnienia
    // ========================================================================

    private void requestPhonePermissions() {
        List<String> permissionsNeeded = new ArrayList<>();

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.READ_CALL_LOG)
                != PackageManager.PERMISSION_GRANTED) {
            permissionsNeeded.add(Manifest.permission.READ_CALL_LOG);
        }
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.READ_PHONE_STATE)
                != PackageManager.PERMISSION_GRANTED) {
            permissionsNeeded.add(Manifest.permission.READ_PHONE_STATE);
        }
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.CALL_PHONE)
                != PackageManager.PERMISSION_GRANTED) {
            permissionsNeeded.add(Manifest.permission.CALL_PHONE);
        }
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.SEND_SMS)
                != PackageManager.PERMISSION_GRANTED) {
            permissionsNeeded.add(Manifest.permission.SEND_SMS);
        }
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.RECEIVE_SMS)
                != PackageManager.PERMISSION_GRANTED) {
            permissionsNeeded.add(Manifest.permission.RECEIVE_SMS);
        }
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.TIRAMISU) {
            if (ContextCompat.checkSelfPermission(this, Manifest.permission.POST_NOTIFICATIONS)
                    != PackageManager.PERMISSION_GRANTED) {
                permissionsNeeded.add(Manifest.permission.POST_NOTIFICATIONS);
            }
        }

        if (permissionsNeeded.isEmpty()) {
            Log.d("MainActivity", "Wszystkie uprawnienia telefoniczne przyznane");
            return;
        }

        boolean needCallLogRationale = permissionsNeeded.contains(Manifest.permission.READ_CALL_LOG)
                && ActivityCompat.shouldShowRequestPermissionRationale(this, Manifest.permission.READ_CALL_LOG);

        if (needCallLogRationale) {
            new AlertDialog.Builder(this)
                    .setTitle("Uprawnienie do historii połączeń")
                    .setMessage("Aby aplikacja mogła wyświetlać numer dzwoniącego na komputerze, " +
                            "potrzebuje dostępu do historii połączeń.\n\n" +
                            "Bez tego uprawnienia połączenia przychodzące będą pokazywane " +
                            "jako \"nieznany numer\".\n\n" +
                            "Czy chcesz przyznać to uprawnienie?")
                    .setPositiveButton("Tak, przyznaj", (dialog, which) -> {
                        ActivityCompat.requestPermissions(this,
                                permissionsNeeded.toArray(new String[0]),
                                PERMISSION_REQUEST_CODE);
                    })
                    .setNegativeButton("Nie teraz", (dialog, which) -> {
                        permissionsNeeded.remove(Manifest.permission.READ_CALL_LOG);
                        if (!permissionsNeeded.isEmpty()) {
                            ActivityCompat.requestPermissions(this,
                                    permissionsNeeded.toArray(new String[0]),
                                    PERMISSION_REQUEST_CODE);
                        }
                    })
                    .setCancelable(false)
                    .show();
        } else {
            ActivityCompat.requestPermissions(this,
                    permissionsNeeded.toArray(new String[0]),
                    PERMISSION_REQUEST_CODE);
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        if (requestCode != PERMISSION_REQUEST_CODE) return;

        boolean callLogDenied = false;

        for (int i = 0; i < permissions.length; i++) {
            boolean granted = grantResults[i] == PackageManager.PERMISSION_GRANTED;
            Log.d("MainActivity", "Uprawnienie " + permissions[i] + ": " + (granted ? "PRZYZNANE" : "ODMÓWIONE"));

            if (!granted && Manifest.permission.READ_CALL_LOG.equals(permissions[i])) {
                callLogDenied = true;
            }
        }

        if (callLogDenied) {
            new AlertDialog.Builder(this)
                    .setTitle("Ograniczona funkcjonalność")
                    .setMessage("Bez dostępu do historii połączeń, " +
                            "numer dzwoniącego będzie wyświetlany jako \"nieznany\" na komputerze.\n\n" +
                            "Możesz przyznać to uprawnienie później w ustawieniach aplikacji.")
                    .setPositiveButton("OK", null)
                    .show();
        }

        try {
            IncomingCallTracker.getInstance().registerListener(this);
        } catch (Exception e) {
            Log.w("MainActivity", "Błąd rejestracji trackera po zmianie uprawnień: " + e.getMessage());
        }
    }

    // ========================================================================
    // Moduły
    // ========================================================================

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

            if (key.contains("magazyn")) {
                openReturns("warehouse");
            } else if (key.contains("handlowiec") || key.contains("sprzedaż")) {
                openReturns("sales");
            } else if (key.contains("zwroty") || key.contains("podsumowanie")) {
                startActivity(new Intent(MainActivity.this, SummaryActivity.class));
            } else if (key.contains("wiadomości") || key.contains("wiadomosci")) {
                startActivity(new Intent(MainActivity.this, MessagesActivity.class));
            } else if (key.contains("ustawienia")) {
                startActivity(new Intent(MainActivity.this, UserProfileActivity.class));
            }else if (key.contains("admin") || key.contains("zarządzanie")) {
                    // Otwieramy ekran listy użytkowników
                    startActivity(new Intent(MainActivity.this, com.example.ena.ui.AdminUsersActivity.class));
                } else if (key.contains("reklamacje")) {
                startActivity(new Intent(MainActivity.this, com.example.ena.ui.ComplaintsDashboardActivity.class));
            }

             else {
                Toast.makeText(MainActivity.this, "Moduł w przygotowaniu: " + moduleName, Toast.LENGTH_SHORT).show();
            }
        });

        recyclerModules.setAdapter(adapter);
    }

    private void openReturns(String mode) {
        Intent intent = new Intent(this, ReturnsListActivity.class);
        intent.putExtra("mode", mode);
        startActivity(intent);
    }

    // ========================================================================
    // Lifecycle
    // ========================================================================

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
