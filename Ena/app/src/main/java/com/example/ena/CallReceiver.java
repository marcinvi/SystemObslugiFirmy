package com.example.ena;

import android.Manifest;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageManager;
import android.os.Build;
import android.telephony.TelephonyManager;
import android.util.Log;

import androidx.core.content.ContextCompat;

/**
 * Odbiera zdarzenia o połączeniach przychodzących.
 *
 * === NOWE: LINK REKLAMACYJNY PO ROZMOWIE ===
 *
 * Po zakończeniu ODEBRANEJ rozmowy przychodzącej (RINGING → OFFHOOK → IDLE):
 *   → Sprawdza ustawienie "show_sms_links" w SharedPreferences
 *   → Jeśli włączone: uruchamia SendLinkActivity z numerem klienta
 *
 * Pomija gdy:
 *   → Ustawienie wyłączone (domyślnie wyłączone)
 *   → Połączenie nieodebrane (RINGING → IDLE, bez OFFHOOK)
 *   → Połączenie wychodzące (OFFHOOK → IDLE, bez RINGING)
 *   → Numer nieznany ("unknown")
 */
public class CallReceiver extends BroadcastReceiver {

    private static final String TAG = "EnaCallReceiver";
    private static final String PREFS_NAME = "ena_prefs";
    private static final String PREF_SHOW_SMS_LINKS = "show_sms_links";

    // Debounce
    private static String lastState = "";
    private static long lastStateTimestamp = 0;
    private static final long DEBOUNCE_MS = 1000;

    // Śledzenie stanu rozmowy
    private static boolean wasIncomingCall = false;
    private static boolean wasAnswered = false;
    private static String incomingNumber = null;

    @Override
    public void onReceive(Context context, Intent intent) {
        if (!TelephonyManager.ACTION_PHONE_STATE_CHANGED.equals(intent.getAction())) {
            return;
        }

        String state = intent.getStringExtra(TelephonyManager.EXTRA_STATE);
        if (state == null) return;

        long now = System.currentTimeMillis();
        if (state.equals(lastState) && (now - lastStateTimestamp) < DEBOUNCE_MS) {
            return;
        }
        lastState = state;
        lastStateTimestamp = now;

        logPermissions(context);

        if (TelephonyManager.EXTRA_STATE_RINGING.equals(state)) {
            handleRinging(context, intent);
        } else if (TelephonyManager.EXTRA_STATE_OFFHOOK.equals(state)) {
            handleOffhook();
        } else if (TelephonyManager.EXTRA_STATE_IDLE.equals(state)) {
            handleIdle(context);
        }
    }

    // ========================================================================
    // RINGING
    // ========================================================================

    private void handleRinging(Context context, Intent intent) {
        IncomingCallTracker tracker = IncomingCallTracker.getInstance();
        String number = null;

        wasIncomingCall = true;
        wasAnswered = false;

        // === POZIOM 1: EXTRA_INCOMING_NUMBER z Intent ===
        String intentNumber = intent.getStringExtra(TelephonyManager.EXTRA_INCOMING_NUMBER);
        if (isValidNumber(intentNumber)) {
            number = intentNumber;
            Log.d(TAG, "[Poziom 1] Numer z EXTRA_INCOMING_NUMBER: " + number);
        }

        // === POZIOM 2: PhoneStateListener (IncomingCallTracker) ===
        if (number == null) {
            String trackerNumber = tracker.getBestNumber();
            if (isValidNumber(trackerNumber)) {
                number = trackerNumber;
                Log.d(TAG, "[Poziom 2] Numer z PhoneStateListener: " + number);
            }
        }

        // === POZIOM 3: Natychmiastowy CallLog ===
        if (number == null) {
            String callLogNumber = tracker.queryCallLogForLastIncoming(context);
            if (isValidNumber(callLogNumber)) {
                number = callLogNumber;
                Log.d(TAG, "[Poziom 3] Numer z CallLog (natychmiast): " + number);
            }
        }

        if (isValidNumber(number)) {
            tracker.setNumberFromReceiver(number);
        }

        incomingNumber = (number != null) ? number : "unknown";

        GlobalState.isRinging = true;
        GlobalState.incomingNumber = incomingNumber;

        Log.i(TAG, "=== DZWONI: " + incomingNumber + " (Android " + Build.VERSION.SDK_INT + ") ===");
        BackgroundService.sendPhoneEvent(context, "CALL_RINGING", incomingNumber, null);

        // === POZIOM 4+5: Opóźniony CallLog jeśli numer nieznany ===
        if (!isValidNumber(number)) {
            Log.d(TAG, "[Poziom 4] Numer nieznany, uruchamiam opóźniony CallLog query...");

            tracker.queryCallLogDelayed(context, 1500, new IncomingCallTracker.CallLogResultCallback() {
                @Override
                public void onNumberFound(String delayedNumber) {
                    Log.i(TAG, "[Poziom 4] Numer z CallLog po 1.5s: " + delayedNumber);
                    incomingNumber = delayedNumber;
                    GlobalState.incomingNumber = delayedNumber;
                    BackgroundService.sendPhoneEvent(context, "CALL_RINGING", delayedNumber, null);
                }

                @Override
                public void onNumberNotFound() {
                    Log.d(TAG, "[Poziom 5] Próba po 4s...");
                    tracker.queryCallLogDelayed(context, 2500, new IncomingCallTracker.CallLogResultCallback() {
                        @Override
                        public void onNumberFound(String lateNumber) {
                            Log.i(TAG, "[Poziom 5] Numer z CallLog po 4s: " + lateNumber);
                            incomingNumber = lateNumber;
                            GlobalState.incomingNumber = lateNumber;
                            BackgroundService.sendPhoneEvent(context, "CALL_RINGING", lateNumber, null);
                        }

                        @Override
                        public void onNumberNotFound() {
                            Log.w(TAG, "Nie udało się pobrać numeru żadną metodą.");
                        }
                    });
                }
            });
        }
    }

    // ========================================================================
    // OFFHOOK (rozmowa odebrana)
    // ========================================================================

    private void handleOffhook() {
        if (wasIncomingCall) {
            wasAnswered = true;
            Log.d(TAG, "Rozmowa odebrana (OFFHOOK po RINGING)");
        } else {
            Log.d(TAG, "Połączenie wychodzące (OFFHOOK bez RINGING)");
        }
    }

    // ========================================================================
    // IDLE (koniec połączenia)
    // ========================================================================

    private void handleIdle(Context context) {
        IncomingCallTracker tracker = IncomingCallTracker.getInstance();

        String number = tracker.getBestNumber();

        // Ostatnia szansa: CallLog po zakończeniu
        if (!tracker.isNumberConfirmed()) {
            String callLogNumber = tracker.queryCallLogForLastIncoming(context);
            if (isValidNumber(callLogNumber)) {
                number = callLogNumber;
                incomingNumber = callLogNumber;
                Log.d(TAG, "[IDLE] Numer z CallLog po zakończeniu: " + number);
            }
        }

        String finalNumber = (number != null) ? number : (incomingNumber != null ? incomingNumber : "");

        Log.i(TAG, "=== KONIEC: " + finalNumber +
                " (incoming=" + wasIncomingCall + " answered=" + wasAnswered + ") ===");

        GlobalState.isRinging = false;
        GlobalState.incomingNumber = "";

        BackgroundService.sendPhoneEvent(context, "CALL_IDLE", finalNumber, null);

        // === Pokaż dialog z linkami reklamacyjnymi ===
        if (wasIncomingCall && wasAnswered && isValidNumber(finalNumber)) {
            // Sprawdź czy użytkownik włączył tę opcję w ustawieniach
            if (isSmsLinksEnabled(context)) {
                Log.i(TAG, "Uruchamiam SendLinkActivity dla: " + finalNumber);
                try {
                    Intent linkIntent = new Intent(context, SendLinkActivity.class);
                    linkIntent.putExtra("phone_number", finalNumber);
                    linkIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                    context.startActivity(linkIntent);
                } catch (Exception e) {
                    Log.e(TAG, "Błąd uruchomienia SendLinkActivity: " + e.getMessage());
                }
            } else {
                Log.d(TAG, "Pominięto dialog linków - opcja wyłączona w ustawieniach");
            }
        } else {
            if (!wasIncomingCall) {
                Log.d(TAG, "Pominięto dialog - nie było połączenie przychodzące");
            } else if (!wasAnswered) {
                Log.d(TAG, "Pominięto dialog - połączenie nieodebrane");
            } else {
                Log.d(TAG, "Pominięto dialog - numer nieznany");
            }
        }

        // Reset
        tracker.reset();
        wasIncomingCall = false;
        wasAnswered = false;
        incomingNumber = null;
    }

    // ========================================================================
    // Helpers
    // ========================================================================

    private boolean isValidNumber(String number) {
        return number != null
                && !number.isEmpty()
                && !"unknown".equalsIgnoreCase(number)
                && !"-1".equals(number)
                && !"-2".equals(number);
    }

    /**
     * Sprawdza czy opcja wysyłania linków SMS jest włączona w ustawieniach.
     * Domyślnie: WYŁĄCZONA (false).
     */
    private boolean isSmsLinksEnabled(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        return prefs.getBoolean(PREF_SHOW_SMS_LINKS, false);
    }

    private void logPermissions(Context context) {
        boolean phoneState = ContextCompat.checkSelfPermission(context, Manifest.permission.READ_PHONE_STATE)
                == PackageManager.PERMISSION_GRANTED;
        boolean callLog = ContextCompat.checkSelfPermission(context, Manifest.permission.READ_CALL_LOG)
                == PackageManager.PERMISSION_GRANTED;
        boolean callPhone = ContextCompat.checkSelfPermission(context, Manifest.permission.CALL_PHONE)
                == PackageManager.PERMISSION_GRANTED;
        boolean sendSms = ContextCompat.checkSelfPermission(context, Manifest.permission.SEND_SMS)
                == PackageManager.PERMISSION_GRANTED;

        Log.d(TAG, String.format("Uprawnienia: PHONE_STATE=%s, CALL_LOG=%s, CALL_PHONE=%s, SEND_SMS=%s | API %d",
                phoneState, callLog, callPhone, sendSms, Build.VERSION.SDK_INT));
    }
}
