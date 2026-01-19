package com.example.ena;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.TextView;
import androidx.appcompat.app.AppCompatActivity;
import com.example.ena.api.ApiConfig;
import com.example.ena.ui.MessagesActivity;
import com.example.ena.ui.ReturnsListActivity;
import com.example.ena.ui.SettingsActivity;
import com.example.ena.ui.SummaryActivity;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        TextView txtBaseUrl = findViewById(R.id.txtBaseUrl);
        Button btnWarehouse = findViewById(R.id.btnWarehouse);
        Button btnSales = findViewById(R.id.btnSales);
        Button btnSummary = findViewById(R.id.btnSummary);
        Button btnMessages = findViewById(R.id.btnMessages);
        Button btnSettings = findViewById(R.id.btnSettings);

        updateBaseUrlLabel(txtBaseUrl);

        btnWarehouse.setOnClickListener(v -> openReturns("warehouse"));
        btnSales.setOnClickListener(v -> openReturns("sales"));
        btnSummary.setOnClickListener(v -> startActivity(new Intent(this, SummaryActivity.class)));
        btnMessages.setOnClickListener(v -> startActivity(new Intent(this, MessagesActivity.class)));
        btnSettings.setOnClickListener(v -> startActivity(new Intent(this, SettingsActivity.class)));
    }

    @Override
    protected void onResume() {
        super.onResume();
        TextView txtBaseUrl = findViewById(R.id.txtBaseUrl);
        updateBaseUrlLabel(txtBaseUrl);
    }

    private void updateBaseUrlLabel(TextView label) {
        String baseUrl = ApiConfig.getBaseUrl(this);
        if (baseUrl == null || baseUrl.isEmpty()) {
            label.setText("API: brak konfiguracji");
        } else {
            label.setText("API: " + baseUrl);
        }
    }

    private void openReturns(String mode) {
        Intent intent = new Intent(this, ReturnsListActivity.class);
        intent.putExtra(ReturnsListActivity.EXTRA_MODE, mode);
        startActivity(intent);
    }
}
