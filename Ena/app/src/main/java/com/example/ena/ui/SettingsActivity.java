package com.example.ena.ui;

import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import com.example.ena.R;
import com.example.ena.api.ApiConfig;

public class SettingsActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_settings);

        EditText editBaseUrl = findViewById(R.id.editBaseUrl);
        Button btnSave = findViewById(R.id.btnSaveBaseUrl);

        editBaseUrl.setText(ApiConfig.getBaseUrl(this));

        btnSave.setOnClickListener(v -> {
            ApiConfig.setBaseUrl(this, editBaseUrl.getText().toString());
            Toast.makeText(this, "Zapisano", Toast.LENGTH_SHORT).show();
            finish();
        });
    }
}
