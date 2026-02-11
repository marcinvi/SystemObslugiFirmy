// ==========================================
// ULEPSZONY - ReturnPhotoAdapter.java
// ==========================================

package com.example.ena.ui;

import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.ReturnPhotoDto;
import java.io.File;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class ReturnPhotoAdapter extends RecyclerView.Adapter<ReturnPhotoAdapter.ViewHolder> {
    private final List<ReturnPhotoDto> items = new ArrayList<>();
    private final Context context;
    private final File cacheDir;
    private final ExecutorService executor;
    private final ApiClient apiClient;

    public ReturnPhotoAdapter(Context context) {
        this.context = context;
        this.cacheDir = new File(context.getCacheDir(), "photo_cache");
        if (!cacheDir.exists()) {
            cacheDir.mkdirs();
        }
        this.executor = Executors.newFixedThreadPool(3);
        this.apiClient = new ApiClient(context);
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
        View view = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.item_return_photo, parent, false);
        return new ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
        ReturnPhotoDto photo = items.get(position);
        holder.boundPhotoId = photo.getId();

        holder.txtName.setText(photo.getFileName() != null ? photo.getFileName() : "Zdjęcie");
        holder.txtMeta.setText(buildMeta(photo));

        holder.imgThumbnail.setVisibility(View.GONE);
        holder.progressBar.setVisibility(View.VISIBLE);
        holder.imgThumbnail.setImageDrawable(null);

        loadThumbnail(holder, photo);

        holder.itemView.setOnClickListener(v -> openPhotoInApp(photo));
    }

    @Override
    public int getItemCount() {
        return items.size();
    }

    private void loadThumbnail(ViewHolder holder, ReturnPhotoDto photo) {
        int photoId = photo.getId();
        executor.submit(() -> {
            File cachedFile = new File(cacheDir, "thumb_" + photoId + ".jpg");

            if (cachedFile.exists()) {
                Bitmap bitmap = BitmapFactory.decodeFile(cachedFile.getAbsolutePath());
                if (bitmap != null) {
                    showThumbnail(holder, bitmap, photoId);
                    return;
                }
            }

            apiClient.fetchReturnPhoto(photoId, cacheDir, new ApiClient.PhotoCallback() {
                @Override
                public void onSuccess(File file) {
                    Bitmap fullBitmap = BitmapFactory.decodeFile(file.getAbsolutePath());
                    if (fullBitmap != null) {
                        Bitmap thumbnail = createThumbnail(fullBitmap);
                        saveThumbnail(thumbnail, cachedFile);
                        showThumbnail(holder, thumbnail, photoId);
                    } else {
                        hideProgress(holder, photoId);
                    }
                }

                @Override
                public void onCachedPhoto(File file) {
                    onSuccess(file);
                }

                @Override
                public void onError(String message) {
                    hideProgress(holder, photoId);
                }
            });
        });
    }

    private Bitmap createThumbnail(Bitmap source) {
        int targetWidth = 200;
        int targetHeight = (int) (source.getHeight() * (targetWidth / (float) source.getWidth()));
        return Bitmap.createScaledBitmap(source, targetWidth, targetHeight, true);
    }

    private void saveThumbnail(Bitmap thumbnail, File file) {
        try (java.io.FileOutputStream out = new java.io.FileOutputStream(file)) {
            thumbnail.compress(Bitmap.CompressFormat.JPEG, 80, out);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void showThumbnail(ViewHolder holder, Bitmap bitmap, int photoId) {
        if (holder.itemView.getHandler() != null) {
            holder.itemView.post(() -> {
                if (holder.boundPhotoId != photoId) {
                    return;
                }
                holder.imgThumbnail.setImageBitmap(bitmap);
                holder.imgThumbnail.setVisibility(View.VISIBLE);
                holder.progressBar.setVisibility(View.GONE);
            });
        }
    }

    private void hideProgress(ViewHolder holder, int photoId) {
        if (holder.itemView.getHandler() != null) {
            holder.itemView.post(() -> {
                if (holder.boundPhotoId != photoId) {
                    return;
                }
                holder.progressBar.setVisibility(View.GONE);
            });
        }
    }

    private void openPhotoInApp(ReturnPhotoDto photo) {
        Intent intent = new Intent(context, PhotoPreviewActivity.class);
        intent.putExtra(PhotoPreviewActivity.EXTRA_PHOTO_ID, photo.getId());
        intent.putExtra(PhotoPreviewActivity.EXTRA_PHOTO_NAME, photo.getFileName());
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
        if (photo.getSize() != null && photo.getSize() > 0) {
            if (sb.length() > 0) {
                sb.append(" • ");
            }
            sb.append(formatFileSize(photo.getSize()));
        }
        return sb.length() == 0 ? "Brak danych" : sb.toString();
    }

    private String formatFileSize(long bytes) {
        if (bytes < 1024) return bytes + " B";
        if (bytes < 1024 * 1024) return String.format("%.1f KB", bytes / 1024.0);
        return String.format("%.1f MB", bytes / (1024.0 * 1024.0));
    }

    public void cleanup() {
        executor.shutdownNow();
    }

    static class ViewHolder extends RecyclerView.ViewHolder {
        final TextView txtName;
        final TextView txtMeta;
        final ImageView imgThumbnail;
        final ProgressBar progressBar;
        int boundPhotoId;

        ViewHolder(@NonNull View itemView) {
            super(itemView);
            txtName = itemView.findViewById(R.id.txtPhotoName);
            txtMeta = itemView.findViewById(R.id.txtPhotoMeta);
            imgThumbnail = itemView.findViewById(R.id.imgThumbnail);
            progressBar = itemView.findViewById(R.id.progressBar);
        }
    }
}
