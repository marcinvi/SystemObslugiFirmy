package com.example.ena;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class GlobalState {
    public static boolean isRinging = false;
    public static String incomingNumber = "";

    // Lista SMS (thread-safe)
    public static List<SmsData> smsQueue = Collections.synchronizedList(new ArrayList<>());

    public static class SmsData {
        String number;
        String content;
        public SmsData(String n, String c) { number=n; content=c; }
    }
}