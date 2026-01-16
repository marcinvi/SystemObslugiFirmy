package com.example.ena;

import android.content.Context;
import android.util.Log;
import com.google.gson.Gson;
import java.util.concurrent.TimeUnit;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class HttpSender {

    private static final String TAG = "EnaSender";

    // Klient HTTP (singleton dla wydajności)
    private static final OkHttpClient client = new OkHttpClient.Builder()
            .connectTimeout(5, TimeUnit.SECONDS) // Krótki czas na połączenie
            .writeTimeout(5, TimeUnit.SECONDS)
            .readTimeout(5, TimeUnit.SECONDS)
            .build();

    public static final MediaType JSON = MediaType.get("application/json; charset=utf-8");

    /**
     * Główna metoda do wysyłania zdarzeń do PC.
     *
     * @param context Kontekst aplikacji (do pobrania ustawień)
     * @param type    Typ zdarzenia: "CALL", "SMS", lub "PING"
     * @param number  Numer telefonu (lub "0" dla pingu)
     * @param content Treść wiadomości lub opis
     */
    public static void sendEvent(Context context, String type, String number, String content) {
        // 1. Pobierz adres PC
        String baseUrl = Config.getServerUrl(context);

        if (baseUrl == null || baseUrl.isEmpty()) {
            Log.e(TAG, "Brak skonfigurowanego adresu PC! Zeskanuj kod QR.");
            return;
        }

        // 2. Wybierz endpoint (końcówkę adresu)
        // Jeśli to PING -> /api/ping
        // Jeśli to CALL/SMS -> /api/event
        String endpoint = "/api/event";
        if ("PING".equals(type)) {
            endpoint = "/api/ping";
        }

        String fullUrl = baseUrl + endpoint;

        // 3. Przygotuj dane (Model musi pasować do klasy AndroidEvent w C#)
        EventData data = new EventData(type, number, content);
        String jsonPayload = new Gson().toJson(data);

        // 4. Wyślij w osobnym wątku (Android zabrania sieci w głównym wątku)
        new Thread(() -> {
            try {
                RequestBody body = RequestBody.create(jsonPayload, JSON);

                Request request = new Request.Builder()
                        .url(fullUrl)
                        .post(body)
                        .build();

                // Wykonanie zapytania
                try (Response response = client.newCall(request).execute()) {
                    if (response.isSuccessful()) {
                        Log.d(TAG, "Wysłano pomyślnie: " + type + " -> " + fullUrl);
                    } else {
                        Log.e(TAG, "Błąd serwera: " + response.code());
                    }
                }
            } catch (Exception e) {
                Log.e(TAG, "Błąd połączenia z PC: " + e.getMessage());
                // Tutaj można by dodać logikę ponawiania lub zapisu do kolejki offline
            }
        }).start();
    }

    // --- Klasa pomocnicza do JSON (musi mieć pola jak w C#) ---
    static class EventData {
        String Type;
        String Number;
        String Content;
        String ContactName; // Opcjonalne, na przyszłość

        EventData(String type, String number, String content) {
            this.Type = type;
            this.Number = number;
            this.Content = content;
            this.ContactName = ""; // Na razie puste
        }
    }
}