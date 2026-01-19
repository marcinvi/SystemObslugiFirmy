package com.example.ena;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.telephony.SmsMessage;
public class SmsReceiver extends BroadcastReceiver {
    @Override
    public void onReceive(Context context, Intent intent) {
        if ("android.provider.Telephony.SMS_RECEIVED".equals(intent.getAction())) {
            Bundle bundle = intent.getExtras();
            if (bundle != null) {
                Object[] pdus = (Object[]) bundle.get("pdus");
                if (pdus != null) {
                    // Tablica na części wiadomości
                    SmsMessage[] msgs = new SmsMessage[pdus.length];
                    StringBuilder fullContent = new StringBuilder();
                    String senderNumber = "";

                    for (int i = 0; i < pdus.length; i++) {
                        msgs[i] = SmsMessage.createFromPdu((byte[]) pdus[i]);
                        if (i == 0) senderNumber = msgs[i].getOriginatingAddress();
                        fullContent.append(msgs[i].getMessageBody());
                    }

                    GlobalState.SmsData data = new GlobalState.SmsData(
                            senderNumber,
                            fullContent.toString()
                    );
                    synchronized (GlobalState.smsQueue) {
                        GlobalState.smsQueue.add(data);
                    }
                }
            }
        }
    }
}
