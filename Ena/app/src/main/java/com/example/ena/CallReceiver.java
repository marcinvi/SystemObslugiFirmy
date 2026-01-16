package com.example.ena;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.telephony.TelephonyManager;

public class CallReceiver extends BroadcastReceiver {
    @Override
    public void onReceive(Context context, Intent intent) {
        if (TelephonyManager.ACTION_PHONE_STATE_CHANGED.equals(intent.getAction())) {
            String state = intent.getStringExtra(TelephonyManager.EXTRA_STATE);
            String incomingNumber = intent.getStringExtra(TelephonyManager.EXTRA_INCOMING_NUMBER);

            if (TelephonyManager.EXTRA_STATE_RINGING.equals(state)) {
                MainActivity.czyDzwoniTeraz = true;
                if (incomingNumber != null) MainActivity.numerDzwoniacy = incomingNumber;
            } else if (TelephonyManager.EXTRA_STATE_IDLE.equals(state)) {
                MainActivity.czyDzwoniTeraz = false;
                MainActivity.numerDzwoniacy = "";
            }
        }
    }
}