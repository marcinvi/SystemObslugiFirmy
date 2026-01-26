package com.example.ena.ui;

import android.content.Intent;
import android.os.Bundle;
import android.view.View; // WAŻNY IMPORT DLA View.GONE/VISIBLE
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.ComplaintAddressDto;
import com.example.ena.api.ComplaintCustomerDto;
import com.example.ena.api.ComplaintProductDto;
import com.example.ena.api.ForwardToComplaintRequest;
import com.example.ena.api.RejectCustomerReturnRequest;
import com.example.ena.api.ReturnActionDto;
import com.example.ena.api.ReturnDetailsDto;
import com.example.ena.api.ReturnRejectionDto;

import java.util.List;

public class SalesReturnDetailActivity extends AppCompatActivity {

    public static final String EXTRA_RETURN_ID = "return_id";

    // --- DEKLARACJE ZMIENNYCH UI (Te, o które krzyczał kompilator) ---
    private TextView txtTitle;
    private TextView txtCurrentStatus;
    private TextView txtProductName;
    private TextView txtBuyerName;
    private TextView txtBuyerLogin;
    private TextView txtCondition;
    private TextView txtWarehouseNotes;

    private Button btnRefund;
    private Button btnReject;
    private Button btnComplaint;
    private ImageButton btnBack;

    private RecyclerView listActions;
    private ProgressBar progressBar;
    // ----------------------------------------------------------------

    private ApiClient apiClient;
    private ReturnActionAdapter actionAdapter;
    private int returnId;
    private ReturnDetailsDto returnDetails;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_sales_return_detail);

        returnId = getIntent().getIntExtra(EXTRA_RETURN_ID, 0);
        if (returnId == 0) {
            Toast.makeText(this, "Błąd: Brak ID zwrotu", Toast.LENGTH_SHORT).show();
            finish();
            return;
        }

        apiClient = new ApiClient(this);

        initViews();
        loadData();
    }

    private void initViews() {
        // Powiązanie z XML (ID muszą być w activity_sales_return_detail.xml)
        txtTitle = findViewById(R.id.txtTitle);
        txtCurrentStatus = findViewById(R.id.txtCurrentStatus);
        txtProductName = findViewById(R.id.txtProductName);
        txtBuyerName = findViewById(R.id.txtBuyerName);
        txtBuyerLogin = findViewById(R.id.txtBuyerLogin);
        txtCondition = findViewById(R.id.txtCondition);
        txtWarehouseNotes = findViewById(R.id.txtWarehouseNotes);
        progressBar = findViewById(R.id.progressBar);

        btnBack = findViewById(R.id.btnBack);
        btnRefund = findViewById(R.id.btnRefund);
        btnReject = findViewById(R.id.btnReject);
        btnComplaint = findViewById(R.id.btnComplaint);

        listActions = findViewById(R.id.listActions);

        // Konfiguracja listy
        if (listActions != null) {
            listActions.setLayoutManager(new LinearLayoutManager(this));
            actionAdapter = new ReturnActionAdapter();
            listActions.setAdapter(actionAdapter);
        }

        // Listenery
        if (btnBack != null) btnBack.setOnClickListener(v -> finish());

        if (btnRefund != null) {
            btnRefund.setOnClickListener(v -> {
                Intent intent = new Intent(this, RefundPaymentActivity.class);
                intent.putExtra(RefundPaymentActivity.EXTRA_RETURN_ID, returnId);
                startActivity(intent);
            });
        }

        if (btnReject != null) btnReject.setOnClickListener(v -> showRejectDialog());
        if (btnComplaint != null) btnComplaint.setOnClickListener(v -> confirmComplaint());
    }

    private void loadData() {
        if (progressBar != null) progressBar.setVisibility(View.VISIBLE);

        apiClient.fetchReturnDetails(returnId, new ApiClient.ApiCallback<ReturnDetailsDto>() {
            @Override
            public void onSuccess(ReturnDetailsDto data) {
                runOnUiThread(() -> {
                    returnDetails = data;
                    bindData(data);
                });
            }
            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    if (progressBar != null) progressBar.setVisibility(View.GONE);
                    Toast.makeText(SalesReturnDetailActivity.this, "Błąd: " + message, Toast.LENGTH_SHORT).show();
                });
            }
        });

        // Historia (opcjonalna, nie blokuje błędem)
        apiClient.fetchReturnActions(returnId, new ApiClient.ApiCallback<List<ReturnActionDto>>() {
            @Override
            public void onSuccess(List<ReturnActionDto> data) {
                runOnUiThread(() -> {
                    if (progressBar != null) progressBar.setVisibility(View.GONE);
                    if (actionAdapter != null) actionAdapter.setItems(data);
                });
            }
            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    if (progressBar != null) progressBar.setVisibility(View.GONE);
                });
            }
        });
    }

    private void bindData(ReturnDetailsDto data) {
        if (data == null) return;

        // Używamy getReferenceNumber()
        String ref = data.getReferenceNumber();
        if (ref == null) ref = "ID: " + data.getId();

        if (txtTitle != null) txtTitle.setText(ref);
        if (txtCurrentStatus != null) txtCurrentStatus.setText(data.getStatusWewnetrzny());
        if (txtProductName != null) txtProductName.setText(data.getProductName());
        if (txtBuyerName != null) txtBuyerName.setText(data.getBuyerName());
        if (txtBuyerLogin != null) txtBuyerLogin.setText(data.getBuyerLogin());

        if (txtCondition != null) txtCondition.setText(data.getStanProduktuName() != null ? data.getStanProduktuName() : "Brak oceny");
        if (txtWarehouseNotes != null) txtWarehouseNotes.setText(data.getUwagiMagazynu() != null ? data.getUwagiMagazynu() : "Brak uwag");

        // Blokada przycisków
        boolean isClosed = "Zakończony".equalsIgnoreCase(data.getStatusWewnetrzny());
        boolean isActive = !isClosed && !"Odrzucony".equalsIgnoreCase(data.getStatusWewnetrzny());

        if (btnRefund != null) btnRefund.setEnabled(isActive);
        if (btnReject != null) btnReject.setEnabled(isActive);
        if (btnComplaint != null) btnComplaint.setEnabled(isActive);
    }

    private void showRejectDialog() {
        new AlertDialog.Builder(this)
                .setTitle("Odrzuć zwrot")
                .setMessage("Czy na pewno chcesz odrzucić ten zwrot?")
                .setPositiveButton("Odrzuć", (d, w) -> sendRejection("OTHER", "Decyzja handlowca"))
                .setNegativeButton("Anuluj", null)
                .show();
    }

    private void sendRejection(String code, String reason) {
        if (progressBar != null) progressBar.setVisibility(View.VISIBLE);
        RejectCustomerReturnRequest req = new RejectCustomerReturnRequest(new ReturnRejectionDto(code, reason));

        apiClient.rejectReturn(returnId, req, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    Toast.makeText(SalesReturnDetailActivity.this, "Odrzucono.", Toast.LENGTH_SHORT).show();
                    loadData();
                });
            }
            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    if (progressBar != null) progressBar.setVisibility(View.GONE);
                    Toast.makeText(SalesReturnDetailActivity.this, "Błąd: " + message, Toast.LENGTH_SHORT).show();
                });
            }
        });
    }

    private void confirmComplaint() {
        new AlertDialog.Builder(this)
                .setTitle("Reklamacja")
                .setMessage("Przekazać do reklamacji?")
                .setPositiveButton("Tak", (d, w) -> sendComplaint())
                .setNegativeButton("Nie", null)
                .show();
    }

    private void sendComplaint() {
        if (returnDetails == null) return;

        ComplaintCustomerDto customer = new ComplaintCustomerDto(
                returnDetails.getBuyerName(), "", null, null,
                new ComplaintAddressDto("", "", "")
        );
        ComplaintProductDto product = new ComplaintProductDto(returnDetails.getProductName(), "", null);

        ForwardToComplaintRequest req = new ForwardToComplaintRequest(
                returnId,
                "Decyzja handlowca",
                returnDetails.getUwagiMagazynu(),
                "Przekazano z aplikacji",
                "Handlowiec",
                customer, product
        );

        if (progressBar != null) progressBar.setVisibility(View.VISIBLE);
        apiClient.forwardToComplaints(returnId, req, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    Toast.makeText(SalesReturnDetailActivity.this, "Przekazano.", Toast.LENGTH_SHORT).show();
                    loadData();
                });
            }
            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    if (progressBar != null) progressBar.setVisibility(View.GONE);
                    Toast.makeText(SalesReturnDetailActivity.this, "Błąd: " + message, Toast.LENGTH_SHORT).show();
                });
            }
        });
    }
}