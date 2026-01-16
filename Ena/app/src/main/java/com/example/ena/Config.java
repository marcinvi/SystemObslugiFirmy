package com.example.ena;

import android.content.Context;
import android.content.SharedPreferences;

public class Config {
    private static final String PREF_NAME = "EnaPrefs";
    private static final String KEY_SERVER_URL = "server_url";

    public static void saveServerUrl(Context context, String url) {
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        prefs.edit().putString(KEY_SERVER_URL, url).apply();
    }

    public static String getServerUrl(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        return prefs.getString(KEY_SERVER_URL, null); // Zwraca np. "http://192.168.1.15:5500"
    }
}