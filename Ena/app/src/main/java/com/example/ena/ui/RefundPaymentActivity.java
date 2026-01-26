package com.example.ena.ui;

import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;
import androidx.appcompat.app.AppCompatActivity;
import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.PaymentIdDto;
import com.example.ena.api.PaymentRefundRequest;
import com.example.ena.api.RefundLineItemContextDto;
import com.example.ena.api.RefundLineItemDto;
import com.example.ena.api.RefundValueDto;
import com.example.ena.api.ReturnRefundContextDto;
import java.math.BigDecimal;
import java.math.RoundingMode;
import java.util.ArrayList;
import java.util.List;

public class RefundPaymentActivity extends AppCompatActivity {
    public static final String EXTRA_RETURN_ID = "return_id";

    private TextView txtHeader;
    private ProgressBar progressBar;
    private LinearLayout containerLineItems;
    private CheckBox chkRefundDelivery;
    private EditText editDeliveryAmount;
    private Spinner spinnerReason;
    private EditText editComment;
    private TextView txtTotal;
    private Button btnCancel;
    private Button btnSubmit;

    private final List<LineItemRow> lineItemRows = new ArrayList<>();
    private ReturnRefundContextDto context;
    private int returnId;

    private static class ReasonItem {
        final String code;
        final String label;

        ReasonItem(String code, String label) {
            this.code = code;
            this.label = label;
        }

        @Override
        public String toString() {
            return label;
        }
    }

    private static class LineItemRow {
        final RefundLineItemContextDto item;
        final CheckBox checkBox;
        final EditText amountEdit;

        LineItemRow(RefundLineItemContextDto item, CheckBox checkBox, EditText amountEdit) {
            this.item = item;
            this.checkBox = checkBox;
            this.amountEdit = amountEdit;
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_refund_payment);

        returnId = getIntent().getIntExtra(EXTRA_RETURN_ID, 0);
        txtHeader = findViewById(R.id.txtRefundHeader);
        progressBar = findViewById(R.id.progressRefund);
        containerLineItems = findViewById(R.id.containerLineItems);
        chkRefundDelivery = findViewById(R.id.chkRefundDelivery);
        editDeliveryAmount = findViewById(R.id.editDeliveryAmount);
        spinnerReason = findViewById(R.id.spinnerRefundReason);
        editComment = findViewById(R.id.editRefundComment);
        txtTotal = findViewById(R.id.txtRefundTotal);
        btnCancel = findViewById(R.id.btnRefundCancel);
        btnSubmit = findViewById(R.id.btnRefundSubmit);

        btnCancel.setOnClickListener(v -> finish());
        btnSubmit.setOnClickListener(v -> submitRefund());

        chkRefundDelivery.setOnCheckedChangeListener((buttonView, isChecked) -> {
            editDeliveryAmount.setEnabled(isChecked);
            updateTotal();
        });

        editDeliveryAmount.addTextChangedListener(simpleWatcher());

        setupReasons();
        loadRefundContext();
    }

    private void setupReasons() {
        List<ReasonItem> reasons = new ArrayList<>();
        reasons.add(new ReasonItem("REFUND", "Zwrot (np. odstąpienie od umowy)"));
        reasons.add(new ReasonItem("COMPLAINT", "Reklamacja"));
        reasons.add(new ReasonItem("PRODUCT_NOT_AVAILABLE", "Produkt niedostępny"));
        ArrayAdapter<ReasonItem> adapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, reasons);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinnerReason.setAdapter(adapter);
    }

    private void loadRefundContext() {
        progressBar.setVisibility(View.VISIBLE);
        ApiClient client = new ApiClient(this);
        client.fetchRefundContext(returnId, new ApiClient.ApiCallback<ReturnRefundContextDto>() {
            @Override
            public void onSuccess(ReturnRefundContextDto data) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    context = data;
                    bindContext();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    progressBar.setVisibility(View.GONE);
                    Toast.makeText(RefundPaymentActivity.this, "Błąd: " + message, Toast.LENGTH_LONG).show();
                    finish();
                });
            }
        });
    }

    private void bindContext() {
        if (context == null) {
            return;
        }
        txtHeader.setText("Zwrot wpłaty dla zamówienia: " + context.getOrderId());
        containerLineItems.removeAllViews();
        lineItemRows.clear();

        LayoutInflater inflater = LayoutInflater.from(this);
        for (RefundLineItemContextDto item : context.getLineItems()) {
            View row = inflater.inflate(R.layout.item_refund_line, containerLineItems, false);
            TextView name = row.findViewById(R.id.txtLineName);
            TextView quantity = row.findViewById(R.id.txtLineQuantity);
            EditText amount = row.findViewById(R.id.editLineAmount);
            CheckBox check = row.findViewById(R.id.chkLineRefund);

            name.setText(item.getName());
            quantity.setText("Ilość: " + item.getQuantity());
            amount.setText(item.getPrice().getAmount());

            amount.addTextChangedListener(simpleWatcher());
            check.setOnCheckedChangeListener((buttonView, isChecked) -> updateTotal());

            containerLineItems.addView(row);
            lineItemRows.add(new LineItemRow(item, check, amount));
        }

        if (context.getDelivery() != null && context.getDelivery().getAmount() != null) {
            chkRefundDelivery.setEnabled(true);
            chkRefundDelivery.setChecked(true);
            editDeliveryAmount.setText(context.getDelivery().getAmount());
            editDeliveryAmount.setEnabled(true);
        } else {
            chkRefundDelivery.setChecked(false);
            chkRefundDelivery.setEnabled(false);
            editDeliveryAmount.setText("0.00");
            editDeliveryAmount.setEnabled(false);
        }

        updateTotal();
    }

    private void submitRefund() {
        if (context == null || context.getPaymentId() == null || context.getPaymentId().isEmpty()) {
            Toast.makeText(this, "Brak danych o płatności.", Toast.LENGTH_SHORT).show();
            return;
        }

        ReasonItem reason = spinnerReason.getSelectedItem() instanceof ReasonItem
            ? (ReasonItem) spinnerReason.getSelectedItem()
            : null;
        if (reason == null) {
            Toast.makeText(this, "Wybierz powód zwrotu.", Toast.LENGTH_SHORT).show();
            return;
        }

        List<RefundLineItemDto> items = new ArrayList<>();
        for (LineItemRow row : lineItemRows) {
            if (!row.checkBox.isChecked()) {
                continue;
            }
            BigDecimal amount = parseAmount(row.amountEdit.getText().toString());
            if (amount == null) {
                Toast.makeText(this, "Nieprawidłowa kwota dla produktu: " + row.item.getName(), Toast.LENGTH_LONG).show();
                return;
            }
            items.add(new RefundLineItemDto(
                row.item.getId(),
                "LINE_ITEM",
                null,
                new RefundValueDto(formatAmount(amount), row.item.getPrice().getCurrency())
            ));
        }

        RefundValueDto delivery = null;
        if (chkRefundDelivery.isChecked()) {
            BigDecimal amount = parseAmount(editDeliveryAmount.getText().toString());
            if (amount == null) {
                Toast.makeText(this, "Nieprawidłowa kwota dla dostawy.", Toast.LENGTH_LONG).show();
                return;
            }
            delivery = new RefundValueDto(formatAmount(amount),
                context.getDelivery() != null ? context.getDelivery().getCurrency() : "PLN");
        }

        if (items.isEmpty() && delivery == null) {
            Toast.makeText(this, "Nie wybrano żadnej pozycji do zwrotu.", Toast.LENGTH_SHORT).show();
            return;
        }

        String comment = editComment.getText().toString().trim();
        PaymentRefundRequest request = new PaymentRefundRequest(
            new PaymentIdDto(context.getPaymentId()),
            reason.code,
            items.isEmpty() ? null : items,
            delivery,
            comment.isEmpty() ? null : comment
        );

        btnSubmit.setEnabled(false);
        ApiClient client = new ApiClient(this);
        client.refundPayment(returnId, request, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    btnSubmit.setEnabled(true);
                    Toast.makeText(RefundPaymentActivity.this, "Zwrot wpłaty został zlecony.", Toast.LENGTH_SHORT).show();
                    finish();
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    btnSubmit.setEnabled(true);
                    Toast.makeText(RefundPaymentActivity.this, "Błąd zwrotu wpłaty: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private void updateTotal() {
        BigDecimal total = BigDecimal.ZERO;
        for (LineItemRow row : lineItemRows) {
            if (row.checkBox.isChecked()) {
                BigDecimal amount = parseAmount(row.amountEdit.getText().toString());
                if (amount != null) {
                    total = total.add(amount);
                }
            }
        }

        if (chkRefundDelivery.isChecked()) {
            BigDecimal amount = parseAmount(editDeliveryAmount.getText().toString());
            if (amount != null) {
                total = total.add(amount);
            }
        }

        txtTotal.setText("Suma do zwrotu: " + formatAmount(total) + " zł");
    }

    private TextWatcher simpleWatcher() {
        return new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {
                updateTotal();
            }

            @Override
            public void afterTextChanged(Editable s) {}
        };
    }

    private BigDecimal parseAmount(String raw) {
        if (raw == null) {
            return null;
        }
        String normalized = raw.trim().replace(",", ".");
        if (normalized.isEmpty()) {
            return null;
        }
        try {
            return new BigDecimal(normalized);
        } catch (NumberFormatException ex) {
            return null;
        }
    }

    private String formatAmount(BigDecimal amount) {
        return amount.setScale(2, RoundingMode.HALF_UP).toPlainString();
    }
}
