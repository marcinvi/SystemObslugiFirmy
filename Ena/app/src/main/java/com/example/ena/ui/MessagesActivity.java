package com.example.ena.ui;

import android.os.Bundle;
import android.widget.ImageButton;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.view.View;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.MessageDto;

import java.util.List;

public class MessagesActivity extends AppCompatActivity {

    private RecyclerView recyclerView;
    private MessageAdapter adapter;
    private SwipeRefreshLayout swipeRefresh;
    private ProgressBar progressBar;
    private TextView txtEmpty;
    private ImageButton btnBack;
    private ApiClient apiClient;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_messages);

        apiClient = new ApiClient(this);

        recyclerView = findViewById(R.id.recyclerMessages);
        swipeRefresh = findViewById(R.id.swipeRefresh);
        progressBar = findViewById(R.id.progressBar);
        txtEmpty = findViewById(R.id.txtEmpty);
        btnBack = findViewById(R.id.btnBack);

        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        adapter = new MessageAdapter();
        recyclerView.setAdapter(adapter);

        btnBack.setOnClickListener(v -> finish());
        swipeRefresh.setOnRefreshListener(this::loadMessages);

        loadMessages();
    }

    private void loadMessages() {
        if (!swipeRefresh.isRefreshing()) progressBar.setVisibility(View.VISIBLE);
        txtEmpty.setVisibility(View.GONE);

        apiClient.fetchMessages(new ApiClient.ApiCallback<List<MessageDto>>() {
            @Override
            public void onSuccess(List<MessageDto> data) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    swipeRefresh.setRefreshing(false);
                    adapter.setItems(data);
                    if (data == null || data.isEmpty()) {
                        txtEmpty.setVisibility(View.VISIBLE);
                    }
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    swipeRefresh.setRefreshing(false);
                    Toast.makeText(MessagesActivity.this, "Błąd: " + message, Toast.LENGTH_SHORT).show();
                });
            }
        });
    }
}