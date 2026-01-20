package com.example.ena.ui;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.R;
import com.example.ena.api.ReturnListItemDto;
import java.util.ArrayList;
import java.util.List;

public class ReturnListAdapter extends RecyclerView.Adapter<ReturnListAdapter.ViewHolder> {

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
        String statusWew = item.getStatusWewnetrzny() != null ? item.getStatusWewnetrzny() : "";
        String statusAll = item.getStatusAllegro() != null ? translateStatus(item.getStatusAllegro()) : "";
        String status = statusWew;
        if (!statusAll.isEmpty()) {
            status = status.isEmpty() ? statusAll : status + " / " + statusAll;
        }
        holder.txtStatus.setText(status.isEmpty() ? "Brak statusu" : status);
        holder.itemView.setOnClickListener(v -> listener.onItemClick(item));
    }

    @Override
    public int getItemCount() {
        return items.size();
    }

    static class ViewHolder extends RecyclerView.ViewHolder {
        final TextView txtRef;
        final TextView txtProduct;
        final TextView txtBuyer;
        final TextView txtStatus;

        ViewHolder(@NonNull View itemView) {
            super(itemView);
            txtRef = itemView.findViewById(R.id.txtRef);
            txtProduct = itemView.findViewById(R.id.txtProduct);
            txtBuyer = itemView.findViewById(R.id.txtBuyer);
            txtStatus = itemView.findViewById(R.id.txtStatus);
        }
    }

    private String translateStatus(String status) {
        switch (status) {
            case "DELIVERED":
                return "Dostarczono";
            case "IN_TRANSIT":
                return "W drodze";
            case "READY_FOR_PICKUP":
                return "Gotowy do odbioru";
            case "CREATED":
                return "Utworzono";
            case "COMMISSION_REFUNDED":
                return "Zwrócono prowizję";
            default:
                return status;
        }
    }
}
