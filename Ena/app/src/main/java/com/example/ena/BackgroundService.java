package com.example.ena;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.database.Cursor;
import android.net.Uri;
import android.net.wifi.WifiManager;
import android.os.Build;
import android.os.Handler;
import android.os.IBinder;
import android.os.Looper;
import android.provider.MediaStore;
import android.telephony.SmsManager;
import android.text.format.Formatter;
import android.util.Log;

import androidx.core.app.NotificationCompat;
import androidx.core.app.NotificationManagerCompat;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.example.ena.api.ApiConfig;
import com.example.ena.api.ApiClient;
import com.example.ena.api.NotificationDto;
import com.example.ena.UserSession;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.concurrent.TimeUnit;

import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

import fi.iki.elonen.NanoHTTPD;

public class BackgroundService extends Service {

    private static final String TAG = "EnaBackgroundService";
    private MyWebServer server;
    private static final int SERVER_PORT = 8080;
    private static final String CHANNEL_ID = "ENA_SRV";
    private static final String NOTIFICATIONS_CHANNEL_ID = "ENA_NOTIFICATIONS";

    // Interwały
    private static final long HEARTBEAT_INTERVAL_MS = 30_000;        // 30s
    private static final long COMMAND_POLL_INTERVAL_MS = 5_000;       // 5s
    private static final long NOTIFICATION_POLL_INTERVAL_MS = 60_000; // 60s

    private static final String PREFS_NOTIFICATIONS = "ena_notifications";
    private static final String PREF_LAST_NOTIFICATION_ID = "last_notification_id";

    private final Handler handler = new Handler(Looper.getMainLooper());

    // HTTP Client do komunikacji z API
    private static final OkHttpClient apiHttpClient = new OkHttpClient.Builder()
            .connectTimeout(10, TimeUnit.SECONDS)
            .readTimeout(10, TimeUnit.SECONDS)
            .writeTimeout(10, TimeUnit.SECONDS)
            .build();
    private static final MediaType JSON_MEDIA = MediaType.get("application/json; charset=utf-8");
    private final Gson gson = new Gson();

    // === HEARTBEAT ===
    private final Runnable heartbeatRunner = new Runnable() {
        @Override
        public void run() {
            sendHeartbeat();
            handler.postDelayed(this, HEARTBEAT_INTERVAL_MS);
        }
    };

    // === COMMAND POLLING ===
    private final Runnable commandPollRunner = new Runnable() {
        @Override
        public void run() {
            pollAndExecuteCommands();
            handler.postDelayed(this, COMMAND_POLL_INTERVAL_MS);
        }
    };

    // === NOTIFICATIONS POLLING ===
    private final Runnable notificationsPoller = new Runnable() {
        @Override
        public void run() {
            pollNotifications();
            handler.postDelayed(this, NOTIFICATION_POLL_INTERVAL_MS);
        }
    };

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        startForegroundNotification();

        // === OBSŁUGA AKCJI: Uruchomienie SendLinkActivity ===
        // Wywoływane z CallReceiver - foreground service ma uprawnienie
        // do startowania Activity na Android 10-11
        if (intent != null && "com.example.ena.SHOW_SMS_LINKS".equals(intent.getAction())) {
            String phoneNumber = intent.getStringExtra("phone_number");
            long traceId = intent.getLongExtra("trace_id", -1L);
            if (phoneNumber != null && !phoneNumber.isEmpty()) {
                Log.i(TAG, "=== BackgroundService: Uruchamiam SendLinkActivity dla: " + phoneNumber
                        + " trace=" + traceId + " ===");
                try {
                    Intent linkIntent = new Intent(this, SendLinkActivity.class);
                    linkIntent.putExtra("phone_number", phoneNumber);
                    if (traceId != -1L) {
                        linkIntent.putExtra("trace_id", traceId);
                    }
                    linkIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TOP);
                    startActivity(linkIntent);
                    Log.d(TAG, "SendLinkActivity uruchomione z foreground service");
                } catch (Exception e) {
                    Log.w(TAG, "Nie udało się uruchomić SendLinkActivity z FG service: " + e.getMessage());
                }
            }
            return START_STICKY;
        }

        // NOWE: Zarejestruj IncomingCallTracker (PhoneStateListener)
        // Musi być na wątku z Looper - główny wątek jest OK
        try {
            IncomingCallTracker.getInstance().registerListener(this);
            Log.d(TAG, "IncomingCallTracker zarejestrowany");
        } catch (Exception e) {
            Log.e(TAG, "Błąd rejestracji IncomingCallTracker: " + e.getMessage());
        }

        // Uruchom heartbeat i polling komend
        startHeartbeat();
        startCommandPolling();
        startNotificationsPolling();

        // Uruchom serwer HTTP (dla kompatybilności wstecznej)
        try {
            if (server == null) {
                server = new MyWebServer();
                server.start();
                Log.d(TAG, "Serwer NanoHTTPD wystartował na porcie: " + SERVER_PORT);
            }
        } catch (IOException e) {
            Log.e(TAG, "Błąd startu NanoHTTPD: " + e.getMessage());
        }

        return START_STICKY;
    }

    // =====================================================================
    // NOWE: Wysyłanie zdarzeń do API (wywoływane z CallReceiver/SmsReceiver)
    // =====================================================================

    /**
     * Wysyła zdarzenie (CALL_RINGING, CALL_IDLE, SMS_RECEIVED) do API.
     * Wywoływane statycznie z CallReceiver i SmsReceiver.
     */
    public static void sendPhoneEvent(Context context, String eventType, String phoneNumber, String content) {
        String userLogin = getUserLogin(context);
        if (userLogin == null || userLogin.isEmpty()) {
            Log.w(TAG, "Nie można wysłać zdarzenia - brak zalogowanego użytkownika");
            return;
        }

        String baseUrl = ApiConfig.getBaseUrl(context);
        if (baseUrl == null || baseUrl.isEmpty()) {
            Log.w(TAG, "Nie można wysłać zdarzenia - brak adresu API");
            // Fallback: wyślij też starym sposobem (HttpSender)
            HttpSender.sendEvent(context, eventType.equals("CALL_RINGING") ? "CALL" : eventType, phoneNumber, content);
            return;
        }

        // Wyślij w osobnym wątku
        new Thread(() -> {
            try {
                String url = baseUrl.replaceAll("/$", "") + "/api/phone/event";

                String json = new Gson().toJson(new PhoneEventPayload(userLogin, eventType, phoneNumber, content));
                RequestBody body = RequestBody.create(json, JSON_MEDIA);

                Request request = new Request.Builder()
                        .url(url)
                        .post(body)
                        .addHeader("X-User", userLogin)
                        .build();

                try (Response response = apiHttpClient.newCall(request).execute()) {
                    if (response.isSuccessful()) {
                        Log.d(TAG, "Zdarzenie wysłane do API: " + eventType + " nr=" + phoneNumber);
                    } else {
                        Log.e(TAG, "API błąd: " + response.code());
                        // Fallback do starego sposobu
                        HttpSender.sendEvent(context, eventType, phoneNumber, content);
                    }
                }
            } catch (Exception e) {
                Log.e(TAG, "Błąd wysyłania do API: " + e.getMessage());
                // Fallback do starego sposobu
                HttpSender.sendEvent(context, eventType, phoneNumber, content);
            }
        }).start();
    }

    // =====================================================================
    // HEARTBEAT
    // =====================================================================

    private void sendHeartbeat() {
        String userLogin = getUserLogin(this);
        String baseUrl = ApiConfig.getBaseUrl(this);
        if (userLogin == null || userLogin.isEmpty() || baseUrl == null || baseUrl.isEmpty()) return;

        new Thread(() -> {
            try {
                String url = baseUrl.replaceAll("/$", "") + "/api/phone/heartbeat";
                String json = gson.toJson(new HeartbeatPayload(
                        userLogin,
                        Build.MODEL,
                        "1.0"
                ));

                Request request = new Request.Builder()
                        .url(url)
                        .post(RequestBody.create(json, JSON_MEDIA))
                        .addHeader("X-User", userLogin)
                        .build();

                try (Response response = apiHttpClient.newCall(request).execute()) {
                    if (response.isSuccessful()) {
                        Log.v(TAG, "Heartbeat wysłany");
                    }
                }
            } catch (Exception e) {
                Log.w(TAG, "Błąd heartbeat: " + e.getMessage());
            }
        }).start();
    }

    // =====================================================================
    // POLLING KOMEND (DIAL, SEND_SMS)
    // =====================================================================

    private void pollAndExecuteCommands() {
        String userLogin = getUserLogin(this);
        String baseUrl = ApiConfig.getBaseUrl(this);
        if (userLogin == null || userLogin.isEmpty() || baseUrl == null || baseUrl.isEmpty()) return;

        new Thread(() -> {
            try {
                String url = baseUrl.replaceAll("/$", "") + "/api/phone/commands/" + userLogin;

                Request request = new Request.Builder()
                        .url(url)
                        .get()
                        .addHeader("X-User", userLogin)
                        .build();

                try (Response response = apiHttpClient.newCall(request).execute()) {
                    if (response.isSuccessful() && response.body() != null) {
                        String body = response.body().string();
                        ApiCommandsResponse parsed = gson.fromJson(body, ApiCommandsResponse.class);

                        if (parsed != null && parsed.success && parsed.data != null) {
                            for (PhoneCommandItem cmd : parsed.data) {
                                executeCommand(cmd, baseUrl, userLogin);
                            }
                        }
                    }
                }
            } catch (Exception e) {
                Log.w(TAG, "Błąd pollowania komend: " + e.getMessage());
            }
        }).start();
    }

    private void executeCommand(PhoneCommandItem cmd, String baseUrl, String userLogin) {
        String resultStatus = "SUCCESS";

        try {
            switch (cmd.commandType) {
                case "DIAL":
                    // Wykonaj połączenie telefoniczne
                    Intent callIntent = new Intent(Intent.ACTION_CALL);
                    callIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                    callIntent.setData(Uri.parse("tel:" + cmd.phoneNumber));
                    getApplicationContext().startActivity(callIntent);
                    Log.d(TAG, "Wykonano DIAL: " + cmd.phoneNumber);
                    break;

                case "SEND_SMS":
                    // Wyślij SMS
                    SmsManager smsManager = SmsManager.getDefault();
                    if (cmd.content != null && !cmd.content.isEmpty()) {
                        ArrayList<String> parts = smsManager.divideMessage(cmd.content);
                        smsManager.sendMultipartTextMessage(cmd.phoneNumber, null, parts, null, null);
                        Log.d(TAG, "Wysłano SMS do: " + cmd.phoneNumber);
                    } else {
                        resultStatus = "FAILED";
                    }
                    break;

                default:
                    Log.w(TAG, "Nieznana komenda: " + cmd.commandType);
                    resultStatus = "FAILED";
                    break;
            }
        } catch (Exception e) {
            Log.e(TAG, "Błąd wykonania komendy " + cmd.commandType + ": " + e.getMessage());
            resultStatus = "FAILED";
        }

        // Potwierdź wykonanie komendy
        reportCommandResult(baseUrl, userLogin, cmd.id, resultStatus);
    }

    private void reportCommandResult(String baseUrl, String userLogin, int commandId, String status) {
        try {
            String url = baseUrl.replaceAll("/$", "") + "/api/phone/command/" + commandId + "/result";
            String json = "{\"status\":\"" + status + "\"}";

            Request request = new Request.Builder()
                    .url(url)
                    .post(RequestBody.create(json, JSON_MEDIA))
                    .addHeader("X-User", userLogin)
                    .build();

            apiHttpClient.newCall(request).execute().close();
        } catch (Exception e) {
            Log.w(TAG, "Błąd raportu komendy: " + e.getMessage());
        }
    }

    // =====================================================================
    // HELPERS
    // =====================================================================

    static String getUserLogin(Context context) {
        // Najpierw sprawdź UserSession (zalogowany użytkownik)
        if (UserSession.isLoggedIn(context)) {
            String login = UserSession.getLogin(context);
            if (login != null && !login.isEmpty()) return login;
        }
        // Fallback: PairingManager (stary sposób)
        String pairedUser = PairingManager.getPairedUser(context);
        if (pairedUser != null && !pairedUser.isEmpty()) return pairedUser;
        return null;
    }

    private void startHeartbeat() {
        handler.removeCallbacks(heartbeatRunner);
        handler.postDelayed(heartbeatRunner, 2_000); // Pierwszy heartbeat po 2s
    }

    private void startCommandPolling() {
        handler.removeCallbacks(commandPollRunner);
        handler.postDelayed(commandPollRunner, 3_000); // Pierwszy poll po 3s
    }

    private void startNotificationsPolling() {
        handler.removeCallbacks(notificationsPoller);
        handler.postDelayed(notificationsPoller, 5_000);
    }

    // =====================================================================
    // Istniejący kod: Foreground Notification, NanoHTTPD, Notifications polling
    // =====================================================================

    private void startForegroundNotification() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            NotificationManager manager = (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);
            if (manager != null) {
                NotificationChannel channel = new NotificationChannel(
                        CHANNEL_ID, "Ena Server Background Service", NotificationManager.IMPORTANCE_LOW);
                manager.createNotificationChannel(channel);

                NotificationChannel notificationsChannel = new NotificationChannel(
                        NOTIFICATIONS_CHANNEL_ID, "Powiadomienia zwrotów", NotificationManager.IMPORTANCE_DEFAULT);
                manager.createNotificationChannel(notificationsChannel);
            }
        }

        String ip = getIpAddress();
        String userLogin = getUserLogin(this);

        Notification notification = new NotificationCompat.Builder(this, CHANNEL_ID)
                .setContentTitle("ENA Mobile - aktywna")
                .setContentText("Użytkownik: " + (userLogin != null ? userLogin : "niezalogowany") + " | IP: " + ip)
                .setSmallIcon(R.mipmap.ic_launcher)
                .setOngoing(true)
                .build();

        startForeground(1, notification);
    }

    private String getIpAddress() {
        String ip = NetworkUtils.getIPAddress(true);
        if (ip == null || ip.isEmpty()) {
            try {
                WifiManager wm = (WifiManager) getApplicationContext().getSystemService(WIFI_SERVICE);
                ip = Formatter.formatIpAddress(wm.getConnectionInfo().getIpAddress());
            } catch (Exception e) {
                ip = "0.0.0.0";
            }
        }
        return ip;
    }

    @Override
    public void onDestroy() {
        handler.removeCallbacks(heartbeatRunner);
        handler.removeCallbacks(commandPollRunner);
        handler.removeCallbacks(notificationsPoller);

        // NOWE: Wyrejestruj IncomingCallTracker
        try {
            IncomingCallTracker.getInstance().unregisterListener(this);
        } catch (Exception e) {
            Log.w(TAG, "Błąd wyrejestrowania IncomingCallTracker: " + e.getMessage());
        }

        if (server != null) {
            server.stop();
        }
        super.onDestroy();
    }

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    private void pollNotifications() {
        if (!UserSession.isLoggedIn(this) && !PairingManager.isPaired(this)) return;

        ApiClient client = new ApiClient(this);
        client.fetchNotifications(true, new ApiClient.ApiCallback<List<NotificationDto>>() {
            @Override
            public void onSuccess(List<NotificationDto> data) {
                if (data == null || data.isEmpty()) return;
                int lastId = getLastNotificationId();
                for (NotificationDto notification : data) {
                    if (notification == null) continue;
                    int notificationId = notification.getId();
                    if (notificationId <= lastId) continue;
                    showNotification(notification);
                    updateLastNotificationId(notificationId);
                    client.markNotificationRead(notificationId, new ApiClient.ApiCallback<Void>() {
                        @Override public void onSuccess(Void ignored) { }
                        @Override public void onError(String message) { }
                    });
                }
            }
            @Override
            public void onError(String message) { }
        });
    }

    private void showNotification(NotificationDto notification) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.TIRAMISU
                && !NotificationManagerCompat.from(this).areNotificationsEnabled()) {
            return;
        }
        String title = notification.getTytul();
        if (title == null || title.trim().isEmpty()) title = "Nowe powiadomienie";
        String content = notification.getTresc();
        if (content == null || content.trim().isEmpty()) content = "Masz nowe powiadomienie.";

        NotificationCompat.Builder builder = new NotificationCompat.Builder(this, NOTIFICATIONS_CHANNEL_ID)
                .setSmallIcon(R.mipmap.ic_launcher)
                .setContentTitle(title)
                .setContentText(content)
                .setStyle(new NotificationCompat.BigTextStyle().bigText(content))
                .setAutoCancel(true)
                .setPriority(NotificationCompat.PRIORITY_DEFAULT);

        NotificationManagerCompat.from(this).notify(notification.getId(), builder.build());
    }

    private int getLastNotificationId() {
        SharedPreferences prefs = getSharedPreferences(PREFS_NOTIFICATIONS, MODE_PRIVATE);
        return prefs.getInt(PREF_LAST_NOTIFICATION_ID, 0);
    }

    private void updateLastNotificationId(int id) {
        SharedPreferences prefs = getSharedPreferences(PREFS_NOTIFICATIONS, MODE_PRIVATE);
        int current = prefs.getInt(PREF_LAST_NOTIFICATION_ID, 0);
        if (id > current) prefs.edit().putInt(PREF_LAST_NOTIFICATION_ID, id).apply();
    }

    // =====================================================================
    // Serwer HTTP (NanoHTTPD) - zachowany dla kompatybilności wstecznej
    // =====================================================================

    private class MyWebServer extends NanoHTTPD {
        public MyWebServer() { super(SERVER_PORT); }

        @Override
        public NanoHTTPD.Response serve(IHTTPSession session) {
            String uri = session.getUri();
            Map<String, String> parms = session.getParms();
            boolean isPaired = PairingManager.isPaired(getApplicationContext());

            if (isPaired) PairingManager.touchLastSeen(getApplicationContext());

            // Zachowaj istniejącą logikę serwera HTTP dla kompatybilności...
            if (uri.equals("/pair/status")) {
                String user = PairingManager.getPairedUser(getApplicationContext());
                String apiBaseUrl = ApiConfig.getBaseUrl(getApplicationContext());
                return newFixedLengthResponse(NanoHTTPD.Response.Status.OK, "application/json",
                        gson.toJson(new PairingStatus(isPaired, user, apiBaseUrl)));
            }

            if (uri.equals("/stan")) {
                StatusData status = new StatusData(GlobalState.isRinging, GlobalState.incomingNumber);
                return newFixedLengthResponse(NanoHTTPD.Response.Status.OK, "application/json", gson.toJson(status));
            }

            if (uri.equals("/sms")) {
                List<GlobalState.SmsData> copy;
                synchronized (GlobalState.smsQueue) {
                    copy = new ArrayList<>(GlobalState.smsQueue);
                    GlobalState.smsQueue.clear();
                }
                return newFixedLengthResponse(NanoHTTPD.Response.Status.OK, "application/json", gson.toJson(copy));
            }

            if (uri.equals("/wyslij")) {
                String numer = parms.get("numer");
                String tresc = parms.get("tresc");
                if (numer == null || tresc == null)
                    return newFixedLengthResponse(NanoHTTPD.Response.Status.BAD_REQUEST, "text/plain", "Brak danych");
                try {
                    SmsManager smsManager = SmsManager.getDefault();
                    ArrayList<String> parts = smsManager.divideMessage(tresc);
                    smsManager.sendMultipartTextMessage(numer, null, parts, null, null);
                    return newFixedLengthResponse(NanoHTTPD.Response.Status.OK, "text/plain", "OK");
                } catch (Exception e) {
                    return newFixedLengthResponse(NanoHTTPD.Response.Status.INTERNAL_ERROR, "text/plain", e.getMessage());
                }
            }

            if (uri.equals("/call")) {
                String numer = parms.get("number");
                if (numer == null || numer.isEmpty())
                    return newFixedLengthResponse(NanoHTTPD.Response.Status.BAD_REQUEST, "text/plain", "Brak numeru");
                try {
                    Intent callIntent = new Intent(Intent.ACTION_CALL);
                    callIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                    callIntent.setData(Uri.parse("tel:" + numer));
                    getApplicationContext().startActivity(callIntent);
                    return newFixedLengthResponse(NanoHTTPD.Response.Status.OK, "text/plain", "OK");
                } catch (Exception e) {
                    return newFixedLengthResponse(NanoHTTPD.Response.Status.INTERNAL_ERROR, "text/plain", e.getMessage());
                }
            }

            return newFixedLengthResponse("Ena Server działa.");
        }
    }

    // =====================================================================
    // Klasy pomocnicze
    // =====================================================================

    static class StatusData {
        boolean dzwoni;
        String numer;
        StatusData(boolean d, String n) { this.dzwoni = d; this.numer = n; }
    }

    static class PairingStatus {
        boolean paired;
        String user;
        String apiBaseUrl;
        PairingStatus(boolean p, String u, String a) { this.paired = p; this.user = u; this.apiBaseUrl = a; }
    }

    // Payloady JSON do API
    static class PhoneEventPayload {
        String userLogin;
        String eventType;
        String phoneNumber;
        String content;
        PhoneEventPayload(String u, String t, String n, String c) {
            this.userLogin = u; this.eventType = t; this.phoneNumber = n; this.content = c;
        }
    }

    static class HeartbeatPayload {
        String userLogin;
        String phoneModel;
        String appVersion;
        HeartbeatPayload(String u, String m, String v) {
            this.userLogin = u; this.phoneModel = m; this.appVersion = v;
        }
    }

    static class PhoneCommandItem {
        int id;
        String commandType;
        String phoneNumber;
        String content;
    }

    static class ApiCommandsResponse {
        boolean success;
        List<PhoneCommandItem> data;
    }
}
