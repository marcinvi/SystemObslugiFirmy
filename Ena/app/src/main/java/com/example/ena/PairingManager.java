package com.example.ena;

import android.content.Context;
import android.content.SharedPreferences;

import java.security.SecureRandom;

public class PairingManager {
    private static final String PREFS_NAME = "ena_pairing";
    private static final String KEY_CODE = "pairing_code";
    private static final String KEY_PAIRED = "paired";
    private static final SecureRandom RANDOM = new SecureRandom();

    private PairingManager() {
    }

    public static String getOrCreateCode(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        String code = prefs.getString(KEY_CODE, null);
        if (code == null || code.isEmpty()) {
            code = generateCode();
            prefs.edit().putString(KEY_CODE, code).apply();
        }
        return code;
    }

    public static boolean isPaired(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        return prefs.getBoolean(KEY_PAIRED, false);
    }

    public static void setPaired(Context context, boolean paired) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        prefs.edit().putBoolean(KEY_PAIRED, paired).apply();
    }

    public static boolean verifyCode(Context context, String code) {
        if (code == null) {
            return false;
        }
        String expected = getOrCreateCode(context);
        return expected.equals(code.trim());
    }

    public static void reset(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        prefs.edit().remove(KEY_PAIRED).remove(KEY_CODE).apply();
    }

    private static String generateCode() {
        int value = 100000 + RANDOM.nextInt(900000);
        return String.valueOf(value);
    }
}
