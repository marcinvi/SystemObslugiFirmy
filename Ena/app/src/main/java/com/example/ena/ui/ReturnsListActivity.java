package com.example.ena.ui;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.ReturnListItemDto;
import java.util.List;

public class ReturnsListActivity extends AppCompatActivity {
    public static final String EXTRA_MODE = "mode";

    private ReturnListAdapter adapter;
    private ProgressBar progressBar;
    private TextView txtEmpty;
    private TextView txtHeader;
    private String mode;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_returns_list);

        mode = getIntent().getStringExtra(EXTRA_MODE);
        progressBar = findViewById(R.id.progress);
        txtEmpty = findViewById(R.id.txtEmpty);
        txtHeader = findViewById(R.id.txtHeader);

        if ("sales".equals(mode)) {
            txtHeader.setText("Handlowiec - moje zwroty");
        } else {
            txtHeader.setText("Magazyn - zwroty");
        }

        RecyclerView recyclerView = findViewById(R.id.listReturns);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        adapter = new ReturnListAdapter(this::openDetails);
        recyclerView.setAdapter(adapter);

        loadReturns();
    }

    private void loadReturns() {
        progressBar.setVisibility(View.VISIBLE);
        txtEmpty.setVisibility(View.GONE);

        ApiClient client = new ApiClient(this);
        ApiClient.ApiCallback<List<ReturnListItemDto>> callback = new ApiClient.ApiCallback<List<ReturnListItemDto>>() {
            @Override
            public void onSuccess(List<ReturnListItemDto> data) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
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
                    txtEmpty.setVisibility(View.VISIBLE);
                    Toast.makeText(ReturnsListActivity.this, "Błąd: " + message, Toast.LENGTH_LONG).show();
                });
            }
        };

        if ("sales".equals(mode)) {
            client.fetchAssignedReturns("", callback);
        } else {
            client.fetchReturns("", callback);
        }
    }

    private void openDetails(ReturnListItemDto item) {
        Intent intent = new Intent(this, ReturnDetailActivity.class);
        intent.putExtra(ReturnDetailActivity.EXTRA_RETURN_ID, item.getId());
        startActivity(intent);
    }
}
