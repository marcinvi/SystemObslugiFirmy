package com.example.ena.ui;

import android.os.Bundle;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.AutoCompleteTextView;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.Spinner;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.ManualReturnMetaDto;
import com.example.ena.api.ManualReturnRecipientDto;
import com.example.ena.api.ReturnManualCreateRequest;
import com.example.ena.api.StatusDto;
import java.util.ArrayList;
import java.util.List;

public class ManualReturnActivity extends AppCompatActivity {
    public static final String EXTRA_WAYBILL = "extra_waybill";

    private EditText editBuyerName;
    private EditText editBuyerStreet;
    private EditText editBuyerZip;
    private EditText editBuyerCity;
    private EditText editBuyerPhone;
    private EditText editWaybill;
    private AutoCompleteTextView editCarrier;
    private AutoCompleteTextView editProduct;
    private Spinner spinnerStanProduktu;
    private EditText editNotes;
    private CheckBox chkAllSales;
    private LinearLayout layoutHandlowcy;
    private Button btnSave;
    private Button btnCancel;
    private ProgressBar progressBar;

    private final List<StatusDto> stanProduktuStatuses = new ArrayList<>();
    private final List<ManualReturnRecipientDto> handlowcy = new ArrayList<>();
    private boolean isUpdatingSelectAll;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_manual_return);

        editBuyerName = findViewById(R.id.editBuyerName);
        editBuyerStreet = findViewById(R.id.editBuyerStreet);
        editBuyerZip = findViewById(R.id.editBuyerZip);
        editBuyerCity = findViewById(R.id.editBuyerCity);
        editBuyerPhone = findViewById(R.id.editBuyerPhone);
        editWaybill = findViewById(R.id.editWaybill);
        editCarrier = findViewById(R.id.editCarrier);
        editProduct = findViewById(R.id.editProduct);
        spinnerStanProduktu = findViewById(R.id.spinnerStanProduktu);
        editNotes = findViewById(R.id.editNotes);
        chkAllSales = findViewById(R.id.chkAllSales);
        layoutHandlowcy = findViewById(R.id.layoutHandlowcy);
        btnSave = findViewById(R.id.btnSaveManual);
        btnCancel = findViewById(R.id.btnCancelManual);
        progressBar = findViewById(R.id.progressManual);

        String waybill = getIntent().getStringExtra(EXTRA_WAYBILL);
        if (waybill != null && !waybill.trim().isEmpty()) {
            editWaybill.setText(waybill.trim());
        }

        btnCancel.setOnClickListener(v -> finish());
        btnSave.setOnClickListener(v -> submitManualReturn());

        chkAllSales.setOnCheckedChangeListener((buttonView, isChecked) -> {
            if (isUpdatingSelectAll) {
                return;
            }
            setAllRecipientsChecked(isChecked);
        });

        loadStatuses();
        loadManualMeta();
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
                    List<String> names = new ArrayList<>();
                    for (StatusDto status : stanProduktuStatuses) {
                        names.add(status.getNazwa());
                    }
                    ArrayAdapter<String> adapter = new ArrayAdapter<>(
                        ManualReturnActivity.this,
                        android.R.layout.simple_spinner_item,
                        names
                    );
                    adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
                    spinnerStanProduktu.setAdapter(adapter);
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> Toast.makeText(ManualReturnActivity.this, "Błąd statusów: " + message, Toast.LENGTH_LONG).show());
            }
        });
    }

    private void loadManualMeta() {
        progressBar.setVisibility(View.VISIBLE);
        ApiClient client = new ApiClient(this);
        client.fetchManualReturnMeta(new ApiClient.ApiCallback<ManualReturnMetaDto>() {
            @Override
            public void onSuccess(ManualReturnMetaDto data) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    setupAutoComplete(editCarrier, data != null ? data.getPrzewoznicy() : null);
                    setupAutoComplete(editProduct, data != null ? data.getProdukty() : null);
                    setupRecipients(data != null ? data.getHandlowcy() : null);
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    Toast.makeText(ManualReturnActivity.this, "Błąd danych: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private void setupAutoComplete(AutoCompleteTextView view, List<String> items) {
        List<String> data = items == null ? new ArrayList<>() : items;
        ArrayAdapter<String> adapter = new ArrayAdapter<>(
            this,
            android.R.layout.simple_dropdown_item_1line,
            data
        );
        view.setAdapter(adapter);
    }

    private void setupRecipients(List<ManualReturnRecipientDto> items) {
        layoutHandlowcy.removeAllViews();
        handlowcy.clear();
        if (items != null) {
            handlowcy.addAll(items);
        }
        for (ManualReturnRecipientDto recipient : handlowcy) {
            CheckBox checkBox = new CheckBox(this);
            checkBox.setText(recipient.getNazwaWyswietlana());
            checkBox.setTag(recipient.getId());
            checkBox.setOnCheckedChangeListener((buttonView, isChecked) -> updateSelectAllState());
            layoutHandlowcy.addView(checkBox);
        }
        updateSelectAllState();
    }

    private void setAllRecipientsChecked(boolean checked) {
        isUpdatingSelectAll = true;
        try {
            for (int i = 0; i < layoutHandlowcy.getChildCount(); i++) {
                View view = layoutHandlowcy.getChildAt(i);
                if (view instanceof CheckBox) {
                    ((CheckBox) view).setChecked(checked);
                }
            }
        } finally {
            isUpdatingSelectAll = false;
        }
    }

    private void updateSelectAllState() {
        isUpdatingSelectAll = true;
        try {
            boolean allChecked = layoutHandlowcy.getChildCount() > 0;
            boolean anyChecked = false;
            for (int i = 0; i < layoutHandlowcy.getChildCount(); i++) {
                View view = layoutHandlowcy.getChildAt(i);
                if (view instanceof CheckBox) {
                    boolean checked = ((CheckBox) view).isChecked();
                    allChecked = allChecked && checked;
                    anyChecked = anyChecked || checked;
                }
            }
            chkAllSales.setChecked(allChecked && anyChecked);
        } finally {
            isUpdatingSelectAll = false;
        }
    }

    private void submitManualReturn() {
        String waybill = editWaybill.getText().toString().trim();
        if (waybill.isEmpty()) {
            Toast.makeText(this, "Numer listu przewozowego jest wymagany.", Toast.LENGTH_SHORT).show();
            return;
        }

        String buyerName = editBuyerName.getText().toString().trim();
        if (buyerName.isEmpty()) {
            Toast.makeText(this, "Imię i nazwisko nadawcy jest wymagane.", Toast.LENGTH_SHORT).show();
            return;
        }

        int statusId = getSelectedStatusId();
        if (statusId <= 0) {
            Toast.makeText(this, "Stan produktu jest wymagany.", Toast.LENGTH_SHORT).show();
            return;
        }

        List<Integer> selectedHandlowcy = getSelectedRecipients();
        if (selectedHandlowcy.isEmpty()) {
            Toast.makeText(this, "Wybierz co najmniej jednego handlowca.", Toast.LENGTH_SHORT).show();
            return;
        }

        ReturnManualCreateRequest request = new ReturnManualCreateRequest(
            waybill,
            emptyToNull(editProduct.getText().toString().trim()),
            emptyToNull(editCarrier.getText().toString().trim()),
            statusId,
            emptyToNull(editNotes.getText().toString().trim()),
            buyerName,
            emptyToNull(editBuyerStreet.getText().toString().trim()),
            emptyToNull(editBuyerZip.getText().toString().trim()),
            emptyToNull(editBuyerCity.getText().toString().trim()),
            emptyToNull(editBuyerPhone.getText().toString().trim()),
            selectedHandlowcy
        );

        btnSave.setEnabled(false);
        progressBar.setVisibility(View.VISIBLE);

        ApiClient client = new ApiClient(this);
        client.createManualReturn(request, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    btnSave.setEnabled(true);
                    Toast.makeText(ManualReturnActivity.this, "Dodano zwrot ręczny.", Toast.LENGTH_LONG).show();
                    finish();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    btnSave.setEnabled(true);
                    Toast.makeText(ManualReturnActivity.this, "Błąd zapisu: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private int getSelectedStatusId() {
        int index = spinnerStanProduktu.getSelectedItemPosition();
        if (index < 0 || index >= stanProduktuStatuses.size()) {
            return 0;
        }
        return stanProduktuStatuses.get(index).getId();
    }

    private List<Integer> getSelectedRecipients() {
        List<Integer> selected = new ArrayList<>();
        for (int i = 0; i < layoutHandlowcy.getChildCount(); i++) {
            View view = layoutHandlowcy.getChildAt(i);
            if (view instanceof CheckBox) {
                CheckBox checkBox = (CheckBox) view;
                if (checkBox.isChecked()) {
                    Object tag = checkBox.getTag();
                    if (tag instanceof Integer) {
                        selected.add((Integer) tag);
                    }
                }
            }
        }
        return selected;
    }

    private String emptyToNull(String value) {
        return value == null || value.isEmpty() ? null : value;
    }
}
