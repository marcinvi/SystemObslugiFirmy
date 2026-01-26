package com.example.ena.ui;
import android.content.Intent;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;
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
import com.example.ena.api.ReturnActionDto;
import com.example.ena.api.ReturnDecisionRequest;
import com.example.ena.api.ReturnDetailsDto;
import com.example.ena.api.RejectCustomerReturnRequest;
import com.example.ena.api.ReturnRejectionDto;
import com.example.ena.api.StatusDto;
import com.example.ena.api.ReturnForwardToWarehouseRequest;

import java.util.ArrayList;
import java.util.List;

public class SalesReturnDetailActivity extends AppCompatActivity {
    public static final String EXTRA_RETURN_ID = "return_id";
    private TextView txtTitle;
    private TextView txtCurrentStatus;
    private TextView txtProductName;
    private TextView txtBuyerName;
    private TextView txtBuyerLogin;
    private TextView txtCondition;
    private TextView txtWarehouseNotes;
    private ImageButton btnBack;
    private Button btnRefund;
    private Button btnReject;
    private Button btnComplaint;
    private ProgressBar progressBar;

    private ReturnActionAdapter actionAdapter;
    private final List<DecisionItem> decyzje = new ArrayList<>();
    private ReturnDetailsDto details;
    private int returnId;
    private static final String[] REQUIRED_DECISIONS = new String[]{
            "Na półkę",
            "Ponowna wysyłka",
            "Reklamacje",
            "Inne"
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_sales_return_detail);

        returnId = getIntent().getIntExtra(EXTRA_RETURN_ID, 0);
        txtTitle = findViewById(R.id.txtTitle);
        txtCurrentStatus = findViewById(R.id.txtCurrentStatus);
        txtProductName = findViewById(R.id.txtProductName);
        txtBuyerName = findViewById(R.id.txtBuyerName);
        txtBuyerLogin = findViewById(R.id.txtBuyerLogin);
        txtCondition = findViewById(R.id.txtCondition);
        txtWarehouseNotes = findViewById(R.id.txtWarehouseNotes);
        btnBack = findViewById(R.id.btnBack);
        btnRefund = findViewById(R.id.btnRefund);
        btnReject = findViewById(R.id.btnReject);
        btnComplaint = findViewById(R.id.btnComplaint);
        progressBar = findViewById(R.id.progressBar);

        RecyclerView listActions = findViewById(R.id.listActions);
        listActions.setLayoutManager(new LinearLayoutManager(this));
        actionAdapter = new ReturnActionAdapter();
        listActions.setAdapter(actionAdapter);

        btnBack.setOnClickListener(v -> finish());
        btnRefund.setOnClickListener(v -> openRefundPayment());
        btnReject.setOnClickListener(v -> showRejectDialog());
        btnComplaint.setOnClickListener(v -> handlePrimaryAction());

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
        txtTitle.setText(ref.isEmpty() ? "Decyzja dla zwrotu" : "Decyzja dla zwrotu: " + ref);
        txtCurrentStatus.setText(safe(details.getStatusWewnetrzny(), "Brak"));

        txtProductName.setText(safe(details.getProductName(), "Brak"));
        String buyerName = safe(details.getBuyerName(), "");
        if (buyerName.isEmpty()) {
            buyerName = safe(details.getBuyerLogin(), "Brak danych");
        }
        txtBuyerName.setText(buyerName);
        txtBuyerLogin.setText(safe(details.getBuyerLogin(), ""));

        txtCondition.setText("Stan: " + safe(details.getStanProduktuName(), "Brak"));
        txtWarehouseNotes.setText(safe(details.getUwagiMagazynu(), "Brak"));
        boolean isManual = details.isManual();
        boolean hasAllegroReturn = details.getAllegroReturnId() != null && !details.getAllegroReturnId().isEmpty();
        boolean hasOrderId = details.getOrderId() != null && !details.getOrderId().isEmpty();
        btnReject.setVisibility(!isManual && hasAllegroReturn ? View.VISIBLE : View.GONE);
        btnRefund.setVisibility(!isManual && hasOrderId ? View.VISIBLE : View.GONE);
        if (isManual) {
            btnComplaint.setText("WYŚLIJ INFORMACJE O ZWROCIE");
        } else {
            btnComplaint.setText("REKLAMACJA / INNE");
        }
    }

    private void handlePrimaryAction() {
        if (details != null && details.isManual()) {
            showManualInfoDialog();
        } else {
            showDecisionDialog();
        }
    }

    private void showManualInfoDialog() {
        EditText commentInput = new EditText(this);
        commentInput.setHint("Dodatkowy komentarz (opcjonalnie)");

        new AlertDialog.Builder(this)
                .setTitle("Wyślij informacje o zwrocie")
                .setView(commentInput)
                .setPositiveButton("Wyślij", (dialog, which) -> submitManualInfo(commentInput.getText().toString().trim()))
                .setNegativeButton("Anuluj", null)
                .show();
    }

    private void submitManualInfo(String comment) {
        btnComplaint.setEnabled(false);
        ApiClient client = new ApiClient(this);
        ReturnForwardToWarehouseRequest request = new ReturnForwardToWarehouseRequest(
                TextUtils.isEmpty(comment) ? null : comment
        );
        client.forwardToWarehouse(returnId, request, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    btnComplaint.setEnabled(true);
                    Toast.makeText(SalesReturnDetailActivity.this, "Informacja o zwrocie została wysłana.", Toast.LENGTH_SHORT).show();
                    loadActions();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    btnComplaint.setEnabled(true);
                    Toast.makeText(SalesReturnDetailActivity.this, "Błąd wysyłki informacji: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private void loadDecyzje() {
        ApiClient client = new ApiClient(this);
        client.fetchStatuses("DecyzjaHandlowca", new ApiClient.ApiCallback<List<StatusDto>>() {
            @Override
            public void onSuccess(List<StatusDto> data) {
                runOnUiThread(() -> {
                    decyzje.clear();
                    if (data != null) {
                        decyzje.addAll(buildDecisionItems(data));
                    }
                    if (!hasAllRequiredDecisions(data)) {
                        Toast.makeText(SalesReturnDetailActivity.this,
                                "Brakuje wymaganych decyzji: Na półkę, Ponowna wysyłka, Reklamacje, Inne.",
                                Toast.LENGTH_LONG).show();
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

    private void showDecisionDialog() {
        if (decyzje.isEmpty()) {
            Toast.makeText(this, "Brak dostępnych decyzji handlowca.", Toast.LENGTH_SHORT).show();
            return;
        }
        if (!hasAllRequiredDecisions(null)) {
            Toast.makeText(this, "Brak pełnej listy decyzji. Uzupełnij konfigurację statusów.", Toast.LENGTH_SHORT).show();
        }

        LinearLayout container = new LinearLayout(this);
        container.setOrientation(LinearLayout.VERTICAL);
        int padding = (int) (16 * getResources().getDisplayMetrics().density);
        container.setPadding(padding, padding, padding, padding);

        Spinner spinner = new Spinner(this);
        ArrayAdapter<String> adapter = new ArrayAdapter<>(
                this,
                android.R.layout.simple_spinner_item,
                toDecisionNames(decyzje)
        );
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(adapter);
        int preselectIndex = getDecisionIndexById(details != null ? details.getDecyzjaHandlowcaId() : null);
        if (preselectIndex >= 0) {
            spinner.setSelection(preselectIndex);
        }

        EditText commentInput = new EditText(this);
        commentInput.setHint("Komentarz (opcjonalnie)");
        if (details != null) {
            commentInput.setText(safe(details.getKomentarzHandlowca()));
        }

        container.addView(spinner);
        container.addView(commentInput);

        AlertDialog dialog = new AlertDialog.Builder(this)
                .setTitle("Decyzja handlowca")
                .setView(container)
                .setPositiveButton("Zapisz decyzję", null)
                .setNeutralButton("Przekaż do reklamacji", null)
                .setNegativeButton("Anuluj", null)
                .create();

        dialog.setOnShowListener(dialogInterface -> {
            Button saveButton = dialog.getButton(AlertDialog.BUTTON_POSITIVE);
            Button complaintsButton = dialog.getButton(AlertDialog.BUTTON_NEUTRAL);
            saveButton.setOnClickListener(v -> {
                int decisionId = getDecisionIdFromSpinner(spinner);
                if (decisionId <= 0) {
                    Toast.makeText(this, "Wybierz decyzję handlowca.", Toast.LENGTH_SHORT).show();
                    return;
                }
                String decisionName = getDecisionNameFromSpinner(spinner);
                if ("Inne".equalsIgnoreCase(decisionName) && commentInput.getText().toString().trim().isEmpty()) {
                    Toast.makeText(this, "Dla decyzji 'Inne' wymagany jest komentarz.", Toast.LENGTH_SHORT).show();
                    return;
                }
                submitDecision(decisionId, commentInput.getText().toString().trim(), false);
                dialog.dismiss();
            });
            complaintsButton.setOnClickListener(v -> {
                DecisionItem complaintDecision = getDecisionByName("Reklamacje");
                if (complaintDecision == null) {
                    Toast.makeText(this, "Brak decyzji 'Reklamacje' w konfiguracji.", Toast.LENGTH_SHORT).show();
                    return;
                }
                submitDecision(complaintDecision.id, commentInput.getText().toString().trim(), true);
                dialog.dismiss();
            });
        });

        dialog.show();
    }

    private void submitDecision(int decisionId, String comment, boolean forwardToComplaints) {
        ReturnDecisionRequest request = new ReturnDecisionRequest();
        request.decyzjaId = decisionId;
        request.komentarz = TextUtils.isEmpty(comment) ? null : comment;

        btnComplaint.setEnabled(false);
        ApiClient client = new ApiClient(this);
        client.submitDecision(returnId, request, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    btnComplaint.setEnabled(true);
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
                    btnComplaint.setEnabled(true);
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
        String comment = details != null ? safe(details.getKomentarzHandlowca()) : "";

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

        btnReject.setEnabled(false);
        ApiClient client = new ApiClient(this);
        client.rejectReturn(returnId, request, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    btnReject.setEnabled(true);
                    Toast.makeText(SalesReturnDetailActivity.this, "Zwrot został odrzucony w Allegro.", Toast.LENGTH_SHORT).show();
                    loadDetails();
                    loadActions();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    btnReject.setEnabled(true);
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

    private int getDecisionIdFromSpinner(Spinner spinner) {
        int index = spinner.getSelectedItemPosition();
        if (index < 0 || index >= decyzje.size()) {
            return 0;
        }
        return decyzje.get(index).id;
    }

    private String getDecisionNameFromSpinner(Spinner spinner) {
        int index = spinner.getSelectedItemPosition();
        if (index < 0 || index >= decyzje.size()) {
            return "";
        }
        return decyzje.get(index).displayName;
    }

    private int getDecisionIndexById(Integer decisionId) {
        if (decisionId == null) {
            return -1;
        }
        for (int i = 0; i < decyzje.size(); i++) {
            if (decyzje.get(i).id == decisionId) {
                return i;
            }
        }
        return -1;
    }

    private List<String> toDecisionNames(List<DecisionItem> items) {
        List<String> names = new ArrayList<>();
        for (DecisionItem item : items) {
            names.add(item.displayName);
        }
        return names;
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
            return new String[]{"", ""};
        }
        String trimmed = fullName.trim();
        if (trimmed.isEmpty()) {
            return new String[]{"", ""};
        }
        String[] parts = trimmed.split("\\s+", 2);
        if (parts.length == 1) {
            return new String[]{parts[0], ""};
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

    private boolean hasAllRequiredDecisions(List<StatusDto> statuses) {
        List<DecisionItem> source = statuses == null ? decyzje : buildDecisionItems(statuses);
        for (String required : REQUIRED_DECISIONS) {
            boolean found = false;
            for (DecisionItem item : source) {
                if (required.equalsIgnoreCase(item.displayName)) {
                    found = true;
                    break;
                }
            }
            if (!found) {
                return false;
            }
        }
        return true;
    }

    private DecisionItem getDecisionByName(String displayName) {
        for (DecisionItem item : decyzje) {
            if (item.displayName.equalsIgnoreCase(displayName)) {
                return item;
            }
        }
        return null;
    }

    private List<DecisionItem> buildDecisionItems(List<StatusDto> statuses) {
        List<DecisionItem> result = new ArrayList<>();
        if (statuses == null) {
            return result;
        }
        for (StatusDto status : statuses) {
            String normalized = normalizeDecisionName(status.getNazwa());
            String displayName = normalized.isEmpty() ? status.getNazwa() : normalized;
            result.add(new DecisionItem(status.getId(), displayName, status.getNazwa()));
        }
        return result;
    }

    private String normalizeDecisionName(String name) {
        if (name == null) {
            return "";
        }
        String trimmed = name.trim();
        switch (trimmed) {
            case "Na półkę":
                return "Na półkę";
            case "Ponowna wysyłka":
            case "Ponowna wysylka":
                return "Ponowna wysyłka";
            case "Reklamacje":
            case "Przekaż do reklamacji":
            case "Przekaz do reklamacji":
                return "Reklamacje";
            case "Inne":
                return "Inne";
            default:
                return "";
        }
    }

    private static class DecisionItem {
        final int id;
        final String displayName;
        final String dbName;

        DecisionItem(int id, String displayName, String dbName) {
            this.id = id;
            this.displayName = displayName;
            this.dbName = dbName;
        }
    }
}
