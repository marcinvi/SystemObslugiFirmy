package com.example.ena;

import android.app.Application;
import android.content.Intent;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;
import android.widget.Toast;

public class EnaApplication extends Application {
    @Override
    public void onCreate() {
        super.onCreate();

        // Globalny łapacz błędów
        Thread.setDefaultUncaughtExceptionHandler((thread, e) -> {
            Log.e("ENA_CRASH", "Nieoczekiwany błąd: " + e.getMessage(), e);

            new Handler(Looper.getMainLooper()).post(() -> {
                Toast.makeText(getApplicationContext(),
                        "KRYTYCZNY BŁĄD: " + e.getClass().getSimpleName() + "\n" + e.getMessage(),
                        Toast.LENGTH_LONG).show();
            });

            // Opcjonalnie: Opóźnij zamknięcie, aby user zdążył przeczytać
            try { Thread.sleep(3000); } catch (InterruptedException ignored) {}

            System.exit(1);
        });
    }
}