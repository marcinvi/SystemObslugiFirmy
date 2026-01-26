package com.example.ena.ui;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Color;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.KeyEvent;
import android.view.View;
import android.view.inputmethod.EditorInfo;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.activity.result.ActivityResultLauncher;
import androidx.annotation.NonNull;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.PaginatedResponse;
import com.example.ena.api.ReturnListItemDto;
import com.example.ena.api.ReturnSyncRequest;
import com.example.ena.api.ReturnSyncResponse;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.journeyapps.barcodescanner.ScanContract;
import com.journeyapps.barcodescanner.ScanIntentResult;
import com.journeyapps.barcodescanner.ScanOptions;

import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class ReturnsListActivity extends AppCompatActivity {

    public static final String EXTRA_MODE = "mode";
    private static final int CAMERA_PERMISSION_REQUEST = 2001;
    private static final String KEY_PENDING_SCAN = "pending_scan_code";

    private RecyclerView recyclerView;
    private ReturnListAdapter adapter;
    private SwipeRefreshLayout swipeRefresh;
    private ProgressBar progressBar;
    private TextView txtHeader, txtCount, txtEmpty;
    private ImageButton btnBack, btnRefresh, btnSync, btnClearSearch;
    private EditText editSearch;
    private FloatingActionButton btnScanCode;

    // Kontener filtrów
    private LinearLayout filtersContainer;
    private Button btnFilter1, btnFilter2, btnFilter3;

    private ApiClient apiClient;
    private String currentMode = "warehouse";
    private final Handler searchHandler = new Handler(Looper.getMainLooper());

    // Zmienne stanu filtrów
    private String currentStatusWewnetrzny = null;
    private String currentStatusAllegro = null;
    private String pendingScannedCode = null;

    private final ActivityResultLauncher<ScanOptions> scanLauncher =
            registerForActivityResult(new ScanContract(), this::handleScanResult);

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_returns_list);

        if (getIntent().hasExtra(EXTRA_MODE)) {
            currentMode = getIntent().getStringExtra(EXTRA_MODE);
        }

        apiClient = new ApiClient(this);
        initViews();
        setupRecyclerView();
        setupListeners();

        if (savedInstanceState != null) {
            pendingScannedCode = savedInstanceState.getString(KEY_PENDING_SCAN);
            if (pendingScannedCode != null) showManualReturnPrompt(pendingScannedCode);
        }

        // --- KLUCZOWA NAPRAWA LOGIKI STARTOWEJ ---
        if ("sales".equals(currentMode)) {
            setupSalesMode();
        } else {
            setupWarehouseMode();
        }
    }

    @Override
    protected void onSaveInstanceState(@NonNull Bundle outState) {
        super.onSaveInstanceState(outState);
        if (pendingScannedCode != null) outState.putString(KEY_PENDING_SCAN, pendingScannedCode);
    }

    private void initViews() {
        recyclerView = findViewById(R.id.listReturns);
        swipeRefresh = findViewById(R.id.swipeRefresh);
        progressBar = findViewById(R.id.progress);
        txtHeader = findViewById(R.id.txtHeader);
        txtCount = findViewById(R.id.txtCount);
        txtEmpty = findViewById(R.id.txtEmpty);
        editSearch = findViewById(R.id.editSearch);
        btnBack = findViewById(R.id.btnBack);
        btnRefresh = findViewById(R.id.btnRefresh);
        btnSync = findViewById(R.id.btnSync);
        btnClearSearch = findViewById(R.id.btnClearSearch);
        btnScanCode = findViewById(R.id.btnScanCode);
        loadingOverlay = findViewById(R.id.loadingOverlay);
        txtLoadingMessage = findViewById(R.id.txtLoadingMessage);

        // Filtry
        filtersContainer = findViewById(R.id.filtersContainer); // Upewnij się, że ID w XML to filtersContainer
        if (filtersContainer == null) {
            // Fallback jeśli XML jest stary - pobieramy buttony bezpośrednio
            btnFilter1 = findViewById(R.id.btnFilterOczekujace);
            btnFilter2 = findViewById(R.id.btnFilterWDrodze);
            btnFilter3 = findViewById(R.id.btnFilterWszystkie);
        } else {
            btnFilter1 = findViewById(R.id.btnFilterOczekujace);
            btnFilter2 = findViewById(R.id.btnFilterWDrodze);
            btnFilter3 = findViewById(R.id.btnFilterWszystkie);
        }
    }

    private void setupWarehouseMode() {
        txtHeader.setText("Magazyn: Przyjęcia");
        btnSync.setVisibility(View.VISIBLE);
        btnScanCode.setVisibility(View.VISIBLE);
        editSearch.setHint("Skanuj kod paczki...");

        // Konfiguracja przycisków
        btnFilter1.setText("Do przyjęcia");
        btnFilter1.setOnClickListener(v -> applyFilter("Oczekuje na przyjęcie", null, v));

        btnFilter2.setText("W drodze");
        btnFilter2.setVisibility(View.VISIBLE);
        btnFilter2.setOnClickListener(v -> applyFilter(null, "IN_TRANSIT", v));

        btnFilter3.setText("Wszystkie");
        btnFilter3.setOnClickListener(v -> applyFilter(null, null, v));

        // Start: Do przyjęcia
        applyFilter("Oczekuje na przyjęcie", null, btnFilter1);
    }

    private void setupSalesMode() {
        txtHeader.setText("Moje Sprawy");
        btnSync.setVisibility(View.GONE);
        btnScanCode.setVisibility(View.GONE);
        editSearch.setHint("Szukaj klienta, produktu...");

        // Konfiguracja przycisków
        btnFilter1.setText("Do decyzji");
        btnFilter1.setOnClickListener(v -> applyFilter("Oczekuje na decyzję handlowca", null, v));

        btnFilter2.setText("Zakończone");
        btnFilter2.setVisibility(View.VISIBLE);
        btnFilter2.setOnClickListener(v -> applyFilter("Zakończony", null, v));

        btnFilter3.setText("Wszystkie");
        btnFilter3.setOnClickListener(v -> applyFilter(null, null, v));

        // Start: Do decyzji
        applyFilter("Oczekuje na decyzję handlowca", null, btnFilter1);
    }

    private void applyFilter(String statusWew, String statusAll, View activeButton) {
        this.currentStatusWewnetrzny = statusWew;
        this.currentStatusAllegro = statusAll;
        updateFilterVisuals((Button) activeButton);
        loadReturns();
    }

    private void updateFilterVisuals(Button active) {
        Button[] buttons = {btnFilter1, btnFilter2, btnFilter3};
        int activeColor = ContextCompat.getColor(this, R.color.primary_color);
        if (activeColor == 0) activeColor = Color.parseColor("#1976D2");

        for (Button btn : buttons) {
            if (btn == null) continue;
            if (btn == active) {
                btn.setBackgroundColor(activeColor);
                btn.setTextColor(Color.WHITE);
            } else {
                btn.setBackgroundColor(Color.parseColor("#E0E0E0"));
                btn.setTextColor(Color.parseColor("#333333"));
            }
        }
    }

    private void setupRecyclerView() {
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        adapter = new ReturnListAdapter(this::openReturnDetails);
        recyclerView.setAdapter(adapter);
    }

    private void setupListeners() {
        btnBack.setOnClickListener(v -> finish());
        swipeRefresh.setOnRefreshListener(this::loadReturns);
        btnRefresh.setOnClickListener(v -> { swipeRefresh.setRefreshing(true); loadReturns(); });

        // Obsługa szukania
        btnClearSearch.setOnClickListener(v -> editSearch.setText(""));
        editSearch.addTextChangedListener(new TextWatcher() {
            @Override public void beforeTextChanged(CharSequence s, int start, int count, int after) {}
            @Override public void onTextChanged(CharSequence s, int start, int before, int count) {
                btnClearSearch.setVisibility(s.length() > 0 ? View.VISIBLE : View.GONE);
                searchHandler.removeCallbacksAndMessages(null);
                searchHandler.postDelayed(() -> loadReturns(), 500);
            }
            @Override public void afterTextChanged(Editable s) {}
        });

        editSearch.setOnEditorActionListener((v, actionId, event) -> {
            if (actionId == EditorInfo.IME_ACTION_SEARCH || actionId == EditorInfo.IME_ACTION_DONE) {
                String code = editSearch.getText().toString().trim();
                if (!code.isEmpty()) {
                    if ("warehouse".equals(currentMode)) findReturnByCode(code); else loadReturns();
                }
                return true;
            }
            return false;
        });

        btnScanCode.setOnClickListener(v -> startCameraScan());

        btnSync.setOnClickListener(v -> {
            progressBar.setVisibility(View.VISIBLE);
            apiClient.syncReturns(new ReturnSyncRequest(null, null), new ApiClient.ApiCallback<ReturnSyncResponse>() {
                @Override public void onSuccess(ReturnSyncResponse data) {
                    runOnUiThread(() -> {
                        progressBar.setVisibility(View.GONE);
                        String msg = "Pobrano: " + (data != null ? data.getReturnsProcessed() : 0);
                        Toast.makeText(ReturnsListActivity.this, msg, Toast.LENGTH_SHORT).show();
                        loadReturns();
                    });
                }
                @Override public void onError(String msg) {
                    runOnUiThread(() -> { progressBar.setVisibility(View.GONE); Toast.makeText(ReturnsListActivity.this, "Błąd: " + msg, Toast.LENGTH_SHORT).show(); });
                }
            });
        });
    }

    private void loadReturns() {
        if (!swipeRefresh.isRefreshing()) progressBar.setVisibility(View.VISIBLE);
        txtEmpty.setVisibility(View.GONE);

        String query = buildQueryString();

        ApiClient.ApiCallback<PaginatedResponse<ReturnListItemDto>> callback = new ApiClient.ApiCallback<PaginatedResponse<ReturnListItemDto>>() {
            @Override
            public void onSuccess(PaginatedResponse<ReturnListItemDto> data) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    swipeRefresh.setRefreshing(false);
                    List<ReturnListItemDto> items = data != null ? data.getItems() : new ArrayList<>();
                    adapter.setItems(items);
                    txtCount.setText("Wyników: " + (data != null ? data.getTotalItems() : 0));
                    if (items.isEmpty()) txtEmpty.setVisibility(View.VISIBLE);
                });
            }
            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    swipeRefresh.setRefreshing(false);
                    Toast.makeText(ReturnsListActivity.this, "Błąd: " + message, Toast.LENGTH_SHORT).show();
                });
            }
        };

        // RÓŻNE ENDPOINTY DLA RÓŻNYCH RÓL
        if ("sales".equals(currentMode)) {
            apiClient.fetchAssignedReturns(query, callback);
        } else {
            apiClient.fetchReturns(query, callback);
        }
    }

    private String buildQueryString() {
        StringBuilder sb = new StringBuilder("?page=1&pageSize=100");
        String search = editSearch.getText().toString().trim();
        if (!search.isEmpty()) sb.append("&search=").append(encode(search));

        if (currentStatusWewnetrzny != null) sb.append("&statusWewnetrzny=").append(encode(currentStatusWewnetrzny));
        if (currentStatusAllegro != null) sb.append("&statusAllegro=").append(encode(currentStatusAllegro));

        sb.append("&sortBy=CreatedAt&sortDesc=true");
        return sb.toString();
    }

    private String encode(String value) {
        try { return URLEncoder.encode(value, StandardCharsets.UTF_8.toString()); } catch (Exception e) { return value; }
    }

    private void startCameraScan() {
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.CAMERA) != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.CAMERA}, CAMERA_PERMISSION_REQUEST);
            return;
        }
        ScanOptions options = new ScanOptions();
        options.setPrompt("Zeskanuj kod");
        options.setBeepEnabled(true);
        options.setOrientationLocked(true);
        options.setCaptureActivity(com.journeyapps.barcodescanner.CaptureActivity.class);
        scanLauncher.launch(options);
    }

    private void handleScanResult(ScanIntentResult result) {
        if (result.getContents() != null) {
            String code = result.getContents().trim();
            editSearch.setText(code);
            findReturnByCode(code);
        }
    }

    private void findReturnByCode(String code) {
        progressBar.setVisibility(View.VISIBLE);
        String cleanCode = extractCoreWaybill(code);
        String query = "?page=1&pageSize=1&search=" + encode(cleanCode);

        apiClient.fetchReturns(query, new ApiClient.ApiCallback<PaginatedResponse<ReturnListItemDto>>() {
            @Override
            public void onSuccess(PaginatedResponse<ReturnListItemDto> data) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    if (data != null && data.getItems() != null && !data.getItems().isEmpty()) {
                        openReturnDetails(data.getItems().get(0));
                    } else {
                        pendingScannedCode = cleanCode;
                        showManualReturnPrompt(cleanCode);
                    }
                });
            }
            @Override
            public void onError(String msg) {
                runOnUiThread(() -> { progressBar.setVisibility(View.GONE); Toast.makeText(ReturnsListActivity.this, "Błąd: " + msg, Toast.LENGTH_SHORT).show(); });
            }
        });
    }

    private void showManualReturnPrompt(String code) {
        pendingScannedCode = code;
        new AlertDialog.Builder(this)
                .setTitle("Brak zwrotu")
                .setMessage("Nie znaleziono: " + code + "\nCzy dodać ręcznie?")
                .setPositiveButton("Tak", (d, w) -> {
                    pendingScannedCode = null;
                    Intent intent = new Intent(this, ManualReturnActivity.class);
                    intent.putExtra(ManualReturnActivity.EXTRA_WAYBILL, code);
                    startActivity(intent);
                })
                .setNegativeButton("Nie", (d, w) -> pendingScannedCode = null)
                .show();
    }

    private String extractCoreWaybill(String input) {
        if (input == null) return "";
        Matcher dpdMatch = Pattern.compile("^%.{7}([a-zA-Z0-9]{14})").matcher(input);
        if (dpdMatch.find()) return dpdMatch.group(1);
        return input.replaceAll("[^a-zA-Z0-9]", "");
    }

    private void openReturnDetails(ReturnListItemDto item) {
        Intent intent;
        if ("sales".equals(currentMode)) {
            intent = new Intent(this, SalesReturnDetailActivity.class);
        } else {
            intent = new Intent(this, ReturnDetailActivity.class);
        }
        intent.putExtra("return_id", item.getId());
        startActivity(intent);
    }
}
