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

    public interface OnItemClickListener {
        void onItemClick(ReturnListItemDto item);
    }

    private final List<ReturnListItemDto> items = new ArrayList<>();
    private final OnItemClickListener listener;

    public ReturnListAdapter(OnItemClickListener listener) {
        this.listener = listener;
    }

    public void setItems(List<ReturnListItemDto> data) {
        items.clear();
        if (data != null) {
            items.addAll(data);
        }
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
        String decision = item.getDecyzjaHandlowca() != null ? item.getDecyzjaHandlowca() : "Brak decyzji";
        holder.txtAction.setText(decision.toUpperCase(Locale.ROOT));
        DecisionStyle style = resolveDecisionStyle(decision);
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
        final TextView txtAction;
        final View statusStrip;
        final FrameLayout decisionContainer;

        ViewHolder(@NonNull View itemView) {
            super(itemView);
            txtRef = itemView.findViewById(R.id.txtRef);
            txtDate = itemView.findViewById(R.id.txtDate);
            txtProduct = itemView.findViewById(R.id.txtProduct);
            txtBuyer = itemView.findViewById(R.id.txtBuyer);
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
        static final DecisionStyle RESEND = new DecisionStyle(0xFF1E88E5, 0xFFE3F2FD, 0xFF1565C0);
        static final DecisionStyle COMPLAINT = new DecisionStyle(0xFFFF8F00, 0xFFFFF3E0, 0xFFF57C00);
        static final DecisionStyle OTHER = new DecisionStyle(0xFFD32F2F, 0xFFFFEBEE, 0xFFC62828);
        static final DecisionStyle NEUTRAL = new DecisionStyle(0xFF9E9E9E, 0xFFF5F5F5, 0xFF616161);
    }
}
