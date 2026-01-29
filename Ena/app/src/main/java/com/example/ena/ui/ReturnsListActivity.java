package com.example.ena.ui;

import android.Manifest;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.text.Editable;
import android.text.TextUtils;
import android.text.TextWatcher;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ProgressBar;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;
import androidx.annotation.Nullable;
import androidx.appcompat.app.AlertDialog;
import androidx.core.app.ActivityCompat;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.PaginatedResponse;
import com.example.ena.api.ReturnListItemDto;
import com.example.ena.api.ReturnSyncRequest;
import com.example.ena.api.ReturnSyncResponse;
import com.journeyapps.barcodescanner.ScanContract;
import com.journeyapps.barcodescanner.ScanIntentResult;
import com.journeyapps.barcodescanner.ScanOptions;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.util.List;
import java.util.function.IntConsumer;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import androidx.activity.result.ActivityResultLauncher;
import com.google.android.material.floatingactionbutton.FloatingActionButton;

public class ReturnsListActivity extends AppCompatActivity {
    public static final String EXTRA_MODE = "mode";
    private static final int CAMERA_PERMISSION_REQUEST = 2001;
    private static final String STATE_PENDING_MANUAL_CODE = "pending_manual_code";
    private static final String STATUS_WEW_OCZEKUJE_NA_PRZYJECIE = "Oczekuje na przyjęcie";

    private ReturnListAdapter adapter;
    private ProgressBar progressBar;
    private TextView txtEmpty;
    private TextView txtHeader;
    private TextView txtCount;
    private EditText editSearch;
    private Spinner spinnerStatusAllegro;
    private Button btnFilterOczekujace;
    private Button btnFilterNaDecyzje;
    private Button btnFilterPoDecyzji;
    private Button btnFilterWTrakcie; // ✅ NOWA ZAKŁADKA
    private Button btnFilterWDrodze;
    private Button btnFilterWszystkie;
    private ImageButton btnRefresh;
    private ImageButton btnSync;
    private FloatingActionButton btnScanCode;
    private FloatingActionButton btnManualReturn;
    private View loadingOverlay;
    private TextView txtLoadingMessage;
    private View filtersDecisionRow;
    private String mode;
    private final Handler searchHandler = new Handler(Looper.getMainLooper());
    private String currentStatusWewnetrzny;
    private String currentStatusAllegro;
    private int pendingCount;
    private int inProgressCount; // ✅ LICZNIK DLA "W TRAKCIE"
    private int completedCount;
    private String pendingManualCode;
    private boolean isManualDialogVisible;

    private final ActivityResultLauncher<ScanOptions> scanLauncher =
            registerForActivityResult(new ScanContract(), this::handleScanResult);

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_returns_list);

        mode = getIntent().getStringExtra(EXTRA_MODE);
        if (savedInstanceState != null) {
            pendingManualCode = savedInstanceState.getString(STATE_PENDING_MANUAL_CODE);
        }

        progressBar = findViewById(R.id.progressBarSync);

        txtEmpty = findViewById(R.id.txtEmpty);
        txtHeader = findViewById(R.id.txtHeader);
        txtCount = findViewById(R.id.txtCount);
        editSearch = findViewById(R.id.editSearch);
        spinnerStatusAllegro = findViewById(R.id.spinnerStatusAllegro);
        btnFilterOczekujace = findViewById(R.id.btnFilterOczekujace);
        btnFilterNaDecyzje = findViewById(R.id.btnFilterNaDecyzje);
        btnFilterPoDecyzji = findViewById(R.id.btnFilterPoDecyzji);
        btnFilterWTrakcie = findViewById(R.id.btnFilterWTrakcie); // ✅ INICJALIZACJA
        btnFilterWDrodze = findViewById(R.id.btnFilterWDrodze);
        btnFilterWszystkie = findViewById(R.id.btnFilterWszystkie);
        btnRefresh = findViewById(R.id.btnRefresh);
        btnSync = findViewById(R.id.btnSync);
        btnScanCode = findViewById(R.id.btnScanCode);
        btnManualReturn = findViewById(R.id.btnManualReturn);
        loadingOverlay = findViewById(R.id.loadingOverlay);
        txtLoadingMessage = findViewById(R.id.txtLoadingMessage);
        filtersDecisionRow = findViewById(R.id.filtersDecisionRow);

        ImageButton btnBack = findViewById(R.id.btnBack);
        if (btnBack != null) {
            btnBack.setOnClickListener(v -> finish());
        }

        if ("sales".equals(mode)) {
            txtHeader.setText("Handlowiec - moje zwroty");
        } else {
            txtHeader.setText("Magazyn - zwroty");
        }

        RecyclerView recyclerView = findViewById(R.id.listReturns);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        ReturnListAdapter.DisplayMode displayMode;
        if ("sales".equals(mode)) {
            displayMode = ReturnListAdapter.DisplayMode.WAREHOUSE_STATUS;
        } else if ("warehouse".equals(mode)) {
            displayMode = ReturnListAdapter.DisplayMode.WAREHOUSE_SCANNER;
        } else {
            displayMode = ReturnListAdapter.DisplayMode.DECISION;
        }
        adapter = new ReturnListAdapter(this::openDetails, displayMode);
        recyclerView.setAdapter(adapter);

        setupFilters();
        setupSearch();
        btnRefresh.setOnClickListener(v -> loadReturns());
        btnScanCode.setOnClickListener(v -> startCodeScan());
        if (btnManualReturn != null) {
            btnManualReturn.setOnClickListener(v -> openManualReturnForm(null));
        }
        if ("sales".equals(mode)) {
            btnSync.setVisibility(View.GONE);
            if (btnManualReturn != null) {
                btnManualReturn.setVisibility(View.GONE);
            }
        } else if ("warehouse".equals(mode)) {
            btnSync.setOnClickListener(v -> syncReturns());
            if (btnManualReturn != null) {
                btnManualReturn.setVisibility(View.VISIBLE);
            }
        } else {
            btnSync.setVisibility(View.GONE);
            if (btnManualReturn != null) {
                btnManualReturn.setVisibility(View.GONE);
            }
        }

        loadReturns();
    }

    @Override
    protected void onResume() {
        super.onResume();
        if (pendingManualCode != null && !isManualDialogVisible && !"sales".equals(mode)) {
            showManualReturnPrompt(pendingManualCode);
        }
    }

    @Override
    protected void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        if (pendingManualCode != null) {
            outState.putString(STATE_PENDING_MANUAL_CODE, pendingManualCode);
        }
    }

    private void setupFilters() {
        if ("sales".equals(mode)) {
            setupSalesFilters();
            return;
        }
        if ("warehouse".equals(mode)) {
            setupWarehouseFilters();
            return;
        }
        setupSummaryFilters();
    }

    // ✅ ZAKTUALIZOWANA METODA - 3 ZAKŁADKI DLA HANDLOWCA
    private void setupSalesFilters() {
        spinnerStatusAllegro.setVisibility(View.GONE);
        btnFilterOczekujace.setVisibility(View.GONE);
        btnFilterWDrodze.setVisibility(View.GONE);
        btnFilterWszystkie.setVisibility(View.GONE);
        if (filtersDecisionRow != null) {
            filtersDecisionRow.setVisibility(View.VISIBLE);
        }

        // Zakładka 1: Nowe sprawy
        btnFilterNaDecyzje.setText("Nowe sprawy");
        btnFilterNaDecyzje.setOnClickListener(v -> {
            currentStatusWewnetrzny = "Oczekuje na decyzję handlowca";
            currentStatusAllegro = null;
            setActiveFilter(btnFilterNaDecyzje);
            loadReturns();
        });

        // ✅ Zakładka 2: W trakcie (NOWA!)
        if (btnFilterWTrakcie != null) {
            btnFilterWTrakcie.setVisibility(View.VISIBLE);
            btnFilterWTrakcie.setText("W trakcie");
            btnFilterWTrakcie.setOnClickListener(v -> {
                currentStatusWewnetrzny = "Po decyzji";
                currentStatusAllegro = null;
                setActiveFilter(btnFilterWTrakcie);
                loadReturns();
            });
        }

        // Zakładka 3: Zakończone
        btnFilterPoDecyzji.setText("Zakończone");
        btnFilterPoDecyzji.setOnClickListener(v -> {
            currentStatusWewnetrzny = "Zakończony";
            currentStatusAllegro = null;
            setActiveFilter(btnFilterPoDecyzji);
            loadReturns();
        });

        // Domyślnie: Nowe sprawy
        setActiveFilter(btnFilterNaDecyzje);
        currentStatusWewnetrzny = "Oczekuje na decyzję handlowca";
        currentStatusAllegro = null;
        updateSalesCounts();
    }

    private void setupWarehouseFilters() {
        spinnerStatusAllegro.setVisibility(View.GONE);
        btnFilterNaDecyzje.setVisibility(View.GONE);
        btnFilterPoDecyzji.setVisibility(View.GONE);
        if (btnFilterWTrakcie != null) {
            btnFilterWTrakcie.setVisibility(View.GONE);
        }
        if (filtersDecisionRow != null) {
            filtersDecisionRow.setVisibility(View.GONE);
        }

        btnFilterOczekujace.setText("Dostarczone");

        btnFilterOczekujace.setOnClickListener(v -> {
            currentStatusWewnetrzny = STATUS_WEW_OCZEKUJE_NA_PRZYJECIE;
            currentStatusAllegro = "DELIVERED";
            setActiveFilter(btnFilterOczekujace);
            loadReturns();
        });
        btnFilterWDrodze.setOnClickListener(v -> {
            currentStatusWewnetrzny = STATUS_WEW_OCZEKUJE_NA_PRZYJECIE;
            currentStatusAllegro = "IN_TRANSIT";
            setActiveFilter(btnFilterWDrodze);
            loadReturns();
        });
        btnFilterWszystkie.setOnClickListener(v -> {
            currentStatusWewnetrzny = STATUS_WEW_OCZEKUJE_NA_PRZYJECIE;
            currentStatusAllegro = null;
            setActiveFilter(btnFilterWszystkie);
            loadReturns();
        });

        setActiveFilter(btnFilterWszystkie);
        currentStatusWewnetrzny = STATUS_WEW_OCZEKUJE_NA_PRZYJECIE;
        currentStatusAllegro = null;
    }

    private void setupSummaryFilters() {
        spinnerStatusAllegro.setVisibility(View.GONE);
        btnFilterOczekujace.setVisibility(View.GONE);
        btnFilterWDrodze.setVisibility(View.GONE);
        btnFilterWszystkie.setVisibility(View.GONE);
        if (filtersDecisionRow != null) {
            filtersDecisionRow.setVisibility(View.VISIBLE);
        }

        btnFilterNaDecyzje.setText("Czekają na decyzję");
        btnFilterNaDecyzje.setOnClickListener(v -> {
            currentStatusWewnetrzny = "Oczekuje na decyzję handlowca";
            currentStatusAllegro = null;
            setActiveFilter(btnFilterNaDecyzje);
            loadReturns();
        });

        if (btnFilterWTrakcie != null) {
            btnFilterWTrakcie.setVisibility(View.VISIBLE);
            btnFilterWTrakcie.setText("Po decyzji");
            btnFilterWTrakcie.setOnClickListener(v -> {
                currentStatusWewnetrzny = "Po decyzji";
                currentStatusAllegro = null;
                setActiveFilter(btnFilterWTrakcie);
                loadReturns();
            });
        }

        btnFilterPoDecyzji.setText("Zakończone");
        btnFilterPoDecyzji.setOnClickListener(v -> {
            currentStatusWewnetrzny = "Zakończony";
            currentStatusAllegro = null;
            setActiveFilter(btnFilterPoDecyzji);
            loadReturns();
        });

        setActiveFilter(btnFilterWTrakcie != null ? btnFilterWTrakcie : btnFilterNaDecyzje);
        currentStatusWewnetrzny = "Po decyzji";
        currentStatusAllegro = null;
    }

    private void setupSearch() {
        editSearch.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {
            }

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                searchHandler.removeCallbacksAndMessages(null);
                searchHandler.postDelayed(ReturnsListActivity.this::loadReturns, 300);
            }

            @Override
            public void afterTextChanged(Editable s) {
            }
        });
    }

    private void loadReturns() {
        if (progressBar != null) progressBar.setVisibility(View.VISIBLE);
        if (txtEmpty != null) txtEmpty.setVisibility(View.GONE);
        if (txtCount != null) txtCount.setText("Wyświetlono: 0");

        hideLoadingOverlay();

        ApiClient client = new ApiClient(this);
        ApiClient.ApiCallback<PaginatedResponse<ReturnListItemDto>> callback = new ApiClient.ApiCallback<PaginatedResponse<ReturnListItemDto>>() {
            @Override
            public void onSuccess(PaginatedResponse<ReturnListItemDto> data) {
                runOnUiThread(() -> {
                    if (progressBar != null) progressBar.setVisibility(View.GONE);
                    List<ReturnListItemDto> items = data != null ? data.getItems() : null;
                    adapter.setItems(items);
                    if (items == null || items.isEmpty()) {
                        if (txtEmpty != null) txtEmpty.setVisibility(View.VISIBLE);
                    }
                    int count = items == null ? 0 : items.size();
                    if (txtCount != null) txtCount.setText("Wyświetlono: " + count);

                    if ("sales".equals(mode)) {
                        updateSalesCounts();
                    }
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    if (progressBar != null) progressBar.setVisibility(View.GONE);
                    if (txtEmpty != null) txtEmpty.setVisibility(View.VISIBLE);
                    Toast.makeText(ReturnsListActivity.this, "Błąd: " + message, Toast.LENGTH_LONG).show();
                });
            }
        };

        String query = buildQueryString();
        if ("sales".equals(mode)) {
            client.fetchAssignedReturns(query, callback);
        } else {
            client.fetchReturns(query, callback);
        }
    }

    private void openDetails(ReturnListItemDto item) {
        openDetailsById(item.getId());
    }

    private void openDetailsById(int returnId) {
        Intent intent;
        if ("sales".equals(mode)) {
            intent = new Intent(this, SalesReturnDetailActivity.class);
            intent.putExtra(SalesReturnDetailActivity.EXTRA_RETURN_ID, returnId);
        } else {
            intent = new Intent(this, ReturnDetailActivity.class);
            intent.putExtra(ReturnDetailActivity.EXTRA_RETURN_ID, returnId);
        }
        startActivity(intent);
    }

    private void syncReturns() {
        showLoadingOverlay("Trwa synchronizacja z Allegro...");

        if (progressBar != null) progressBar.setVisibility(View.GONE);

        if (btnSync != null) btnSync.setEnabled(false);

        ApiClient client = new ApiClient(this);
        ReturnSyncRequest request = new ReturnSyncRequest(null, null);

        client.syncReturns(request, new ApiClient.ApiCallback<ReturnSyncResponse>() {
            @Override
            public void onSuccess(ReturnSyncResponse data) {
                runOnUiThread(() -> {
                    hideLoadingOverlay();
                    if (btnSync != null) btnSync.setEnabled(true);

                    String message = "Synchronizacja zakończona.";
                    if (data != null) {
                        message = "Pobrano: " + data.getReturnsFetched() +
                                ", Przetworzono: " + data.getReturnsProcessed() +
                                " (Konta: " + data.getAccountsProcessed() + ").";

                        if (data.getErrors() != null && !data.getErrors().isEmpty()) {
                            String errorDetails = TextUtils.join("\n", data.getErrors());
                            Toast.makeText(ReturnsListActivity.this, "Błędy:\n" + errorDetails, Toast.LENGTH_LONG).show();
                        } else {
                            Toast.makeText(ReturnsListActivity.this, message, Toast.LENGTH_LONG).show();
                        }
                    }
                    loadReturns();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    hideLoadingOverlay();
                    if (btnSync != null) btnSync.setEnabled(true);
                    Toast.makeText(ReturnsListActivity.this, "Krytyczny błąd API: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    // ✅ ZAKTUALIZOWANA METODA - OBSŁUGA 3 PRZYCISKÓW
    private void setActiveFilter(Button activeButton) {
        Button[] buttons = new Button[] {
                btnFilterOczekujace,
                btnFilterNaDecyzje,
                btnFilterWTrakcie, // ✅ DODANY
                btnFilterPoDecyzji,
                btnFilterWDrodze,
                btnFilterWszystkie
        };
        for (Button button : buttons) {
            if (button == null) continue;
            if (button == activeButton) {
                button.setBackgroundColor(ContextCompat.getColor(this, android.R.color.holo_blue_light));
            } else {
                button.setBackgroundColor(ContextCompat.getColor(this, android.R.color.darker_gray));
            }
        }
    }

    private String buildQueryString() {
        StringBuilder sb = new StringBuilder("?");
        int pageSize = "warehouse".equals(mode) ? 0 : 100;
        sb.append("page=1&pageSize=").append(pageSize);

        String searchValue = editSearch.getText().toString().trim();
        if (!searchValue.isEmpty()) {
            sb.append("&search=").append(encode(searchValue));
        }

        String statusWew = currentStatusWewnetrzny;
        if (statusWew != null && !statusWew.isEmpty()) {
            sb.append("&statusWewnetrzny=").append(encode(statusWew));
        }

        String statusAll = currentStatusAllegro;
        if (statusAll != null && !statusAll.isEmpty()) {
            sb.append("&statusAllegro=").append(encode(statusAll));
        }

        return sb.toString();
    }

    // ✅ ZAKTUALIZOWANA METODA - POBIERANIE LICZNIKÓW DLA 3 ZAKŁADEK
    private void updateSalesCounts() {
        ApiClient client = new ApiClient(this);

        // Licznik: Nowe sprawy
        fetchSalesCount(client, "Oczekuje na decyzję handlowca", count -> {
            pendingCount = count;
            btnFilterNaDecyzje.setText("Nowe sprawy (" + pendingCount + ")");
        });

        // ✅ Licznik: W trakcie
        fetchSalesCount(client, "Po decyzji", count -> {
            inProgressCount = count;
            if (btnFilterWTrakcie != null) {
                btnFilterWTrakcie.setText("W trakcie (" + inProgressCount + ")");
            }
        });

        // Licznik: Zakończone
        fetchSalesCount(client, "Zakończony", count -> {
            completedCount = count;
            btnFilterPoDecyzji.setText("Zakończone (" + completedCount + ")");
        });
    }

    private void fetchSalesCount(ApiClient client, String status, IntConsumer callback) {
        String query = "?page=1&pageSize=1&statusWewnetrzny=" + encode(status);
        client.fetchAssignedReturns(query, new ApiClient.ApiCallback<PaginatedResponse<ReturnListItemDto>>() {
            @Override
            public void onSuccess(PaginatedResponse<ReturnListItemDto> data) {
                int total = data != null ? data.getTotalItems() : 0;
                runOnUiThread(() -> callback.accept(total));
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> callback.accept(0));
            }
        });
    }

    private void startCodeScan() {
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.CAMERA)
                != android.content.pm.PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, new String[] { Manifest.permission.CAMERA }, CAMERA_PERMISSION_REQUEST);
            return;
        }
        ScanOptions options = new ScanOptions();
        options.setPrompt("Zeskanuj kod kreskowy lub QR");
        options.setBeepEnabled(true);
        options.setOrientationLocked(true);
        options.setDesiredBarcodeFormats(ScanOptions.ALL_CODE_TYPES);
        scanLauncher.launch(options);
    }

    private void handleScanResult(ScanIntentResult result) {
        if (result.getContents() == null) {
            return;
        }
        String code = result.getContents().trim();
        if (code.isEmpty()) {
            return;
        }
        String coreCode = extractCoreWaybill(code);
        searchHandler.removeCallbacksAndMessages(null);
        editSearch.setText(coreCode);
        findReturnByCode(coreCode);
    }

    private void findReturnByCode(String code) {
        progressBar.setVisibility(View.VISIBLE);
        showLoadingOverlay("Szukam zwrotu...");
        ApiClient client = new ApiClient(this);
        String query = "?page=1&pageSize=100&search=" + encode(code);
        ApiClient.ApiCallback<PaginatedResponse<ReturnListItemDto>> callback = new ApiClient.ApiCallback<PaginatedResponse<ReturnListItemDto>>() {
            @Override
            public void onSuccess(PaginatedResponse<ReturnListItemDto> data) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    hideLoadingOverlay();
                    List<ReturnListItemDto> items = data != null ? data.getItems() : null;
                    adapter.setItems(items);
                    int count = items == null ? 0 : items.size();
                    txtCount.setText("Wyświetlono: " + count);
                    txtEmpty.setVisibility(count == 0 ? View.VISIBLE : View.GONE);

                    if (count == 1) {
                        openDetails(items.get(0));
                        return;
                    }
                    if (count == 0) {
                        if ("sales".equals(mode)) {
                            Toast.makeText(ReturnsListActivity.this, "Brak zwrotu dla kodu: " + code, Toast.LENGTH_LONG).show();
                        } else {
                            showManualReturnPrompt(code);
                        }
                    }
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    hideLoadingOverlay();
                    Toast.makeText(ReturnsListActivity.this, "Nie znaleziono zwrotu: " + message, Toast.LENGTH_LONG).show();
                    loadReturns();
                });
            }
        };

        if ("sales".equals(mode)) {
            client.fetchAssignedReturns(query, callback);
        } else {
            client.fetchReturns(query, callback);
        }
    }

    private void showManualReturnPrompt(String code) {
        pendingManualCode = code;
        AlertDialog dialog = new AlertDialog.Builder(this)
                .setTitle("Nie znaleziono zwrotu")
                .setMessage("Nie znaleziono zwrotu dla numeru listu: " + code + ".\n\nCzy chcesz dodać nowy zwrot ręcznie?")
                .setPositiveButton("Dodaj ręcznie", (dialogInterface, which) -> {
                    pendingManualCode = null;
                    openManualReturnForm(code);
                })
                .setNegativeButton("Anuluj", (dialogInterface, which) -> pendingManualCode = null)
                .create();
        dialog.setOnShowListener(dialogInterface -> isManualDialogVisible = true);
        dialog.setOnDismissListener(dialogInterface -> {
            isManualDialogVisible = false;
            if (!isChangingConfigurations()) {
                pendingManualCode = null;
            }
        });
        dialog.show();
    }

    private void openManualReturnForm(@Nullable String waybill) {
        Intent intent = new Intent(this, ManualReturnActivity.class);
        if (waybill != null && !waybill.trim().isEmpty()) {
            intent.putExtra(ManualReturnActivity.EXTRA_WAYBILL, waybill);
        }
        startActivity(intent);
    }

    private String extractCoreWaybill(String scannedText) {
        if (scannedText == null || scannedText.trim().isEmpty()) {
            return "";
        }
        Matcher dpdMatch = Pattern.compile("^%.{7}([a-zA-Z0-9]{14})").matcher(scannedText);
        if (dpdMatch.find()) {
            return dpdMatch.group(1);
        }
        Matcher genericMatch = Pattern.compile("[a-zA-Z0-9]{10,}").matcher(scannedText);
        if (genericMatch.find()) {
            return genericMatch.group();
        }
        return scannedText.replaceAll("[^a-zA-Z0-9]", "");
    }

    private String encode(@Nullable String value) {
        if (value == null) {
            return "";
        }
        return URLEncoder.encode(value, StandardCharsets.UTF_8);
    }

    private void showLoadingOverlay(String message) {
        if (txtLoadingMessage != null) {
            txtLoadingMessage.setText(message);
        }
        if (loadingOverlay != null) {
            loadingOverlay.setVisibility(View.VISIBLE);
        }
    }

    private void hideLoadingOverlay() {
        if (loadingOverlay != null) {
            loadingOverlay.setVisibility(View.GONE);
        }
    }
}
