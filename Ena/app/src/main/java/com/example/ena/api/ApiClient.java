package com.example.ena.api;

import android.content.Context;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.reflect.TypeToken;
import android.util.Log;
import com.example.ena.PairingManager;
import com.example.ena.NetworkUtils;
import java.io.IOException;
import java.lang.reflect.Type;
import java.time.OffsetDateTime;
import java.util.ArrayList;
import java.util.List;
import java.security.cert.CertPathValidatorException;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;
import javax.net.ssl.HostnameVerifier;
import javax.net.ssl.SSLContext;
import javax.net.ssl.SSLHandshakeException;
import javax.net.ssl.SSLSession;
import javax.net.ssl.TrustManager;
import javax.net.ssl.X509TrustManager;
import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.HttpUrl;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;
import java.util.concurrent.TimeUnit;

public class ApiClient {
    private static final MediaType JSON = MediaType.parse("application/json; charset=utf-8");
    private static final OkHttpClient CLIENT = new OkHttpClient.Builder()
        .connectTimeout(5, TimeUnit.SECONDS)
        .readTimeout(10, TimeUnit.SECONDS)
        .writeTimeout(10, TimeUnit.SECONDS)
        .build();
    private static final OkHttpClient UNSAFE_TLS_CLIENT = buildUnsafeTlsClient();
    private static final Gson GSON = new GsonBuilder()
        .registerTypeAdapter(OffsetDateTime.class, new OffsetDateTimeAdapter())
        .create();

    public interface ApiCallback<T> {
        void onSuccess(T data);

        void onError(String message);
    }

    private final Context context;

    public ApiClient(Context context) {
        this.context = context.getApplicationContext();
    }

    private String buildUrl(String path) {
        String base = ApiConfig.getBaseUrl(context);
        String url = buildUrlWithBase(base, path);
        Log.d("ApiClient", "Building URL: base='" + base + "', path='" + path + "', result='" + url + "'");
        return url;
    }

    private String buildUrlWithBase(String base, String path) {
        if (base == null || base.isEmpty()) {
            return null;
        }
        if (base.endsWith("/")) {
            base = base.substring(0, base.length() - 1);
        }
        if (!path.startsWith("/")) {
            path = "/" + path;
        }
        return base + path;
    }

    private <T> void get(String path, Type type, ApiCallback<T> callback) {
        String url = buildUrl(path);
        if (url == null) {
            callback.onError("Brak adresu API. Ustaw go w Ustawieniach.");
            return;
        }
        executeGetWithFallback(url, path, type, callback);
    }

    private void sendJson(String path, Object payload, String method, ApiCallback<Void> callback) {
        String url = buildUrl(path);
        if (url == null) {
            callback.onError("Brak adresu API. Ustaw go w Ustawieniach.");
            return;
        }
        String json = GSON.toJson(payload);
        RequestBody body = RequestBody.create(json, JSON);
        executeSendWithFallback(url, path, method, body, callback);
    }

    private Request.Builder buildRequest(String url) {
        Request.Builder builder = new Request.Builder().url(url);
        String user = PairingManager.getPairedUser(context);
        if (user != null && !user.isEmpty()) {
            builder.addHeader("X-User", user);
        }
        return builder;
    }

    private void executeSendWithFallback(String url, String path, String method, RequestBody body, ApiCallback<Void> callback) {
        Request request = buildRequest(url).method(method, body).build();
        OkHttpClient client = selectClient(url);
        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                String failedUrl = call.request().url().toString();
                Log.e("ApiClient", "Request failed: " + failedUrl, e);
                if (tryCleartextFallback(failedUrl, path, method, body, callback, e)) {
                    return;
                }
                if (tryUnsafeTlsFallback(failedUrl, path, method, body, callback, e)) {
                    return;
                }
                retrySendWithFallback(path, method, body, callback, e);
            }

            @Override
            public void onResponse(Call call, Response response) {
                if (!response.isSuccessful()) {
                    callback.onError("HTTP " + response.code());
                    return;
                }
                callback.onSuccess(null);
            }
        });
    }

    private <T> void executeGetWithFallback(String url, String path, Type type, ApiCallback<T> callback) {
        Request request = buildRequest(url).get().build();
        OkHttpClient client = selectClient(url);
        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                String failedUrl = call.request().url().toString();
                Log.e("ApiClient", "Request failed: " + failedUrl, e);
                if (tryCleartextFallback(failedUrl, path, type, callback, e)) {
                    return;
                }
                if (tryUnsafeTlsFallback(failedUrl, path, type, callback, e)) {
                    return;
                }
                retryGetWithFallback(path, type, callback, e);
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                if (!response.isSuccessful()) {
                    callback.onError("HTTP " + response.code());
                    return;
                }
                String body = response.body() != null ? response.body().string() : "";
                ApiResponse<T> apiResponse = GSON.fromJson(body, type);
                if (apiResponse == null || !apiResponse.isSuccess() || apiResponse.getData() == null) {
                    String message = apiResponse != null ? apiResponse.getMessage() : "Brak danych z API";
                    callback.onError(message != null ? message : "Brak danych z API");
                    return;
                }
                callback.onSuccess(apiResponse.getData());
            }
        });
    }

    private <T> void retryGetWithFallback(String path, Type type, ApiCallback<T> callback, IOException originalError) {
        // Próbujemy z fallback URL
        String fallback = ApiConfig.getFallbackBaseUrl(context);
        
        if (fallback != null && !fallback.isEmpty()) {
            String fallbackUrl = buildUrlWithBase(fallback, path);
            if (fallbackUrl != null) {
                Log.d("ApiClient", "Trying fallback URL: " + fallbackUrl);
                Request request = buildRequest(fallbackUrl).get().build();
                OkHttpClient client = selectClient(fallbackUrl);
                client.newCall(request).enqueue(new Callback() {
                    @Override
                    public void onFailure(Call call, IOException e) {
                        Log.e("ApiClient", "Fallback also failed", e);
                        // Fallback też nie działa - próbujemy automatyczne wykrywanie
                        tryAutoDiscovery(path, type, callback, originalError);
                    }

                    @Override
                    public void onResponse(Call call, Response response) throws IOException {
                        if (!response.isSuccessful()) {
                            tryAutoDiscovery(path, type, callback, originalError);
                            return;
                        }
                        String body = response.body() != null ? response.body().string() : "";
                        ApiResponse<T> apiResponse = GSON.fromJson(body, type);
                        if (apiResponse == null || !apiResponse.isSuccess() || apiResponse.getData() == null) {
                            String message = apiResponse != null ? apiResponse.getMessage() : "Brak danych z API";
                            callback.onError(message != null ? message : "Brak danych z API");
                            return;
                        }
                        // Sukces - aktualizujemy base URL
                        ApiConfig.setBaseUrl(context, fallback);
                        Log.d("ApiClient", "Fallback succeeded, updated base URL to: " + fallback);
                        callback.onSuccess(apiResponse.getData());
                    }
                });
                return;
            }
        }
        
        // Brak fallback - próbujemy automatyczne wykrywanie
        tryAutoDiscovery(path, type, callback, originalError);
    }

    private <T> void tryAutoDiscovery(String path, Type type, ApiCallback<T> callback, IOException originalError) {
        Log.d("ApiClient", "Starting auto-discovery...");
        
        // Pobieramy lokalny IP telefonu
        String phoneIp = getLocalIpAddress();
        if (phoneIp == null) {
            callback.onError(originalError.getMessage());
            return;
        }
        
        // Wyciągamy segment sieci (np. 192.168.1)
        String networkPrefix = phoneIp.substring(0, phoneIp.lastIndexOf('.'));
        Log.d("ApiClient", "Network prefix: " + networkPrefix);
        
        // Próbujemy najczęstsze IP w sieci lokalnej
        List<String> candidateIps = new ArrayList<>();
        candidateIps.add(networkPrefix + ".1");   // Router/Gateway
        candidateIps.add(networkPrefix + ".100"); // Często używane przez komputery
        candidateIps.add(networkPrefix + ".101");
        candidateIps.add(networkPrefix + ".102");
        candidateIps.add(networkPrefix + ".103");
        candidateIps.add(networkPrefix + ".104");
        candidateIps.add(networkPrefix + ".105");
        candidateIps.add(networkPrefix + ".106");
        
        tryNextCandidate(candidateIps, 0, path, type, callback, originalError);
    }

    private <T> void tryNextCandidate(List<String> candidates, int index, String path, Type type, 
                                       ApiCallback<T> callback, IOException originalError) {
        if (index >= candidates.size()) {
            // Wszystkie próby wyczerpane
            callback.onError("Nie znaleziono działającego serwera API. " + originalError.getMessage());
            return;
        }
        
        String candidateIp = candidates.get(index);
        String candidateUrl = "http://" + candidateIp + ":50875";
        String fullUrl = buildUrlWithBase(candidateUrl, path);
        
        if (fullUrl == null) {
            tryNextCandidate(candidates, index + 1, path, type, callback, originalError);
            return;
        }
        
        Log.d("ApiClient", "Trying candidate: " + candidateUrl);
        
        Request request = buildRequest(fullUrl).get().build();
        OkHttpClient client = selectClient(fullUrl);
        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                // Ta próba nie powiodła się - próbujemy następną
                tryNextCandidate(candidates, index + 1, path, type, callback, originalError);
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                if (!response.isSuccessful()) {
                    // Ta próba nie powiodła się - próbujemy następną
                    tryNextCandidate(candidates, index + 1, path, type, callback, originalError);
                    return;
                }
                
                String body = response.body() != null ? response.body().string() : "";
                ApiResponse<T> apiResponse = GSON.fromJson(body, type);
                if (apiResponse == null || !apiResponse.isSuccess() || apiResponse.getData() == null) {
                    // Ta próba nie powiodła się - próbujemy następną
                    tryNextCandidate(candidates, index + 1, path, type, callback, originalError);
                    return;
                }
                
                // SUKCES! Znaleźliśmy działający serwer
                ApiConfig.setBaseUrl(context, candidateUrl);
                ApiConfig.setFallbackBaseUrl(context, candidateUrl);
                Log.d("ApiClient", "Auto-discovery succeeded! New API URL: " + candidateUrl);
                callback.onSuccess(apiResponse.getData());
            }
        });
    }

    private String getLocalIpAddress() {
        String ip = NetworkUtils.getIPAddress(true);
        if (ip == null || ip.isEmpty()) {
            Log.e("ApiClient", "Failed to get local IP");
            return null;
        }
        return ip;
    }

    private void retrySendWithFallback(String path, String method, RequestBody body, ApiCallback<Void> callback, IOException originalError) {
        String fallback = ApiConfig.getFallbackBaseUrl(context);
        
        if (fallback == null || fallback.isEmpty()) {
            callback.onError(originalError.getMessage());
            return;
        }
        
        String fallbackUrl = buildUrlWithBase(fallback, path);
        if (fallbackUrl == null) {
            callback.onError(originalError.getMessage());
            return;
        }
        Request fallbackRequest = buildRequest(fallbackUrl).method(method, body).build();
        OkHttpClient client = selectClient(fallbackUrl);
        client.newCall(fallbackRequest).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                callback.onError(originalError.getMessage());
            }

            @Override
            public void onResponse(Call call, Response response) {
                if (!response.isSuccessful()) {
                    callback.onError("HTTP " + response.code());
                    return;
                }
                ApiConfig.setBaseUrl(context, fallback);
                callback.onSuccess(null);
            }
        });
    }

    private boolean tryCleartextFallback(String url, String path, String method, RequestBody body, ApiCallback<Void> callback, IOException originalError) {
        if (!isTlsTrustError(originalError)) {
            return false;
        }
        String httpUrl = toHttpUrl(url);
        if (httpUrl == null || httpUrl.equals(url)) {
            return false;
        }
        Log.w("ApiClient", "TLS trust error, retrying over HTTP: " + httpUrl);
        Request request = buildRequest(httpUrl).method(method, body).build();
        OkHttpClient client = selectClient(httpUrl);
        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                String failedUrl = call.request().url().toString();
                if (tryUnsafeTlsFallback(failedUrl, path, method, body, callback, e)) {
                    return;
                }
                retrySendWithFallback(path, method, body, callback, originalError);
            }

            @Override
            public void onResponse(Call call, Response response) {
                if (!response.isSuccessful()) {
                    retrySendWithFallback(path, method, body, callback, originalError);
                    return;
                }
                String baseUrl = extractBaseUrl(httpUrl);
                if (baseUrl != null) {
                    ApiConfig.setBaseUrl(context, baseUrl);
                    ApiConfig.setFallbackBaseUrl(context, baseUrl);
                }
                callback.onSuccess(null);
            }
        });
        return true;
    }

    private <T> boolean tryCleartextFallback(String url, String path, Type type, ApiCallback<T> callback, IOException originalError) {
        if (!isTlsTrustError(originalError)) {
            return false;
        }
        String httpUrl = toHttpUrl(url);
        if (httpUrl == null || httpUrl.equals(url)) {
            return false;
        }
        Log.w("ApiClient", "TLS trust error, retrying over HTTP: " + httpUrl);
        Request request = buildRequest(httpUrl).get().build();
        OkHttpClient client = selectClient(httpUrl);
        client.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                String failedUrl = call.request().url().toString();
                if (tryUnsafeTlsFallback(failedUrl, path, type, callback, e)) {
                    return;
                }
                retryGetWithFallback(path, type, callback, originalError);
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                if (!response.isSuccessful()) {
                    retryGetWithFallback(path, type, callback, originalError);
                    return;
                }
                String body = response.body() != null ? response.body().string() : "";
                ApiResponse<T> apiResponse = GSON.fromJson(body, type);
                if (apiResponse == null || !apiResponse.isSuccess() || apiResponse.getData() == null) {
                    retryGetWithFallback(path, type, callback, originalError);
                    return;
                }
                String baseUrl = extractBaseUrl(httpUrl);
                if (baseUrl != null) {
                    ApiConfig.setBaseUrl(context, baseUrl);
                    ApiConfig.setFallbackBaseUrl(context, baseUrl);
                }
                callback.onSuccess(apiResponse.getData());
            }
        });
        return true;
    }

    private boolean isTlsTrustError(IOException error) {
        if (error instanceof SSLHandshakeException) {
            return true;
        }
        Throwable current = error;
        while (current != null) {
            if (current instanceof CertPathValidatorException || current instanceof CertificateException) {
                return true;
            }
            current = current.getCause();
        }
        String message = error.getMessage();
        return message != null && message.contains("Trust anchor");
    }

    private OkHttpClient selectClient(String url) {
        if (url != null && url.startsWith("https://") && isLocalNetworkUrl(url)) {
            return UNSAFE_TLS_CLIENT;
        }
        return CLIENT;
    }

    private boolean isLocalNetworkUrl(String url) {
        HttpUrl parsed = HttpUrl.parse(url);
        if (parsed == null) {
            return false;
        }
        String host = parsed.host();
        if ("localhost".equals(host)) {
            return true;
        }
        String[] parts = host.split("\\.");
        if (parts.length != 4) {
            return false;
        }
        try {
            int first = Integer.parseInt(parts[0]);
            int second = Integer.parseInt(parts[1]);
            if (first == 10) {
                return true;
            }
            if (first == 127) {
                return true;
            }
            if (first == 192 && second == 168) {
                return true;
            }
            return first == 172 && second >= 16 && second <= 31;
        } catch (NumberFormatException ex) {
            return false;
        }
    }

    private boolean tryUnsafeTlsFallback(String url, String path, String method, RequestBody body, ApiCallback<Void> callback, IOException originalError) {
        if (!isTlsTrustError(originalError) || url == null || !url.startsWith("https://")) {
            return false;
        }
        if (!isLocalNetworkUrl(url)) {
            return false;
        }
        Log.w("ApiClient", "TLS trust error, retrying with unsafe TLS client: " + url);
        Request request = buildRequest(url).method(method, body).build();
        UNSAFE_TLS_CLIENT.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                retrySendWithFallback(path, method, body, callback, originalError);
            }

            @Override
            public void onResponse(Call call, Response response) {
                if (!response.isSuccessful()) {
                    retrySendWithFallback(path, method, body, callback, originalError);
                    return;
                }
                String baseUrl = extractBaseUrl(url);
                if (baseUrl != null) {
                    ApiConfig.setBaseUrl(context, baseUrl);
                    ApiConfig.setFallbackBaseUrl(context, baseUrl);
                }
                callback.onSuccess(null);
            }
        });
        return true;
    }

    private <T> boolean tryUnsafeTlsFallback(String url, String path, Type type, ApiCallback<T> callback, IOException originalError) {
        if (!isTlsTrustError(originalError) || url == null || !url.startsWith("https://")) {
            return false;
        }
        if (!isLocalNetworkUrl(url)) {
            return false;
        }
        Log.w("ApiClient", "TLS trust error, retrying with unsafe TLS client: " + url);
        Request request = buildRequest(url).get().build();
        UNSAFE_TLS_CLIENT.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                retryGetWithFallback(path, type, callback, originalError);
            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                if (!response.isSuccessful()) {
                    retryGetWithFallback(path, type, callback, originalError);
                    return;
                }
                String body = response.body() != null ? response.body().string() : "";
                ApiResponse<T> apiResponse = GSON.fromJson(body, type);
                if (apiResponse == null || !apiResponse.isSuccess() || apiResponse.getData() == null) {
                    retryGetWithFallback(path, type, callback, originalError);
                    return;
                }
                String baseUrl = extractBaseUrl(url);
                if (baseUrl != null) {
                    ApiConfig.setBaseUrl(context, baseUrl);
                    ApiConfig.setFallbackBaseUrl(context, baseUrl);
                }
                callback.onSuccess(apiResponse.getData());
            }
        });
        return true;
    }

    private static OkHttpClient buildUnsafeTlsClient() {
        try {
            TrustManager[] trustAllCerts = new TrustManager[]{
                new X509TrustManager() {
                    @Override
                    public void checkClientTrusted(X509Certificate[] chain, String authType) {
                    }

                    @Override
                    public void checkServerTrusted(X509Certificate[] chain, String authType) {
                    }

                    @Override
                    public X509Certificate[] getAcceptedIssuers() {
                        return new X509Certificate[0];
                    }
                }
            };
            SSLContext sslContext = SSLContext.getInstance("TLS");
            sslContext.init(null, trustAllCerts, new java.security.SecureRandom());
            HostnameVerifier hostnameVerifier = new HostnameVerifier() {
                @Override
                public boolean verify(String hostname, SSLSession session) {
                    return true;
                }
            };
            return CLIENT.newBuilder()
                .sslSocketFactory(sslContext.getSocketFactory(), (X509TrustManager) trustAllCerts[0])
                .hostnameVerifier(hostnameVerifier)
                .build();
        } catch (Exception e) {
            return CLIENT;
        }
    }

    private String toHttpUrl(String url) {
        if (url == null) {
            return null;
        }
        if (url.startsWith("https://")) {
            return "http://" + url.substring("https://".length());
        }
        return url;
    }

    private String extractBaseUrl(String url) {
        HttpUrl parsed = HttpUrl.parse(url);
        if (parsed == null) {
            return null;
        }
        int port = parsed.port();
        boolean isDefaultPort = (parsed.scheme().equals("http") && port == 80)
            || (parsed.scheme().equals("https") && port == 443);
        return parsed.scheme() + "://" + parsed.host() + (isDefaultPort ? "" : ":" + port);
    }

    public void fetchReturns(String query, ApiCallback<PaginatedResponse<ReturnListItemDto>> callback) {
        Type type = new TypeToken<ApiResponse<PaginatedResponse<ReturnListItemDto>>>() {
        }.getType();
        get("api/returns" + query, type, callback);
    }

    public void fetchAssignedReturns(String query, ApiCallback<PaginatedResponse<ReturnListItemDto>> callback) {
        Type type = new TypeToken<ApiResponse<PaginatedResponse<ReturnListItemDto>>>() {
        }.getType();
        get("api/returns/assigned" + query, type, callback);
    }

    public void fetchReturnDetails(int id, ApiCallback<ReturnDetailsDto> callback) {
        Type type = new TypeToken<ApiResponse<ReturnDetailsDto>>() {
        }.getType();
        get("api/returns/" + id, type, callback);
    }

    public void submitWarehouseUpdate(int id, ReturnWarehouseUpdateRequest payload, ApiCallback<Void> callback) {
        sendJson("api/returns/" + id + "/warehouse", payload, "PATCH", callback);
    }

    public void submitDecision(int id, ReturnDecisionRequest payload, ApiCallback<Void> callback) {
        sendJson("api/returns/" + id + "/decision", payload, "PATCH", callback);
    }

    public void fetchSummary(ApiCallback<ReturnSummaryResponse> callback) {
        Type type = new TypeToken<ApiResponse<ReturnSummaryResponse>>() {
        }.getType();
        get("api/returns/summary", type, callback);
    }

    public void fetchMessages(ApiCallback<List<MessageDto>> callback) {
        Type type = new TypeToken<ApiResponse<List<MessageDto>>>() {
        }.getType();
        get("api/messages", type, callback);
    }
}
