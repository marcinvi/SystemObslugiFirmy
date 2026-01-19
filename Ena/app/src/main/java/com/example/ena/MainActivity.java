package com.example.ena;

import android.Manifest;
import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.widget.Button;
import android.widget.TextView;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import androidx.appcompat.app.AppCompatActivity;
import com.example.ena.api.ApiConfig;
import com.example.ena.ui.MessagesActivity;
import com.example.ena.ui.ReturnsListActivity;
import com.example.ena.ui.SettingsActivity;
import com.example.ena.ui.SummaryActivity;
import java.util.List;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        TextView txtBaseUrl = findViewById(R.id.txtBaseUrl);
        TextView txtPhoneIp = findViewById(R.id.txtPhoneIp);
        TextView txtPairCode = findViewById(R.id.txtPairCode);
        TextView txtPairedUser = findViewById(R.id.txtPairedUser);
        Button btnWarehouse = findViewById(R.id.btnWarehouse);
        Button btnSales = findViewById(R.id.btnSales);
        Button btnSummary = findViewById(R.id.btnSummary);
        Button btnMessages = findViewById(R.id.btnMessages);
        Button btnSettings = findViewById(R.id.btnSettings);

        updateBaseUrlLabel(txtBaseUrl);
        updatePhoneInfo(txtPhoneIp, txtPairCode, txtPairedUser);
        startBackgroundServer();
        requestRuntimePermissions();

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
        TextView txtPairedUser = findViewById(R.id.txtPairedUser);
        updateBaseUrlLabel(txtBaseUrl);
        updatePhoneInfo(txtPhoneIp, txtPairCode, txtPairedUser);
    }

    private void updateBaseUrlLabel(TextView label) {
        String baseUrl = ApiConfig.getBaseUrl(this);
        if (baseUrl == null || baseUrl.isEmpty()) {
            label.setText("API: brak konfiguracji");
        } else {
            label.setText("API: " + baseUrl);
        }
    }

    private void updatePhoneInfo(TextView ipLabel, TextView codeLabel, TextView userLabel) {
        String ip = NetworkUtils.getIPAddress(true);
        if (ip == null || ip.isEmpty()) {
            ip = "brak IP";
        }
        ipLabel.setText("Telefon IP: " + ip + ":8080");
        String code = PairingManager.getOrCreateCode(this);
        codeLabel.setText("Kod parowania: " + code);
        String user = PairingManager.getPairedUser(this);
        if (user == null || user.isEmpty()) {
            userLabel.setText("Użytkownik: nie sparowano");
        } else {
            userLabel.setText("Użytkownik: " + user);
        }
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
}
