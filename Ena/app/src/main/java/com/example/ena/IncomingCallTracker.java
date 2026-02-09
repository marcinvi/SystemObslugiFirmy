package com.example.ena;

import android.Manifest;
import android.content.Context;
import android.content.pm.PackageManager;
import android.database.Cursor;
import android.os.Build;
import android.os.Handler;
import android.os.Looper;
import android.provider.CallLog;
import android.telephony.PhoneStateListener;
import android.telephony.TelephonyCallback;
import android.telephony.TelephonyManager;
import android.util.Log;

import androidx.annotation.RequiresApi;
import androidx.core.content.ContextCompat;

/**
 * Singleton śledzący numer telefonu dzwoniącego.
 *
 * Rozwiązanie wielopoziomowe:
 * POZIOM 1: PhoneStateListener / TelephonyCallback (rejestrowany w BackgroundService)
 * POZIOM 2: EXTRA_INCOMING_NUMBER z Intent w BroadcastReceiver (CallReceiver)
 * POZIOM 3: Zapytanie CallLog z opóźnieniem (1.5s)
 * POZIOM 4: Zapytanie CallLog przy IDLE (koniec połączenia)
 */
public class IncomingCallTracker {

    private static final String TAG = "EnaCallTracker";

    private static IncomingCallTracker instance;

    private volatile String currentNumber = null;
    private volatile boolean isRinging = false;
    private volatile long ringingTimestamp = 0;
    private volatile boolean numberConfirmed = false;

    private PhoneStateListener legacyListener;
    private Object telephonyCallback;

    private final Handler handler = new Handler(Looper.getMainLooper());

    private IncomingCallTracker() {}

    public static synchronized IncomingCallTracker getInstance() {
        if (instance == null) {
            instance = new IncomingCallTracker();
        }
        return instance;
    }

    // ==========================================================================
    // Rejestracja listenerów (wywoływane z BackgroundService)
    // ==========================================================================

    public void registerListener(Context context) {
        TelephonyManager tm = (TelephonyManager) context.getSystemService(Context.TELEPHONY_SERVICE);
        if (tm == null) {
            Log.e(TAG, "TelephonyManager niedostępny");
            return;
        }

        boolean hasPhoneState = ContextCompat.checkSelfPermission(context, Manifest.permission.READ_PHONE_STATE)
                == PackageManager.PERMISSION_GRANTED;
        boolean hasCallLog = ContextCompat.checkSelfPermission(context, Manifest.permission.READ_CALL_LOG)
                == PackageManager.PERMISSION_GRANTED;

        Log.d(TAG, "Uprawnienia: READ_PHONE_STATE=" + hasPhoneState + ", READ_CALL_LOG=" + hasCallLog);

        if (!hasPhoneState) {
            Log.w(TAG, "Brak READ_PHONE_STATE - nie można zarejestrować listenera");
            return;
        }

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
            registerTelephonyCallback(context, tm);
        } else {
            registerLegacyListener(tm);
        }
    }

    public void unregisterListener(Context context) {
        TelephonyManager tm = (TelephonyManager) context.getSystemService(Context.TELEPHONY_SERVICE);
        if (tm == null) return;

        // Wyrejestruj TelephonyCallback (API 31+)
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S && telephonyCallback != null) {
            try {
                tm.unregisterTelephonyCallback((TelephonyCallback) telephonyCallback);
                Log.d(TAG, "TelephonyCallback wyrejestrowany");
            } catch (Exception e) {
                Log.w(TAG, "Błąd wyrejestrowania TelephonyCallback: " + e.getMessage());
            }
            telephonyCallback = null;
        }

        // Wyrejestruj legacy PhoneStateListener (zarejestrowany ZAWSZE - także na API 31+)
        if (legacyListener != null) {
            try {
                tm.listen(legacyListener, PhoneStateListener.LISTEN_NONE);
                Log.d(TAG, "PhoneStateListener wyrejestrowany");
            } catch (Exception e) {
                Log.w(TAG, "Błąd wyrejestrowania PhoneStateListener: " + e.getMessage());
            }
            legacyListener = null;
        }
    }

    @SuppressWarnings("deprecation")
    private void registerLegacyListener(TelephonyManager tm) {
        legacyListener = new PhoneStateListener() {
            @Override
            public void onCallStateChanged(int state, String incomingNumber) {
                Log.d(TAG, "[PhoneStateListener] state=" + state + " number=" + incomingNumber);
                handleCallState(state, incomingNumber);
            }
        };

        try {
            tm.listen(legacyListener, PhoneStateListener.LISTEN_CALL_STATE);
            Log.d(TAG, "PhoneStateListener zarejestrowany (API < 31)");
        } catch (SecurityException e) {
            Log.e(TAG, "SecurityException rejestracji listenera: " + e.getMessage());
        }
    }

    @RequiresApi(api = Build.VERSION_CODES.S)
    private void registerTelephonyCallback(Context context, TelephonyManager tm) {
        EnaCallStateCallback callback = new EnaCallStateCallback();

        try {
            tm.registerTelephonyCallback(context.getMainExecutor(), callback);
            telephonyCallback = callback;
            Log.d(TAG, "TelephonyCallback zarejestrowany (API 31+)");
        } catch (SecurityException e) {
            Log.e(TAG, "SecurityException rejestracji TelephonyCallback: " + e.getMessage());
        }

        // KRYTYCZNE: Na API 31+ TelephonyCallback.CallStateListener NIE daje numeru!
        // Rejestrujemy RÓWNIEŻ legacy PhoneStateListener który nadal działa
        // i nadal dostarcza numer na wielu urządzeniach (Samsung, Xiaomi, Pixel).
        // Deprecated != usunięty - działa nawet na Android 14.
        registerLegacyListener(tm);
        Log.d(TAG, "Legacy PhoneStateListener RÓWNIEŻ zarejestrowany (fallback na numer)");
    }

    @RequiresApi(api = Build.VERSION_CODES.S)
    private class EnaCallStateCallback extends TelephonyCallback implements TelephonyCallback.CallStateListener {
        @Override
        public void onCallStateChanged(int state) {
            Log.d(TAG, "[TelephonyCallback] state=" + state);
            if (state == TelephonyManager.CALL_STATE_RINGING) {
                isRinging = true;
                ringingTimestamp = System.currentTimeMillis();
            } else if (state == TelephonyManager.CALL_STATE_IDLE) {
                isRinging = false;
            }
        }
    }

    // ==========================================================================
    // Obsługa stanu połączenia
    // ==========================================================================

    private void handleCallState(int state, String phoneNumber) {
        if (state == TelephonyManager.CALL_STATE_RINGING) {
            isRinging = true;
            ringingTimestamp = System.currentTimeMillis();

            if (phoneNumber != null && !phoneNumber.isEmpty() && !"unknown".equalsIgnoreCase(phoneNumber)) {
                currentNumber = phoneNumber;
                numberConfirmed = true;
                Log.d(TAG, "Numer z PhoneStateListener: " + phoneNumber);
            }
        } else if (state == TelephonyManager.CALL_STATE_IDLE) {
            isRinging = false;
        }
    }

    // ==========================================================================
    // Metody publiczne
    // ==========================================================================

    public void setNumberFromReceiver(String number) {
        if (number != null && !number.isEmpty() && !"unknown".equalsIgnoreCase(number)) {
            currentNumber = number;
            numberConfirmed = true;
            Log.d(TAG, "Numer z BroadcastReceiver: " + number);
        }
    }

    public String getBestNumber() {
        if (numberConfirmed && currentNumber != null) {
            return currentNumber;
        }
        return null;
    }

    public boolean isNumberConfirmed() {
        return numberConfirmed;
    }

    public void reset() {
        currentNumber = null;
        numberConfirmed = false;
        isRinging = false;
        ringingTimestamp = 0;
    }

    public void queryCallLogDelayed(Context context, long delayMs, CallLogResultCallback callback) {
        handler.postDelayed(() -> {
            String number = queryCallLogForLastIncoming(context);
            if (number != null && !number.isEmpty()) {
                if (!numberConfirmed) {
                    currentNumber = number;
                    numberConfirmed = true;
                    Log.d(TAG, "Numer z CallLog (opóźniony): " + number);
                }
                if (callback != null) {
                    callback.onNumberFound(number);
                }
            } else {
                Log.d(TAG, "CallLog nadal nie ma numeru po " + delayMs + "ms");
                if (callback != null) {
                    callback.onNumberNotFound();
                }
            }
        }, delayMs);
    }

    public String queryCallLogForLastIncoming(Context context) {
        if (ContextCompat.checkSelfPermission(context, Manifest.permission.READ_CALL_LOG)
                != PackageManager.PERMISSION_GRANTED) {
            Log.w(TAG, "Brak READ_CALL_LOG - nie można odpytać CallLog");
            return null;
        }

        Cursor cursor = null;
        try {
            long thirtySecsAgo = System.currentTimeMillis() - 30_000;

            cursor = context.getContentResolver().query(
                    CallLog.Calls.CONTENT_URI,
                    new String[]{
                            CallLog.Calls.NUMBER,
                            CallLog.Calls.TYPE,
                            CallLog.Calls.DATE,
                            CallLog.Calls.CACHED_NAME
                    },
                    CallLog.Calls.DATE + " > ? AND " + CallLog.Calls.TYPE + " IN (?, ?)",
                    new String[]{
                            String.valueOf(thirtySecsAgo),
                            String.valueOf(CallLog.Calls.INCOMING_TYPE),
                            String.valueOf(CallLog.Calls.MISSED_TYPE)
                    },
                    CallLog.Calls.DATE + " DESC LIMIT 1"
            );

            if (cursor != null && cursor.moveToFirst()) {
                String number = cursor.getString(cursor.getColumnIndexOrThrow(CallLog.Calls.NUMBER));
                if (number != null && !number.isEmpty()) {
                    Log.d(TAG, "Znaleziono w CallLog: " + number);
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

    // ==========================================================================
    // Callback
    // ==========================================================================

    public interface CallLogResultCallback {
        void onNumberFound(String number);
        void onNumberNotFound();
    }
}
