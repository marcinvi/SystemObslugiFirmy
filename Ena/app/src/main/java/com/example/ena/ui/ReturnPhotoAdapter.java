package com.example.ena.ui;

import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.R;
import com.example.ena.api.ApiConfig;
import com.example.ena.api.ReturnPhotoDto;
import java.util.ArrayList;
import java.util.List;

public class ReturnPhotoAdapter extends RecyclerView.Adapter<ReturnPhotoAdapter.ViewHolder> {
    private final List<ReturnPhotoDto> items = new ArrayList<>();
    private final Context context;

    public ReturnPhotoAdapter(Context context) {
        this.context = context;
    }

    public void setItems(List<ReturnPhotoDto> data) {
        items.clear();
        if (data != null) {
            items.addAll(data);
        }
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_return_photo, parent, false);
        return new ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
        ReturnPhotoDto photo = items.get(position);
        holder.txtName.setText(photo.getFileName() != null ? photo.getFileName() : "Zdjęcie");
        String meta = buildMeta(photo);
        holder.txtMeta.setText(meta);
        holder.itemView.setOnClickListener(v -> openPhoto(photo));
    }

    @Override
    public int getItemCount() {
        return items.size();
    }

    private void openPhoto(ReturnPhotoDto photo) {
        String url = photo.getUrl();
        if (url == null || url.trim().isEmpty()) {
            return;
        }
        String base = ApiConfig.getBaseUrl(context);
        if (base != null && url.startsWith("/")) {
            url = base.endsWith("/") ? base.substring(0, base.length() - 1) + url : base + url;
        }
        Intent intent = new Intent(Intent.ACTION_VIEW, Uri.parse(url));
        intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        context.startActivity(intent);
    }

    private String buildMeta(ReturnPhotoDto photo) {
        StringBuilder sb = new StringBuilder();
        if (photo.getAddedAt() != null && !photo.getAddedAt().isEmpty()) {
            sb.append(photo.getAddedAt());
        }
        if (photo.getAddedByName() != null && !photo.getAddedByName().isEmpty()) {
            if (sb.length() > 0) {
                sb.append(" • ");
            }
            sb.append(photo.getAddedByName());
        }
        return sb.length() == 0 ? "Brak danych" : sb.toString();
    }

    static class ViewHolder extends RecyclerView.ViewHolder {
        final TextView txtName;
        final TextView txtMeta;

        ViewHolder(@NonNull View itemView) {
            super(itemView);
            txtName = itemView.findViewById(R.id.txtPhotoName);
            txtMeta = itemView.findViewById(R.id.txtPhotoMeta);
        }
    }
}
