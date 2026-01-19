package com.example.ena.ui;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.R;
import com.example.ena.api.MessageDto;
import java.util.ArrayList;
import java.util.List;

public class MessageAdapter extends RecyclerView.Adapter<MessageAdapter.ViewHolder> {
    private final List<MessageDto> items = new ArrayList<>();

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
        String title = item.tytul != null && !item.tytul.isEmpty() ? item.tytul : "Wiadomość #" + item.id;
        holder.txtTitle.setText(title);
        holder.txtBody.setText(item.tresc != null ? item.tresc : "");
        String meta = "Nadawca: " + item.nadawcaId + " • Odbiorca: " + item.odbiorcaId;
        if (item.dotyczyZwrotuId != null) {
            meta += " • Zwrot: " + item.dotyczyZwrotuId;
        }
        holder.txtMeta.setText(meta);
    }

    @Override
    public int getItemCount() {
        return items.size();
    }

    static class ViewHolder extends RecyclerView.ViewHolder {
        final TextView txtTitle;
        final TextView txtBody;
        final TextView txtMeta;

        ViewHolder(@NonNull View itemView) {
            super(itemView);
            txtTitle = itemView.findViewById(R.id.txtMessageTitle);
            txtBody = itemView.findViewById(R.id.txtMessageBody);
            txtMeta = itemView.findViewById(R.id.txtMessageMeta);
        }
    }
}
