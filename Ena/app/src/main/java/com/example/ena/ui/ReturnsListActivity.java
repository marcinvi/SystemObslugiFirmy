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
    private Button btnFilterWDrodze;
    private Button btnFilterWszystkie;
    private ImageButton btnRefresh;
    private ImageButton btnSync;
    private FloatingActionButton btnScanCode;
    private View loadingOverlay;
    private TextView txtLoadingMessage;
    private String mode;
    private final Handler searchHandler = new Handler(Looper.getMainLooper());
    private String currentStatusWewnetrzny;
    private String currentStatusAllegro;
    private int pendingCount;
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
        progressBar = findViewById(R.id.progress);
        txtEmpty = findViewById(R.id.txtEmpty);
        txtHeader = findViewById(R.id.txtHeader);
        txtCount = findViewById(R.id.txtCount);
        editSearch = findViewById(R.id.editSearch);
        spinnerStatusAllegro = findViewById(R.id.spinnerStatusAllegro);
        btnFilterOczekujace = findViewById(R.id.btnFilterOczekujace);
        btnFilterNaDecyzje = findViewById(R.id.btnFilterNaDecyzje);
        btnFilterPoDecyzji = findViewById(R.id.btnFilterPoDecyzji);
        btnFilterWDrodze = findViewById(R.id.btnFilterWDrodze);
        btnFilterWszystkie = findViewById(R.id.btnFilterWszystkie);
        btnRefresh = findViewById(R.id.btnRefresh);
        btnSync = findViewById(R.id.btnSync);
        btnScanCode = findViewById(R.id.btnScanCode);
        loadingOverlay = findViewById(R.id.loadingOverlay);
        txtLoadingMessage = findViewById(R.id.txtLoadingMessage);

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
        if ("sales".equals(mode)) {
            btnSync.setVisibility(View.GONE);
        } else {
            btnSync.setOnClickListener(v -> syncReturns());
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
        spinnerStatusAllegro.setVisibility(View.GONE);
        btnFilterOczekujace.setVisibility(View.GONE);
        btnFilterNaDecyzje.setVisibility(View.GONE);
        btnFilterPoDecyzji.setVisibility(View.GONE);
        btnFilterWDrodze.setVisibility(View.GONE);
        btnFilterWszystkie.setVisibility(View.GONE);
    }

    private void setupSalesFilters() {
        spinnerStatusAllegro.setVisibility(View.GONE);
        btnFilterOczekujace.setVisibility(View.GONE);
        btnFilterWDrodze.setVisibility(View.GONE);
        btnFilterWszystkie.setVisibility(View.GONE);

        btnFilterNaDecyzje.setText("Nowe sprawy");
        btnFilterPoDecyzji.setText("Zakończone");

        btnFilterNaDecyzje.setOnClickListener(v -> {
            currentStatusWewnetrzny = "Oczekuje na decyzję handlowca";
            currentStatusAllegro = null;
            setActiveFilter(btnFilterNaDecyzje);
            loadReturns();
        });
        btnFilterPoDecyzji.setOnClickListener(v -> {
            currentStatusWewnetrzny = "Zakończony";
            currentStatusAllegro = null;
            setActiveFilter(btnFilterPoDecyzji);
            loadReturns();
        });

        setActiveFilter(btnFilterNaDecyzje);
        currentStatusWewnetrzny = "Oczekuje na decyzję handlowca";
        currentStatusAllegro = null;
        updateSalesCounts();
    }

    private void setupWarehouseFilters() {
        spinnerStatusAllegro.setVisibility(View.GONE);
        btnFilterNaDecyzje.setVisibility(View.GONE);
        btnFilterPoDecyzji.setVisibility(View.GONE);

        btnFilterOczekujace.setText("Dostarczone");

        btnFilterOczekujace.setOnClickListener(v -> {
            currentStatusWewnetrzny = null;
            currentStatusAllegro = "DELIVERED";
            setActiveFilter(btnFilterOczekujace);
            loadReturns();
        });
        btnFilterWDrodze.setOnClickListener(v -> {
            currentStatusWewnetrzny = null;
            currentStatusAllegro = "IN_TRANSIT";
            setActiveFilter(btnFilterWDrodze);
            loadReturns();
        });
        btnFilterWszystkie.setOnClickListener(v -> {
            currentStatusWewnetrzny = null;
            currentStatusAllegro = null;
            setActiveFilter(btnFilterWszystkie);
            loadReturns();
        });

        setActiveFilter(btnFilterOczekujace);
        currentStatusWewnetrzny = null;
        currentStatusAllegro = "DELIVERED";
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
        progressBar.setVisibility(View.VISIBLE);
        txtEmpty.setVisibility(View.GONE);
        txtCount.setText("Wyświetlono: 0");
        showLoadingOverlay("Wczytywanie zwrotów...");

        ApiClient client = new ApiClient(this);
        ApiClient.ApiCallback<PaginatedResponse<ReturnListItemDto>> callback = new ApiClient.ApiCallback<PaginatedResponse<ReturnListItemDto>>() {
            @Override
            public void onSuccess(PaginatedResponse<ReturnListItemDto> data) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    hideLoadingOverlay();
                    List<ReturnListItemDto> items = data != null ? data.getItems() : null;
                    adapter.setItems(items);
                    if (items == null || items.isEmpty()) {
                        txtEmpty.setVisibility(View.VISIBLE);
                    }
                    int count = items == null ? 0 : items.size();
                    txtCount.setText("Wyświetlono: " + count);
                    if ("sales".equals(mode)) {
                        updateSalesCounts();
                    }
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    hideLoadingOverlay();
                    txtEmpty.setVisibility(View.VISIBLE);
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
        progressBar.setVisibility(View.VISIBLE);
        btnSync.setEnabled(false);
        ApiClient client = new ApiClient(this);
        ReturnSyncRequest request = new ReturnSyncRequest(null, null);
        client.syncReturns(request, new ApiClient.ApiCallback<ReturnSyncResponse>() {
            @Override
            public void onSuccess(ReturnSyncResponse data) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    btnSync.setEnabled(true);
                    String message = "Synchronizacja zakończona.";
                    if (data != null) {
                        message = "Synchronizacja: " + data.getReturnsProcessed() + " zwrotów (konta: "
                                + data.getAccountsProcessed() + ").";
                        if (data.getErrors() != null && !data.getErrors().isEmpty()) {
                            message += " Błędy: " + TextUtils.join("; ", data.getErrors());
                        }
                    }
                    Toast.makeText(ReturnsListActivity.this, message, Toast.LENGTH_LONG).show();
                    loadReturns();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    btnSync.setEnabled(true);
                    Toast.makeText(ReturnsListActivity.this, "Błąd synchronizacji: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private void setActiveFilter(Button activeButton) {
        Button[] buttons = new Button[] {
                btnFilterOczekujace,
                btnFilterNaDecyzje,
                btnFilterPoDecyzji,
                btnFilterWDrodze,
                btnFilterWszystkie
        };
        for (Button button : buttons) {
            if (button == activeButton) {
                button.setBackgroundColor(ContextCompat.getColor(this, android.R.color.holo_blue_light));
            } else {
                button.setBackgroundColor(ContextCompat.getColor(this, android.R.color.darker_gray));
            }
        }
    }

    private String buildQueryString() {
        StringBuilder sb = new StringBuilder("?");
        sb.append("page=1&pageSize=100");

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

    private void updateSalesCounts() {
        ApiClient client = new ApiClient(this);
        fetchSalesCount(client, "Oczekuje na decyzję handlowca", count -> {
            pendingCount = count;
            btnFilterNaDecyzje.setText("Nowe sprawy (" + pendingCount + ")");
        });
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

    private void openManualReturnForm(String waybill) {
        Intent intent = new Intent(this, ManualReturnActivity.class);
        intent.putExtra(ManualReturnActivity.EXTRA_WAYBILL, waybill);
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
