package com.example.ena.ui;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
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

    // Elementy interfejsu (muszą pasować do XML!)
    private TextView txtTitle, txtCurrentStatus;
    private TextView txtProductName, txtBuyerName, txtBuyerLogin;
    private TextView txtCondition, txtWarehouseNotes;

    private Button btnRefund, btnReject, btnComplaint;
    private ImageButton btnBack;

    private RecyclerView listActions;
    private ProgressBar progressBar;

    private ApiClient apiClient;
    private ReturnActionAdapter actionAdapter;

    private int returnId;
    private ReturnDetailsDto returnDetails;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_sales_return_detail);

        // Pobranie ID zwrotu - jeśli 0, to błąd
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
        // Mapowanie widoków
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

        // Konfiguracja listy historii
        listActions.setLayoutManager(new LinearLayoutManager(this));
        actionAdapter = new ReturnActionAdapter();
        listActions.setAdapter(actionAdapter);

        // Obsługa kliknięć
        btnBack.setOnClickListener(v -> finish());

        btnRefund.setOnClickListener(v -> {
            Intent intent = new Intent(this, RefundPaymentActivity.class);
            intent.putExtra(RefundPaymentActivity.EXTRA_RETURN_ID, returnId);
            startActivity(intent);
        });

        btnReject.setOnClickListener(v -> showRejectDialog());
        btnComplaint.setOnClickListener(v -> confirmComplaint());
    }

    private void loadData() {
        if (progressBar != null) progressBar.setVisibility(View.VISIBLE);

        // 1. Pobranie szczegółów zwrotu
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
                    Toast.makeText(SalesReturnDetailActivity.this, "Błąd: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });

        // 2. Pobranie historii akcji
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
                // Błąd historii nie blokuje UI
            }
        });
    }

    private void bindData(ReturnDetailsDto data) {
        if (data == null) return;

        String ref = data.getReferenceNumber();
        if (ref == null) ref = "ID: " + data.getId();

        // Ustawianie tekstów z bezpiecznymi null-checkami
        if (txtTitle != null) txtTitle.setText(ref);
        if (txtCurrentStatus != null) txtCurrentStatus.setText(data.getStatusWewnetrzny());
        if (txtProductName != null) txtProductName.setText(data.getProductName());
        if (txtBuyerName != null) txtBuyerName.setText(data.getBuyerName());
        if (txtBuyerLogin != null) txtBuyerLogin.setText(data.getBuyerLogin());

        if (txtCondition != null) txtCondition.setText(
                data.getStanProduktuName() != null ? data.getStanProduktuName() : "Brak oceny"
        );
        if (txtWarehouseNotes != null) txtWarehouseNotes.setText(
                data.getUwagiMagazynu() != null ? data.getUwagiMagazynu() : "Brak uwag"
        );

        // Logika blokowania przycisków (np. jeśli zwrot zakończony)
        boolean isActive = !"Zakończony".equalsIgnoreCase(data.getStatusWewnetrzny())
                && !"Odrzucony".equalsIgnoreCase(data.getStatusWewnetrzny());

        if (btnRefund != null) btnRefund.setEnabled(isActive);
        if (btnReject != null) btnReject.setEnabled(isActive);
        if (btnComplaint != null) btnComplaint.setEnabled(isActive);
    }

    private void showRejectDialog() {
        new AlertDialog.Builder(this)
                .setTitle("Odrzuć Zwrot")
                .setMessage("Czy na pewno chcesz odrzucić ten zwrot?\nKlient nie otrzyma zwrotu pieniędzy.")
                .setPositiveButton("Odrzuć", (dialog, which) -> sendRejection())
                .setNegativeButton("Anuluj", null)
                .show();
    }

    private void sendRejection() {
        if (progressBar != null) progressBar.setVisibility(View.VISIBLE);

        // Domyślny powód: Decyzja handlowca (OTHER)
        RejectCustomerReturnRequest req = new RejectCustomerReturnRequest(
                new ReturnRejectionDto("OTHER", "Decyzja Handlowca")
        );

        apiClient.rejectReturn(returnId, req, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    Toast.makeText(SalesReturnDetailActivity.this, "Zwrot został odrzucony.", Toast.LENGTH_SHORT).show();
                    loadData(); // Odśwież widok
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
                .setTitle("Przekaż do Reklamacji")
                .setMessage("Czy przekazać ten zwrot do działu technicznego/reklamacji?")
                .setPositiveButton("Przekaż", (d, w) -> sendComplaint())
                .setNegativeButton("Anuluj", null)
                .show();
    }

    private void sendComplaint() {
        if (returnDetails == null) return;
        if (progressBar != null) progressBar.setVisibility(View.VISIBLE);

        // Tworzenie obiektów wymaganych przez API
        ComplaintCustomerDto customer = new ComplaintCustomerDto(
                returnDetails.getBuyerName(), "", null, null,
                new ComplaintAddressDto("", "", "")
        );
        ComplaintProductDto product = new ComplaintProductDto(returnDetails.getProductName(), "", null);

        ForwardToComplaintRequest req = new ForwardToComplaintRequest(
                returnId,
                "Przekazano przez Handlowca (Mobile)",
                returnDetails.getUwagiMagazynu(),
                "Decyzja w aplikacji mobilnej",
                "Handlowiec",
                customer, product
        );

        apiClient.forwardToComplaints(returnId, req, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    Toast.makeText(SalesReturnDetailActivity.this, "Przekazano do reklamacji.", Toast.LENGTH_SHORT).show();
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