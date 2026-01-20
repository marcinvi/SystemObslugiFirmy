package com.example.ena.api;

import android.content.Context;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.reflect.TypeToken;
import com.example.ena.PairingManager;
import java.io.IOException;
import java.lang.reflect.Type;
import java.time.OffsetDateTime;
import java.util.List;
import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class ApiClient {
    private static final MediaType JSON = MediaType.parse("application/json; charset=utf-8");
    private static final OkHttpClient CLIENT = new OkHttpClient();
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
        return buildUrlWithBase(base, path);
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
        CLIENT.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
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
        CLIENT.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
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
        String fallback = ApiConfig.getFallbackBaseUrl(context);
        String fallbackUrl = buildUrlWithBase(fallback, path);
        if (fallbackUrl == null) {
            callback.onError(originalError.getMessage());
            return;
        }
        Request request = buildRequest(fallbackUrl).get().build();
        CLIENT.newCall(request).enqueue(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {
                callback.onError(originalError.getMessage());
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
                T data = apiResponse.getData();
                ApiConfig.setBaseUrl(context, fallback);
                callback.onSuccess(data);
            }
        });
    }

    private void retrySendWithFallback(String path, String method, RequestBody body, ApiCallback<Void> callback, IOException originalError) {
        String fallback = ApiConfig.getFallbackBaseUrl(context);
        String fallbackUrl = buildUrlWithBase(fallback, path);
        if (fallbackUrl == null) {
            callback.onError(originalError.getMessage());
            return;
        }
        Request fallbackRequest = buildRequest(fallbackUrl).method(method, body).build();
        CLIENT.newCall(fallbackRequest).enqueue(new Callback() {
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
