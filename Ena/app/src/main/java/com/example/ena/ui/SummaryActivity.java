package com.example.ena.ui;

import android.os.Bundle;
import android.widget.TextView;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.ReturnSummaryResponse;

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
        if (response == null || response.stats == null) {
            return "Brak danych";
        }
        StringBuilder sb = new StringBuilder();
        sb.append("Łącznie: ").append(response.stats.total).append("\n");
        sb.append("Do decyzji: ").append(response.stats.doDecyzji).append("\n");
        sb.append("Zakończone: ").append(response.stats.zakonczone).append("\n");
        if (response.items != null && !response.items.isEmpty()) {
            sb.append("\nOstatnie zwroty:\n");
            int limit = Math.min(5, response.items.size());
            for (int i = 0; i < limit; i++) {
                sb.append("• ").append(response.items.get(i).numerZwrotu)
                    .append(" - ").append(response.items.get(i).status)
                    .append("\n");
            }
        }
        return sb.toString();
    }
}
