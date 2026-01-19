package com.example.ena.ui;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.R;
import com.example.ena.api.ReturnListItem;
import java.util.ArrayList;
import java.util.List;

public class ReturnListAdapter extends RecyclerView.Adapter<ReturnListAdapter.ViewHolder> {

    public interface OnItemClickListener {
        void onItemClick(ReturnListItem item);
    }

    private final List<ReturnListItem> items = new ArrayList<>();
    private final OnItemClickListener listener;

    public ReturnListAdapter(OnItemClickListener listener) {
        this.listener = listener;
    }

    public void setItems(List<ReturnListItem> data) {
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
        ReturnListItem item = items.get(position);
        if (item == null) {
            holder.txtRef.setText("Brak danych");
            holder.txtProduct.setText("");
            holder.txtBuyer.setText("");
            holder.txtStatus.setText("");
            holder.itemView.setOnClickListener(null);
            return;
        }
        holder.txtRef.setText(item.referenceNumber != null ? item.referenceNumber : "Zwrot #" + item.id);
        holder.txtProduct.setText(item.productName != null ? item.productName : "Brak produktu");
        holder.txtBuyer.setText(item.buyerName != null ? item.buyerName : "");
        String statusWew = item.statusWewnetrzny != null ? item.statusWewnetrzny : "";
        String statusAll = item.statusAllegro != null ? item.statusAllegro : "";
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
}
