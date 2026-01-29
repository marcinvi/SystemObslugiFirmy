package com.example.ena.ui;

import android.os.Bundle;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
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
import com.example.ena.api.ForwardToComplaintRequest;
import com.example.ena.api.ReturnActionCreateRequest;
import com.example.ena.api.ReturnActionDto;
import com.example.ena.api.ReturnDetailsDto;
import com.example.ena.api.ReturnForwardToSalesRequest;
import com.example.ena.api.ComplaintAddressDto;
import com.example.ena.api.ComplaintCustomerDto;
import com.example.ena.api.ComplaintProductDto;
import com.example.ena.api.StatusDto;
import com.example.ena.PairingManager;
import java.util.ArrayList;
import java.util.List;

public class ReturnDetailActivity extends AppCompatActivity {
    public static final String EXTRA_RETURN_ID = "return_id";
    public static final String EXTRA_READ_ONLY = "read_only";

    private TextView txtHeaderNumber;
    private TextView txtHeaderStatus;
    private TextView txtProductName;
    private TextView txtOfferId;
    private TextView txtQuantity;
    private TextView txtReason;
    private TextView txtWaybill;
    private TextView txtCarrier;
    private TextView txtBuyerName;
    private TextView txtBuyerAddress;
    private TextView txtBuyerPhone;
    private TextView txtAssignedSales;
    private Button btnShowAddresses;
    private EditText editUwagiMagazynu;
    private android.widget.Spinner spinnerStanProduktu;
    private Button btnForwardToSales;
    private Button btnForwardToComplaints;
    private Button btnCompleteReturn; // ✅ NOWY PRZYCISK
    private Button btnCancel;
    private Button btnCloseReturn;
    private Button btnAddResendInfo;
    private EditText editWarehouseAction;
    private Button btnWarehouseAddAction;
    private ProgressBar progressBar;
    private int returnId;
    private ReturnDetailsDto details;
    private final List<StatusDto> stanProduktuStatuses = new ArrayList<>();
    private ReturnActionAdapter actionAdapter;
    private boolean isReadOnly;
    private DecisionType decisionType = DecisionType.NONE;

    private enum DecisionType {
        NONE,
        RESEND,
        COMPLAINTS,
        STOCK,
        OTHER
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_return_detail);

        returnId = getIntent().getIntExtra(EXTRA_RETURN_ID, 0);
        isReadOnly = getIntent().getBooleanExtra(EXTRA_READ_ONLY, false);

        ImageButton btnBack = findViewById(R.id.btnBack);
        if (btnBack != null) {
            btnBack.setOnClickListener(v -> finish());
        }

        txtHeaderNumber = findViewById(R.id.txtHeaderNumber);
        txtHeaderStatus = findViewById(R.id.txtHeaderStatus);
        txtProductName = findViewById(R.id.txtProductName);
        txtOfferId = findViewById(R.id.txtOfferId);
        txtQuantity = findViewById(R.id.txtQuantity);
        txtReason = findViewById(R.id.txtReason);
        txtWaybill = findViewById(R.id.txtWaybill);
        txtCarrier = findViewById(R.id.txtCarrier);
        txtBuyerName = findViewById(R.id.txtBuyerName);
        txtBuyerAddress = findViewById(R.id.txtBuyerAddress);
        txtBuyerPhone = findViewById(R.id.txtBuyerPhone);
        txtAssignedSales = findViewById(R.id.txtAssignedSales);

        btnShowAddresses = findViewById(R.id.btnShowAddresses);
        editUwagiMagazynu = findViewById(R.id.editUwagiMagazynu);
        spinnerStanProduktu = findViewById(R.id.spinnerStanProduktu);

        btnForwardToSales = findViewById(R.id.btnForwardToSales);
        btnForwardToComplaints = findViewById(R.id.btnForwardToComplaints);
        btnCompleteReturn = findViewById(R.id.btnCompleteReturn); // ✅ INICJALIZACJA

        btnCancel = findViewById(R.id.btnCancel);
        btnCloseReturn = findViewById(R.id.btnCloseReturn);
        if (btnCloseReturn != null) {
            btnCloseReturn.setVisibility(View.GONE);
        }

        btnAddResendInfo = findViewById(R.id.btnAddResendInfo);
        editWarehouseAction = findViewById(R.id.editWarehouseAction);
        btnWarehouseAddAction = findViewById(R.id.btnWarehouseAddAction);
        progressBar = findViewById(R.id.progressDetail);

        RecyclerView listActions = findViewById(R.id.listWarehouseActions);
        listActions.setLayoutManager(new LinearLayoutManager(this));
        actionAdapter = new ReturnActionAdapter();
        listActions.setAdapter(actionAdapter);

        btnShowAddresses.setOnClickListener(v -> showAddressesDialog());
        btnForwardToSales.setOnClickListener(v -> showForwardDialog());
        btnForwardToComplaints.setOnClickListener(v -> showForwardToComplaintsDialog());
        btnWarehouseAddAction.setOnClickListener(v -> submitWarehouseAction());

        // ✅ LISTENER DLA NOWEGO PRZYCISKU
        if (btnCompleteReturn != null) {
            btnCompleteReturn.setOnClickListener(v -> handleCompleteAction());
        }

        if (btnCancel != null) btnCancel.setVisibility(View.GONE);

        if (btnAddResendInfo != null) {
            btnAddResendInfo.setOnClickListener(v -> showResendInfoDialog());
        }

        if (isReadOnly) {
            btnForwardToSales.setVisibility(View.GONE);
        }

        loadStatuses();
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
                    bindDetails(data);
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    Toast.makeText(ReturnDetailActivity.this, "Błąd: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private void bindDetails(ReturnDetailsDto data) {
        if (data == null) {
            return;
        }
        String referenceNumber = safe(data.getReferenceNumber());
        txtHeaderNumber.setText(referenceNumber.isEmpty() ? "Zwrot" : "Zwrot #" + referenceNumber);
        txtHeaderStatus.setText(safe(data.getStatusWewnetrzny(), "Brak statusu"));

        txtProductName.setText(safe(data.getProductName(), "Brak"));
        txtOfferId.setText(safe(data.getOfferId(), "Brak"));
        txtQuantity.setText("Ilość: " + valueOrPlaceholder(data.getQuantity()));
        String reasonText = formatReason(data.getReason());
        if (reasonText.isEmpty()) {
            reasonText = data.isManual() ? "Brak (zwrot ręczny)" : "Brak";
        }
        txtReason.setText(reasonText);

        txtWaybill.setText(safe(data.getWaybill(), "Brak"));
        txtCarrier.setText(safe(data.getCarrierName(), "Brak"));

        String buyerName = safe(data.getBuyerName(), "");
        if (buyerName.isEmpty()) {
            buyerName = safe(data.getBuyerLogin(), "Brak danych");
        } else if (!isBlank(data.getBuyerLogin())) {
            buyerName = buyerName + " (" + data.getBuyerLogin() + ")";
        }
        txtBuyerName.setText(buyerName);
        txtBuyerAddress.setText(safe(data.getBuyerAddress(), "Brak adresu"));
        txtBuyerPhone.setText(safe(data.getBuyerPhone(), "Brak"));
        txtAssignedSales.setText(buildAssignedSalesLabel(data));

        editUwagiMagazynu.setText(safe(data.getUwagiMagazynu()));
        preselectStanProduktu(data.getStanProduktuId());

        String statusWewnetrzny = safe(data.getStatusWewnetrzny());
        boolean isPoDecyzji = isAfterDecisionStatus(statusWewnetrzny);
        boolean isZakonczony = "Zakończony".equalsIgnoreCase(statusWewnetrzny) || "Archiwalny".equalsIgnoreCase(statusWewnetrzny);
        btnForwardToSales.setEnabled(!isZakonczony);
        btnForwardToComplaints.setEnabled(!isZakonczony);

        if (btnAddResendInfo != null) {
            btnAddResendInfo.setVisibility(View.GONE);
        }

        // ✅ LOGIKA WYŚWIETLANIA PRZYCISKÓW
        if (isPoDecyzji) {
            String decyzja = safe(data.getDecyzjaHandlowcaName());
            decisionType = resolveDecisionType(decyzja);

            btnForwardToSales.setVisibility(View.GONE);
            btnForwardToComplaints.setVisibility(View.GONE);
            if (btnCompleteReturn != null) {
                btnCompleteReturn.setVisibility(View.GONE);
            }

            switch (decisionType) {
                case RESEND:
                    if (btnCompleteReturn != null) {
                        btnCompleteReturn.setText("Wysłano ponownie");
                        btnCompleteReturn.setVisibility(isReadOnly ? View.GONE : View.VISIBLE);
                    }
                    if (btnAddResendInfo != null) {
                        btnAddResendInfo.setVisibility(View.GONE);
                    }
                    break;
                case COMPLAINTS:
                    btnForwardToComplaints.setText("Przekazano fizycznie na dział reklamacji");
                    btnForwardToComplaints.setVisibility(isReadOnly ? View.GONE : View.VISIBLE);
                    break;
                case STOCK:
                    if (btnCompleteReturn != null) {
                        btnCompleteReturn.setText("Wróciło na stan");
                        btnCompleteReturn.setVisibility(isReadOnly ? View.GONE : View.VISIBLE);
                    }
                    break;
                case OTHER:
                default:
                    if (btnCompleteReturn != null) {
                        btnCompleteReturn.setText("Zwrot zakończony zgodnie z decyzją");
                        btnCompleteReturn.setVisibility(isReadOnly ? View.GONE : View.VISIBLE);
                    }
                    break;
            }
        } else {
            decisionType = DecisionType.NONE;
            btnForwardToSales.setVisibility(isReadOnly || isZakonczony ? View.GONE : View.VISIBLE);
            btnForwardToComplaints.setText("Reklamacja / inne");
            btnForwardToComplaints.setVisibility(isZakonczony ? View.GONE : View.VISIBLE);
            if (btnCompleteReturn != null) {
                btnCompleteReturn.setVisibility(View.GONE);
            }
        }

        // Stary przycisk "Zamknij" zawsze ukryty (używamy nowego)
        if (btnCloseReturn != null) {
            btnCloseReturn.setVisibility(View.GONE);
        }
    }

    private void loadStatuses() {
        ApiClient client = new ApiClient(this);
        client.fetchStatuses("StanProduktu", new ApiClient.ApiCallback<List<StatusDto>>() {
            @Override
            public void onSuccess(List<StatusDto> data) {
                runOnUiThread(() -> {
                    stanProduktuStatuses.clear();
                    if (data != null) {
                        stanProduktuStatuses.addAll(data);
                    }
                    ArrayAdapter<String> adapter = new ArrayAdapter<>(
                            ReturnDetailActivity.this,
                            android.R.layout.simple_spinner_item,
                            toStatusNames(stanProduktuStatuses)
                    );
                    adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
                    spinnerStanProduktu.setAdapter(adapter);
                    if (details != null) {
                        preselectStanProduktu(details.getStanProduktuId());
                    }
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> Toast.makeText(ReturnDetailActivity.this, "Błąd statusów: " + message, Toast.LENGTH_LONG).show());
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
                runOnUiThread(() -> Toast.makeText(ReturnDetailActivity.this, "Błąd historii: " + message, Toast.LENGTH_LONG).show());
            }
        });
    }

    private void showForwardDialog() {
        if (details == null) {
            Toast.makeText(this, "Brak danych zwrotu", Toast.LENGTH_SHORT).show();
            return;
        }
        if (details.isManual()) {
            Toast.makeText(this, "Zwrot ręczny - skontaktuj się z handlowcem bezpośrednio.", Toast.LENGTH_LONG).show();
            return;
        }
        new AlertDialog.Builder(this)
                .setTitle("Przekaż do handlowca")
                .setMessage("Czy na pewno chcesz zapisać zmiany i przekazać zwrot do handlowca?")
                .setPositiveButton("Przekaż", (dialog, which) -> submitForwardToSales())
                .setNegativeButton("Anuluj", null)
                .show();
    }

    private void showForwardToComplaintsDialog() {
        if (details == null) {
            Toast.makeText(this, "Brak danych zwrotu", Toast.LENGTH_SHORT).show();
            return;
        }
        boolean isPoDecyzji = isAfterDecisionStatus(safe(details.getStatusWewnetrzny()));
        String title = isPoDecyzji ? "Przekazanie do reklamacji" : "Przekaż do reklamacji";
        String message = isPoDecyzji
                ? "Czy potwierdzasz fizyczne przekazanie zwrotu do działu reklamacji?"
                : "Czy na pewno chcesz przekazać zwrot do działu reklamacji?";
        new AlertDialog.Builder(this)
                .setTitle(title)
                .setMessage(message)
                .setPositiveButton("Przekaż", (dialog, which) -> submitForwardToComplaints())
                .setNegativeButton("Anuluj", null)
                .show();
    }

    // ✅ NOWA METODA - DIALOG POTWIERDZENIA
    private void handleCompleteAction() {
        if (decisionType == DecisionType.RESEND) {
            showResendCompleteDialog();
            return;
        }

        String actionText = resolveCompletionActionText();
        showDecisionCompleteDialog("Zakończ zwrot",
                "Czy na pewno chcesz zakończyć ten zwrot?\n\nZwrot zostanie oznaczony jako \"Zakończony\".",
                actionText);
    }

    private void submitForwardToSales() {
        int stanId = getSelectedStatusId();
        if (stanId <= 0) {
            Toast.makeText(this, "Przed przekazaniem wybierz stan produktu.", Toast.LENGTH_SHORT).show();
            return;
        }
        String uwagi = editUwagiMagazynu.getText().toString().trim();
        ReturnForwardToSalesRequest req = new ReturnForwardToSalesRequest(
                stanId,
                uwagi.isEmpty() ? null : uwagi
        );
        btnForwardToSales.setEnabled(false);
        ApiClient client = new ApiClient(this);
        client.forwardToSales(returnId, req, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    btnForwardToSales.setEnabled(true);
                    Toast.makeText(ReturnDetailActivity.this, "Przekazano do handlowca", Toast.LENGTH_SHORT).show();
                    finish();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    btnForwardToSales.setEnabled(true);
                    Toast.makeText(ReturnDetailActivity.this, "Błąd: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private void submitForwardToComplaints() {
        btnForwardToComplaints.setEnabled(false);
        ApiClient client = new ApiClient(this);
        ForwardToComplaintRequest request = buildComplaintRequest();
        client.forwardToComplaints(returnId, request, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    btnForwardToComplaints.setEnabled(true);
                    Toast.makeText(ReturnDetailActivity.this, "Przekazano do reklamacji.", Toast.LENGTH_SHORT).show();
                    finish();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    btnForwardToComplaints.setEnabled(true);
                    Toast.makeText(ReturnDetailActivity.this, "Błąd przekazania: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private void submitWarehouseAction() {
        String text = editWarehouseAction.getText().toString().trim();
        if (text.isEmpty()) {
            Toast.makeText(this, "Treść działania nie może być pusta.", Toast.LENGTH_SHORT).show();
            return;
        }
        btnWarehouseAddAction.setEnabled(false);
        ApiClient client = new ApiClient(this);
        client.addReturnAction(returnId, new ReturnActionCreateRequest(text), new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    btnWarehouseAddAction.setEnabled(true);
                    editWarehouseAction.setText("");
                    loadActions();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    btnWarehouseAddAction.setEnabled(true);
                    Toast.makeText(ReturnDetailActivity.this, "Błąd zapisu działania: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private void showResendInfoDialog() {
        if (decisionType != DecisionType.RESEND) {
            Toast.makeText(this, "Brak decyzji o ponownej wysyłce.", Toast.LENGTH_SHORT).show();
            return;
        }
        showResendCompleteDialog();
    }

    private void showResendCompleteDialog() {
        View view = getLayoutInflater().inflate(R.layout.dialog_resend_info, null);
        EditText editDate = view.findViewById(R.id.editResendDate);
        android.widget.Spinner spinnerCarrier = view.findViewById(R.id.spinnerResendCarrier);
        EditText editCarrierOther = view.findViewById(R.id.editResendCarrierOther);
        EditText editWaybill = view.findViewById(R.id.editResendWaybill);
        editDate.setText(DateUtils.today());
        ArrayAdapter<CharSequence> carrierAdapter = ArrayAdapter.createFromResource(
                this,
                R.array.resend_carrier_options,
                android.R.layout.simple_spinner_item
        );
        carrierAdapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinnerCarrier.setAdapter(carrierAdapter);
        spinnerCarrier.setOnItemSelectedListener(new android.widget.AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(android.widget.AdapterView<?> parent, View view, int position, long id) {
                String value = String.valueOf(parent.getItemAtPosition(position));
                if ("Inny".equalsIgnoreCase(value)) {
                    editCarrierOther.setVisibility(View.VISIBLE);
                } else {
                    editCarrierOther.setVisibility(View.GONE);
                    editCarrierOther.setText("");
                }
            }

            @Override
            public void onNothingSelected(android.widget.AdapterView<?> parent) {
                editCarrierOther.setVisibility(View.GONE);
                editCarrierOther.setText("");
            }
        });
        new AlertDialog.Builder(this)
                .setTitle("Ponowna wysyłka")
                .setView(view)
                .setPositiveButton("Zatwierdź", (dialog, which) -> submitResendInfo(
                        editDate.getText().toString().trim(),
                        resolveCarrierValue(spinnerCarrier, editCarrierOther),
                        editWaybill.getText().toString().trim()
                ))
                .setNegativeButton("Anuluj", null)
                .show();
    }

    private void submitResendInfo(String date, String carrier, String waybill) {
        if (carrier.isEmpty()) {
            Toast.makeText(this, "Uzupełnij przewoźnika.", Toast.LENGTH_SHORT).show();
            return;
        }
        if (waybill.isEmpty()) {
            Toast.makeText(this, "Uzupełnij numer listu przewozowego.", Toast.LENGTH_SHORT).show();
            return;
        }
        String resendDate = date.isEmpty() ? DateUtils.today() : date;
        String actionText = "Wysłano ponownie dnia " + resendDate + " kurierem " + carrier + " listem przewozowym numer " + waybill + ".";
        completeReturnWithAction(actionText);
    }

    private void showDecisionCompleteDialog(String title, String message, String actionText) {
        new AlertDialog.Builder(this)
                .setTitle(title)
                .setMessage(message)
                .setPositiveButton("Tak, zakończ", (dialog, which) -> completeReturnWithAction(actionText))
                .setNegativeButton("Anuluj", null)
                .show();
    }

    private void completeReturnWithAction(String actionText) {
        if (btnCompleteReturn != null) {
            btnCompleteReturn.setEnabled(false);
        }
        progressBar.setVisibility(View.VISIBLE);

        ApiClient client = new ApiClient(this);
        client.addReturnAction(returnId, new ReturnActionCreateRequest(actionText), new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                client.completeReturn(returnId, new ApiClient.ApiCallback<Void>() {
                    @Override
                    public void onSuccess(Void data) {
                        runOnUiThread(() -> {
                            progressBar.setVisibility(View.GONE);
                            if (btnCompleteReturn != null) {
                                btnCompleteReturn.setEnabled(true);
                            }
                            Toast.makeText(ReturnDetailActivity.this, "Zwrot zakończony.", Toast.LENGTH_SHORT).show();
                            finish();
                        });
                    }

                    @Override
                    public void onError(String message) {
                        runOnUiThread(() -> {
                            progressBar.setVisibility(View.GONE);
                            if (btnCompleteReturn != null) {
                                btnCompleteReturn.setEnabled(true);
                            }
                            Toast.makeText(ReturnDetailActivity.this, "Błąd zakończenia: " + message, Toast.LENGTH_LONG).show();
                        });
                    }
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    if (btnCompleteReturn != null) {
                        btnCompleteReturn.setEnabled(true);
                    }
                    Toast.makeText(ReturnDetailActivity.this, "Błąd zapisu: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private String resolveCompletionActionText() {
        switch (decisionType) {
            case STOCK:
                return "Zwrot wrócił na stan.";
            case OTHER:
                return "Zwrot zakończony zgodnie z decyzją.";
            case RESEND:
                return "Ponowna wysyłka zakończona.";
            default:
                return "Zwrot zakończony zgodnie z decyzją.";
        }
    }

    private DecisionType resolveDecisionType(String decyzja) {
        if (decyzja == null) {
            return DecisionType.OTHER;
        }
        String normalized = decyzja.trim().toLowerCase();
        if (normalized.contains("reklam")) {
            return DecisionType.COMPLAINTS;
        }
        if (normalized.contains("półk") || normalized.contains("polk")) {
            return DecisionType.STOCK;
        }
        if (normalized.contains("wysył") || normalized.contains("wysyl") || normalized.contains("ponown")) {
            return DecisionType.RESEND;
        }
        if (normalized.contains("inne")) {
            return DecisionType.OTHER;
        }
        return DecisionType.OTHER;
    }

    private boolean isAfterDecisionStatus(String status) {
        if (status == null) {
            return false;
        }
        String normalized = status.trim().toLowerCase();
        return normalized.equals("po decyzji")
                || normalized.equals("oczekuje na realizację")
                || normalized.equals("oczekuje na realizacje");
    }

    private String resolveCarrierValue(android.widget.Spinner spinner, EditText editOther) {
        String selected = spinner.getSelectedItem() == null ? "" : spinner.getSelectedItem().toString().trim();
        if ("Inny".equalsIgnoreCase(selected)) {
            return editOther.getText().toString().trim();
        }
        return selected;
    }

    private static class DateUtils {
        private static final java.text.SimpleDateFormat DATE_FORMAT = new java.text.SimpleDateFormat("dd.MM.yyyy", java.util.Locale.getDefault());

        static String today() {
            return DATE_FORMAT.format(new java.util.Date());
        }
    }

    private void showAddressesDialog() {
        if (details == null) {
            Toast.makeText(this, "Brak danych zwrotu", Toast.LENGTH_SHORT).show();
            return;
        }
        StringBuilder content = new StringBuilder();
        appendAddressSection(content, "Dostawa", details.getDeliveryName(), details.getDeliveryAddress(), details.getDeliveryPhone());
        appendAddressSection(content, "Kupujący", details.getBuyerName(), details.getBuyerAddressRaw(), details.getBuyerPhoneRaw());
        if (content.length() == 0) {
            content.append("Brak dodatkowych adresów.");
        }
        new AlertDialog.Builder(this)
                .setTitle("Adresy")
                .setMessage(content.toString())
                .setPositiveButton("OK", null)
                .show();
    }

    private void appendAddressSection(StringBuilder builder, String title, String name, String address, String phone) {
        boolean hasData = !isBlank(name) || !isBlank(address) || !isBlank(phone);
        if (!hasData) {
            return;
        }
        if (builder.length() > 0) {
            builder.append("\n\n");
        }
        builder.append(title).append(":\n");
        if (!isBlank(name)) {
            builder.append(name).append("\n");
        }
        if (!isBlank(address)) {
            builder.append(address).append("\n");
        }
        if (!isBlank(phone)) {
            builder.append("Tel: ").append(phone);
        }
    }

    private void preselectStanProduktu(Integer stanId) {
        if (stanId == null) {
            return;
        }
        for (int i = 0; i < stanProduktuStatuses.size(); i++) {
            if (stanProduktuStatuses.get(i).getId() == stanId) {
                spinnerStanProduktu.setSelection(i);
                return;
            }
        }
    }

    private int getSelectedStatusId() {
        int index = spinnerStanProduktu.getSelectedItemPosition();
        if (index < 0 || index >= stanProduktuStatuses.size()) {
            return 0;
        }
        return stanProduktuStatuses.get(index).getId();
    }

    private List<String> toStatusNames(List<StatusDto> statuses) {
        List<String> names = new ArrayList<>();
        for (StatusDto status : statuses) {
            names.add(status.getNazwa());
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

    private String valueOrPlaceholder(Integer value) {
        return value == null ? "Brak" : String.valueOf(value);
    }

    private boolean isBlank(String value) {
        return value == null || value.trim().isEmpty();
    }

    private String buildAssignedSalesLabel(ReturnDetailsDto data) {
        String assignedName = safe(data.getAssignedSalesName());
        String assignedId = data.getAssignedSalesId() != null ? String.valueOf(data.getAssignedSalesId()) : "";
        if (!assignedName.isEmpty()) {
            return assignedId.isEmpty() ? assignedName : assignedName + " (ID: " + assignedId + ")";
        }
        if (!assignedId.isEmpty()) {
            return "ID: " + assignedId;
        }
        return "Brak przypisanego opiekuna";
    }

    private String formatReason(String reason) {
        if (reason == null || reason.trim().isEmpty()) {
            return "";
        }
        String trimmed = reason.trim();
        String code = extractReasonCode(trimmed);
        if (code.isEmpty()) {
            return trimmed;
        }
        String translated = translateReasonCode(code);
        String suffix = trimmed.substring(code.length()).trim();
        String description = translated.isEmpty() ? code : translated + " (" + code + ")";
        if (suffix.isEmpty()) {
            return description;
        }
        if (suffix.startsWith(":")) {
            suffix = suffix.substring(1).trim();
        }
        if (suffix.startsWith("(") && suffix.endsWith(")")) {
            suffix = suffix.substring(1, suffix.length() - 1).trim();
        }
        return suffix.isEmpty() ? description : description + " — " + suffix;
    }

    private String extractReasonCode(String reason) {
        int end = 0;
        while (end < reason.length()) {
            char ch = reason.charAt(end);
            if (Character.isLetterOrDigit(ch) || ch == '_') {
                end++;
            } else {
                break;
            }
        }
        if (end == 0) {
            return "";
        }
        String code = reason.substring(0, end);
        return code.equals(code.toUpperCase()) ? code : "";
    }

    private String translateReasonCode(String code) {
        switch (code) {
            case "DONT_LIKE_IT":
                return "Nie podoba się";
            case "NOT_AS_DESCRIBED":
                return "Nie zgodny z opisem";
            case "DAMAGED":
                return "Uszkodzony";
            case "MISSING_PARTS":
                return "Brakujące elementy";
            case "WRONG_ITEM":
                return "Niewłaściwy produkt";
            case "NO_LONGER_NEEDED":
                return "Niepotrzebny";
            case "BETTER_PRICE_FOUND":
                return "Znaleziono lepszą cenę";
            case "ORDERED_BY_MISTAKE":
                return "Zakup przez pomyłkę";
            case "DEFECTIVE":
                return "Wadliwy";
            case "DELIVERED_TOO_LATE":
                return "Dostarczony zbyt późno";
            case "PRODUCT_DOES_NOT_WORK":
                return "Produkt nie działa";
            case "QUALITY_UNSATISFACTORY":
                return "Niezadowalająca jakość";
            default:
                return "";
        }
    }

    private ForwardToComplaintRequest buildComplaintRequest() {
        String buyerName = safe(details.getBuyerName(), "");
        String[] nameParts = splitName(buyerName);
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
        if (przekazal == null || przekazal.trim().isEmpty()) {
            przekazal = "Magazyn";
        }
        return new ForwardToComplaintRequest(
                returnId,
                emptyToNull(details.getReason()),
                emptyToNull(details.getUwagiMagazynu()),
                null,
                przekazal,
                customer,
                product
        );
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

    @Override
    public boolean onOptionsItemSelected(android.view.MenuItem item) {
        if (item.getItemId() == android.R.id.home) {
            finish();
            return true;
        }
        return super.onOptionsItemSelected(item);
    }
}
