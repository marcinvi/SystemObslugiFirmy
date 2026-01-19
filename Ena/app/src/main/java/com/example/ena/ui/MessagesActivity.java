package com.example.ena.ui;

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
import com.example.ena.api.MessageDto;
import java.util.List;

public class MessagesActivity extends AppCompatActivity {
    private MessageAdapter adapter;
    private ProgressBar progressBar;
    private TextView txtEmpty;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_messages);

        progressBar = findViewById(R.id.progressMessages);
        txtEmpty = findViewById(R.id.txtMessagesEmpty);
        RecyclerView recyclerView = findViewById(R.id.listMessages);
        recyclerView.setLayoutManager(new LinearLayoutManager(this));
        adapter = new MessageAdapter();
        recyclerView.setAdapter(adapter);

        loadMessages();
    }

    private void loadMessages() {
        progressBar.setVisibility(View.VISIBLE);
        txtEmpty.setVisibility(View.GONE);

        ApiClient client = new ApiClient(this);
        client.fetchMessages(new ApiClient.ApiCallback<List<MessageDto>>() {
            @Override
            public void onSuccess(List<MessageDto> data) {
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
                    Toast.makeText(MessagesActivity.this, "Błąd: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }
}
