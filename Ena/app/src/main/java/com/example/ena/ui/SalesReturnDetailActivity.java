package com.example.ena.ui;

import android.os.Bundle;
import android.text.TextUtils;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;
import android.content.Intent;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.PairingManager;
import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.ComplaintAddressDto;
import com.example.ena.api.ComplaintCustomerDto;
import com.example.ena.api.ComplaintProductDto;
import com.example.ena.api.ForwardToComplaintRequest;
import com.example.ena.api.ReturnActionCreateRequest;
import com.example.ena.api.ReturnActionDto;
import com.example.ena.api.ReturnDecisionRequest;
import com.example.ena.api.ReturnDetailsDto;
import com.example.ena.api.ReturnForwardToWarehouseRequest;
import com.example.ena.api.RejectCustomerReturnRequest;
import com.example.ena.api.ReturnRejectionDto;
import com.example.ena.api.StatusDto;
import java.time.OffsetDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.List;

public class SalesReturnDetailActivity extends AppCompatActivity {
    public static final String EXTRA_RETURN_ID = "return_id";
    private static final DateTimeFormatter DATE_FORMAT = DateTimeFormatter.ofPattern("dd.MM.yyyy HH:mm");

    private TextView txtHeaderNumber;
    private TextView txtHeaderStatus;
    private TextView txtProductName;
    private TextView txtBuyerLogin;
    private TextView txtAccountName;
    private TextView txtOrderDate;
    private TextView txtInvoice;
    private TextView txtStanProduktu;
    private TextView txtPrzyjetyPrzez;
    private TextView txtUwagiMagazynu;
    private TextView txtDataPrzyjecia;
    private Spinner spinnerDecyzja;
    private EditText editKomentarz;
    private EditText editNoweDzialanie;
    private Button btnDodajDzialanie;
    private Button btnPodejmijDecyzje;
    private Button btnPrzekazDoReklamacji;
    private Button btnAnuluj;
    private Button btnPrzekazDoMagazynu;
    private Button btnOdrzucZwrot;
    private Button btnZwrotWplaty;
    private ProgressBar progressBar;

    private ReturnActionAdapter actionAdapter;
    private final List<StatusDto> decyzje = new ArrayList<>();
    private ReturnDetailsDto details;
    private int returnId;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_sales_return_detail);

        returnId = getIntent().getIntExtra(EXTRA_RETURN_ID, 0);
        txtHeaderNumber = findViewById(R.id.txtHeaderNumber);
        txtHeaderStatus = findViewById(R.id.txtHeaderStatus);
        txtProductName = findViewById(R.id.txtProductName);
        txtBuyerLogin = findViewById(R.id.txtBuyerLogin);
        txtAccountName = findViewById(R.id.txtAccountName);
        txtOrderDate = findViewById(R.id.txtOrderDate);
        txtInvoice = findViewById(R.id.txtInvoice);
        txtStanProduktu = findViewById(R.id.txtStanProduktu);
        txtPrzyjetyPrzez = findViewById(R.id.txtPrzyjetyPrzez);
        txtUwagiMagazynu = findViewById(R.id.txtUwagiMagazynu);
        txtDataPrzyjecia = findViewById(R.id.txtDataPrzyjecia);
        spinnerDecyzja = findViewById(R.id.spinnerDecyzja);
        editKomentarz = findViewById(R.id.editKomentarz);
        editNoweDzialanie = findViewById(R.id.editNoweDzialanie);
        btnDodajDzialanie = findViewById(R.id.btnDodajDzialanie);
        btnPodejmijDecyzje = findViewById(R.id.btnPodejmijDecyzje);
        btnPrzekazDoReklamacji = findViewById(R.id.btnPrzekazDoReklamacji);
        btnAnuluj = findViewById(R.id.btnAnuluj);
        btnPrzekazDoMagazynu = findViewById(R.id.btnPrzekazDoMagazynu);
        btnOdrzucZwrot = findViewById(R.id.btnOdrzucZwrot);
        btnZwrotWplaty = findViewById(R.id.btnZwrotWplaty);
        progressBar = findViewById(R.id.progressDetail);

        RecyclerView listActions = findViewById(R.id.listActions);
        listActions.setLayoutManager(new LinearLayoutManager(this));
        actionAdapter = new ReturnActionAdapter();
        listActions.setAdapter(actionAdapter);

        btnAnuluj.setOnClickListener(v -> finish());
        btnPodejmijDecyzje.setOnClickListener(v -> confirmSubmitDecision(false));
        btnPrzekazDoReklamacji.setOnClickListener(v -> confirmSubmitDecision(true));
        btnDodajDzialanie.setOnClickListener(v -> submitAction());
        btnPrzekazDoMagazynu.setOnClickListener(v -> promptForwardToWarehouse());
        btnOdrzucZwrot.setOnClickListener(v -> showRejectDialog());
        btnZwrotWplaty.setOnClickListener(v -> openRefundPayment());

        loadDecyzje();
        loadDetails();
        loadActions();
    }

    private void loadDetails() {
        progressBar.setVisibility(View.VISIBLE);
        ApiClient client = new ApiClient(this);
        client.fetchReturnDetails(returnId, new ApiClient.ApiCallback<ReturnDetailsDto>() {
            @Override
            public void onSuccess(ReturnDetailsDto data) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    details = data;
                    bindDetails();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    Toast.makeText(SalesReturnDetailActivity.this, "Błąd: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private void bindDetails() {
        if (details == null) {
            return;
        }
        String ref = safe(details.getReferenceNumber());
        txtHeaderNumber.setText(ref.isEmpty() ? "Decyzja dla zwrotu" : "Decyzja dla zwrotu: " + ref);
        txtHeaderStatus.setText("Status: " + safe(details.getStatusWewnetrzny(), "Brak"));

        txtProductName.setText(safe(details.getProductName(), "Brak"));
        txtBuyerLogin.setText(safe(details.getBuyerLogin(), "Brak"));
        String account = safe(details.getAllegroAccountName(), "Brak");
        if (details.isManual()) {
            account = "Ręczny";
        }
        txtAccountName.setText(account);
        txtOrderDate.setText(formatDate(details.getCreatedAt()));
        txtInvoice.setText(safe(details.getInvoiceNumber(), "Brak"));

        txtStanProduktu.setText(safe(details.getStanProduktuName(), "Brak"));
        txtPrzyjetyPrzez.setText(safe(details.getPrzyjetyPrzezName(), "Brak"));
        txtUwagiMagazynu.setText(safe(details.getUwagiMagazynu(), "Brak"));
        txtDataPrzyjecia.setText(formatDate(details.getDataPrzyjecia()));

        editKomentarz.setText(safe(details.getKomentarzHandlowca()));
        preselectDecision(details.getDecyzjaHandlowcaId());

        btnPrzekazDoMagazynu.setVisibility(details.isManual() ? View.VISIBLE : View.GONE);
        boolean hasAllegroReturn = details.getAllegroReturnId() != null && !details.getAllegroReturnId().isEmpty();
        boolean hasOrderId = details.getOrderId() != null && !details.getOrderId().isEmpty();
        btnOdrzucZwrot.setVisibility(hasAllegroReturn ? View.VISIBLE : View.GONE);
        btnZwrotWplaty.setVisibility(hasOrderId ? View.VISIBLE : View.GONE);
    }

    private void loadDecyzje() {
        ApiClient client = new ApiClient(this);
        client.fetchStatuses("DecyzjaHandlowca", new ApiClient.ApiCallback<List<StatusDto>>() {
            @Override
            public void onSuccess(List<StatusDto> data) {
                runOnUiThread(() -> {
                    decyzje.clear();
                    if (data != null) {
                        decyzje.addAll(data);
                    }
                    ArrayAdapter<String> adapter = new ArrayAdapter<>(
                            SalesReturnDetailActivity.this,
                            android.R.layout.simple_spinner_item,
                            toStatusNames(decyzje)
                    );
                    adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
                    spinnerDecyzja.setAdapter(adapter);
                    if (details != null) {
                        preselectDecision(details.getDecyzjaHandlowcaId());
                    }
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> Toast.makeText(SalesReturnDetailActivity.this, "Błąd statusów: " + message, Toast.LENGTH_LONG).show());
            }
        });
    }

    private void loadActions() {
        ApiClient client = new ApiClient(this);
        client.fetchReturnActions(returnId, new ApiClient.ApiCallback<List<ReturnActionDto>>() {
            @Override
            public void onSuccess(List<ReturnActionDto> data) {
                runOnUiThread(() -> actionAdapter.setItems(data));
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> Toast.makeText(SalesReturnDetailActivity.this, "Błąd historii: " + message, Toast.LENGTH_LONG).show());
            }
        });
    }

    private void submitAction() {
        String text = editNoweDzialanie.getText().toString().trim();
        if (text.isEmpty()) {
            Toast.makeText(this, "Treść działania nie może być pusta.", Toast.LENGTH_SHORT).show();
            return;
        }
        btnDodajDzialanie.setEnabled(false);
        ApiClient client = new ApiClient(this);
        client.addReturnAction(returnId, new ReturnActionCreateRequest(text), new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    btnDodajDzialanie.setEnabled(true);
                    editNoweDzialanie.setText("");
                    loadActions();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    btnDodajDzialanie.setEnabled(true);
                    Toast.makeText(SalesReturnDetailActivity.this, "Błąd zapisu działania: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private void confirmSubmitDecision(boolean forwardToComplaints) {
        if (getSelectedDecisionId() <= 0) {
            Toast.makeText(this, "Wybierz decyzję handlowca.", Toast.LENGTH_SHORT).show();
            return;
        }
        if (forwardToComplaints && !isComplaintsDecisionSelected()) {
            Toast.makeText(this, "Wybierz decyzję 'Przekaż do reklamacji'.", Toast.LENGTH_SHORT).show();
            return;
        }
        new AlertDialog.Builder(this)
                .setTitle("Zatwierdź decyzję")
                .setMessage(forwardToComplaints
                        ? "Czy na pewno chcesz zapisać decyzję i przekazać zwrot do reklamacji?"
                        : "Czy na pewno chcesz zapisać decyzję dla tego zwrotu?")
                .setPositiveButton("Zapisz", (dialog, which) -> submitDecision(forwardToComplaints))
                .setNegativeButton("Anuluj", null)
                .show();
    }

    private void submitDecision(boolean forwardToComplaints) {
        int decisionId = getSelectedDecisionId();
        if (decisionId <= 0) {
            Toast.makeText(this, "Wybierz decyzję handlowca.", Toast.LENGTH_SHORT).show();
            return;
        }
        String comment = editKomentarz.getText().toString().trim();
        ReturnDecisionRequest request = new ReturnDecisionRequest();
        request.decyzjaId = decisionId;
        request.komentarz = TextUtils.isEmpty(comment) ? null : comment;

        btnPodejmijDecyzje.setEnabled(false);
        btnPrzekazDoReklamacji.setEnabled(false);
        ApiClient client = new ApiClient(this);
        client.submitDecision(returnId, request, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    btnPodejmijDecyzje.setEnabled(true);
                    btnPrzekazDoReklamacji.setEnabled(true);
                    if (forwardToComplaints) {
                        forwardToComplaints();
                    } else {
                        Toast.makeText(SalesReturnDetailActivity.this, "Decyzja została zapisana.", Toast.LENGTH_SHORT).show();
                    }
                    loadDetails();
                    loadActions();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    btnPodejmijDecyzje.setEnabled(true);
                    btnPrzekazDoReklamacji.setEnabled(true);
                    Toast.makeText(SalesReturnDetailActivity.this, "Błąd zapisu decyzji: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private void forwardToComplaints() {
        if (details == null) {
            return;
        }
        ForwardToComplaintRequest request = buildComplaintRequest();
        ApiClient client = new ApiClient(this);
        client.forwardToComplaints(returnId, request, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> Toast.makeText(SalesReturnDetailActivity.this, "Przekazano do reklamacji.", Toast.LENGTH_LONG).show());
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> Toast.makeText(SalesReturnDetailActivity.this, "Nie udało się przekazać do reklamacji: " + message, Toast.LENGTH_LONG).show());
            }
        });
    }

    private ForwardToComplaintRequest buildComplaintRequest() {
        String buyerName = safe(details.getBuyerName(), "");
        String[] nameParts = splitName(buyerName);
        String comment = editKomentarz.getText().toString().trim();

        ComplaintAddressDto address = new ComplaintAddressDto(
                safe(details.getBuyerAddress(), details.getBuyerAddressRaw()),
                null,
                null
        );
        ComplaintCustomerDto customer = new ComplaintCustomerDto(
                nameParts[0],
                nameParts[1],
                null,
                emptyToNull(details.getBuyerPhone()),
                address
        );
        ComplaintProductDto product = new ComplaintProductDto(
                safe(details.getProductName(), "Nieznany produkt"),
                emptyToNull(details.getInvoiceNumber()),
                null
        );
        String przekazal = PairingManager.getPairedUser(this);
        if (TextUtils.isEmpty(przekazal)) {
            przekazal = "Handlowiec";
        }
        return new ForwardToComplaintRequest(
                returnId,
                emptyToNull(details.getReason()),
                emptyToNull(details.getUwagiMagazynu()),
                TextUtils.isEmpty(comment) ? null : comment,
                przekazal,
                customer,
                product
        );
    }

    private void promptForwardToWarehouse() {
        if (details == null || !details.isManual()) {
            return;
        }
        EditText input = new EditText(this);
        input.setHint("Komentarz dla magazynu (opcjonalnie)");
        new AlertDialog.Builder(this)
                .setTitle("Przekaż do magazynu")
                .setView(input)
                .setPositiveButton("Przekaż", (dialog, which) -> forwardToWarehouse(input.getText().toString().trim()))
                .setNegativeButton("Anuluj", null)
                .show();
    }

    private void forwardToWarehouse(String comment) {
        ReturnForwardToWarehouseRequest request = new ReturnForwardToWarehouseRequest(comment.isEmpty() ? null : comment);
        ApiClient client = new ApiClient(this);
        client.forwardToWarehouse(returnId, request, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    Toast.makeText(SalesReturnDetailActivity.this, "Przekazano do magazynu.", Toast.LENGTH_SHORT).show();
                    loadDetails();
                    loadActions();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> Toast.makeText(SalesReturnDetailActivity.this, "Błąd przekazania: " + message, Toast.LENGTH_LONG).show());
            }
        });
    }

    private static class RejectionReasonItem {
        final String code;
        final String description;

        RejectionReasonItem(String code, String description) {
            this.code = code;
            this.description = description;
        }

        @Override
        public String toString() {
            return description;
        }
    }

    private void showRejectDialog() {
        if (details == null || details.getAllegroReturnId() == null || details.getAllegroReturnId().isEmpty()) {
            Toast.makeText(this, "Brak identyfikatora zwrotu Allegro.", Toast.LENGTH_SHORT).show();
            return;
        }

        View view = getLayoutInflater().inflate(R.layout.dialog_reject_return, null);
        Spinner spinner = view.findViewById(R.id.spinnerRejectionReason);
        EditText editReason = view.findViewById(R.id.editRejectionReason);

        List<RejectionReasonItem> reasons = new ArrayList<>();
        reasons.add(new RejectionReasonItem("BUYER_FAULT", "Wina kupującego"));
        reasons.add(new RejectionReasonItem("SENT_AFTER_TIME", "Wysłano po terminie"));
        reasons.add(new RejectionReasonItem("WRONG_ADDRESS", "Wysłano na zły adres"));
        reasons.add(new RejectionReasonItem("RETURN_NOT_REGISTERED_IN_SYSTEM", "Zwrot niezarejestrowany w systemie"));
        reasons.add(new RejectionReasonItem("OTHER", "Inny powód (wymaga uzasadnienia)"));

        ArrayAdapter<RejectionReasonItem> adapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, reasons);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(adapter);

        new AlertDialog.Builder(this)
                .setTitle("Odrzuć zwrot")
                .setView(view)
                .setPositiveButton("Odrzuć", (dialog, which) -> submitRejection(spinner, editReason))
                .setNegativeButton("Anuluj", null)
                .show();
    }

    private void submitRejection(Spinner spinner, EditText editReason) {
        if (spinner.getSelectedItem() == null) {
            Toast.makeText(this, "Wybierz powód odrzucenia.", Toast.LENGTH_SHORT).show();
            return;
        }

        RejectionReasonItem selected = (RejectionReasonItem) spinner.getSelectedItem();
        String reasonText = editReason.getText().toString().trim();
        RejectCustomerReturnRequest request = new RejectCustomerReturnRequest(
                new ReturnRejectionDto(selected.code, reasonText.isEmpty() ? null : reasonText)
        );

        btnOdrzucZwrot.setEnabled(false);
        ApiClient client = new ApiClient(this);
        client.rejectReturn(returnId, request, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    btnOdrzucZwrot.setEnabled(true);
                    Toast.makeText(SalesReturnDetailActivity.this, "Zwrot został odrzucony w Allegro.", Toast.LENGTH_SHORT).show();
                    loadDetails();
                    loadActions();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    btnOdrzucZwrot.setEnabled(true);
                    Toast.makeText(SalesReturnDetailActivity.this, "Błąd odrzucenia zwrotu: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private void openRefundPayment() {
        Intent intent = new Intent(this, RefundPaymentActivity.class);
        intent.putExtra(RefundPaymentActivity.EXTRA_RETURN_ID, returnId);
        startActivity(intent);
    }

    private int getSelectedDecisionId() {
        int index = spinnerDecyzja.getSelectedItemPosition();
        if (index < 0 || index >= decyzje.size()) {
            return 0;
        }
        return decyzje.get(index).getId();
    }

    private String getSelectedDecisionName() {
        int index = spinnerDecyzja.getSelectedItemPosition();
        if (index < 0 || index >= decyzje.size()) {
            return "";
        }
        return decyzje.get(index).getNazwa();
    }

    private void preselectDecision(Integer decisionId) {
        if (decisionId == null) {
            return;
        }
        for (int i = 0; i < decyzje.size(); i++) {
            if (decyzje.get(i).getId() == decisionId) {
                spinnerDecyzja.setSelection(i);
                return;
            }
        }
    }

    private List<String> toStatusNames(List<StatusDto> statuses) {
        List<String> names = new ArrayList<>();
        for (StatusDto status : statuses) {
            names.add(status.getNazwa());
        }
        return names;
    }

    private String formatDate(OffsetDateTime date) {
        if (date == null) {
            return "Brak";
        }
        return DATE_FORMAT.format(date);
    }

    private String safe(String value) {
        return value == null ? "" : value;
    }

    private String safe(String value, String fallback) {
        if (value == null || value.trim().isEmpty()) {
            return fallback;
        }
        return value;
    }

    private String[] splitName(String fullName) {
        if (fullName == null) {
            return new String[] { "", "" };
        }
        String trimmed = fullName.trim();
        if (trimmed.isEmpty()) {
            return new String[] { "", "" };
        }
        String[] parts = trimmed.split("\\s+", 2);
        if (parts.length == 1) {
            return new String[] { parts[0], "" };
        }
        return parts;
    }

    private String emptyToNull(String value) {
        if (value == null) {
            return null;
        }
        String trimmed = value.trim();
        return trimmed.isEmpty() ? null : trimmed;
    }

    private boolean isComplaintsDecisionSelected() {
        String decisionName = getSelectedDecisionName();
        return "Przekaż do reklamacji".equalsIgnoreCase(decisionName);
    }
}
