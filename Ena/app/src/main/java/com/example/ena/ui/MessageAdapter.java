package com.example.ena.ui;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.R;
import com.example.ena.api.MessageDto;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.List;

public class MessageAdapter extends RecyclerView.Adapter<MessageAdapter.ViewHolder> {

    private final List<MessageDto> items = new ArrayList<>();
    private static final DateTimeFormatter DATE_FMT = DateTimeFormatter.ofPattern("dd.MM HH:mm");

    public void setItems(List<MessageDto> data) {
        items.clear();
        if (data != null) {
            items.addAll(data);
        }
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_message, parent, false);
        return new ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
        MessageDto item = items.get(position);

        holder.txtSender.setText(item.getSender() != null ? item.getSender() : "System");
        holder.txtContent.setText(item.getContent());

        try {
            if (item.getCreatedAt() != null) {
                holder.txtDate.setText(DATE_FMT.format(item.getCreatedAt()));
            } else {
                holder.txtDate.setText("");
            }
        } catch (Exception e) {
            holder.txtDate.setText("");
        }
    }

    @Override
    public int getItemCount() {
        return items.size();
    }

    static class ViewHolder extends RecyclerView.ViewHolder {
        // Deklarujemy pola jako publiczne lub finalne, aby Adapter je widział
        final TextView txtSender;
        final TextView txtDate;
        final TextView txtContent;

        ViewHolder(@NonNull View itemView) {
            super(itemView);
            txtSender = itemView.findViewById(R.id.txtSender);
            txtDate = itemView.findViewById(R.id.txtDate);
            txtContent = itemView.findViewById(R.id.txtContent);
        }
    }
}