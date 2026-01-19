package com.example.ena.ui;

import android.app.AlertDialog;
import android.os.Bundle;
import android.text.InputType;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.ReturnDecisionRequest;
import com.example.ena.api.ReturnDetails;
import com.example.ena.api.ReturnWarehouseUpdateRequest;

public class ReturnDetailActivity extends AppCompatActivity {
    public static final String EXTRA_RETURN_ID = "return_id";

    private TextView txtDetailContent;
    private Button btnWarehouseUpdate;
    private Button btnDecision;
    private int returnId;
    private ReturnDetails details;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_return_detail);

        returnId = getIntent().getIntExtra(EXTRA_RETURN_ID, 0);
        txtDetailContent = findViewById(R.id.txtDetailContent);
        btnWarehouseUpdate = findViewById(R.id.btnWarehouseUpdate);
        btnDecision = findViewById(R.id.btnDecision);

        btnWarehouseUpdate.setOnClickListener(v -> showWarehouseDialog());
        btnDecision.setOnClickListener(v -> showDecisionDialog());

        loadDetails();
    }

    private void loadDetails() {
        ApiClient client = new ApiClient(this);
        client.fetchReturnDetails(returnId, new ApiClient.ApiCallback<ReturnDetails>() {
            @Override
            public void onSuccess(ReturnDetails data) {
                runOnUiThread(() -> {
                    details = data;
                    txtDetailContent.setText(buildDetailsText(data));
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> Toast.makeText(ReturnDetailActivity.this, "Błąd: " + message, Toast.LENGTH_LONG).show());
            }
        });
    }

    private String buildDetailsText(ReturnDetails data) {
        if (data == null) {
            return "Brak danych";
        }
        StringBuilder sb = new StringBuilder();
        sb.append("Nr: ").append(safe(data.referenceNumber)).append("\n");
        sb.append("Status wewn.: ").append(safe(data.statusWewnetrzny)).append("\n");
        sb.append("Status Allegro: ").append(safe(data.statusAllegro)).append("\n\n");
        sb.append("Klient: ").append(safe(data.buyerName)).append(" ( ").append(safe(data.buyerLogin)).append(")\n");
        sb.append("Telefon: ").append(safe(data.buyerPhone)).append("\n");
        sb.append("Adres: ").append(safe(data.buyerAddress)).append("\n\n");
        sb.append("Produkt: ").append(safe(data.productName)).append("\n");
        sb.append("Powód: ").append(safe(data.reason)).append("\n");
        sb.append("Stan produktu: ").append(safe(data.stanProduktuName)).append("\n");
        sb.append("Uwagi magazynu: ").append(safe(data.uwagiMagazynu)).append("\n\n");
        sb.append("Decyzja handlowca: ").append(safe(data.decyzjaHandlowcaName)).append("\n");
        sb.append("Komentarz: ").append(safe(data.komentarzHandlowca)).append("\n");
        return sb.toString();
    }

    private void showWarehouseDialog() {
        if (details == null) {
            Toast.makeText(this, "Brak danych zwrotu", Toast.LENGTH_SHORT).show();
            return;
        }
        LinearLayout layout = new LinearLayout(this);
        layout.setOrientation(LinearLayout.VERTICAL);

        EditText editStan = new EditText(this);
        editStan.setHint("Stan produktu ID");
        editStan.setInputType(InputType.TYPE_CLASS_NUMBER);
        layout.addView(editStan);

        EditText editUwagi = new EditText(this);
        editUwagi.setHint("Uwagi magazynu");
        layout.addView(editUwagi);

        EditText editPrzyjetyPrzez = new EditText(this);
        editPrzyjetyPrzez.setHint("Przyjęty przez ID");
        editPrzyjetyPrzez.setInputType(InputType.TYPE_CLASS_NUMBER);
        layout.addView(editPrzyjetyPrzez);

        new AlertDialog.Builder(this)
            .setTitle("Aktualizacja magazynu")
            .setView(layout)
            .setPositiveButton("Zapisz", (dialog, which) -> {
                ReturnWarehouseUpdateRequest req = new ReturnWarehouseUpdateRequest();
                req.stanProduktuId = parseInt(editStan.getText().toString(), 0);
                req.uwagiMagazynu = editUwagi.getText().toString();
                req.przyjetyPrzezId = parseInt(editPrzyjetyPrzez.getText().toString(), 0);
                req.dataPrzyjecia = null;
                submitWarehouse(req);
            })
            .setNegativeButton("Anuluj", null)
            .show();
    }

    private void showDecisionDialog() {
        if (details == null) {
            Toast.makeText(this, "Brak danych zwrotu", Toast.LENGTH_SHORT).show();
            return;
        }
        LinearLayout layout = new LinearLayout(this);
        layout.setOrientation(LinearLayout.VERTICAL);

        EditText editDecision = new EditText(this);
        editDecision.setHint("Decyzja ID");
        editDecision.setInputType(InputType.TYPE_CLASS_NUMBER);
        layout.addView(editDecision);

        EditText editComment = new EditText(this);
        editComment.setHint("Komentarz");
        layout.addView(editComment);

        new AlertDialog.Builder(this)
            .setTitle("Decyzja handlowca")
            .setView(layout)
            .setPositiveButton("Zapisz", (dialog, which) -> {
                ReturnDecisionRequest req = new ReturnDecisionRequest();
                req.decyzjaId = parseInt(editDecision.getText().toString(), 0);
                req.komentarz = editComment.getText().toString();
                submitDecision(req);
            })
            .setNegativeButton("Anuluj", null)
            .show();
    }

    private void submitWarehouse(ReturnWarehouseUpdateRequest req) {
        btnWarehouseUpdate.setEnabled(false);
        ApiClient client = new ApiClient(this);
        client.submitWarehouseUpdate(returnId, req, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    btnWarehouseUpdate.setEnabled(true);
                    Toast.makeText(ReturnDetailActivity.this, "Zapisano magazyn", Toast.LENGTH_SHORT).show();
                    loadDetails();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    btnWarehouseUpdate.setEnabled(true);
                    Toast.makeText(ReturnDetailActivity.this, "Błąd: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private void submitDecision(ReturnDecisionRequest req) {
        btnDecision.setEnabled(false);
        ApiClient client = new ApiClient(this);
        client.submitDecision(returnId, req, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    btnDecision.setEnabled(true);
                    Toast.makeText(ReturnDetailActivity.this, "Zapisano decyzję", Toast.LENGTH_SHORT).show();
                    loadDetails();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    btnDecision.setEnabled(true);
                    Toast.makeText(ReturnDetailActivity.this, "Błąd: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private int parseInt(String value, int fallback) {
        try {
            return Integer.parseInt(value);
        } catch (NumberFormatException e) {
            return fallback;
        }
    }

    private String safe(String value) {
        return value == null ? "" : value;
    }
}
