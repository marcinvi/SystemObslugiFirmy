package com.example.ena;

import android.Manifest;
import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import androidx.appcompat.app.AppCompatActivity;
import com.example.ena.api.ApiConfig;
import com.example.ena.ui.MessagesActivity;
import com.example.ena.ui.ReturnsListActivity;
import com.example.ena.ui.SettingsActivity;
import com.example.ena.ui.SummaryActivity;
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
    private static final MediaType JSON = MediaType.get("application/json; charset=utf-8");
    private static final OkHttpClient CLIENT = new OkHttpClient.Builder()
            .connectTimeout(5, TimeUnit.SECONDS)
            .writeTimeout(5, TimeUnit.SECONDS)
            .readTimeout(5, TimeUnit.SECONDS)
            .build();

    private final ActivityResultLauncher<ScanOptions> qrLauncher =
            registerForActivityResult(new ScanContract(), this::handleQrResult);

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        TextView txtBaseUrl = findViewById(R.id.txtBaseUrl);
        TextView txtPhoneIp = findViewById(R.id.txtPhoneIp);
        TextView txtPairCode = findViewById(R.id.txtPairCode);
        TextView txtPairingHint = findViewById(R.id.txtPairingHint);
        Button btnScanQr = findViewById(R.id.btnScanQr);
        Button btnWarehouse = findViewById(R.id.btnWarehouse);
        Button btnSales = findViewById(R.id.btnSales);
        Button btnSummary = findViewById(R.id.btnSummary);
        Button btnMessages = findViewById(R.id.btnMessages);
        Button btnSettings = findViewById(R.id.btnSettings);

        updateBaseUrlLabel(txtBaseUrl);
        updatePhoneInfo(txtPhoneIp, txtPairCode);
        updatePairingHint(txtPairingHint);
        startBackgroundServer();
        requestRuntimePermissions();

        btnScanQr.setOnClickListener(v -> startQrScan());
        btnWarehouse.setOnClickListener(v -> openReturns("warehouse"));
        btnSales.setOnClickListener(v -> openReturns("sales"));
        btnSummary.setOnClickListener(v -> startActivity(new Intent(this, SummaryActivity.class)));
        btnMessages.setOnClickListener(v -> startActivity(new Intent(this, MessagesActivity.class)));
        btnSettings.setOnClickListener(v -> startActivity(new Intent(this, SettingsActivity.class)));
    }

    @Override
    protected void onResume() {
        super.onResume();
        TextView txtBaseUrl = findViewById(R.id.txtBaseUrl);
        TextView txtPhoneIp = findViewById(R.id.txtPhoneIp);
        TextView txtPairCode = findViewById(R.id.txtPairCode);
        TextView txtPairingHint = findViewById(R.id.txtPairingHint);
        updateBaseUrlLabel(txtBaseUrl);
        updatePhoneInfo(txtPhoneIp, txtPairCode);
        updatePairingHint(txtPairingHint);
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
        boolean paired = PairingManager.isPaired(this);
        hintLabel.setVisibility(paired ? android.view.View.GONE : android.view.View.VISIBLE);
    }

    private void startQrScan() {
        ScanOptions options = new ScanOptions();
        options.setPrompt("Zeskanuj QR z komputera");
        options.setBeepEnabled(true);
        options.setOrientationLocked(true);
        options.setDesiredBarcodeFormats(ScanOptions.QR_CODE);
        qrLauncher.launch(options);
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
