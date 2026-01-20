package com.example.ena.ui;

import android.os.Bundle;
import android.widget.TextView;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.ReturnSummaryItemDto;
import com.example.ena.api.ReturnSummaryResponse;
import com.example.ena.api.ReturnSummaryStatsDto;

public class SummaryActivity extends AppCompatActivity {

    private TextView txtSummaryContent;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_summary);

        txtSummaryContent = findViewById(R.id.txtSummaryContent);
        loadSummary();
    }

    private void loadSummary() {
        ApiClient client = new ApiClient(this);
        client.fetchSummary(new ApiClient.ApiCallback<ReturnSummaryResponse>() {
            @Override
            public void onSuccess(ReturnSummaryResponse data) {
                runOnUiThread(() -> txtSummaryContent.setText(buildSummaryText(data)));
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> Toast.makeText(SummaryActivity.this, "Błąd: " + message, Toast.LENGTH_LONG).show());
            }
        });
    }

    private String buildSummaryText(ReturnSummaryResponse response) {
        if (response == null || response.getStats() == null) {
            return "Brak danych";
        }
        ReturnSummaryStatsDto stats = response.getStats();
        StringBuilder sb = new StringBuilder();
        sb.append("Łącznie: ").append(stats.getTotal()).append("\n");
        sb.append("Do decyzji: ").append(stats.getDoDecyzji()).append("\n");
        sb.append("Zakończone: ").append(stats.getZakonczone()).append("\n");
        if (response.getItems() != null && !response.getItems().isEmpty()) {
            sb.append("\nOstatnie zwroty:\n");
            int limit = Math.min(5, response.getItems().size());
            for (int i = 0; i < limit; i++) {
                ReturnSummaryItemDto item = response.getItems().get(i);
                sb.append("• ").append(item.getNumerZwrotu())
                    .append(" - ").append(item.getStatus())
                    .append("\n");
            }
        }
        return sb.toString();
    }
}
