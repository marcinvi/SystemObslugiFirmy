package com.example.ena;

import com.google.gson.annotations.SerializedName;

/**
 * Model danych dla konfiguracji z QR Code
 * Odpowiada strukturze JSON generowanej przez aplikację Windows
 */
public class QrConfigModel {

    @SerializedName("version")
    public String version;

    @SerializedName("type")
    public String type;

    @SerializedName("config")
    public ConfigData config;

    @SerializedName("signature")
    public String signature;

    /**
     * Dane konfiguracyjne
     */
    public static class ConfigData {
        @SerializedName("apiBaseUrl")
        public String apiBaseUrl;

        @SerializedName("phoneIp")
        public String phoneIp;

        @SerializedName("pairingCode")
        public String pairingCode;

        @SerializedName("userName")
        public String userName;

        @SerializedName("timestamp")
        public String timestamp; // Format: ISO 8601 (2025-01-20T10:30:00Z)
    }

    /**
     * Konstruktor domyślny
     */
    public QrConfigModel() {
    }

    /**
     * Sprawdza czy wszystkie wymagane pola są wypełnione
     */
    public boolean hasAllRequiredFields() {
        return version != null && !version.isEmpty()
                && type != null && !type.isEmpty()
                && signature != null && !signature.isEmpty()
                && config != null
                && config.apiBaseUrl != null && !config.apiBaseUrl.isEmpty()
                && config.phoneIp != null && !config.phoneIp.isEmpty()
                && config.pairingCode != null && !config.pairingCode.isEmpty()
                && config.userName != null && !config.userName.isEmpty()
                && config.timestamp != null && !config.timestamp.isEmpty();
    }

    /**
     * Zwraca czytelny opis konfiguracji
     */
    @Override
    public String toString() {
        if (config == null) {
            return "QrConfigModel{brak danych}";
        }
        return "QrConfigModel{" +
                "version='" + version + '\'' +
                ", type='" + type + '\'' +
                ", apiBaseUrl='" + config.apiBaseUrl + '\'' +
                ", phoneIp='" + config.phoneIp + '\'' +
                ", userName='" + config.userName + '\'' +
                ", timestamp='" + config.timestamp + '\'' +
                '}';
    }
}
