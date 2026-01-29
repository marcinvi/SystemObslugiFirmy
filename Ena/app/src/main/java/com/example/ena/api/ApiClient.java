package com.example.ena.api;

import android.content.Context;
import android.database.Cursor;
import android.net.Uri;
import android.provider.OpenableColumns;
import com.example.ena.NetworkUtils;
import com.example.ena.PairingManager;
import com.example.ena.UserSession;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.reflect.TypeToken;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.lang.reflect.Type;
import java.net.SocketTimeoutException;
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
import okhttp3.MultipartBody;
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
            .connectTimeout(60, TimeUnit.SECONDS)
            .readTimeout(60, TimeUnit.SECONDS)
            .writeTimeout(60, TimeUnit.SECONDS)
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
                    // ZMIANA: Zwiększono timeout z 3s na 60s, aby synchronizacja się nie zrywała
                    .connectTimeout(60, TimeUnit.SECONDS)
                    .readTimeout(60, TimeUnit.SECONDS)
                    .writeTimeout(60, TimeUnit.SECONDS)
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
    public void lookupReturn(String code, ApiCallback<ReturnDetailsDto> callback) {
        fetchReturnByCode(code, callback); // Po prostu przekieruj do istniejącej metody
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
        String token = UserSession.getToken(context);
        if (token != null && !token.isEmpty()) {
            builder.addHeader("Authorization", "Bearer " + token);
        }
        String user = UserSession.getLogin(context);
        if (user == null || user.isEmpty()) {
            user = PairingManager.getPairedUser(context);
        }
        if (user != null && !user.isEmpty()) {
            builder.addHeader("X-User", user);
        }
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

    private <T> void sendJsonWithResponse(String path, Object payload, String method, Type type, ApiCallback<T> callback) {
        String url = buildUrl(path);
        if (url == null) { callback.onError("Brak adresu API."); return; }
        RequestBody body = RequestBody.create(GSON.toJson(payload), JSON);
        executeSendWithResponseWithFallback(url, path, method, body, type, callback);
    }

    private <T> void sendMultipartWithResponse(String path, RequestBody body, Type type, ApiCallback<T> callback) {
        String url = buildUrl(path);
        if (url == null) { callback.onError("Brak adresu API."); return; }
        executeSendWithResponseWithFallback(url, path, "POST", body, type, callback);
    }

    private <T> void executeGetWithFallback(String url, String path, Type type, ApiCallback<T> callback) {
        Request request = buildRequest(url).get().build();
        selectClient(url).newCall(request).enqueue(new Callback() {
            @Override public void onFailure(Call call, IOException e) { retryGetWithFallback(path, type, callback, e); }
            @Override public void onResponse(Call call, Response response) throws IOException {
                try (Response res = response) {
                    if (res.isSuccessful()) {
                        handleResponse(res, type, callback);
                    } else {
                        callback.onError("Błąd serwera (HTTP " + res.code() + ")");
                    }
                }
            }
        });
    }

    private void executeSendWithFallback(String url, String path, String method, RequestBody body, ApiCallback<Void> callback) {
        Request request = buildRequest(url).method(method, body).build();
        selectClient(url).newCall(request).enqueue(new Callback() {
            @Override public void onFailure(Call call, IOException e) { retrySendWithFallback(path, method, body, callback, e); }
            @Override public void onResponse(Call call, Response response) {
                try (Response res = response) {
                    if (res.isSuccessful()) {
                        callback.onSuccess(null);
                    } else {
                        retrySendWithFallback(path, method, body, callback, new IOException("HTTP " + res.code()));
                    }
                }
            }
        });
    }

    private <T> void executeSendWithResponseWithFallback(String url, String path, String method, RequestBody body, Type type, ApiCallback<T> callback) {
        Request request = buildRequest(url).method(method, body).build();
        selectClient(url).newCall(request).enqueue(new Callback() {
            @Override public void onFailure(Call call, IOException e) { retrySendWithResponseWithFallback(path, method, body, type, callback, e); }
            @Override public void onResponse(Call call, Response response) throws IOException {
                try (Response res = response) {
                    if (res.isSuccessful()) {
                        handleResponse(res, type, callback);
                    } else {
                        retrySendWithResponseWithFallback(path, method, body, type, callback, new IOException("HTTP " + res.code()));
                    }
                }
            }
        });
    }

    private <T> void handleResponse(Response response, Type type, ApiCallback<T> callback) throws IOException {
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
                    try (Response res = response) {
                        if (res.isSuccessful()) {
                            ApiConfig.setBaseUrl(context, fallback);
                            handleResponse(res, type, callback);
                        } else {
                            callback.onError("Błąd serwera (HTTP " + res.code() + ")");
                        }
                    }
                }
            });
        } else tryAutoDiscovery(path, type, callback, error);
    }

    private void retrySendWithFallback(String path, String method, RequestBody body, ApiCallback<Void> callback, IOException error) {
        String fallback = ApiConfig.getFallbackBaseUrl(context);
        if (fallback != null && !fallback.isEmpty()) {
            String url = fallback + (path.startsWith("/") ? "" : "/") + path;
            selectClient(url).newCall(buildRequest(url).method(method, body).build()).enqueue(new Callback() {
                @Override public void onFailure(Call call, IOException e) { callback.onError(formatNetworkError(error)); }
                @Override public void onResponse(Call call, Response response) {
                    try (Response res = response) {
                        if (res.isSuccessful()) {
                            ApiConfig.setBaseUrl(context, fallback);
                            callback.onSuccess(null);
                        } else {
                            callback.onError(formatNetworkError(error));
                        }
                    }
                }
            });
        } else callback.onError(formatNetworkError(error));
    }

    private <T> void retrySendWithResponseWithFallback(String path, String method, RequestBody body, Type type, ApiCallback<T> callback, IOException error) {
        String fallback = ApiConfig.getFallbackBaseUrl(context);
        if (fallback != null && !fallback.isEmpty()) {
            String url = fallback + (path.startsWith("/") ? "" : "/") + path;
            selectClient(url).newCall(buildRequest(url).method(method, body).build()).enqueue(new Callback() {
                @Override public void onFailure(Call call, IOException e) { callback.onError(formatNetworkError(error)); }
                @Override public void onResponse(Call call, Response response) throws IOException {
                    try (Response res = response) {
                        if (res.isSuccessful()) {
                            ApiConfig.setBaseUrl(context, fallback);
                            handleResponse(res, type, callback);
                        } else {
                            callback.onError(formatNetworkError(error));
                        }
                    }
                }
            });
        } else callback.onError(formatNetworkError(error));
    }

    private <T> void tryAutoDiscovery(String path, Type type, ApiCallback<T> callback, IOException error) {
        String ip = NetworkUtils.getIPAddress(true);
        if (ip == null || ip.isEmpty()) { callback.onError(formatNetworkError(error)); return; }
        int lastDot = ip.lastIndexOf('.');
        if (lastDot <= 0) { callback.onError(formatNetworkError(error)); return; }
        String prefix = ip.substring(0, lastDot);
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
                try (Response res = response) {
                    if (res.isSuccessful()) {
                        ApiConfig.setBaseUrl(context, base);
                        ApiConfig.setFallbackBaseUrl(context, base);
                        handleResponse(res, type, cb);
                    } else {
                        tryNextCandidate(ips, idx + 1, path, type, cb, err);
                    }
                }
            }
        });
    }

    private String formatNetworkError(IOException error) {
        if (error == null) {
            return "Błąd połączenia.";
        }
        if (error instanceof SocketTimeoutException) {
            return "Przekroczono limit czasu połączenia.";
        }
        String message = error.getMessage();
        if (message == null || message.trim().isEmpty()) {
            return "Błąd połączenia.";
        }
        if (message.startsWith("HTTP ")) {
            return "Błąd serwera (" + message + ")";
        }
        return message;
    }

    // --- METODY WYMAGANE PRZEZ TWOJE ACTIVITY ---

    // NOWA METODA: Pobieranie listy użytkowników
    public void fetchUsers(ApiCallback<List<String>> callback) {
        Type type = new TypeToken<ApiResponse<List<String>>>(){}.getType();
        get("api/auth/users", type, callback);
    }

    public void fetchReturns(String query, ApiCallback<PaginatedResponse<ReturnListItemDto>> callback) {
        Type type = new TypeToken<ApiResponse<PaginatedResponse<ReturnListItemDto>>>(){}.getType();
        get("api/returns" + query, type, callback);
    }

    public void login(LoginRequest request, ApiCallback<LoginResponse> callback) {
        Type type = new TypeToken<ApiResponse<LoginResponse>>(){}.getType();
        sendJsonWithResponse("api/auth/login", request, "POST", type, callback);
    }

    public void fetchAssignedModules(ApiCallback<List<String>> callback) {
        Type type = new TypeToken<ApiResponse<List<String>>>(){}.getType();
        get("api/modules/assigned", type, callback);
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

    public void fetchReturnActions(int id, ApiCallback<List<ReturnActionDto>> callback) {
        Type type = new TypeToken<ApiResponse<List<ReturnActionDto>>>(){}.getType();
        get("api/returns/" + id + "/actions", type, callback);
    }

    public void fetchReturnPhotos(int id, ApiCallback<List<ReturnPhotoDto>> callback) {
        Type type = new TypeToken<ApiResponse<List<ReturnPhotoDto>>>(){}.getType();
        get("api/returns/" + id + "/photos", type, callback);
    }

    public void uploadReturnPhoto(int id, Uri uri, ApiCallback<ReturnPhotoDto> callback) {
        if (uri == null) {
            callback.onError("Brak zdjęcia.");
            return;
        }
        String fileName = resolveFileName(uri);
        String mime = context.getContentResolver().getType(uri);
        MediaType mediaType = MediaType.parse(mime != null ? mime : "image/jpeg");
        byte[] data;
        try {
            data = readBytes(uri);
        } catch (IOException e) {
            callback.onError("Nie udało się odczytać zdjęcia.");
            return;
        }

        RequestBody fileBody = RequestBody.create(data, mediaType);
        MultipartBody requestBody = new MultipartBody.Builder()
                .setType(MultipartBody.FORM)
                .addFormDataPart("file", fileName, fileBody)
                .build();

        Type type = new TypeToken<ApiResponse<ReturnPhotoDto>>(){}.getType();
        sendMultipartWithResponse("api/returns/" + id + "/photos", requestBody, type, callback);
    }

    public void addReturnAction(int id, ReturnActionCreateRequest payload, ApiCallback<Void> callback) {
        sendJson("api/returns/" + id + "/actions", payload, "POST", callback);
    }

    public void fetchManualReturnMeta(ApiCallback<ManualReturnMetaDto> callback) {
        Type type = new TypeToken<ApiResponse<ManualReturnMetaDto>>(){}.getType();
        get("api/returns/manual/meta", type, callback);
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

    public void completeReturn(int id, ApiCallback<Void> callback) {
        sendJson("api/returns/" + id + "/complete", new Object(), "POST", callback);
    }

    public void forwardToComplaints(int id, ForwardToComplaintRequest payload, ApiCallback<Void> callback) {
        sendJson("api/returns/" + id + "/forward-to-complaints", payload, "POST", callback);
    }

    public void forwardToWarehouse(int id, ReturnForwardToWarehouseRequest payload, ApiCallback<Void> callback) {
        sendJson("api/returns/" + id + "/forward-to-warehouse", payload, "POST", callback);
    }

    public void fetchRefundContext(int id, ApiCallback<ReturnRefundContextDto> callback) {
        Type type = new TypeToken<ApiResponse<ReturnRefundContextDto>>(){}.getType();
        get("api/returns/" + id + "/refund-context", type, callback);
    }

    public void rejectReturn(int id, RejectCustomerReturnRequest payload, ApiCallback<Void> callback) {
        sendJson("api/returns/" + id + "/reject", payload, "POST", callback);
    }

    public void refundPayment(int id, PaymentRefundRequest payload, ApiCallback<Void> callback) {
        sendJson("api/returns/" + id + "/refund", payload, "POST", callback);
    }

    public void fetchSummary(ApiCallback<ReturnSummaryResponse> callback) {
        Type type = new TypeToken<ApiResponse<ReturnSummaryResponse>>(){}.getType();
        get("api/returns/summary", type, callback);
    }

    public void syncReturns(ReturnSyncRequest payload, ApiCallback<ReturnSyncResponse> callback) {
        Type type = new TypeToken<ApiResponse<ReturnSyncResponse>>(){}.getType();
        sendJsonWithResponse("api/returns/sync", payload, "POST", type, callback);
    }

    public void fetchMessages(ApiCallback<List<MessageDto>> callback) {
        Type type = new TypeToken<ApiResponse<List<MessageDto>>>(){}.getType();
        get("api/messages", type, callback);
    }

    public void fetchNotifications(boolean onlyUnread, ApiCallback<List<NotificationDto>> callback) {
        Type type = new TypeToken<ApiResponse<List<NotificationDto>>>(){}.getType();
        String path = "api/notifications" + (onlyUnread ? "?onlyUnread=true" : "");
        get(path, type, callback);
    }

    public void markNotificationRead(int id, ApiCallback<Void> callback) {
        sendJson("api/notifications/" + id + "/mark-read", new Object(), "POST", callback);
    }

    public void fetchStatuses(String typeValue, ApiCallback<List<StatusDto>> callback) {
        Type type = new TypeToken<ApiResponse<List<StatusDto>>>(){}.getType();
        String encoded = typeValue == null ? "" : URLEncoder.encode(typeValue, StandardCharsets.UTF_8);
        get("api/returns/statuses?type=" + encoded, type, callback);
    }

    public void createManualReturn(ReturnManualCreateRequest payload, ApiCallback<Void> callback) {
        sendJson("api/returns/manual", payload, "POST", callback);
    }

    private String resolveFileName(Uri uri) {
        String name = null;
        Cursor cursor = context.getContentResolver().query(uri, null, null, null, null);
        if (cursor != null) {
            try {
                int nameIndex = cursor.getColumnIndex(OpenableColumns.DISPLAY_NAME);
                if (nameIndex >= 0 && cursor.moveToFirst()) {
                    name = cursor.getString(nameIndex);
                }
            } finally {
                cursor.close();
            }
        }
        return name != null && !name.trim().isEmpty() ? name : "zdjecie.jpg";
    }

    private byte[] readBytes(Uri uri) throws IOException {
        try (InputStream inputStream = context.getContentResolver().openInputStream(uri);
             ByteArrayOutputStream buffer = new ByteArrayOutputStream()) {
            if (inputStream == null) {
                throw new IOException("Brak strumienia wejściowego");
            }
            byte[] data = new byte[8192];
            int nRead;
            while ((nRead = inputStream.read(data, 0, data.length)) != -1) {
                buffer.write(data, 0, nRead);
            }
            buffer.flush();
            return buffer.toByteArray();
        }
    }
}
