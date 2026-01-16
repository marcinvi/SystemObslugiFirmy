package com.example.ena;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.net.Uri;
import android.net.wifi.WifiManager;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.provider.Settings;
import android.telephony.SmsManager;
import android.text.format.Formatter;
import android.view.WindowManager;
import android.widget.ScrollView;
import android.widget.TextView;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import fi.iki.elonen.NanoHTTPD;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import org.json.JSONArray;
import org.json.JSONObject;

public class MainActivity extends AppCompatActivity {

    private MyServer server;
    private TextView infoText;

    public static String numerDzwoniacy = "";
    public static boolean czyDzwoniTeraz = false;
    public static final List<JSONObject> smsQueue = new ArrayList<>();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);

        ScrollView scroll = new ScrollView(this);
        infoText = new TextView(this);
        infoText.setTextSize(14);
        infoText.setPadding(40, 40, 40, 40);
        scroll.addView(infoText);
        setContentView(scroll);

        // 1. NAJWAŻNIEJSZE: Wymuszenie pełnego dostępu do plików
        checkFullStorageAccess();
        checkOtherPermissions();

        try {
            server = new MyServer();
            server.start();
        } catch (IOException e) {
            infoText.setText("Błąd serwera: " + e.getMessage());
        }

        new Thread(() -> {
            while(true) {
                try { Thread.sleep(1000); updateUI(); } catch (Exception e) {}
            }
        }).start();
    }

    private void checkFullStorageAccess() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
            if (!Environment.isExternalStorageManager()) {
                Intent intent = new Intent(Settings.ACTION_MANAGE_APP_ALL_FILES_ACCESS_PERMISSION);
                Uri uri = Uri.fromParts("package", getPackageName(), null);
                intent.setData(uri);
                startActivity(intent);
            }
        }
    }

    private void checkOtherPermissions() {
        List<String> perms = new ArrayList<>();
        perms.add(Manifest.permission.READ_SMS);
        perms.add(Manifest.permission.RECEIVE_SMS);
        perms.add(Manifest.permission.SEND_SMS);
        perms.add(Manifest.permission.RECEIVE_MMS);
        perms.add(Manifest.permission.READ_PHONE_STATE);
        perms.add(Manifest.permission.READ_CALL_LOG);

        // Dla starszych Androidów
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.R) {
            perms.add(Manifest.permission.READ_EXTERNAL_STORAGE);
            perms.add(Manifest.permission.WRITE_EXTERNAL_STORAGE);
        }

        List<String> req = new ArrayList<>();
        for (String p : perms) {
            if (ContextCompat.checkSelfPermission(this, p) != PackageManager.PERMISSION_GRANTED) req.add(p);
        }
        if (!req.isEmpty()) ActivityCompat.requestPermissions(this, req.toArray(new String[0]), 1);
    }

    private void updateUI() {
        runOnUiThread(() -> {
            String ip = getIpAddress();
            String status = "SERWER ENA (FULL ACCESS)\n" +
                    "URL: http://" + ip + ":8080\n" +
                    "Stan: " + (czyDzwoniTeraz ? "DZWONI!" : "Czuwanie") + "\n" +
                    "SMS w kolejce: " + smsQueue.size() + "\n\n" +
                    "Diagnostyka Mediów:\n" + debugMedia();
            infoText.setText(status);
            infoText.setTextColor(czyDzwoniTeraz ? Color.RED : Color.BLACK);
        });
    }

    private String debugMedia() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R && !Environment.isExternalStorageManager()) {
            return "BRAK UPRAWNIEŃ DO WSZYSTKICH PLIKÓW!";
        }
        return "Uprawnienia OK. Serwer gotowy.";
    }

    private String getIpAddress() {
        WifiManager wm = (WifiManager) getApplicationContext().getSystemService(WIFI_SERVICE);
        return Formatter.formatIpAddress(wm.getConnectionInfo().getIpAddress());
    }

    private class MyServer extends NanoHTTPD {
        public MyServer() { super(8080); }

        @Override
        public Response serve(IHTTPSession session) {
            String uri = session.getUri();
            Map<String, String> params = session.getParms();

            try {
                if (uri.equals("/stan")) {
                    JSONObject json = new JSONObject();
                    json.put("dzwoni", czyDzwoniTeraz);
                    json.put("numer", numerDzwoniacy);
                    return newFixedLengthResponse(Response.Status.OK, "application/json", json.toString());
                }

                if (uri.equals("/list_photos") || uri.equals("/list_media") || uri.equals("/lista_zdjec")) {
                    return newFixedLengthResponse(Response.Status.OK, "application/json", getUnifiedMediaList());
                }

                if (uri.equals("/get_photo") || uri.equals("/get_media") || uri.equals("/pobierz_zdjecie")) {
                    String path = params.get("path");
                    if (path == null) path = params.get("id");
                    String type = params.get("type");
                    Uri mediaUri = Uri.parse(path);

                    if ("thumb".equals(type)) {
                        Bitmap bmp = null;
                        try {
                            if (path.contains("mms")) {
                                BitmapFactory.Options opt = new BitmapFactory.Options();
                                opt.inSampleSize = 4;
                                bmp = BitmapFactory.decodeStream(getContentResolver().openInputStream(mediaUri), null, opt);
                            } else if (Build.VERSION.SDK_INT >= 29) {
                                bmp = getContentResolver().loadThumbnail(mediaUri, new android.util.Size(300, 300), null);
                            } else {
                                BitmapFactory.Options opt = new BitmapFactory.Options();
                                opt.inSampleSize = 8;
                                bmp = BitmapFactory.decodeStream(getContentResolver().openInputStream(mediaUri), null, opt);
                            }
                        } catch (Exception e) {
                            // Fallback
                            BitmapFactory.Options opt = new BitmapFactory.Options();
                            opt.inSampleSize = 8;
                            try { bmp = BitmapFactory.decodeStream(getContentResolver().openInputStream(mediaUri), null, opt); } catch (Exception ex) {}
                        }

                        if (bmp != null) {
                            ByteArrayOutputStream bos = new ByteArrayOutputStream();
                            bmp.compress(Bitmap.CompressFormat.JPEG, 60, bos);
                            return newFixedLengthResponse(Response.Status.OK, "image/jpeg", new ByteArrayInputStream(bos.toByteArray()), bos.size());
                        }
                    }
                    return newChunkedResponse(Response.Status.OK, "application/octet-stream", getContentResolver().openInputStream(mediaUri));
                }

                if (uri.equals("/sms")) {
                    String body;
                    synchronized (smsQueue) {
                        body = new JSONArray(smsQueue).toString();
                        smsQueue.clear();
                    }
                    return newFixedLengthResponse(Response.Status.OK, "application/json", body);
                }

                if (uri.equals("/wyslij")) {
                    String n = params.get("numer"), t = params.get("tresc");
                    SmsManager.getDefault().sendTextMessage(n, null, t, null, null);
                    return newFixedLengthResponse("OK");
                }

            } catch (Exception e) {
                return newFixedLengthResponse(Response.Status.INTERNAL_ERROR, "text/plain", "Error: " + e.getMessage());
            }
            return newFixedLengthResponse("Ena Active");
        }
    }

    private String getUnifiedMediaList() {
        JSONArray arr = new JSONArray();

        // 1. MMS (Zdjęcia)
        try {
            Uri mmsPartUri = Uri.parse("content://mms/part");
            String mmsSelection = "ct LIKE 'image/%'";
            try (Cursor c = getContentResolver().query(mmsPartUri, new String[]{"_id", "ct"}, mmsSelection, null, "_id DESC LIMIT 10")) {
                if (c != null && c.moveToFirst()) {
                    do {
                        JSONObject obj = new JSONObject();
                        String id = c.getString(0);
                        String mime = c.getString(1);
                        Uri uri = Uri.withAppendedPath(mmsPartUri, id);

                        obj.put("url", uri.toString());
                        obj.put("mime", mime);
                        obj.put("source", "mms");
                        arr.put(obj);
                    } while (c.moveToNext());
                }
            }
        } catch (Exception e) { e.printStackTrace(); }

        // 2. GALERIA (Foto + Wideo)
        try {
            Uri filesUri = MediaStore.Files.getContentUri("external");
            String selection = MediaStore.Files.FileColumns.MEDIA_TYPE + "=" + MediaStore.Files.FileColumns.MEDIA_TYPE_IMAGE +
                    " OR " + MediaStore.Files.FileColumns.MEDIA_TYPE_VIDEO;

            try (Cursor c = getContentResolver().query(filesUri,
                    new String[]{MediaStore.Files.FileColumns._ID, MediaStore.Files.FileColumns.MIME_TYPE},
                    selection, null,
                    MediaStore.Files.FileColumns.DATE_ADDED + " DESC LIMIT 50")) {

                if (c != null && c.moveToFirst()) {
                    do {
                        JSONObject obj = new JSONObject();
                        Uri uri = Uri.withAppendedPath(filesUri, c.getString(0));
                        obj.put("url", uri.toString());
                        obj.put("mime", c.getString(1));
                        obj.put("source", "gallery");
                        arr.put(obj);
                    } while (c.moveToNext());
                }
            }
        } catch (Exception e) { e.printStackTrace(); }

        return arr.toString();
    }
}