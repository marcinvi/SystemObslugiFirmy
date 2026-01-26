package com.example.ena;

import android.content.Context;
import android.content.SharedPreferences;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

public class UserSession {
    private static final String PREFS_NAME = "ena_user_session";
    private static final String KEY_TOKEN = "token";
    private static final String KEY_REFRESH = "refresh_token";
    private static final String KEY_LOGIN = "login";
    private static final String KEY_DISPLAY_NAME = "display_name";
    private static final String KEY_USER_ID = "user_id";
    private static final String KEY_MODULES = "modules";
    private static final String KEY_RECENT_LOGINS = "recent_logins";
    private static final int MAX_RECENT_LOGINS = 5;

    private UserSession() {
    }

    // --- NOWE METODY NAPRAWIAJĄCE BŁĄD W MAINACTIVITY ---

    public static String getUserName(Context context) {
        String displayName = getDisplayName(context);
        if (displayName != null && !displayName.trim().isEmpty()) {
            return displayName;
        }
        return getLogin(context);
    }

    public static void logout(Context context) {
        clear(context);
    }

    // ----------------------------------------------------

    public static void saveLogin(Context context, String login, String displayName, int userId, String token, String refreshToken) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        prefs.edit()
                .putString(KEY_LOGIN, login == null ? "" : login)
                .putString(KEY_DISPLAY_NAME, displayName == null ? "" : displayName)
                .putInt(KEY_USER_ID, userId)
                .putString(KEY_TOKEN, token == null ? "" : token)
                .putString(KEY_REFRESH, refreshToken == null ? "" : refreshToken)
                .apply();
        saveRecentLogin(context, login);
    }

    public static boolean isLoggedIn(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        String token = prefs.getString(KEY_TOKEN, "");
        return token != null && !token.isEmpty();
    }

    public static String getToken(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        return prefs.getString(KEY_TOKEN, "");
    }

    public static String getLogin(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        return prefs.getString(KEY_LOGIN, "");
    }

    public static String getDisplayName(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        return prefs.getString(KEY_DISPLAY_NAME, "");
    }

    public static int getUserId(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        return prefs.getInt(KEY_USER_ID, 0);
    }

    public static void saveModules(Context context, List<String> modules) {
        Set<String> moduleSet = new HashSet<>();
        if (modules != null) {
            moduleSet.addAll(modules);
        }
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        prefs.edit().putStringSet(KEY_MODULES, moduleSet).apply();
    }

    public static List<String> getModules(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        Set<String> moduleSet = prefs.getStringSet(KEY_MODULES, new HashSet<>());
        return moduleSet == null ? new ArrayList<>() : new ArrayList<>(moduleSet);
    }

    public static List<String> getRecentLogins(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        String raw = prefs.getString(KEY_RECENT_LOGINS, "[]");
        List<String> logins = new ArrayList<>();
        try {
            org.json.JSONArray array = new org.json.JSONArray(raw);
            for (int i = 0; i < array.length(); i++) {
                String value = array.optString(i, "");
                if (value != null && !value.isEmpty()) {
                    logins.add(value);
                }
            }
        } catch (org.json.JSONException ignored) {
        }
        return logins;
    }

    public static void saveRecentLogin(Context context, String login) {
        if (login == null || login.trim().isEmpty()) {
            return;
        }
        List<String> logins = getRecentLogins(context);
        logins.remove(login);
        logins.add(0, login);
        while (logins.size() > MAX_RECENT_LOGINS) {
            logins.remove(logins.size() - 1);
        }
        org.json.JSONArray array = new org.json.JSONArray();
        for (String item : logins) {
            array.put(item);
        }
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        prefs.edit().putString(KEY_RECENT_LOGINS, array.toString()).apply();
    }

    public static void clear(Context context) {
        SharedPreferences prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE);
        prefs.edit().clear().apply();
    }
}