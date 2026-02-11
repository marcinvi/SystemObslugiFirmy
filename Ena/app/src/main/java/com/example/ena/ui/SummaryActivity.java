package com.example.ena.ui;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;

import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.PaginatedResponse;
import com.example.ena.api.ReturnListItemDto;

import java.util.ArrayList;

public class SummaryActivity extends AppCompatActivity {

    // UI
    private RecyclerView recyclerView;
    private ReturnListAdapter adapter;
    private SwipeRefreshLayout swipeRefresh;
    private ProgressBar progressBar;
    private TextView txtNoData;
    private ImageButton btnBack, btnRefresh;

    // Logika
    private ApiClient apiClient;
    private View filtersContainer;
    private Button btnFilterPending;
    private Button btnFilterAction;
    private Button btnFilterDone;
    private Button btnFilterOther;
    private String currentStatusWewnetrzny;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_summary);

        apiClient = new ApiClient(this);

        initViews();
        setupRecyclerView();
        setupFilters();
        loadReturns();
    }

    private void initViews() {
        recyclerView = findViewById(R.id.recyclerSummary);
        swipeRefresh = findViewById(R.id.swipeRefresh);
        progressBar = findViewById(R.id.progressBar);
        txtNoData = findViewById(R.id.txtNoData);
        btnBack = findViewById(R.id.btnBack);
        btnRefresh = findViewById(R.id.btnRefresh);
        filtersContainer = findViewById(R.id.summaryFilters);
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
        }, ReturnListAdapter.DisplayMode.SUMMARY);
        recyclerView.setAdapter(adapter);
    }

    private void setupFilters() {
        if (filtersContainer == null) {
            return;
        }

        filtersContainer.setVisibility(View.VISIBLE);

        btnFilterPending.setText("Nowe");
        btnFilterPending.setOnClickListener(v -> {
            currentStatusWewnetrzny = "Po decyzji";
            setActiveFilter(btnFilterPending);
            loadReturns();
        });

        btnFilterDone.setText("Zakończone");
        btnFilterDone.setOnClickListener(v -> {
            currentStatusWewnetrzny = "Zakończony";
            setActiveFilter(btnFilterDone);
            loadReturns();
        });

        btnFilterAction.setText("Oczekują na decyzję");
        btnFilterAction.setOnClickListener(v -> {
            currentStatusWewnetrzny = "Oczekuje na decyzję handlowca";
            setActiveFilter(btnFilterAction);
            loadReturns();
        });

        if (btnFilterOther != null) {
            btnFilterOther.setVisibility(View.GONE);
        }

        currentStatusWewnetrzny = "Po decyzji";
        setActiveFilter(btnFilterPending);
    }

    private void setActiveFilter(Button activeButton) {
        Button[] buttons = new Button[] { btnFilterPending, btnFilterAction, btnFilterDone };
        for (Button button : buttons) {
            if (button == null) continue;
            if (button == activeButton) {
                button.setBackgroundColor(getResources().getColor(android.R.color.holo_blue_light));
            } else {
                button.setBackgroundColor(getResources().getColor(android.R.color.darker_gray));
            }
        }
    }

    private void loadReturns() {
        if (!swipeRefresh.isRefreshing()) progressBar.setVisibility(View.VISIBLE);
        txtNoData.setVisibility(View.GONE);

        // Budujemy zapytanie
        String query = "?page=1&pageSize=0"
                + "&excludeStatusWewnetrzny=Oczekuje%20na%20przyj%C4%99cie"
                + "&sortByLastAction=true";

        if (currentStatusWewnetrzny != null && !currentStatusWewnetrzny.isEmpty()) {
            query += "&statusWewnetrzny=" + encode(currentStatusWewnetrzny);
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
        return URLEncoder.encode(value, StandardCharsets.UTF_8);
    }

}
