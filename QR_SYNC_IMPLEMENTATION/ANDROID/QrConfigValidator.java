package com.example.ena;

import com.google.gson.Gson;

import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;
import java.util.TimeZone;

/**
 * Walidator konfiguracji z QR Code
 * Sprawdza poprawność danych, podpisu i czasu wygaśnięcia
 */
public class QrConfigValidator {

    private static final String EXPECTED_VERSION = "1.0";
    private static final String EXPECTED_TYPE = "ENA_SYNC";
    private static final int EXPIRY_MINUTES = 5; // QR Code ważny przez 5 minut

    /**
     * Waliduje kompletną konfigurację z QR Code
     * @param qrConfig Konfiguracja do walidacji
     * @return null jeśli OK, komunikat błędu jeśli niepoprawna
     */
    public static String validate(QrConfigModel qrConfig) {
        if (qrConfig == null) {
            return "Brak danych konfiguracji";
        }

        // 1. Sprawdź czy wszystkie pola są wypełnione
        if (!qrConfig.hasAllRequiredFields()) {
            return "Niepełne dane konfiguracji - brakuje wymaganych pól";
        }

        // 2. Sprawdź wersję
        if (!EXPECTED_VERSION.equals(qrConfig.version)) {
            return "Nieobsługiwana wersja: " + qrConfig.version + 
                   " (oczekiwana: " + EXPECTED_VERSION + ")";
        }

        // 3. Sprawdź typ
        if (!EXPECTED_TYPE.equals(qrConfig.type)) {
            return "Niepoprawny typ: " + qrConfig.type + 
                   " (oczekiwany: " + EXPECTED_TYPE + ")";
        }

        // 4. Sprawdź kod parowania (musi być 6 cyfr)
        if (qrConfig.config.pairingCode.length() != 6) {
            return "Niepoprawny kod parowania (musi być 6 cyfr)";
        }

        // 5. Sprawdź URL API
        if (!isValidUrl(qrConfig.config.apiBaseUrl)) {
            return "Niepoprawny URL API: " + qrConfig.config.apiBaseUrl;
        }

        // 6. Sprawdź IP telefonu
        if (!isValidIp(qrConfig.config.phoneIp)) {
            return "Niepoprawny adres IP: " + qrConfig.config.phoneIp;
        }

        // 7. Sprawdź czy nie wygasł
        String expiryError = checkExpiry(qrConfig.config.timestamp);
        if (expiryError != null) {
            return expiryError;
        }

        // 8. Sprawdź podpis
        String signatureError = verifySignature(qrConfig);
        if (signatureError != null) {
            return signatureError;
        }

        // Wszystko OK
        return null;
    }

    /**
     * Sprawdza czy URL jest poprawny
     */
    private static boolean isValidUrl(String url) {
        if (url == null || url.isEmpty()) {
            return false;
        }
        return url.startsWith("http://") || url.startsWith("https://");
    }

    /**
     * Sprawdza czy IP jest poprawny (podstawowa walidacja)
     */
    private static boolean isValidIp(String ip) {
        if (ip == null || ip.isEmpty()) {
            return false;
        }
        String[] parts = ip.split("\\.");
        if (parts.length != 4) {
            return false;
        }
        try {
            for (String part : parts) {
                int num = Integer.parseInt(part);
                if (num < 0 || num > 255) {
                    return false;
                }
            }
            return true;
        } catch (NumberFormatException e) {
            return false;
        }
    }

    /**
     * Sprawdza czy QR Code nie wygasł
     */
    private static String checkExpiry(String timestampStr) {
        try {
            // Parsuj timestamp (format ISO 8601)
            SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'", Locale.US);
            sdf.setTimeZone(TimeZone.getTimeZone("UTC"));
            Date timestamp = sdf.parse(timestampStr);

            if (timestamp == null) {
                return "Niepoprawny format daty";
            }

            // Sprawdź czy nie wygasł (5 minut)
            Date expiryTime = new Date(timestamp.getTime() + (EXPIRY_MINUTES * 60 * 1000));
            Date now = new Date();

            if (now.after(expiryTime)) {
                long minutesAgo = (now.getTime() - timestamp.getTime()) / (60 * 1000);
                return "QR Code wygasł " + minutesAgo + " minut temu.\n\n" +
                       "Wygeneruj nowy kod w aplikacji Windows.";
            }

            return null; // OK
        } catch (ParseException e) {
            return "Niepoprawny format daty: " + timestampStr;
        } catch (Exception e) {
            return "Błąd sprawdzania daty: " + e.getMessage();
        }
    }

    /**
     * Weryfikuje podpis SHA256
     */
    private static String verifySignature(QrConfigModel qrConfig) {
        try {
            // 1. Wygeneruj JSON bez podpisu (tak jak w Windows)
            Gson gson = new Gson();
            String jsonWithoutSignature = gson.toJson(qrConfig.config);

            // 2. Oblicz SHA256
            String calculatedSignature = sha256(jsonWithoutSignature);

            // 3. Porównaj z podpisem z QR Code
            if (!calculatedSignature.equals(qrConfig.signature)) {
                return "Niepoprawny podpis - dane mogły zostać zmodyfikowane.\n\n" +
                       "Nie używaj tego QR Code!";
            }

            return null; // OK
        } catch (Exception e) {
            return "Błąd weryfikacji podpisu: " + e.getMessage();
        }
    }

    /**
     * Oblicza hash SHA256
     */
    private static String sha256(String input) throws Exception {
        MessageDigest digest = MessageDigest.getInstance("SHA-256");
        byte[] hash = digest.digest(input.getBytes(StandardCharsets.UTF_8));
        
        // Konwertuj do hex string
        StringBuilder hexString = new StringBuilder();
        for (byte b : hash) {
            String hex = Integer.toHexString(0xff & b);
            if (hex.length() == 1) {
                hexString.append('0');
            }
            hexString.append(hex);
        }
        return hexString.toString();
    }

    /**
     * Sprawdza czy timestamp nie jest w przyszłości
     */
    private static boolean isInFuture(String timestampStr) {
        try {
            SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'", Locale.US);
            sdf.setTimeZone(TimeZone.getTimeZone("UTC"));
            Date timestamp = sdf.parse(timestampStr);
            
            if (timestamp == null) {
                return false;
            }

            Date now = new Date();
            return timestamp.after(now);
        } catch (Exception e) {
            return false;
        }
    }
}
