package com.example.ena;

import android.Manifest;
import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.app.AlertDialog;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.appcompat.widget.Toolbar;
import androidx.drawerlayout.widget.DrawerLayout;
import com.example.ena.api.ApiConfig;
import com.example.ena.api.ApiClient;
import com.example.ena.UserSession;
import com.example.ena.ui.MessagesActivity;
import com.example.ena.ui.LoginActivity;
import com.example.ena.ui.ReturnsListActivity;
import com.example.ena.ui.SettingsActivity;
import com.example.ena.ui.SummaryActivity;
import com.google.android.material.navigation.NavigationView;
import com.google.gson.Gson;
import com.journeyapps.barcodescanner.ScanContract;
import com.journeyapps.barcodescanner.ScanIntentResult;
import com.journeyapps.barcodescanner.ScanOptions;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;
import java.util.List;
import java.util.concurrent.TimeUnit;

import androidx.activity.result.ActivityResultLauncher;

public class MainActivity extends AppCompatActivity {
    private static final long PAIRING_STALE_MS = TimeUnit.SECONDS.toMillis(15);
    private static final long PAIRING_REFRESH_MS = TimeUnit.SECONDS.toMillis(2);
    private static final MediaType JSON = MediaType.get("application/json; charset=utf-8");
    private static final OkHttpClient CLIENT = new OkHttpClient.Builder()
            .connectTimeout(5, TimeUnit.SECONDS)
            .writeTimeout(5, TimeUnit.SECONDS)
            .readTimeout(5, TimeUnit.SECONDS)
            .build();

    private final Handler pairingHandler = new Handler(Looper.getMainLooper());
    private final Runnable pairingRefresh = new Runnable() {
        @Override
        public void run() {
            updatePairingHint(txtPairingHint);
            pairingHandler.postDelayed(this, PAIRING_REFRESH_MS);
        }
    };
    private TextView txtBaseUrl;
    private TextView txtPhoneIp;
    private TextView txtPairCode;
    private TextView txtPairingHint;
    private TextView txtUserName;
    private TextView txtCurrentModule;
    private TextView txtModulesEmpty;
    private DrawerLayout drawerLayout;
    private NavigationView navigationView;
    private TextView txtDrawerUser;
    private TextView txtDrawerStatus;
    private Button btnWarehouse;
    private Button btnSales;
    private Button btnSummary;
    private Button btnMessages;
    private Button btnSettings;

    private final ActivityResultLauncher<ScanOptions> qrLauncher =
            registerForActivityResult(new ScanContract(), this::handleQrResult);

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (!UserSession.isLoggedIn(this)) {
            redirectToLogin();
            return;
        }
        setContentView(R.layout.activity_main);

        drawerLayout = findViewById(R.id.drawerLayout);
        navigationView = findViewById(R.id.navigationView);
        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(
                this,
                drawerLayout,
                toolbar,
                R.string.navigation_drawer_open,
                R.string.navigation_drawer_close
        );
        drawerLayout.addDrawerListener(toggle);
        toggle.syncState();

        if (navigationView != null) {
            navigationView.setNavigationItemSelectedListener(item -> {
                int id = item.getItemId();
                if (id == R.id.nav_warehouse) {
                    openReturns("warehouse");
                    setCurrentModuleLabel("Magazyn");
                } else if (id == R.id.nav_sales) {
                    openReturns("sales");
                    setCurrentModuleLabel("Handlowiec");
                } else if (id == R.id.nav_summary) {
                    startActivity(new Intent(this, SummaryActivity.class));
                    setCurrentModuleLabel("Zwroty");
                } else if (id == R.id.nav_messages) {
                    startActivity(new Intent(this, MessagesActivity.class));
                    setCurrentModuleLabel("Wiadomości");
                } else if (id == R.id.nav_settings) {
                    startActivity(new Intent(this, SettingsActivity.class));
                    setCurrentModuleLabel("Ustawienia");
                } else if (id == R.id.nav_logout) {
                    logout();
                }
                drawerLayout.closeDrawers();
                return true;
            });

            if (navigationView.getHeaderCount() > 0) {
                TextView headerUser = navigationView.getHeaderView(0).findViewById(R.id.txtDrawerUser);
                TextView headerStatus = navigationView.getHeaderView(0).findViewById(R.id.txtDrawerStatus);
                txtDrawerUser = headerUser;
                txtDrawerStatus = headerStatus;
            }
        }

        txtUserName = findViewById(R.id.txtUserName);
        txtCurrentModule = findViewById(R.id.txtCurrentModule);
        txtModulesEmpty = findViewById(R.id.txtModulesEmpty);
        txtBaseUrl = findViewById(R.id.txtBaseUrl);
        txtPhoneIp = findViewById(R.id.txtPhoneIp);
        txtPairCode = findViewById(R.id.txtPairCode);
        txtPairingHint = findViewById(R.id.txtPairingHint);
        Button btnScanQr = findViewById(R.id.btnScanQr);
        Button btnResetPairing = findViewById(R.id.btnResetPairing);
        btnWarehouse = findViewById(R.id.btnWarehouse);
        btnSales = findViewById(R.id.btnSales);
        btnSummary = findViewById(R.id.btnSummary);
        btnMessages = findViewById(R.id.btnMessages);
        btnSettings = findViewById(R.id.btnSettings);

        updateUserName();
        updateBaseUrlLabel(txtBaseUrl);
        updatePhoneInfo(txtPhoneIp, txtPairCode);
        updatePairingHint(txtPairingHint);
        applyModuleVisibility(UserSession.getModules(this));
        startBackgroundServer();
        requestRuntimePermissions();

        btnScanQr.setOnClickListener(v -> startQrScan());
        btnResetPairing.setOnClickListener(v -> confirmResetPairing());
        btnWarehouse.setOnClickListener(v -> {
            openReturns("warehouse");
            setCurrentModuleLabel("Magazyn");
        });
        btnSales.setOnClickListener(v -> {
            openReturns("sales");
            setCurrentModuleLabel("Handlowiec");
        });
        btnSummary.setOnClickListener(v -> {
            startActivity(new Intent(this, SummaryActivity.class));
            setCurrentModuleLabel("Zwroty");
        });
        btnMessages.setOnClickListener(v -> {
            startActivity(new Intent(this, MessagesActivity.class));
            setCurrentModuleLabel("Wiadomości");
        });
        btnSettings.setOnClickListener(v -> {
            startActivity(new Intent(this, SettingsActivity.class));
            setCurrentModuleLabel("Ustawienia");
        });
    }

    @Override
    protected void onResume() {
        super.onResume();
        if (!UserSession.isLoggedIn(this)) {
            redirectToLogin();
            return;
        }
        updateUserName();
        updateBaseUrlLabel(txtBaseUrl);
        updatePhoneInfo(txtPhoneIp, txtPairCode);
        updatePairingHint(txtPairingHint);
        startPairingHintRefresh();
        loadAssignedModules();
    }

    @Override
    protected void onPause() {
        stopPairingHintRefresh();
        super.onPause();
    }

    private void startPairingHintRefresh() {
        pairingHandler.removeCallbacks(pairingRefresh);
        pairingHandler.post(pairingRefresh);
    }

    private void stopPairingHintRefresh() {
        pairingHandler.removeCallbacks(pairingRefresh);
    }

    private void updateBaseUrlLabel(TextView label) {
        String baseUrl = ApiConfig.getBaseUrl(this);
        if (baseUrl == null || baseUrl.isEmpty()) {
            label.setText("API: brak konfiguracji");
        } else {
            label.setText("API: " + baseUrl);
        }
    }

    private void updatePhoneInfo(TextView ipLabel, TextView codeLabel) {
        String ip = NetworkUtils.getIPAddress(true);
        if (ip == null || ip.isEmpty()) {
            ip = "brak IP";
        }
        ipLabel.setText("Telefon IP: " + ip + ":8080");
        String code = PairingManager.getOrCreateCode(this);
        codeLabel.setText("Kod parowania: " + code);
    }

    private void updatePairingHint(TextView hintLabel) {
        if (hintLabel == null) {
            return;
        }
        String statusMessage = buildPairingStatusMessage();
        hintLabel.setText(statusMessage);
        updateDrawerStatus(statusMessage);
    }

    private String formatStaleDuration(long diffMs) {
        long minutes = TimeUnit.MILLISECONDS.toMinutes(diffMs);
        if (minutes <= 0) {
            return "przed chwilą";
        }
        if (minutes == 1) {
            return "1 minutę temu";
        }
        if (minutes < 5) {
            return minutes + " minuty temu";
        }
        return minutes + " minut temu";
    }

    private void startQrScan() {
        ScanOptions options = new ScanOptions();
        options.setPrompt("Zeskanuj QR z komputera");
        options.setBeepEnabled(true);
        options.setOrientationLocked(true);
        options.setDesiredBarcodeFormats(ScanOptions.QR_CODE);
        qrLauncher.launch(options);
    }

    private void confirmResetPairing() {
        new AlertDialog.Builder(this)
                .setTitle("Rozłącz parowanie")
                .setMessage("Czy na pewno chcesz wyczyścić parowanie? Telefon będzie można przypisać do innego użytkownika.")
                .setPositiveButton("Tak, rozłącz", (dialog, which) -> resetPairing())
                .setNegativeButton("Anuluj", null)
                .show();
    }

    private void resetPairing() {
        PairingManager.reset(this);
        updateUserName();
        updatePairingHint(txtPairingHint);
        updatePhoneInfo(txtPhoneIp, txtPairCode);
        showToast("Parowanie zostało wyczyszczone.");
    }

    private void handleQrResult(ScanIntentResult result) {
        if (result.getContents() == null) {
            return;
        }

        try {
            QrPairingPayload payload = new Gson().fromJson(result.getContents(), QrPairingPayload.class);
            if (payload == null || payload.pcIp == null || payload.pcIp.isEmpty() || payload.pcPort <= 0) {
                Toast.makeText(this, "Niepoprawny QR parowania.", Toast.LENGTH_LONG).show();
                return;
            }

            if (payload.apiBaseUrl != null && !payload.apiBaseUrl.isEmpty()) {
                ApiConfig.setBaseUrl(this, payload.apiBaseUrl);
                Config.saveServerUrl(this, payload.apiBaseUrl);
            }

            if (payload.user != null) {
                PairingManager.setPairedUser(this, payload.user);
            }

            sendPairingRequest(payload);
        } catch (Exception ex) {
            Toast.makeText(this, "Błąd odczytu QR: " + ex.getMessage(), Toast.LENGTH_LONG).show();
        }
    }

    private void sendPairingRequest(QrPairingPayload payload) {
        String phoneIp = NetworkUtils.getIPAddress(true);
        if (phoneIp == null || phoneIp.isEmpty()) {
            Toast.makeText(this, "Brak IP telefonu.", Toast.LENGTH_LONG).show();
            return;
        }

        String pairingCode = PairingManager.getOrCreateCode(this);
        QrPairingRequest requestPayload = new QrPairingRequest(payload.token, phoneIp, pairingCode);
        String json = new Gson().toJson(requestPayload);

        new Thread(() -> {
            try {
                String url = "http://" + payload.pcIp + ":" + payload.pcPort + "/pair";
                RequestBody body = RequestBody.create(json, JSON);
                Request request = new Request.Builder()
                        .url(url)
                        .post(body)
                        .build();

                try (Response response = CLIENT.newCall(request).execute()) {
                    if (!response.isSuccessful()) {
                        showToast("Błąd parowania: HTTP " + response.code());
                        return;
                    }
                }
                showToast("Wysłano dane parowania do komputera.");
            } catch (Exception ex) {
                showToast("Błąd parowania: " + ex.getMessage());
            }
        }).start();
    }

    private void showToast(String message) {
        runOnUiThread(() -> Toast.makeText(this, message, Toast.LENGTH_LONG).show());
    }

    private void startBackgroundServer() {
        Intent intent = new Intent(this, BackgroundService.class);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            startForegroundService(intent);
        } else {
            startService(intent);
        }
    }

    private void requestRuntimePermissions() {
        String[] permissions;
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.TIRAMISU) {
            permissions = new String[] {
                    Manifest.permission.RECEIVE_SMS,
                    Manifest.permission.READ_SMS,
                    Manifest.permission.SEND_SMS,
                    Manifest.permission.READ_PHONE_STATE,
                    Manifest.permission.CALL_PHONE,
                    Manifest.permission.READ_MEDIA_IMAGES,
                    Manifest.permission.POST_NOTIFICATIONS
            };
        } else {
            permissions = new String[] {
                    Manifest.permission.RECEIVE_SMS,
                    Manifest.permission.READ_SMS,
                    Manifest.permission.SEND_SMS,
                    Manifest.permission.READ_PHONE_STATE,
                    Manifest.permission.CALL_PHONE,
                    Manifest.permission.READ_EXTERNAL_STORAGE
            };
        }
        List<String> missing = new java.util.ArrayList<>();
        for (String permission : permissions) {
            if (ContextCompat.checkSelfPermission(this, permission) != android.content.pm.PackageManager.PERMISSION_GRANTED) {
                missing.add(permission);
            }
        }
        if (!missing.isEmpty()) {
            ActivityCompat.requestPermissions(this, missing.toArray(new String[0]), 1001);
        }
    }

    private void openReturns(String mode) {
        Intent intent = new Intent(this, ReturnsListActivity.class);
        intent.putExtra(ReturnsListActivity.EXTRA_MODE, mode);
        startActivity(intent);
    }

    private void updateUserName() {
        String user = UserSession.getDisplayName(this);
        if (user == null || user.isEmpty()) {
            user = UserSession.getLogin(this);
        }
        if (user == null || user.isEmpty()) {
            user = "Nie sparowano";
        }
        if (txtUserName != null) {
            txtUserName.setText("Użytkownik: " + user);
        }
        if (txtDrawerUser != null) {
            txtDrawerUser.setText("Użytkownik: " + user);
        }
    }

    private void updateDrawerStatus(String status) {
        if (txtDrawerStatus != null) {
            txtDrawerStatus.setText(status);
        }
    }

    private void setCurrentModuleLabel(String moduleName) {
        if (txtCurrentModule != null && moduleName != null && !moduleName.isEmpty()) {
            txtCurrentModule.setText(moduleName);
        }
    }

    private String buildPairingStatusMessage() {
        if (PairingManager.isPaired(this)) {
            String user = PairingManager.getPairedUser(this);
            long lastSeen = PairingManager.getLastSeen(this);
            long now = System.currentTimeMillis();
            boolean active = lastSeen > 0 && now - lastSeen <= PAIRING_STALE_MS;
            if (active) {
                if (user == null || user.isEmpty()) {
                    return "Telefon sparowany z aplikacją. Połączenie aktywne.";
                }
                return "Telefon sparowany z użytkownikiem: " + user + ". Połączenie aktywne.";
            }

            String staleSuffix = lastSeen > 0 ? " Ostatni kontakt: " + formatStaleDuration(now - lastSeen) + "." : "";
            if (user == null || user.isEmpty()) {
                return "Telefon sparowany, ale brak połączenia z komputerem." + staleSuffix;
            }
            return "Telefon sparowany z użytkownikiem: " + user + ", ale brak połączenia z komputerem." + staleSuffix;
        }

        return "Hej! Jestem gotowy do pracy! Zeskanuj kod z aplikacji na Windows :)";
    }

    private void loadAssignedModules() {
        ApiClient apiClient = new ApiClient(this);
        apiClient.fetchAssignedModules(new ApiClient.ApiCallback<java.util.List<String>>() {
            @Override
            public void onSuccess(java.util.List<String> data) {
                runOnUiThread(() -> {
                    UserSession.saveModules(MainActivity.this, data);
                    applyModuleVisibility(data);
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> applyModuleVisibility(UserSession.getModules(MainActivity.this)));
            }
        });
    }

    private void applyModuleVisibility(java.util.List<String> modules) {
        boolean allowWarehouse = hasModule(modules, "Magazyn");
        boolean allowSales = hasModule(modules, "Handlowiec");
        boolean allowSummary = hasModule(modules, "Zwroty");
        boolean allowMessages = hasModule(modules, "Wiadomosci") || hasModule(modules, "Wiadomości");

        setVisible(btnWarehouse, allowWarehouse);
        setVisible(btnSales, allowSales);
        setVisible(btnSummary, allowSummary);
        setVisible(btnMessages, allowMessages);

        if (navigationView != null) {
            if (navigationView.getMenu().findItem(R.id.nav_warehouse) != null) {
                navigationView.getMenu().findItem(R.id.nav_warehouse).setVisible(allowWarehouse);
            }
            if (navigationView.getMenu().findItem(R.id.nav_sales) != null) {
                navigationView.getMenu().findItem(R.id.nav_sales).setVisible(allowSales);
            }
            if (navigationView.getMenu().findItem(R.id.nav_summary) != null) {
                navigationView.getMenu().findItem(R.id.nav_summary).setVisible(allowSummary);
            }
            if (navigationView.getMenu().findItem(R.id.nav_messages) != null) {
                navigationView.getMenu().findItem(R.id.nav_messages).setVisible(allowMessages);
            }
        }

        boolean anyModule = allowWarehouse || allowSales || allowSummary || allowMessages;
        setVisible(txtModulesEmpty, !anyModule);
        if (!anyModule && txtCurrentModule != null) {
            txtCurrentModule.setText("Brak modułów");
        }
    }

    private boolean hasModule(java.util.List<String> modules, String name) {
        if (modules == null || name == null) {
            return false;
        }
        for (String module : modules) {
            if (module != null && module.equalsIgnoreCase(name)) {
                return true;
            }
        }
        return false;
    }

    private void setVisible(android.view.View view, boolean visible) {
        if (view != null) {
            view.setVisibility(visible ? android.view.View.VISIBLE : android.view.View.GONE);
        }
    }

    private void logout() {
        UserSession.clear(this);
        redirectToLogin();
    }

    private void redirectToLogin() {
        Intent intent = new Intent(this, LoginActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        startActivity(intent);
        finish();
    }

    static class QrPairingPayload {
        String pcIp;
        int pcPort;
        String token;
        String user;
        String apiBaseUrl;
    }

    static class QrPairingRequest {
        String token;
        String phoneIp;
        String pairingCode;

        QrPairingRequest(String token, String phoneIp, String pairingCode) {
            this.token = token;
            this.phoneIp = phoneIp;
            this.pairingCode = pairingCode;
        }
    }
}
