package com.example.ena;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.database.Cursor;
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
import com.example.ena.api.ApiConfig;
import com.example.ena.api.ApiClient;
import com.example.ena.api.NotificationDto;
import com.example.ena.UserSession;

import fi.iki.elonen.NanoHTTPD;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

public class BackgroundService extends Service {

    private MyWebServer server;
    private static final int SERVER_PORT = 8080;
    private static final String CHANNEL_ID = "ENA_SRV";
    private static final String NOTIFICATIONS_CHANNEL_ID = "ENA_NOTIFICATIONS";
    private static final long NOTIFICATION_POLL_INTERVAL_MS = 60_000;
    private static final String PREFS_NOTIFICATIONS = "ena_notifications";
    private static final String PREF_LAST_NOTIFICATION_ID = "last_notification_id";

    private final Handler handler = new Handler(Looper.getMainLooper());
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
        startNotificationsPolling();

        try {
            if (server == null) {
                server = new MyWebServer();
                server.start();
                Log.d("EnaServer", "Serwer wystartował na porcie: " + SERVER_PORT);
            }
        } catch (IOException e) {
            Log.e("EnaServer", "Błąd startu NanoHTTPD: " + e.getMessage());
        }

        return START_STICKY;
    }

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

        Notification notification = new NotificationCompat.Builder(this, CHANNEL_ID)
                .setContentTitle("Serwer Ena jest aktywny")
                .setContentText("Adres IP: " + ip + ":" + SERVER_PORT)
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
        stopNotificationsPolling();
        if (server != null) {
            server.stop();
            Log.d("EnaServer", "Serwer został zatrzymany.");
        }
        super.onDestroy();
    }

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    private void startNotificationsPolling() {
        handler.removeCallbacks(notificationsPoller);
        handler.postDelayed(notificationsPoller, 5_000);
    }

    private void stopNotificationsPolling() {
        handler.removeCallbacks(notificationsPoller);
    }

    private void pollNotifications() {
        if (!UserSession.isLoggedIn(this) && !PairingManager.isPaired(this)) {
            return;
        }

        ApiClient client = new ApiClient(this);
        client.fetchNotifications(true, new ApiClient.ApiCallback<List<NotificationDto>>() {
            @Override
            public void onSuccess(List<NotificationDto> data) {
                if (data == null || data.isEmpty()) {
                    return;
                }
                int lastId = getLastNotificationId();
                for (NotificationDto notification : data) {
                    if (notification == null) {
                        continue;
                    }
                    int notificationId = notification.getId();
                    if (notificationId <= lastId) {
                        continue;
                    }
                    showNotification(notification);
                    updateLastNotificationId(notificationId);
                    client.markNotificationRead(notificationId, new ApiClient.ApiCallback<Void>() {
                        @Override
                        public void onSuccess(Void ignored) { }

                        @Override
                        public void onError(String message) {
                            Log.w("EnaNotifications", "Błąd oznaczania powiadomienia: " + message);
                        }
                    });
                }
            }

            @Override
            public void onError(String message) {
                Log.w("EnaNotifications", "Błąd pobierania powiadomień: " + message);
            }
        });
    }

    private void showNotification(NotificationDto notification) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.TIRAMISU
                && NotificationManagerCompat.from(this).areNotificationsEnabled() == false) {
            return;
        }
        String title = notification.getTytul();
        if (title == null || title.trim().isEmpty()) {
            title = "Nowe powiadomienie";
        }
        String content = notification.getTresc();
        if (content == null || content.trim().isEmpty()) {
            content = "Masz nowe powiadomienie.";
        }
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
        if (id > current) {
            prefs.edit().putInt(PREF_LAST_NOTIFICATION_ID, id).apply();
        }
    }

    // --- LOGIKA SERWERA HTTP ---
    private class MyWebServer extends NanoHTTPD {
        public MyWebServer() {
            super(SERVER_PORT);
        }

        @Override
        public Response serve(IHTTPSession session) {
            String uri = session.getUri();
            Map<String, String> parms = session.getParms();
            String pairingCode = PairingManager.getOrCreateCode(getApplicationContext());
            boolean isPaired = PairingManager.isPaired(getApplicationContext());
            if (isPaired) {
                PairingManager.touchLastSeen(getApplicationContext());
            }

            if (uri.equals("/pair/status")) {
                String user = PairingManager.getPairedUser(getApplicationContext());
                String apiBaseUrl = ApiConfig.getBaseUrl(getApplicationContext());
                PairingStatus status = new PairingStatus(
                        isPaired,
                        user,
                        apiBaseUrl
                );
                return newFixedLengthResponse(Response.Status.OK, "application/json", new Gson().toJson(status));
            }

            if (uri.equals("/pair/disconnect")) {
                PairingManager.reset(getApplicationContext());
                return newFixedLengthResponse(Response.Status.OK, "text/plain", "OK");
            }

            if (uri.equals("/pair")) {
                String code = parms.get("code");
                if (PairingManager.verifyCode(getApplicationContext(), code)) {
                    PairingManager.setPaired(getApplicationContext(), true);
                    PairingManager.touchLastSeen(getApplicationContext());
                    String user = PairingManager.getPairedUser(getApplicationContext());
                    String apiBaseUrl = ApiConfig.getBaseUrl(getApplicationContext());
                    PairingStatus status = new PairingStatus(true, user, apiBaseUrl);
                    return newFixedLengthResponse(Response.Status.OK, "application/json", new Gson().toJson(status));
                }
                return newFixedLengthResponse(Response.Status.FORBIDDEN, "text/plain", "Niepoprawny kod parowania");
            }

            if (uri.equals("/pair/config")) {
                String code = parms.get("code");
                String apiBaseUrl = parms.get("apiBaseUrl");
                String user = parms.get("user");
                if (!PairingManager.verifyCode(getApplicationContext(), code)) {
                    return newFixedLengthResponse(Response.Status.FORBIDDEN, "text/plain", "Niepoprawny kod parowania");
                }
                if (apiBaseUrl != null && !apiBaseUrl.trim().isEmpty()) {
                    String cleanUrl = apiBaseUrl.trim();
                    ApiConfig.setBaseUrl(getApplicationContext(), cleanUrl);
                    // POPRAWKA: Ustawiamy fallback URL na ten sam co base URL
                    ApiConfig.setFallbackBaseUrl(getApplicationContext(), cleanUrl);
                    Log.d("EnaServer", "Configured API URLs: base=" + cleanUrl + ", fallback=" + cleanUrl);
                }
                if (user != null) {
                    PairingManager.setPairedUser(getApplicationContext(), user);
                }
                PairingManager.setPaired(getApplicationContext(), true);
                PairingManager.touchLastSeen(getApplicationContext());
                PairingStatus status = new PairingStatus(true, PairingManager.getPairedUser(getApplicationContext()),
                        ApiConfig.getBaseUrl(getApplicationContext()));
                return newFixedLengthResponse(Response.Status.OK, "application/json", new Gson().toJson(status));
            }

            if (!isPaired) {
                return newFixedLengthResponse(Response.Status.FORBIDDEN, "text/plain",
                        "Telefon nie jest sparowany. Kod: " + pairingCode);
            }

            // 1. Sprawdzanie stanu dzwonienia
            if (uri.equals("/stan")) {
                StatusData status = new StatusData(GlobalState.isRinging, GlobalState.incomingNumber);
                return newFixedLengthResponse(Response.Status.OK, "application/json", new Gson().toJson(status));
            }

            // 2. Pobieranie nowych SMS-ów przez PC
            if (uri.equals("/sms")) {
                List<GlobalState.SmsData> copy;
                synchronized (GlobalState.smsQueue) {
                    copy = new ArrayList<>(GlobalState.smsQueue);
                    GlobalState.smsQueue.clear();
                }
                return newFixedLengthResponse(Response.Status.OK, "application/json", new Gson().toJson(copy));
            }

            // 3. Wysyłanie SMS zlecane przez PC
            if (uri.equals("/wyslij")) {
                String numer = parms.get("numer");
                String tresc = parms.get("tresc");

                if (numer == null || tresc == null) {
                    return newFixedLengthResponse(Response.Status.BAD_REQUEST, "text/plain", "Brak numeru lub treści");
                }

                try {
                    SmsManager smsManager = SmsManager.getDefault();
                    ArrayList<String> parts = smsManager.divideMessage(tresc);
                    smsManager.sendMultipartTextMessage(numer, null, parts, null, null);

                    return newFixedLengthResponse(Response.Status.OK, "text/plain", "OK");
                } catch (Exception e) {
                    return newFixedLengthResponse(Response.Status.INTERNAL_ERROR, "text/plain", "Błąd Androida: " + e.getMessage());
                }
            }

            // 4. Wykonanie połączenia telefonicznego (/call?number=...)
            if (uri.equals("/call")) {
                String numer = parms.get("number");
                if (numer == null || numer.isEmpty()) {
                    return newFixedLengthResponse(Response.Status.BAD_REQUEST, "text/plain", "Brak numeru");
                }
                try {
                    Intent callIntent = new Intent(Intent.ACTION_CALL);
                    callIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                    callIntent.setData(android.net.Uri.parse("tel:" + numer));
                    getApplicationContext().startActivity(callIntent);
                    return newFixedLengthResponse(Response.Status.OK, "text/plain", "OK");
                } catch (Exception e) {
                    return newFixedLengthResponse(Response.Status.INTERNAL_ERROR, "text/plain", "Błąd połączenia: " + e.getMessage());
                }
            }

            // 4. LISTA ZDJĘĆ Z TELEFONU (/list_photos)
            if (uri.equals("/list_photos")) {
                List<String> photos = new ArrayList<>();
                String[] projection = { MediaStore.Images.Media.DATA };
                Cursor cursor = getContentResolver().query(
                        MediaStore.Images.Media.EXTERNAL_CONTENT_URI,
                        projection, null, null, MediaStore.Images.Media.DATE_TAKEN + " DESC LIMIT 50"
                );

                if (cursor != null) {
                    while (cursor.moveToNext()) {
                        photos.add(cursor.getString(0));
                    }
                    cursor.close();
                }
                return newFixedLengthResponse(Response.Status.OK, "application/json", new Gson().toJson(photos));
            }

            // 5. POBIERANIE KONKRETNEGO ZDJĘCIA (/get_photo?path=...)
            if (uri.equals("/get_photo")) {
                String path = parms.get("path");
                if (path != null) {
                    File file = new File(path);
                    if (file.exists()) {
                        try {
                            FileInputStream fis = new FileInputStream(file);
                            return newFixedLengthResponse(Response.Status.OK, "image/jpeg", fis, file.length());
                        } catch (Exception e) {
                            return newFixedLengthResponse(Response.Status.INTERNAL_ERROR, "text/plain", "Błąd odczytu pliku");
                        }
                    }
                }
                return newFixedLengthResponse(Response.Status.NOT_FOUND, "text/plain", "Plik nie istnieje");
            }

            return newFixedLengthResponse("Ena Server działa poprawnie.");
        }
    }

    static class StatusData {
        boolean dzwoni;
        String numer;
        StatusData(boolean d, String n) {
            this.dzwoni = d;
            this.numer = n;
        }
    }

    static class PairingStatus {
        boolean paired;
        String user;
        String apiBaseUrl;
        PairingStatus(boolean paired, String user, String apiBaseUrl) {
            this.paired = paired;
            this.user = user;
            this.apiBaseUrl = apiBaseUrl;
        }
    }
}
