package com.example.ena;

import android.Manifest;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.telephony.SmsManager;
import android.util.Log;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.ContextCompat;

import com.example.ena.api.ApiConfig;
import com.google.gson.Gson;
import com.google.gson.annotations.SerializedName;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.TimeUnit;

import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

/**
 * Transparentna aktywność wyświetlana po zakończeniu rozmowy telefonicznej.
 * Pobiera listę linków reklamacyjnych z API i pozwala użytkownikowi
 * wysłać wybrany link SMS-em do klienta.
 *
 * Uruchamiana z CallReceiver po przejściu RINGING → OFFHOOK → IDLE.
 *
 * Extras:
 *   "phone_number" - numer telefonu klienta
 */
public class SendLinkActivity extends AppCompatActivity {

    private static final String TAG = "EnaSendLink";

    private String phoneNumber;
    private final Gson gson = new Gson();

    private static final OkHttpClient httpClient = new OkHttpClient.Builder()
            .connectTimeout(10, TimeUnit.SECONDS)
            .readTimeout(10, TimeUnit.SECONDS)
            .build();
    private static final MediaType JSON_MEDIA = MediaType.get("application/json; charset=utf-8");

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        phoneNumber = getIntent().getStringExtra("phone_number");
        if (phoneNumber == null || phoneNumber.isEmpty() || "unknown".equalsIgnoreCase(phoneNumber)) {
            Log.w(TAG, "Brak numeru telefonu - zamykam");
            finish();
            return;
        }

        // Usuń powiadomienie o linkach (jeśli otwarto z notyfikacji)
        try {
            android.app.NotificationManager nm = (android.app.NotificationManager)
                    getSystemService(NOTIFICATION_SERVICE);
            if (nm != null) nm.cancel(9001); // NOTIFICATION_ID_LINK z CallReceiver
        } catch (Exception ignored) {}

        Log.d(TAG, "Otwarto dialog linków dla numeru: " + phoneNumber);
        fetchLinksAndShowDialog();
    }

    private void fetchLinksAndShowDialog() {
        String baseUrl = ApiConfig.getBaseUrl(this);
        if (baseUrl == null || baseUrl.isEmpty()) {
            Log.w(TAG, "Brak adresu API");
            finish();
            return;
        }

        new Thread(() -> {
            try {
                String url = baseUrl.replaceAll("/$", "") + "/api/phone/links";

                Request request = new Request.Builder()
                        .url(url)
                        .get()
                        .build();

                try (Response response = httpClient.newCall(request).execute()) {
                    if (response.isSuccessful() && response.body() != null) {
                        String body = response.body().string();
                        ApiLinksResponse parsed = gson.fromJson(body, ApiLinksResponse.class);

                        if (parsed != null && parsed.success && parsed.data != null && !parsed.data.isEmpty()) {
                            runOnUiThread(() -> showLinksDialog(parsed.data));
                        } else {
                            Log.d(TAG, "Brak aktywnych linków");
                            runOnUiThread(this::finish);
                        }
                    } else {
                        Log.e(TAG, "Błąd pobierania linków: " + response.code());
                        runOnUiThread(this::finish);
                    }
                }
            } catch (Exception e) {
                Log.e(TAG, "Błąd połączenia z API: " + e.getMessage());
                runOnUiThread(this::finish);
            }
        }).start();
    }

    private void showLinksDialog(List<SmsLink> links) {
        String[] linkNames = new String[links.size()];
        for (int i = 0; i < links.size(); i++) {
            linkNames[i] = links.get(i).name;
        }

        new AlertDialog.Builder(this)
                .setTitle("Wyślij link do: " + phoneNumber)
                .setItems(linkNames, (dialog, which) -> {
                    SmsLink selected = links.get(which);
                    sendSmsWithLink(selected);
                })
                .setNegativeButton("Pomiń", (dialog, which) -> {
                    Log.d(TAG, "Pominięto wysyłkę linku");
                    finish();
                })
                .setOnCancelListener(dialog -> {
                    Log.d(TAG, "Dialog anulowany");
                    finish();
                })
                .setCancelable(true)
                .show();
    }

    private void sendSmsWithLink(SmsLink link) {
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.SEND_SMS)
                != PackageManager.PERMISSION_GRANTED) {
            Toast.makeText(this, "Brak uprawnienia do wysyłania SMS", Toast.LENGTH_LONG).show();
            Log.e(TAG, "Brak SEND_SMS permission");
            finish();
            return;
        }

        String smsContent;
        if (link.smsTemplate != null && !link.smsTemplate.isEmpty()) {
            smsContent = link.smsTemplate.replace("{url}", link.url);
        } else {
            smsContent = link.url;
        }

        try {
            SmsManager smsManager = SmsManager.getDefault();
            ArrayList<String> parts = smsManager.divideMessage(smsContent);
            smsManager.sendMultipartTextMessage(phoneNumber, null, parts, null, null);

            Toast.makeText(this, "Wysłano link: " + link.name, Toast.LENGTH_SHORT).show();
            Log.i(TAG, "SMS wysłany do " + phoneNumber + ": " + link.name);

            logLinkSent(link.id, "SENT");
        } catch (Exception e) {
            Toast.makeText(this, "Błąd wysyłki SMS: " + e.getMessage(), Toast.LENGTH_LONG).show();
            Log.e(TAG, "Błąd wysyłki SMS: " + e.getMessage());
            logLinkSent(link.id, "FAILED");
        }

        finish();
    }

    private void logLinkSent(int linkId, String status) {
        String baseUrl = ApiConfig.getBaseUrl(this);
        String userLogin = BackgroundService.getUserLogin(this);
        if (baseUrl == null || userLogin == null) return;

        new Thread(() -> {
            try {
                String url = baseUrl.replaceAll("/$", "") + "/api/phone/links/log";

                SmsLinkLogPayload payload = new SmsLinkLogPayload();
                payload.userLogin = userLogin;
                payload.linkId = linkId;
                payload.phoneNumber = phoneNumber;
                payload.status = status;

                String json = gson.toJson(payload);
                RequestBody body = RequestBody.create(json, JSON_MEDIA);

                Request request = new Request.Builder()
                        .url(url)
                        .post(body)
                        .addHeader("X-User", userLogin)
                        .build();

                httpClient.newCall(request).execute().close();
                Log.d(TAG, "Link log zapisany: linkId=" + linkId + " status=" + status);
            } catch (Exception e) {
                Log.w(TAG, "Błąd zapisu logu linku: " + e.getMessage());
            }
        }).start();
    }

    // ====================================================================
    // Klasy pomocnicze (JSON)
    // ====================================================================

    static class SmsLink {
        int id;
        String name;
        String url;
        String smsTemplate;
    }

    static class ApiLinksResponse {
        boolean success;
        List<SmsLink> data;
    }

    static class SmsLinkLogPayload {
        @SerializedName("userLogin")
        String userLogin;

        @SerializedName("linkId")
        int linkId;

        @SerializedName("phoneNumber")
        String phoneNumber;

        @SerializedName("status")
        String status;
    }
}
