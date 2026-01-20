package com.example.ena;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import com.example.ena.api.ApiConfig;
import com.google.gson.Gson;
import com.google.gson.JsonSyntaxException;
import com.google.zxing.integration.android.IntentIntegrator;
import com.google.zxing.integration.android.IntentResult;

/**
 * Aktywność do skanowania QR Code i automatycznej konfiguracji połączenia
 */
public class QrScanActivity extends AppCompatActivity {

    private static final int CAMERA_PERMISSION_REQUEST = 1001;
    private TextView txtStatus;
    private TextView txtDetails;
    private Button btnScan;
    private Button btnManual;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_qr_scan);

        // Inicjalizacja widoków
        txtStatus = findViewById(R.id.txtStatus);
        txtDetails = findViewById(R.id.txtDetails);
        btnScan = findViewById(R.id.btnScan);
        btnManual = findViewById(R.id.btnManual);

        // Obsługa przycisku skanowania
        btnScan.setOnClickListener(v -> checkCameraPermissionAndScan());

        // Obsługa przycisku ręcznej konfiguracji
        btnManual.setOnClickListener(v -> {
            // Przejdź do ręcznej konfiguracji (jeśli istnieje taki ekran)
            Toast.makeText(this, "Ręczna konfiguracja - TODO", Toast.LENGTH_SHORT).show();
            finish();
        });

        // Sprawdź czy aplikacja jest już skonfigurowana
        checkIfAlreadyConfigured();
    }

    /**
     * Sprawdza czy aplikacja jest już skonfigurowana
     */
    private void checkIfAlreadyConfigured() {
        String apiUrl = ApiConfig.getBaseUrl(this);
        boolean isPaired = PairingManager.isPaired(this);

        if (apiUrl != null && !apiUrl.isEmpty() && isPaired) {
            txtStatus.setText("✅ Aplikacja już skonfigurowana");
            txtDetails.setText(
                    "API: " + apiUrl + "\n" +
                    "Użytkownik: " + PairingManager.getPairedUser(this) + "\n\n" +
                    "Możesz zeskanować nowy QR Code aby zmienić konfigurację."
            );
        } else {
            txtStatus.setText("📱 Gotowy do skanowania");
            txtDetails.setText("Zeskanuj QR Code z aplikacji Windows aby automatycznie skonfigurować połączenie.");
        }
    }

    /**
     * Sprawdza uprawnienia do kamery i uruchamia skaner
     */
    private void checkCameraPermissionAndScan() {
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.CAMERA)
                != PackageManager.PERMISSION_GRANTED) {
            // Brak uprawnień - poproś o nie
            ActivityCompat.requestPermissions(this,
                    new String[]{Manifest.permission.CAMERA},
                    CAMERA_PERMISSION_REQUEST);
        } else {
            // Uprawnienia już przyznane - uruchom skaner
            startQrScanner();
        }
    }

    /**
     * Obsługa wyniku żądania uprawnień
     */
    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions,
                                          @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);

        if (requestCode == CAMERA_PERMISSION_REQUEST) {
            if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                // Uprawnienie przyznane - uruchom skaner
                startQrScanner();
            } else {
                // Uprawnienie odrzucone
                Toast.makeText(this,
                        "Uprawnienie do kamery jest wymagane do skanowania QR Code",
                        Toast.LENGTH_LONG).show();
            }
        }
    }

    /**
     * Uruchamia skaner QR Code
     */
    private void startQrScanner() {
        IntentIntegrator integrator = new IntentIntegrator(this);
        integrator.setDesiredBarcodeFormats(IntentIntegrator.QR_CODE);
        integrator.setPrompt("Zeskanuj QR Code z aplikacji Windows");
        integrator.setCameraId(0);  // Przednia kamera
        integrator.setBeepEnabled(true);
        integrator.setBarcodeImageEnabled(true);
        integrator.setOrientationLocked(true);
        integrator.initiateScan();
    }

    /**
     * Obsługa wyniku skanowania
     */
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        IntentResult result = IntentIntegrator.parseActivityResult(requestCode, resultCode, data);
        if (result != null) {
            if (result.getContents() == null) {
                // Anulowano skanowanie
                Toast.makeText(this, "Anulowano skanowanie", Toast.LENGTH_SHORT).show();
            } else {
                // Zeskanowano kod
                processQrCode(result.getContents());
            }
        } else {
            super.onActivityResult(requestCode, resultCode, data);
        }
    }

    /**
     * Przetwarza zeskanowany QR Code
     */
    private void processQrCode(String qrData) {
        try {
            txtStatus.setText("⏳ Przetwarzanie...");
            txtDetails.setText("Sprawdzam dane z QR Code...");

            // Parsuj JSON
            Gson gson = new Gson();
            QrConfigModel qrConfig = gson.fromJson(qrData, QrConfigModel.class);

            // Waliduj dane
            String validationError = QrConfigValidator.validate(qrConfig);
            if (validationError != null) {
                showError("Błąd walidacji", validationError);
                return;
            }

            // Pokaż potwierdzenie
            showConfirmationDialog(qrConfig);

        } catch (JsonSyntaxException e) {
            showError("Niepoprawny QR Code",
                    "QR Code nie zawiera poprawnych danych konfiguracyjnych.\n\n" +
                    "Upewnij się, że skanujesz kod z aplikacji Windows.");
        } catch (Exception e) {
            showError("Błąd", "Wystąpił nieoczekiwany błąd: " + e.getMessage());
        }
    }

    /**
     * Pokazuje dialog z potwierdzeniem konfiguracji
     */
    private void showConfirmationDialog(QrConfigModel qrConfig) {
        String message = "✅ Znaleziono konfigurację!\n\n" +
                "API: " + qrConfig.config.apiBaseUrl + "\n" +
                "Komputer: " + qrConfig.config.phoneIp + "\n" +
                "Użytkownik: " + qrConfig.config.userName + "\n" +
                "Kod: " + qrConfig.config.pairingCode + "\n\n" +
                "Czy zastosować tę konfigurację?";

        new AlertDialog.Builder(this)
                .setTitle("📱 Konfiguracja")
                .setMessage(message)
                .setPositiveButton("✅ ZASTOSUJ", (dialog, which) -> applyConfiguration(qrConfig))
                .setNegativeButton("❌ ANULUJ", (dialog, which) -> {
                    txtStatus.setText("❌ Anulowano");
                    txtDetails.setText("Konfiguracja nie została zastosowana.");
                })
                .setCancelable(false)
                .show();
    }

    /**
     * Aplikuje konfigurację z QR Code
     */
    private void applyConfiguration(QrConfigModel qrConfig) {
        try {
            txtStatus.setText("⏳ Aplikuję konfigurację...");

            // 1. Zapisz URL API
            ApiConfig.setBaseUrl(this, qrConfig.config.apiBaseUrl);

            // 2. Zapisz IP komputera (dla serwera HTTP)
            Config.saveServerUrl(this, "http://" + qrConfig.config.phoneIp + ":8080");

            // 3. Sparuj telefon
            PairingManager.setPaired(this, true);
            PairingManager.setPairedUser(this, qrConfig.config.userName);

            // 4. Wykonaj parowanie z kodem (opcjonalne - weryfikacja)
            performPairing(qrConfig.config.phoneIp, qrConfig.config.pairingCode);

            // 5. Pokaż sukces
            txtStatus.setText("✅ SUKCES!");
            txtDetails.setText(
                    "Aplikacja została skonfigurowana!\n\n" +
                    "API: " + qrConfig.config.apiBaseUrl + "\n" +
                    "Użytkownik: " + qrConfig.config.userName + "\n\n" +
                    "Możesz teraz korzystać z pełnej funkcjonalności aplikacji."
            );

            // 6. Po 2 sekundach wróć do MainActivity
            btnScan.postDelayed(() -> {
                Intent intent = new Intent(QrScanActivity.this, MainActivity.class);
                intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP | Intent.FLAG_ACTIVITY_NEW_TASK);
                startActivity(intent);
                finish();
            }, 2000);

        } catch (Exception e) {
            showError("Błąd konfiguracji",
                    "Nie udało się zastosować konfiguracji: " + e.getMessage());
        }
    }

    /**
     * Wykonuje parowanie z serwerem HTTP na telefonie (weryfikacja)
     */
    private void performPairing(String phoneIp, String pairingCode) {
        // To jest opcjonalne - możesz to pominąć jeśli nie potrzebujesz dodatkowej weryfikacji
        // W obecnym systemie, sparowanie następuje po prostu przez ustawienie flagi
        // ale możesz dodać tutaj dodatkowe sprawdzenie
    }

    /**
     * Pokazuje błąd
     */
    private void showError(String title, String message) {
        txtStatus.setText("❌ " + title);
        txtDetails.setText(message);

        new AlertDialog.Builder(this)
                .setTitle(title)
                .setMessage(message)
                .setPositiveButton("OK", null)
                .show();
    }
}
