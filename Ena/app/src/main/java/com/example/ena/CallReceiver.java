package com.example.ena;

import android.Manifest;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.database.Cursor;
import android.net.Uri;
import android.os.Build;
import android.provider.CallLog;
import android.telephony.TelephonyManager;
import android.util.Log;

import androidx.core.content.ContextCompat;

/**
 * Odbiera zdarzenia o połączeniach przychodzących.
 *
 * WAŻNA ZMIANA: Zamiast tylko zapisywać stan w GlobalState,
 * teraz NATYCHMIAST wysyła zdarzenie do API (BackgroundService.sendPhoneEvent).
 * Dzięki temu WinForms dostaje info o dzwonieniu nawet gdy app jest zamknięta.
 *
 * FIX: Na nowszych Androidach EXTRA_INCOMING_NUMBER jest często null
 * (wymaga READ_CALL_LOG od API 29+). Dodano fallback przez CallLog.
 */
public class CallReceiver extends BroadcastReceiver {

    private static final String TAG = "EnaCallReceiver";

    // Zapamiętaj ostatni stan, żeby nie wysyłać duplikatów
    private static String lastState = "";
    private static String lastNumber = "";

    @Override
    public void onReceive(Context context, Intent intent) {
        if (!TelephonyManager.ACTION_PHONE_STATE_CHANGED.equals(intent.getAction())) {
            return;
        }

        String state = intent.getStringExtra(TelephonyManager.EXTRA_STATE);
        if (state == null) return;

        // Unikaj duplikatów
        if (state.equals(lastState)) return;
        lastState = state;

        if (TelephonyManager.EXTRA_STATE_RINGING.equals(state)) {
            // === DZWONIENIE ===
            String incomingNumber = intent.getStringExtra(TelephonyManager.EXTRA_INCOMING_NUMBER);

            // FIX: Na Android 10+ EXTRA_INCOMING_NUMBER jest null bez READ_CALL_LOG
            if (incomingNumber == null || incomingNumber.isEmpty()) {
                incomingNumber = tryGetNumberFromCallLog(context);
            }

            // Jeśli nadal null, ustaw "unknown" (ale nadal wyślij zdarzenie!)
            if (incomingNumber == null || incomingNumber.isEmpty()) {
                incomingNumber = "unknown";
                Log.w(TAG, "Nie udało się pobrać numeru dzwoniącego. " +
                        "Upewnij się, że aplikacja ma uprawnienie READ_CALL_LOG.");
            }

            Log.d(TAG, "Dzwoni: " + incomingNumber);

            // Zapisz w GlobalState (dla kompatybilności z NanoHTTPD)
            GlobalState.isRinging = true;
            GlobalState.incomingNumber = incomingNumber;
            lastNumber = incomingNumber;

            // NOWE: Wyślij zdarzenie do API natychmiast
            BackgroundService.sendPhoneEvent(context, "CALL_RINGING", incomingNumber, null);

        } else if (TelephonyManager.EXTRA_STATE_IDLE.equals(state)) {
            // === KONIEC DZWONIENIA ===
            Log.d(TAG, "Koniec dzwonienia");

            GlobalState.isRinging = false;
            GlobalState.incomingNumber = "";

            // NOWE: Wyślij zdarzenie CALL_IDLE do API
            BackgroundService.sendPhoneEvent(context, "CALL_IDLE", lastNumber, null);
            lastNumber = "";

        } else if (TelephonyManager.EXTRA_STATE_OFFHOOK.equals(state)) {
            // Rozmowa trwa (odebrano) - opcjonalnie można wysłać event
            Log.d(TAG, "Rozmowa trwa");
        }
    }

    /**
     * Próba pobrania numeru z CallLog (wymaga READ_CALL_LOG).
     * Na Android 10+ to jedyny sposób na uzyskanie numeru dzwoniącego.
     */
    private String tryGetNumberFromCallLog(Context context) {
        // Sprawdź uprawnienie
        if (ContextCompat.checkSelfPermission(context, Manifest.permission.READ_CALL_LOG)
                != PackageManager.PERMISSION_GRANTED) {
            Log.w(TAG, "Brak uprawnienia READ_CALL_LOG - nie można pobrać numeru");
            return null;
        }

        Cursor cursor = null;
        try {
            // Pobierz ostatni wpis z CallLog
            cursor = context.getContentResolver().query(
                    CallLog.Calls.CONTENT_URI,
                    new String[]{CallLog.Calls.NUMBER, CallLog.Calls.TYPE},
                    CallLog.Calls.TYPE + " = ?",
                    new String[]{String.valueOf(CallLog.Calls.INCOMING_TYPE)},
                    CallLog.Calls.DATE + " DESC LIMIT 1"
            );

            if (cursor != null && cursor.moveToFirst()) {
                String number = cursor.getString(cursor.getColumnIndexOrThrow(CallLog.Calls.NUMBER));
                if (number != null && !number.isEmpty()) {
                    Log.d(TAG, "Numer z CallLog: " + number);
                    return number;
                }
            }
        } catch (Exception e) {
            Log.e(TAG, "Błąd odczytu CallLog: " + e.getMessage());
        } finally {
            if (cursor != null) cursor.close();
        }

        return null;
    }
}
