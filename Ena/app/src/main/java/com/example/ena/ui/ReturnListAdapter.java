package com.example.ena.ui;

import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.R;
import com.example.ena.api.ReturnListItemDto;
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
    private static final DateTimeFormatter DATE_FMT = DateTimeFormatter.ofPattern("dd.MM HH:mm");

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

        // NAPRAWA: Używamy tylko getReferenceNumber() lub ID.
        // Usunięto całkowicie błędne wywołanie getNumerZwrotu()
        String ref = item.getReferenceNumber();
        if (ref == null || ref.isEmpty()) {
            ref = "Zwrot #" + item.getId();
        }
        holder.txtRef.setText(ref);

        holder.txtProduct.setText(item.getProductName() != null ? item.getProductName() : "Brak produktu");
        holder.txtBuyer.setText(item.getBuyerName() != null ? item.getBuyerName() : "");

        try {
            if (item.getCreatedAt() != null) holder.txtDate.setText(DATE_FMT.format(item.getCreatedAt()));
        } catch (Exception e) {}

        String status = item.getStatusWewnetrzny();
        String actionText = (status != null && !status.isEmpty()) ? status : "NIEZNANY";
        int bgColor = Color.parseColor("#EEEEEE");
        int textColor = Color.parseColor("#757575");

        if (status != null) {
            String s = status.toLowerCase();
            if (s.contains("przyjęcie")) { bgColor = Color.parseColor("#E3F2FD"); textColor = Color.parseColor("#1565C0"); }
            else if (s.contains("zakończony")) { bgColor = Color.parseColor("#E8F5E9"); textColor = Color.parseColor("#2E7D32"); }
            else if (s.contains("decyzję")) { bgColor = Color.parseColor("#FFF3E0"); textColor = Color.parseColor("#EF6C00"); }
            else if (s.contains("reklamacj")) { bgColor = Color.parseColor("#FFEBEE"); textColor = Color.parseColor("#C62828"); }
        }

        holder.txtAction.setText(actionText.toUpperCase());
        holder.decisionContainer.getBackground().setTint(bgColor);
        holder.txtAction.setTextColor(textColor);
        holder.statusStrip.setBackgroundColor(textColor);

        holder.itemView.setOnClickListener(v -> listener.onItemClick(item));
    }

    @Override
    public int getItemCount() { return items.size(); }

    static class ViewHolder extends RecyclerView.ViewHolder {
        final TextView txtRef, txtProduct, txtBuyer, txtAction, txtDate;
        final View statusStrip;
        final FrameLayout decisionContainer;

        ViewHolder(@NonNull View itemView) {
            super(itemView);
            txtRef = itemView.findViewById(R.id.txtRef);
            txtDate = itemView.findViewById(R.id.txtDate);
            txtProduct = itemView.findViewById(R.id.txtProduct);
            txtBuyer = itemView.findViewById(R.id.txtBuyer);
            txtAction = itemView.findViewById(R.id.txtAction);
            txtDate = itemView.findViewById(R.id.txtDate);
            statusStrip = itemView.findViewById(R.id.statusStrip);
            decisionContainer = itemView.findViewById(R.id.decisionContainer);
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
        static final DecisionStyle RESEND = new DecisionStyle(0xFFFF8F00, 0xFFFFF3E0, 0xFFF57C00);
        static final DecisionStyle OTHER = new DecisionStyle(0xFFE53935, 0xFFFFEBEE, 0xFFC62828);
    }
}