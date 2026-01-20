package com.example.ena.ui;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;
import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.PaginatedResponse;
import com.example.ena.api.ReturnListItemDto;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.util.Arrays;
import java.util.List;

public class ReturnsListActivity extends AppCompatActivity {
    public static final String EXTRA_MODE = "mode";

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
    private Button btnRefresh;
    private String mode;
    private final Handler searchHandler = new Handler(Looper.getMainLooper());
    private String currentStatusWewnetrzny;
    private String currentStatusAllegro;
    private boolean deliveredOnly;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_returns_list);

        mode = getIntent().getStringExtra(EXTRA_MODE);
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

        if ("sales".equals(mode)) {
            txtHeader.setText("Handlowiec - moje zwroty");
        } else {
            txtHeader.setText("Magazyn - zwroty");
        }

        RecyclerView recyclerView = findViewById(R.id.listReturns);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        adapter = new ReturnListAdapter(this::openDetails);
        recyclerView.setAdapter(adapter);

        setupFilters();
        setupSearch();
        btnRefresh.setOnClickListener(v -> loadReturns());
        loadReturns();
    }

    private void setupFilters() {
        List<String> statuses = Arrays.asList(
                "Wszystkie",
                "Dostarczono",
                "W drodze",
                "Gotowy do odbioru",
                "Utworzono",
                "Zwrócono prowizję"
        );
        android.widget.ArrayAdapter<String> adapter = new android.widget.ArrayAdapter<>(
                this,
                android.R.layout.simple_spinner_item,
                statuses
        );
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinnerStatusAllegro.setAdapter(adapter);

        btnFilterOczekujace.setOnClickListener(v -> {
            deliveredOnly = true;
            currentStatusWewnetrzny = "Oczekuje na przyjęcie";
            setActiveFilter(btnFilterOczekujace);
            loadReturns();
        });
        btnFilterNaDecyzje.setOnClickListener(v -> {
            deliveredOnly = false;
            currentStatusWewnetrzny = "Oczekuje na decyzję handlowca";
            setActiveFilter(btnFilterNaDecyzje);
            loadReturns();
        });
        btnFilterPoDecyzji.setOnClickListener(v -> {
            deliveredOnly = false;
            currentStatusWewnetrzny = "Zakończony";
            setActiveFilter(btnFilterPoDecyzji);
            loadReturns();
        });
        btnFilterWDrodze.setOnClickListener(v -> {
            deliveredOnly = false;
            currentStatusWewnetrzny = null;
            currentStatusAllegro = "IN_TRANSIT";
            spinnerStatusAllegro.setSelection(2);
            setActiveFilter(btnFilterWDrodze);
            loadReturns();
        });
        btnFilterWszystkie.setOnClickListener(v -> {
            deliveredOnly = false;
            currentStatusWewnetrzny = null;
            currentStatusAllegro = null;
            spinnerStatusAllegro.setSelection(0);
            setActiveFilter(btnFilterWszystkie);
            loadReturns();
        });

        spinnerStatusAllegro.setSelection(0);
        spinnerStatusAllegro.setOnItemSelectedListener(new android.widget.AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(android.widget.AdapterView<?> parent, View view, int position, long id) {
                if (position == 0) {
                    currentStatusAllegro = null;
                } else {
                    currentStatusAllegro = translateStatusToApi(parent.getItemAtPosition(position).toString());
                }
                loadReturns();
            }

            @Override
            public void onNothingSelected(android.widget.AdapterView<?> parent) {
            }
        });

        setActiveFilter(btnFilterOczekujace);
        currentStatusWewnetrzny = "Oczekuje na przyjęcie";
        deliveredOnly = true;
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

        ApiClient client = new ApiClient(this);
        ApiClient.ApiCallback<PaginatedResponse<ReturnListItemDto>> callback = new ApiClient.ApiCallback<PaginatedResponse<ReturnListItemDto>>() {
            @Override
            public void onSuccess(PaginatedResponse<ReturnListItemDto> data) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    List<ReturnListItemDto> items = data != null ? data.getItems() : null;
                    adapter.setItems(items);
                    if (items == null || items.isEmpty()) {
                        txtEmpty.setVisibility(View.VISIBLE);
                    }
                    int count = items == null ? 0 : items.size();
                    txtCount.setText("Wyświetlono: " + count);
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
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
        Intent intent = new Intent(this, ReturnDetailActivity.class);
        intent.putExtra(ReturnDetailActivity.EXTRA_RETURN_ID, item.getId());
        startActivity(intent);
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
        } else if (deliveredOnly) {
            sb.append("&statusAllegro=DELIVERED");
        }

        return sb.toString();
    }

    private String translateStatusToApi(String status) {
        if ("Dostarczono".equals(status)) return "DELIVERED";
        if ("W drodze".equals(status)) return "IN_TRANSIT";
        if ("Gotowy do odbioru".equals(status)) return "READY_FOR_PICKUP";
        if ("Utworzono".equals(status)) return "CREATED";
        if ("Zwrócono prowizję".equals(status)) return "COMMISSION_REFUNDED";
        return status;
    }

    private String encode(@Nullable String value) {
        if (value == null) {
            return "";
        }
        return URLEncoder.encode(value, StandardCharsets.UTF_8);
    }
}
