package com.example.ena;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.telephony.SmsMessage;
import android.util.Log;

/**
 * Odbiera przychodzące SMS-y.
 *
 * ZMIANA: Oprócz dodawania do GlobalState.smsQueue (dla NanoHTTPD),
 * natychmiast wysyła zdarzenie SMS_RECEIVED do API.
 */
public class SmsReceiver extends BroadcastReceiver {

    private static final String TAG = "EnaSmsReceiver";

    @Override
    public void onReceive(Context context, Intent intent) {
        if (!"android.provider.Telephony.SMS_RECEIVED".equals(intent.getAction())) {
            return;
        }

        Bundle bundle = intent.getExtras();
        if (bundle == null) return;

        Object[] pdus = (Object[]) bundle.get("pdus");
        if (pdus == null) return;

        SmsMessage[] msgs = new SmsMessage[pdus.length];
        StringBuilder fullContent = new StringBuilder();
        String senderNumber = "";

        for (int i = 0; i < pdus.length; i++) {
            String format = bundle.getString("format", "3gpp");
            msgs[i] = SmsMessage.createFromPdu((byte[]) pdus[i], format);
            if (i == 0) senderNumber = msgs[i].getOriginatingAddress();
            fullContent.append(msgs[i].getMessageBody());
        }

        String content = fullContent.toString();

        Log.d(TAG, "SMS od: " + senderNumber + " treść: " + content);

        // 1. Zachowaj w GlobalState (kompatybilność z NanoHTTPD)
        GlobalState.SmsData data = new GlobalState.SmsData(senderNumber, content);
        synchronized (GlobalState.smsQueue) {
            GlobalState.smsQueue.add(data);
        }

        // 2. NOWE: Wyślij zdarzenie do API natychmiast
        BackgroundService.sendPhoneEvent(context, "SMS_RECEIVED", senderNumber, content);
    }
}
