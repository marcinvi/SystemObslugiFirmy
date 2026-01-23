package com.example.ena.ui;

import android.content.Intent;
import android.os.Bundle;
import android.widget.AutoCompleteTextView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import com.example.ena.MainActivity;
import com.example.ena.R;
import com.example.ena.UserSession;
import com.example.ena.api.ApiClient;
import com.example.ena.api.LoginRequest;
import com.example.ena.api.LoginResponse;
import java.util.ArrayList;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Set;

public class LoginActivity extends AppCompatActivity {
    private AutoCompleteTextView editLogin;
    private EditText editPassword;
    private Button btnLogin;
    private TextView txtBaseUrl;
    private android.widget.ArrayAdapter<String> loginAdapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (UserSession.isLoggedIn(this)) {
            startMain();
            return;
        }
        setContentView(R.layout.activity_login);

        editLogin = findViewById(R.id.editLogin);
        editPassword = findViewById(R.id.editPassword);
        btnLogin = findViewById(R.id.btnLogin);
        txtBaseUrl = findViewById(R.id.txtBaseUrl);
        Button btnConnectServer = findViewById(R.id.btnConnectServer);

        updateBaseUrlLabel();
        setupLoginDropdown();
        loadLoginListFromServer();

        btnConnectServer.setOnClickListener(v ->
                startActivity(new Intent(this, SettingsActivity.class)));

        btnLogin.setOnClickListener(v -> attemptLogin());
    }

    @Override
    protected void onResume() {
        super.onResume();
        updateBaseUrlLabel();
        loadLoginListFromServer();
    }

    private void updateBaseUrlLabel() {
        String baseUrl = com.example.ena.api.ApiConfig.getBaseUrl(this);
        if (baseUrl == null || baseUrl.isEmpty()) {
            txtBaseUrl.setText("API: brak konfiguracji");
        } else {
            txtBaseUrl.setText("API: " + baseUrl);
        }
    }

    private void setupLoginDropdown() {
        loginAdapter = new android.widget.ArrayAdapter<>(
                this,
                android.R.layout.simple_dropdown_item_1line,
                new ArrayList<>()
        );
        editLogin.setAdapter(loginAdapter);
        editLogin.setThreshold(0);
        editLogin.setOnClickListener(v -> editLogin.showDropDown());
        editLogin.setOnFocusChangeListener((v, hasFocus) -> {
            if (hasFocus) {
                editLogin.showDropDown();
            }
        });
        updateLoginAdapter(UserSession.getRecentLogins(this));
    }

    private void loadLoginListFromServer() {
        String baseUrl = com.example.ena.api.ApiConfig.getBaseUrl(this);
        if (baseUrl == null || baseUrl.isEmpty()) {
            updateLoginAdapter(UserSession.getRecentLogins(this));
            return;
        }

        ApiClient apiClient = new ApiClient(this);
        apiClient.fetchLoginList(new ApiClient.ApiCallback<List<String>>() {
            @Override
            public void onSuccess(List<String> data) {
                runOnUiThread(() -> updateLoginAdapter(data));
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> updateLoginAdapter(UserSession.getRecentLogins(LoginActivity.this)));
            }
        });
    }

    private void updateLoginAdapter(List<String> serverLogins) {
        Set<String> merged = new LinkedHashSet<>();
        merged.addAll(UserSession.getRecentLogins(this));
        if (serverLogins != null) {
            for (String login : serverLogins) {
                if (login != null && !login.trim().isEmpty()) {
                    merged.add(login);
                }
            }
        }
        loginAdapter.clear();
        loginAdapter.addAll(merged);
        loginAdapter.notifyDataSetChanged();
    }

    private void attemptLogin() {
        String login = editLogin.getText().toString().trim();
        String password = editPassword.getText().toString();

        if (login.isEmpty() || password.isEmpty()) {
            Toast.makeText(this, "Wpisz login i hasło.", Toast.LENGTH_SHORT).show();
            return;
        }

        String baseUrl = com.example.ena.api.ApiConfig.getBaseUrl(this);
        if (baseUrl == null || baseUrl.isEmpty()) {
            Toast.makeText(this, "Najpierw połącz z serwerem (ustaw API).", Toast.LENGTH_SHORT).show();
            return;
        }

        btnLogin.setEnabled(false);
        btnLogin.setText("Logowanie...");

        ApiClient apiClient = new ApiClient(this);
        apiClient.login(new LoginRequest(login, password), new ApiClient.ApiCallback<LoginResponse>() {
            @Override
            public void onSuccess(LoginResponse data) {
                runOnUiThread(() -> {
                    btnLogin.setEnabled(true);
                    btnLogin.setText("Zaloguj");
                    if (data == null || data.getUser() == null) {
                        Toast.makeText(LoginActivity.this, "Nieprawidłowa odpowiedź serwera.", Toast.LENGTH_SHORT).show();
                        return;
                    }
                    UserSession.saveLogin(
                            LoginActivity.this,
                            data.getUser().getLogin(),
                            data.getUser().getNazwaWyswietlana(),
                            data.getUser().getId(),
                            data.getToken(),
                            data.getRefreshToken()
                    );
                    Toast.makeText(LoginActivity.this, "Zalogowano!", Toast.LENGTH_SHORT).show();
                    startMain();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    btnLogin.setEnabled(true);
                    btnLogin.setText("Zaloguj");
                    Toast.makeText(LoginActivity.this, message == null ? "Błąd logowania" : message, Toast.LENGTH_SHORT).show();
                });
            }
        });
    }

    private void startMain() {
        Intent intent = new Intent(this, MainActivity.class);
        startActivity(intent);
        finish();
    }
}
