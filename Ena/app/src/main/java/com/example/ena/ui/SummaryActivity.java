package com.example.ena.ui;

import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.ContextCompat;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.PaginatedResponse;
import com.example.ena.api.ReturnListItemDto;

import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;

public class SummaryActivity extends AppCompatActivity {

    // UI
    private RecyclerView recyclerView;
    private ReturnListAdapter adapter;
    private SwipeRefreshLayout swipeRefresh;
    private ProgressBar progressBar;
    private TextView txtNoData;
    private ImageButton btnBack, btnRefresh;

    // Filtry
    private Button btnFilterPending;
    private Button btnFilterAction;
    private Button btnFilterDone;
    private Button btnFilterOther;

    // Logika
    private ApiClient apiClient;
    private String currentStatusFilter = "Oczekuje na decyzję handlowca"; // Domyślny start

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_summary);

        apiClient = new ApiClient(this);

        initViews();
        setupRecyclerView();
        setupFilters();

        // Ładujemy domyślny widok
        updateFilterVisuals(btnFilterPending);
        loadReturns();
    }

    private void initViews() {
        recyclerView = findViewById(R.id.recyclerSummary);
        swipeRefresh = findViewById(R.id.swipeRefresh);
        progressBar = findViewById(R.id.progressBar);
        txtNoData = findViewById(R.id.txtNoData);
        btnBack = findViewById(R.id.btnBack);
        btnRefresh = findViewById(R.id.btnRefresh);

        btnFilterPending = findViewById(R.id.btnFilterPending);
        btnFilterAction = findViewById(R.id.btnFilterAction);
        btnFilterDone = findViewById(R.id.btnFilterDone);
        btnFilterOther = findViewById(R.id.btnFilterOther);

        btnBack.setOnClickListener(v -> finish());
        btnRefresh.setOnClickListener(v -> loadReturns());
        swipeRefresh.setOnRefreshListener(this::loadReturns);
    }

    private void setupRecyclerView() {
        recyclerView.setLayoutManager(new LinearLayoutManager(this));

        // Po kliknięciu otwieramy szczegóły.
        // Używamy ReturnDetailActivity jako uniwersalnego podglądu dla kierownika.
        // (Można by też zrobić osobny Activity "ReadOnly", ale ten się nada).
        adapter = new ReturnListAdapter(item -> {
            Intent intent = new Intent(this, ReturnDetailActivity.class);
            intent.putExtra("return_id", item.getId());
            startActivity(intent);
        });
        recyclerView.setAdapter(adapter);
    }

    private void setupFilters() {
        // 1. Oczekujące (na Handlowca)
        btnFilterPending.setOnClickListener(v -> {
            currentStatusFilter = "Oczekuje na decyzję handlowca";
            updateFilterVisuals(btnFilterPending);
            loadReturns();
        });

        // 2. Po decyzji (czyli "Oczekuje na realizację" przez Magazyn)
        btnFilterAction.setOnClickListener(v -> {
            currentStatusFilter = "Oczekuje na realizację";
            updateFilterVisuals(btnFilterAction);
            loadReturns();
        });

        // 3. Zakończone
        btnFilterDone.setOnClickListener(v -> {
            currentStatusFilter = "Zakończony";
            updateFilterVisuals(btnFilterDone);
            loadReturns();
        });

        // 4. Problemy / Inne (Czerwone) - filtrujemy np. Reklamacje
        btnFilterOther.setOnClickListener(v -> {
            currentStatusFilter = "Reklamacja";
            updateFilterVisuals(btnFilterOther);
            loadReturns();
        });
    }

    private void updateFilterVisuals(Button active) {
        Button[] buttons = {btnFilterPending, btnFilterAction, btnFilterDone, btnFilterOther};

        int colorPrimary = ContextCompat.getColor(this, R.color.primary_color);
        if (colorPrimary == 0) colorPrimary = Color.parseColor("#1976D2");
        int colorRed = Color.parseColor("#D32F2F");
        int colorGray = Color.parseColor("#E0E0E0");
        int textGray = Color.parseColor("#333333");

        for (Button btn : buttons) {
            if (btn == active) {
                // Aktywny przycisk
                if (btn == btnFilterOther) {
                    btn.setBackgroundColor(colorRed); // Czerwony dla "Inne"
                } else {
                    btn.setBackgroundColor(colorPrimary); // Niebieski dla reszty
                }
                btn.setTextColor(Color.WHITE);
            } else {
                // Nieaktywny przycisk
                btn.setBackgroundColor(colorGray);
                if (btn == btnFilterOther) {
                    btn.setTextColor(colorRed); // Czerwony tekst dla nieaktywnego "Inne"
                } else {
                    btn.setTextColor(textGray);
                }
            }
        }
    }

    private void loadReturns() {
        if (!swipeRefresh.isRefreshing()) progressBar.setVisibility(View.VISIBLE);
        txtNoData.setVisibility(View.GONE);

        // Budujemy zapytanie
        String query = "?page=1&pageSize=50&sortDesc=true";
        if (currentStatusFilter != null && !currentStatusFilter.isEmpty()) {
            query += "&statusWewnetrzny=" + encode(currentStatusFilter);
        }

        // Pobieramy WSZYSTKIE zwroty (fetchReturns), bo to podgląd ogólny
        apiClient.fetchReturns(query, new ApiClient.ApiCallback<PaginatedResponse<ReturnListItemDto>>() {
            @Override
            public void onSuccess(PaginatedResponse<ReturnListItemDto> data) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    swipeRefresh.setRefreshing(false);
                    if (data != null && data.getItems() != null) {
                        adapter.setItems(data.getItems());
                        if (data.getItems().isEmpty()) {
                            txtNoData.setVisibility(View.VISIBLE);
                        }
                    } else {
                        adapter.setItems(new ArrayList<>());
                        txtNoData.setVisibility(View.VISIBLE);
                    }
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    swipeRefresh.setRefreshing(false);
                    Toast.makeText(SummaryActivity.this, "Błąd: " + message, Toast.LENGTH_SHORT).show();
                });
            }
        });
    }

    private String encode(String value) {
        try {
            return URLEncoder.encode(value, StandardCharsets.UTF_8.toString());
        } catch (Exception e) {
            return value;
        }
    }
}