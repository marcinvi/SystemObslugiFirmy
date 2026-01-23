package com.example.ena.ui;

import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;
import androidx.activity.result.ActivityResultLauncher;
import androidx.appcompat.app.AppCompatActivity;
import com.example.ena.R;
import com.example.ena.api.ApiConfig;
import com.google.zxing.client.android.Intents;
import com.journeyapps.barcodescanner.ScanContract;
import com.journeyapps.barcodescanner.ScanIntentResult;
import com.journeyapps.barcodescanner.ScanOptions;
import org.json.JSONException;
import org.json.JSONObject;

public class SettingsActivity extends AppCompatActivity {
    private EditText editBaseUrl;

    private final ActivityResultLauncher<ScanOptions> qrLauncher =
            registerForActivityResult(new ScanContract(), this::handleQrResult);

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_settings);

        editBaseUrl = findViewById(R.id.editBaseUrl);
        Button btnSave = findViewById(R.id.btnSaveBaseUrl);
        Button btnScanQr = findViewById(R.id.btnScanQr);

        editBaseUrl.setText(ApiConfig.getBaseUrl(this));

        btnSave.setOnClickListener(v -> {
            ApiConfig.setBaseUrl(this, editBaseUrl.getText().toString());
            Toast.makeText(this, "Zapisano", Toast.LENGTH_SHORT).show();
            finish();
        });

        btnScanQr.setOnClickListener(v -> startQrScan());
    }

    private void startQrScan() {
        ScanOptions options = new ScanOptions();
        options.setPrompt("Zeskanuj QR z adresem API");
        options.setBeepEnabled(true);
        options.setOrientationLocked(true);
        options.setDesiredBarcodeFormats(ScanOptions.QR_CODE);
        options.addExtra(Intents.Scan.CHARACTER_SET, "UTF-8");
        qrLauncher.launch(options);
    }

    private void handleQrResult(ScanIntentResult result) {
        if (result == null || result.getContents() == null) {
            return;
        }
        String contents = result.getContents().trim();
        String baseUrl = parseBaseUrl(contents);
        if (baseUrl == null || baseUrl.isEmpty()) {
            Toast.makeText(this, "Nieprawidłowy kod QR.", Toast.LENGTH_LONG).show();
            return;
        }
        editBaseUrl.setText(baseUrl);
        Toast.makeText(this, "Wczytano adres API z QR.", Toast.LENGTH_SHORT).show();
    }

    private String parseBaseUrl(String contents) {
        if (contents == null || contents.isEmpty()) {
            return "";
        }
        String baseUrl = contents;
        if (contents.startsWith("{")) {
            try {
                JSONObject obj = new JSONObject(contents);
                baseUrl = obj.optString("apiBaseUrl", "");
            } catch (JSONException ignored) {
                baseUrl = contents;
            }
        }
        if (!baseUrl.startsWith("http://") && !baseUrl.startsWith("https://")) {
            baseUrl = "http://" + baseUrl;
        }
        return baseUrl.trim();
    }
}
