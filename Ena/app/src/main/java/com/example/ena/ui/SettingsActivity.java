package com.example.ena.ui;

import android.content.SharedPreferences;
import android.os.Bundle;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;

import com.example.ena.Config;
import com.example.ena.PairingManager;
import com.example.ena.R;
import com.example.ena.api.ApiConfig;
import com.google.android.material.textfield.TextInputEditText;

public class SettingsActivity extends AppCompatActivity {

    private static final String PREFS_NAME = "ena_prefs";
    private static final String PREF_SHOW_SMS_LINKS = "show_sms_links";

    private TextInputEditText inputIp, inputPort;
    private TextView txtPairingInfo;
    private Button btnSaveServer, btnResetPairing;
    private ImageButton btnBack;
    private CheckBox chkShowSmsLinks;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_settings);

        initViews();
        loadCurrentSettings();
    }

    private void initViews() {
        inputIp = findViewById(R.id.inputIp);
        inputPort = findViewById(R.id.inputPort);
        txtPairingInfo = findViewById(R.id.txtPairingInfo);
        btnSaveServer = findViewById(R.id.btnSaveServer);
        btnResetPairing = findViewById(R.id.btnResetPairing);
        btnBack = findViewById(R.id.btnBack);
        chkShowSmsLinks = findViewById(R.id.chkShowSmsLinks);

        btnBack.setOnClickListener(v -> finish());
        btnSaveServer.setOnClickListener(v -> saveServerSettings());
        btnResetPairing.setOnClickListener(v -> confirmResetPairing());

        // Checkbox obsługa
        chkShowSmsLinks.setOnCheckedChangeListener((buttonView, isChecked) -> {
            SharedPreferences prefs = getSharedPreferences(PREFS_NAME, MODE_PRIVATE);
            prefs.edit().putBoolean(PREF_SHOW_SMS_LINKS, isChecked).apply();

            if (isChecked) {
                Toast.makeText(this, "Po zakończeniu rozmowy pojawi się opcja wysyłki linku SMS",
                        Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void loadCurrentSettings() {
        String fullUrl = ApiConfig.getBaseUrl(this);
        if (fullUrl != null && !fullUrl.isEmpty()) {
            String clean = fullUrl.replace("http://", "").replace("https://", "");
            String[] parts = clean.split(":");
            if (parts.length > 0) inputIp.setText(parts[0]);
            if (parts.length > 1) {
                String port = parts[1].split("/")[0];
                inputPort.setText(port);
            }
        }

        String user = PairingManager.getPairedUser(this);
        if (user != null && !user.isEmpty()) {
            txtPairingInfo.setText("Sparowano dla użytkownika: " + user);
        } else {
            txtPairingInfo.setText("Urządzenie niesparowane.");
        }

        // Załaduj ustawienie checkboxa (domyślnie wyłączony)
        SharedPreferences prefs = getSharedPreferences(PREFS_NAME, MODE_PRIVATE);
        boolean smsLinksEnabled = prefs.getBoolean(PREF_SHOW_SMS_LINKS, false);
        chkShowSmsLinks.setChecked(smsLinksEnabled);
    }

    private void saveServerSettings() {
        String ip = inputIp.getText().toString().trim();
        String port = inputPort.getText().toString().trim();

        if (ip.isEmpty() || port.isEmpty()) {
            Toast.makeText(this, "Wprowadź IP i Port", Toast.LENGTH_SHORT).show();
            return;
        }

        String newUrl = "http://" + ip + ":" + port;
        ApiConfig.setBaseUrl(this, newUrl);
        Config.saveServerUrl(this, newUrl);

        Toast.makeText(this, "Zapisano! Zrestartuj aplikację.", Toast.LENGTH_LONG).show();
    }

    private void confirmResetPairing() {
        new AlertDialog.Builder(this)
                .setTitle("Reset Parowania")
                .setMessage("Czy na pewno chcesz usunąć powiązanie z komputerem?")
                .setPositiveButton("Tak", (d, w) -> {
                    PairingManager.reset(this);
                    loadCurrentSettings();
                    Toast.makeText(this, "Zresetowano.", Toast.LENGTH_SHORT).show();
                })
                .setNegativeButton("Nie", null)
                .show();
    }
}
