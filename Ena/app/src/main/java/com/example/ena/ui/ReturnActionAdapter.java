package com.example.ena.ui;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.R;
import com.example.ena.api.ReturnActionDto;
import java.time.OffsetDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.List;

public class ReturnActionAdapter extends RecyclerView.Adapter<ReturnActionAdapter.ViewHolder> {
    private static final DateTimeFormatter DATE_FORMAT = DateTimeFormatter.ofPattern("dd.MM.yyyy HH:mm");

    private final List<ReturnActionDto> items = new ArrayList<>();

    public void setItems(List<ReturnActionDto> data) {
        items.clear();
        if (data != null) {
            items.addAll(data);
        }
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_return_action, parent, false);
        return new ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
        ReturnActionDto item = items.get(position);
        holder.txtUser.setText(item.getUzytkownik());
        holder.txtDate.setText(formatDate(item.getData()));
        holder.txtContent.setText(item.getTresc());
    }

    @Override
    public int getItemCount() {
        return items.size();
    }

    private String formatDate(OffsetDateTime date) {
        if (date == null) {
            return "";
        }
        return DATE_FORMAT.format(date);
    }

    static class ViewHolder extends RecyclerView.ViewHolder {
        final TextView txtUser;
        final TextView txtDate;
        final TextView txtContent;

        ViewHolder(@NonNull View itemView) {
            super(itemView);
            txtUser = itemView.findViewById(R.id.txtActionUser);
            txtDate = itemView.findViewById(R.id.txtActionDate);
            txtContent = itemView.findViewById(R.id.txtActionContent);
        }
    }
}