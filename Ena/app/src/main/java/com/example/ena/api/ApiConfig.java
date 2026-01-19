package com.example.ena.api;

import android.content.Context;
import android.content.SharedPreferences;

public final class ApiConfig {
    private static final String PREFS = "ena_prefs";
    private static final String KEY_BASE_URL = "base_url";

    private ApiConfig() {
    }

    public static String getBaseUrl(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS, Context.MODE_PRIVATE);
        return prefs.getString(KEY_BASE_URL, "");
    }

    public static void setBaseUrl(Context context, String baseUrl) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS, Context.MODE_PRIVATE);
        prefs.edit().putString(KEY_BASE_URL, baseUrl == null ? "" : baseUrl.trim()).apply();
    }
}
