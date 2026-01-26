package com.example.ena.ui;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ProgressBar;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.ena.MainActivity;
import com.example.ena.R;
import com.example.ena.UserSession;
import com.example.ena.api.ApiClient;
import com.example.ena.api.ApiConfig;
import com.example.ena.api.LoginRequest;
import com.example.ena.api.LoginResponse;
import com.google.android.material.textfield.TextInputEditText;
import com.google.android.material.textfield.TextInputLayout;

import java.util.ArrayList;
import java.util.List;

public class LoginActivity extends AppCompatActivity {

    private Spinner spinnerLogin;
    private TextInputEditText editPassword;
    private TextInputLayout layoutPassword; // Do wyświetlania błędów przy polu
    private Button btnLogin;
    private ImageButton btnSettings;
    private TextView txtBaseUrl, txtStatus;
    private ProgressBar progressBar;

    private ApiClient apiClient;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        // Automatyczne logowanie, jeśli sesja jest aktywna
        if (UserSession.isLoggedIn(this)) {
            startMain();
            return;
        }

        setContentView(R.layout.activity_login);

        // Inicjalizacja widoków
        spinnerLogin = findViewById(R.id.spinnerLogin);
        editPassword = findViewById(R.id.editPassword);
        layoutPassword = findViewById(R.id.layoutPassword);
        btnLogin = findViewById(R.id.btnLogin);
        btnSettings = findViewById(R.id.btnSettings);
        txtBaseUrl = findViewById(R.id.txtBaseUrl);
        txtStatus = findViewById(R.id.txtStatus);
        progressBar = findViewById(R.id.progressBar);

        apiClient = new ApiClient(this);

        // Obsługa przycisków
        btnSettings.setOnClickListener(v ->
                startActivity(new Intent(this, SettingsActivity.class)));

        btnLogin.setOnClickListener(v -> attemptLogin());

        // Dodaj placeholder do spinnera na start
        setSpinnerPlaceholder("Ładowanie użytkowników...");
    }

    @Override
    protected void onResume() {
        super.onResume();
        checkConfigurationAndLoad();
    }

    private void checkConfigurationAndLoad() {
        String baseUrl = ApiConfig.getBaseUrl(this);

        if (baseUrl == null || baseUrl.isEmpty()) {
            txtBaseUrl.setText("Serwer: Brak konfiguracji");
            txtStatus.setText("Skonfiguruj połączenie z serwerem");
            txtStatus.setTextColor(getResources().getColor(android.R.color.holo_red_dark));
            setSpinnerPlaceholder("Brak połączenia");
            btnLogin.setEnabled(false);
            // Opcjonalnie: od razu otwórz ustawienia
            // startActivity(new Intent(this, SettingsActivity.class));
        } else {
            txtBaseUrl.setText("Serwer: " + baseUrl);
            txtStatus.setText("Zaloguj się do systemu");
            txtStatus.setTextColor(getResources().getColor(R.color.black)); // lub inny kolor
            btnLogin.setEnabled(true);
            loadUsersList();
        }
    }

    private void loadUsersList() {
        setLoadingState(true, false); // Pokaż progress, ale nie blokuj wszystkiego

        apiClient.fetchUsers(new ApiClient.ApiCallback<List<String>>() {
            @Override
            public void onSuccess(List<String> data) {
                runOnUiThread(() -> {
                    setLoadingState(false, false);
                    if (data != null && !data.isEmpty()) {
                        setupSpinner(data);
                        txtStatus.setText("Wybierz użytkownika i wpisz hasło");
                    } else {
                        setSpinnerPlaceholder("Brak użytkowników");
                        showError("Pobrano pustą listę użytkowników.");
                    }
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    setLoadingState(false, false);
                    setSpinnerPlaceholder("Błąd pobierania");

                    String friendlyError = "Błąd połączenia";
                    if (message.contains("Connect")) friendlyError = "Nie można połączyć z serwerem. Sprawdź IP/Wifi.";
                    else if (message.contains("timeout")) friendlyError = "Serwer nie odpowiada (Timeout).";

                    txtStatus.setText(friendlyError);
                    txtStatus.setTextColor(getResources().getColor(android.R.color.holo_red_dark));
                    Toast.makeText(LoginActivity.this, message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private void setupSpinner(List<String> users) {
        ArrayAdapter<String> adapter = new ArrayAdapter<>(
                this,
                android.R.layout.simple_spinner_dropdown_item,
                users
        );
        spinnerLogin.setAdapter(adapter);

        // Próba ustawienia ostatniego loginu
        String lastUser = UserSession.getLogin(this);
        if (lastUser != null && users.contains(lastUser)) {
            spinnerLogin.setSelection(adapter.getPosition(lastUser));
        }
    }

    private void setSpinnerPlaceholder(String text) {
        List<String> placeholder = new ArrayList<>();
        placeholder.add(text);
        setupSpinner(placeholder);
    }

    private void attemptLogin() {
        // Reset błędów
        layoutPassword.setError(null);

        // 1. Walidacja Użytkownika
        Object selectedItem = spinnerLogin.getSelectedItem();
        if (selectedItem == null || selectedItem.toString().equals("Błąd pobierania")
                || selectedItem.toString().equals("Ładowanie użytkowników...")
                || selectedItem.toString().equals("Brak połączenia")) {
            Toast.makeText(this, "Najpierw wybierz poprawnego użytkownika.", Toast.LENGTH_SHORT).show();
            // Spróbuj pobrać ponownie jeśli użytkownik kliknie
            loadUsersList();
            return;
        }

        String login = selectedItem.toString();
        String password = editPassword.getText() != null ? editPassword.getText().toString() : "";

        // 2. Walidacja Hasła
        if (password.isEmpty()) {
            layoutPassword.setError("Wpisz hasło");
            editPassword.requestFocus();
            return;
        }

        // Rozpoczęcie logowania
        setLoadingState(true, true); // Blokuj UI

        apiClient.login(new LoginRequest(login, password), new ApiClient.ApiCallback<LoginResponse>() {
            @Override
            public void onSuccess(LoginResponse data) {
                runOnUiThread(() -> {
                    setLoadingState(false, true);

                    if (data == null || data.getUser() == null) {
                        showError("Otrzymano błędne dane z serwera.");
                        return;
                    }

                    // Zapisz sesję
                    UserSession.saveLogin(
                            LoginActivity.this,
                            data.getUser().getLogin(),
                            data.getUser().getNazwaWyswietlana(),
                            data.getUser().getId(),
                            data.getToken(),
                            data.getRefreshToken()
                    );

                    Toast.makeText(LoginActivity.this, "Zalogowano pomyślnie!", Toast.LENGTH_SHORT).show();
                    startMain();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    setLoadingState(false, true);

                    // Inteligentna obsługa komunikatów
                    if (message.contains("Unauthorized") || message.contains("401")) {
                        layoutPassword.setError("Nieprawidłowe hasło");
                        editPassword.requestFocus();
                    } else if (message.contains("nieaktywne")) {
                        showError("To konto jest zablokowane.");
                    } else if (message.contains("Connect") || message.contains("Failed to connect")) {
                        showError("Brak połączenia z serwerem. Sprawdź ustawienia.");
                    } else {
                        showError("Błąd logowania: " + message);
                    }
                });
            }
        });
    }

    private void setLoadingState(boolean isLoading, boolean blockInput) {
        if (isLoading) {
            progressBar.setVisibility(View.VISIBLE);
            btnLogin.setText(""); // Ukryj tekst, pokaż tylko progress bar
            btnLogin.setEnabled(false);
            if (blockInput) {
                spinnerLogin.setEnabled(false);
                editPassword.setEnabled(false);
                btnSettings.setEnabled(false);
            }
        } else {
            progressBar.setVisibility(View.GONE);
            btnLogin.setText("Zaloguj się");
            btnLogin.setEnabled(true);
            spinnerLogin.setEnabled(true);
            editPassword.setEnabled(true);
            btnSettings.setEnabled(true);
        }
    }

    private void showError(String msg) {
        Toast.makeText(this, msg, Toast.LENGTH_LONG).show();
    }

    private void startMain() {
        Intent intent = new Intent(this, MainActivity.class);
        startActivity(intent);
        finish(); // Zamykamy LoginActivity, żeby "wstecz" nie wracało do logowania
    }
}