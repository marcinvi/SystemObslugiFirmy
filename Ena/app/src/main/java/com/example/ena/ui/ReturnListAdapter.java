package com.example.ena.ui;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.content.res.ColorStateList;
import android.widget.FrameLayout;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.R;
import com.example.ena.api.ReturnListItemDto;
import java.time.OffsetDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class ReturnListAdapter extends RecyclerView.Adapter<ReturnListAdapter.ViewHolder> {
    private static final DateTimeFormatter DATE_FORMAT = DateTimeFormatter.ofPattern("dd.MM HH:mm");

    public enum DisplayMode {
        DECISION,
        WAREHOUSE_STATUS
    }

    public interface OnItemClickListener {
        void onItemClick(ReturnListItemDto item);
    }

    private final List<ReturnListItemDto> items = new ArrayList<>();
    private final OnItemClickListener listener;
    private final DisplayMode displayMode;

    public ReturnListAdapter(OnItemClickListener listener) {
        this(listener, DisplayMode.DECISION);
    }

    public ReturnListAdapter(OnItemClickListener listener, DisplayMode displayMode) {
        this.listener = listener;
        this.displayMode = displayMode == null ? DisplayMode.DECISION : displayMode;
    }

    public void setItems(List<ReturnListItemDto> data) {
        items.clear();
        if (data != null) {
            items.addAll(data);
        }
        items.sort((left, right) -> {
            int leftPriority = statusPriority(left);
            int rightPriority = statusPriority(right);
            if (leftPriority != rightPriority) {
                return Integer.compare(leftPriority, rightPriority);
            }
            OffsetDateTime leftDate = left.getCreatedAt();
            OffsetDateTime rightDate = right.getCreatedAt();
            if (leftDate == null && rightDate == null) {
                return 0;
            }
            if (leftDate == null) {
                return 1;
            }
            if (rightDate == null) {
                return -1;
            }
            return rightDate.compareTo(leftDate);
        });
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_return, parent, false);
        return new ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
        ReturnListItemDto item = items.get(position);
        holder.txtRef.setText(item.getReferenceNumber() != null ? item.getReferenceNumber() : "Zwrot #" + item.getId());
        holder.txtProduct.setText(item.getProductName() != null ? item.getProductName() : "Brak produktu");
        holder.txtBuyer.setText(item.getBuyerName() != null ? item.getBuyerName() : "");
        holder.txtDate.setText(formatDate(item.getCreatedAt()));
        holder.txtStatusAllegro.setText(formatStatusAllegro(item));

        String labelText = resolveActionLabel(item);
        holder.txtAction.setText(labelText.toUpperCase(Locale.ROOT));
        DecisionStyle style = resolveDecisionStyle(labelText);
        holder.statusStrip.setBackgroundColor(style.stripColor);
        holder.decisionContainer.setBackgroundTintList(ColorStateList.valueOf(style.containerColor));
        holder.txtAction.setTextColor(style.textColor);
        holder.itemView.setOnClickListener(v -> listener.onItemClick(item));
    }

    @Override
    public int getItemCount() {
        return items.size();
    }

    static class ViewHolder extends RecyclerView.ViewHolder {
        final TextView txtRef;
        final TextView txtDate;
        final TextView txtProduct;
        final TextView txtBuyer;
        final TextView txtStatusAllegro;
        final TextView txtAction;
        final View statusStrip;
        final FrameLayout decisionContainer;

        ViewHolder(@NonNull View itemView) {
            super(itemView);
            txtRef = itemView.findViewById(R.id.txtRef);
            txtDate = itemView.findViewById(R.id.txtDate);
            txtProduct = itemView.findViewById(R.id.txtProduct);
            txtBuyer = itemView.findViewById(R.id.txtBuyer);
            txtStatusAllegro = itemView.findViewById(R.id.txtStatusAllegro);
            txtAction = itemView.findViewById(R.id.txtAction);
            statusStrip = itemView.findViewById(R.id.statusStrip);
            decisionContainer = itemView.findViewById(R.id.decisionContainer);
        }
    }

    private String formatDate(OffsetDateTime date) {
        if (date == null) {
            return "";
        }
        return DATE_FORMAT.format(date);
    }

    private DecisionStyle resolveDecisionStyle(String decision) {
        if (decision == null) {
            return DecisionStyle.NEUTRAL;
        }
        String normalized = decision.trim().toLowerCase();
        if (normalized.isEmpty() || normalized.contains("brak")) {
            return DecisionStyle.NEUTRAL;
        }
        if (displayMode == DisplayMode.WAREHOUSE_STATUS) {
            return DecisionStyle.NEUTRAL;
        }
        if (normalized.contains("półk") || normalized.contains("polk")) {
            return DecisionStyle.STOCK;
        }
        if (normalized.contains("wysyłka") || normalized.contains("wysylka") || normalized.contains("ponowna")) {
            return DecisionStyle.RESEND;
        }
        if (normalized.contains("reklam")) {
            return DecisionStyle.COMPLAINT;
        }
        if (normalized.contains("inne")) {
            return DecisionStyle.OTHER;
        }
        return DecisionStyle.OTHER;
    }

    private static class DecisionStyle {
        final int stripColor;
        final int containerColor;
        final int textColor;

        private DecisionStyle(int stripColor, int containerColor, int textColor) {
            this.stripColor = stripColor;
            this.containerColor = containerColor;
            this.textColor = textColor;
        }

        static final DecisionStyle STOCK = new DecisionStyle(0xFF43A047, 0xFFE8F5E9, 0xFF2E7D32);
        static final DecisionStyle RESEND = new DecisionStyle(0xFF7E57C2, 0xFFF3E5F5, 0xFF5E35B1);
        static final DecisionStyle COMPLAINT = new DecisionStyle(0xFFFF8F00, 0xFFFFF3E0, 0xFFF57C00);
        static final DecisionStyle OTHER = new DecisionStyle(0xFFD32F2F, 0xFFFFEBEE, 0xFFC62828);
        static final DecisionStyle NEUTRAL = new DecisionStyle(0xFF9E9E9E, 0xFFF5F5F5, 0xFF616161);
    }

    private String resolveActionLabel(ReturnListItemDto item) {
        if (displayMode == DisplayMode.WAREHOUSE_STATUS) {
            String stan = item.getStanProduktu();
            return stan == null || stan.trim().isEmpty() ? "Brak oceny magazynu" : "Stan produktu: " + stan;
        }
        String decision = item.getDecyzjaHandlowca();
        return decision == null || decision.trim().isEmpty() ? "Brak decyzji" : decision;
    }

    private String formatStatusAllegro(ReturnListItemDto item) {
        if (item == null) {
            return "Status: -";
        }
        if (item.isManual()) {
            return "Status: Zwrot ręczny";
        }
        String status = item.getStatusAllegro();
        if (status == null || status.trim().isEmpty()) {
            return "Status: Brak danych";
        }
        return "Status: " + translateStatusAllegro(status.trim());
    }

    private String translateStatusAllegro(String status) {
        switch (status) {
            case "CREATED":
                return "Utworzono";
            case "IN_TRANSIT":
                return "W drodze";
            case "DELIVERED":
                return "Dostarczone";
            case "FINISHED":
                return "Zakończone";
            case "REJECTED":
                return "Odrzucone";
            case "COMMISSION_REFUND_CLAIMED":
                return "Wniosek o zwrot prowizji";
            case "COMMISSION_REFUNDED":
                return "Prowizja zwrócona";
            case "WAREHOUSE_DELIVERED":
                return "Dostarczone do magazynu Allegro";
            case "WAREHOUSE_VERIFICATION":
                return "W weryfikacji magazynu Allegro";
            case "READY_FOR_PICKUP":
                return "Gotowy do odbioru";
            default:
                return status;
        }
    }

    private int statusPriority(ReturnListItemDto item) {
        if (item == null) {
            return 3;
        }
        String status = item.getStatusAllegro();
        if (status == null) {
            return 3;
        }
        switch (status) {
            case "DELIVERED":
                return 0;
            case "IN_TRANSIT":
                return 1;
            case "CREATED":
                return 2;
            default:
                return 3;
        }
    }
}
