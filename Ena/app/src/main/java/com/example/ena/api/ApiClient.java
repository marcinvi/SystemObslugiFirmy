package com.example.ena.api;

import android.content.Context;
import com.example.ena.NetworkUtils;
import com.example.ena.PairingManager;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.reflect.TypeToken;
import java.io.IOException;
import java.lang.reflect.Type;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.security.SecureRandom;
import java.time.OffsetDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.TimeUnit;
import javax.net.ssl.SSLContext;
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

public class ApiClient {
    private static final MediaType JSON = MediaType.parse("application/json; charset=utf-8");
    private static final Gson GSON = new GsonBuilder()
            .registerTypeAdapter(OffsetDateTime.class, new OffsetDateTimeAdapter())
            .create();

    // Standardowy klient
    private static final OkHttpClient CLIENT = new OkHttpClient.Builder()
            .connectTimeout(5, TimeUnit.SECONDS)
            .readTimeout(10, TimeUnit.SECONDS)
            .writeTimeout(10, TimeUnit.SECONDS)
            .build();

    // Klient dla sieci lokalnej (ignoruje błędy SSL/Przekierowania)
    private static final OkHttpClient UNSAFE_TLS_CLIENT = buildUnsafeTlsClient();

    public interface ApiCallback<T> {
        void onSuccess(T data);
        void onError(String message);
    }

    private final Context context;

    public ApiClient(Context context) {
        this.context = context.getApplicationContext();
    }

    private static OkHttpClient buildUnsafeTlsClient() {
        try {
            final TrustManager[] trustAllCerts = new TrustManager[]{
                    new X509TrustManager() {
                        @Override public void checkClientTrusted(java.security.cert.X509Certificate[] chain, String authType) {}
                        @Override public void checkServerTrusted(java.security.cert.X509Certificate[] chain, String authType) {}
                        @Override public java.security.cert.X509Certificate[] getAcceptedIssuers() { return new java.security.cert.X509Certificate[]{}; }
                    }
            };
            final SSLContext sslContext = SSLContext.getInstance("TLS");
            sslContext.init(null, trustAllCerts, new SecureRandom());
            return new OkHttpClient.Builder()
                    .sslSocketFactory(sslContext.getSocketFactory(), (X509TrustManager) trustAllCerts[0])
                    .hostnameVerifier((hostname, session) -> true)
                    .connectTimeout(3, TimeUnit.SECONDS)
                    .followRedirects(true)
                    .followSslRedirects(true)
                    .build();
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    private OkHttpClient selectClient(String url) {
        HttpUrl parsed = HttpUrl.parse(url);
        if (parsed != null && isLocalNetworkHost(parsed.host())) return UNSAFE_TLS_CLIENT;
        return CLIENT;
    }

    private boolean isLocalNetworkHost(String host) {
        if (host == null) return false;
        if (host.equals("localhost") || host.equals("127.0.0.1")) return true;
        String[] parts = host.split("\\.");
        if (parts.length != 4) return false;
        try {
            int f = Integer.parseInt(parts[0]);
            int s = Integer.parseInt(parts[1]);
            return (f == 10) || (f == 192 && s == 168) || (f == 172 && s >= 16 && s <= 31);
        } catch (Exception e) {
            return false;
        }
    }

    // --- FUNDAMENTALNE METODY WYKONAWCZE ---

    private String buildUrl(String path) {
        String base = ApiConfig.getBaseUrl(context);
        if (base == null || base.isEmpty()) return null;
        if (base.endsWith("/")) base = base.substring(0, base.length() - 1);
        if (!path.startsWith("/")) path = "/" + path;
        return base + path;
    }

    private Request.Builder buildRequest(String url) {
        Request.Builder builder = new Request.Builder().url(url);
        String user = PairingManager.getPairedUser(context);
        if (user != null && !user.isEmpty()) builder.addHeader("X-User", user);
        return builder;
    }

    private <T> void get(String path, Type type, ApiCallback<T> callback) {
        String url = buildUrl(path);
        if (url == null) { callback.onError("Brak adresu API."); return; }
        executeGetWithFallback(url, path, type, callback);
    }

    private void sendJson(String path, Object payload, String method, ApiCallback<Void> callback) {
        String url = buildUrl(path);
        if (url == null) { callback.onError("Brak adresu API."); return; }
        RequestBody body = RequestBody.create(GSON.toJson(payload), JSON);
        executeSendWithFallback(url, path, method, body, callback);
    }

    private <T> void executeGetWithFallback(String url, String path, Type type, ApiCallback<T> callback) {
        Request request = buildRequest(url).get().build();
        selectClient(url).newCall(request).enqueue(new Callback() {
            @Override public void onFailure(Call call, IOException e) { retryGetWithFallback(path, type, callback, e); }
            @Override public void onResponse(Call call, Response response) throws IOException {
                handleResponse(response, type, callback, () -> retryGetWithFallback(path, type, callback, new IOException("HTTP " + response.code())));
            }
        });
    }

    private void executeSendWithFallback(String url, String path, String method, RequestBody body, ApiCallback<Void> callback) {
        Request request = buildRequest(url).method(method, body).build();
        selectClient(url).newCall(request).enqueue(new Callback() {
            @Override public void onFailure(Call call, IOException e) { retrySendWithFallback(path, method, body, callback, e); }
            @Override public void onResponse(Call call, Response response) {
                if (response.isSuccessful()) callback.onSuccess(null);
                else retrySendWithFallback(path, method, body, callback, new IOException("HTTP " + response.code()));
            }
        });
    }

    private <T> void handleResponse(Response response, Type type, ApiCallback<T> callback, Runnable onFail) throws IOException {
        if (!response.isSuccessful()) { onFail.run(); return; }
        String body = response.body() != null ? response.body().string() : "";
        ApiResponse<T> apiRes = GSON.fromJson(body, type);
        if (apiRes == null || !apiRes.isSuccess()) {
            callback.onError(apiRes != null ? apiRes.getMessage() : "Błąd serwera");
        } else {
            callback.onSuccess(apiRes.getData());
        }
    }

    // --- RETRY & DISCOVERY ---

    private <T> void retryGetWithFallback(String path, Type type, ApiCallback<T> callback, IOException error) {
        String fallback = ApiConfig.getFallbackBaseUrl(context);
        if (fallback != null && !fallback.isEmpty()) {
            String url = fallback + (path.startsWith("/") ? "" : "/") + path;
            selectClient(url).newCall(buildRequest(url).get().build()).enqueue(new Callback() {
                @Override public void onFailure(Call call, IOException e) { tryAutoDiscovery(path, type, callback, error); }
                @Override public void onResponse(Call call, Response response) throws IOException {
                    if (response.isSuccessful()) { ApiConfig.setBaseUrl(context, fallback); handleResponse(response, type, callback, null); }
                    else tryAutoDiscovery(path, type, callback, error);
                }
            });
        } else tryAutoDiscovery(path, type, callback, error);
    }

    private void retrySendWithFallback(String path, String method, RequestBody body, ApiCallback<Void> callback, IOException error) {
        String fallback = ApiConfig.getFallbackBaseUrl(context);
        if (fallback != null && !fallback.isEmpty()) {
            String url = fallback + (path.startsWith("/") ? "" : "/") + path;
            selectClient(url).newCall(buildRequest(url).method(method, body).build()).enqueue(new Callback() {
                @Override public void onFailure(Call call, IOException e) { callback.onError(error.getMessage()); }
                @Override public void onResponse(Call call, Response response) {
                    if (response.isSuccessful()) { ApiConfig.setBaseUrl(context, fallback); callback.onSuccess(null); }
                    else callback.onError(error.getMessage());
                }
            });
        } else callback.onError(error.getMessage());
    }

    private <T> void tryAutoDiscovery(String path, Type type, ApiCallback<T> callback, IOException error) {
        String ip = NetworkUtils.getIPAddress(true);
        if (ip == null) { callback.onError(error.getMessage()); return; }
        String prefix = ip.substring(0, ip.lastIndexOf('.'));
        List<String> ips = new ArrayList<>();
        ips.add(prefix + ".106"); ips.add(prefix + ".1");
        for (int i = 100; i <= 105; i++) ips.add(prefix + "." + i);
        tryNextCandidate(ips, 0, path, type, callback, error);
    }

    private <T> void tryNextCandidate(List<String> ips, int idx, String path, Type type, ApiCallback<T> cb, IOException err) {
        if (idx >= ips.size()) { cb.onError("Nie znaleziono serwera."); return; }
        String base = "http://" + ips.get(idx) + ":50875";
        String url = base + (path.startsWith("/") ? "" : "/") + path;
        selectClient(url).newCall(buildRequest(url).get().build()).enqueue(new Callback() {
            @Override public void onFailure(Call call, IOException e) { tryNextCandidate(ips, idx + 1, path, type, cb, err); }
            @Override public void onResponse(Call call, Response response) throws IOException {
                if (response.isSuccessful()) {
                    ApiConfig.setBaseUrl(context, base); ApiConfig.setFallbackBaseUrl(context, base);
                    handleResponse(response, type, cb, null);
                } else tryNextCandidate(ips, idx + 1, path, type, cb, err);
            }
        });
    }

    // --- METODY WYMAGANE PRZEZ TWOJE ACTIVITY (BŁĘDY Z OBRAZKA) ---

    public void fetchReturns(String query, ApiCallback<PaginatedResponse<ReturnListItemDto>> callback) {
        Type type = new TypeToken<ApiResponse<PaginatedResponse<ReturnListItemDto>>>(){}.getType();
        get("api/returns" + query, type, callback);
    }

    public void fetchAssignedReturns(String query, ApiCallback<PaginatedResponse<ReturnListItemDto>> callback) {
        Type type = new TypeToken<ApiResponse<PaginatedResponse<ReturnListItemDto>>>(){}.getType();
        get("api/returns/assigned" + query, type, callback);
    }

    public void fetchReturnDetails(int id, ApiCallback<ReturnDetailsDto> callback) {
        Type type = new TypeToken<ApiResponse<ReturnDetailsDto>>(){}.getType();
        get("api/returns/" + id, type, callback);
    }

    public void fetchReturnByCode(String code, ApiCallback<ReturnDetailsDto> callback) {
        Type type = new TypeToken<ApiResponse<ReturnDetailsDto>>(){}.getType();
        String encoded = code == null ? "" : URLEncoder.encode(code, StandardCharsets.UTF_8);
        get("api/returns/lookup?code=" + encoded, type, callback);
    }

    public void fetchReturnStatuses(String typeValue, ApiCallback<List<StatusDto>> callback) {
        Type type = new TypeToken<ApiResponse<List<StatusDto>>>(){}.getType();
        String encoded = typeValue == null ? "" : URLEncoder.encode(typeValue, StandardCharsets.UTF_8);
        get("api/returns/statuses?type=" + encoded, type, callback);
    }

    public void submitWarehouseUpdate(int id, ReturnWarehouseUpdateRequest payload, ApiCallback<Void> callback) {
        sendJson("api/returns/" + id + "/warehouse", payload, "PATCH", callback);
    }

    public void forwardToSales(int id, ReturnForwardToSalesRequest payload, ApiCallback<Void> callback) {
        sendJson("api/returns/" + id + "/forward-to-sales", payload, "POST", callback);
    }

    public void submitDecision(int id, ReturnDecisionRequest payload, ApiCallback<Void> callback) {
        sendJson("api/returns/" + id + "/decision", payload, "PATCH", callback);
    }

    public void fetchSummary(ApiCallback<ReturnSummaryResponse> callback) {
        Type type = new TypeToken<ApiResponse<ReturnSummaryResponse>>(){}.getType();
        get("api/returns/summary", type, callback);
    }

    public void fetchMessages(ApiCallback<List<MessageDto>> callback) {
        Type type = new TypeToken<ApiResponse<List<MessageDto>>>(){}.getType();
        get("api/messages", type, callback);
    }

    public void fetchStatuses(String typeValue, ApiCallback<List<StatusDto>> callback) {
        Type type = new TypeToken<ApiResponse<List<StatusDto>>>(){}.getType();
        String encoded = typeValue == null ? "" : URLEncoder.encode(typeValue, StandardCharsets.UTF_8);
        get("api/returns/statuses?type=" + encoded, type, callback);
    }
}
