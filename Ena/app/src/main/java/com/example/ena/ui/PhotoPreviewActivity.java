package com.example.ena.ui;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Bundle;
import android.util.DisplayMetrics;
import android.view.View;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;
import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;
import com.example.ena.R;
import com.example.ena.api.ApiClient;
import java.io.File;

public class PhotoPreviewActivity extends AppCompatActivity {
    public static final String EXTRA_PHOTO_ID = "extra_photo_id";
    public static final String EXTRA_PHOTO_NAME = "extra_photo_name";

    private ImageView imgPreview;
    private ProgressBar progressPreview;
    private TextView txtPhotoName;
    private TextView txtPhotoError;

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_photo_preview);

        imgPreview = findViewById(R.id.imgPreview);
        progressPreview = findViewById(R.id.progressPreview);
        txtPhotoName = findViewById(R.id.txtPhotoName);
        txtPhotoError = findViewById(R.id.txtPhotoError);
        ImageButton btnClose = findViewById(R.id.btnClosePreview);

        btnClose.setOnClickListener(v -> finish());

        int photoId = getIntent().getIntExtra(EXTRA_PHOTO_ID, -1);
        String photoName = getIntent().getStringExtra(EXTRA_PHOTO_NAME);
        txtPhotoName.setText((photoName == null || photoName.trim().isEmpty()) ? getString(R.string.photo_preview_default_name) : photoName);

        if (photoId <= 0) {
            showError(getString(R.string.photo_preview_error_invalid_id));
            return;
        }

        loadPhoto(photoId);
    }

    private void loadPhoto(int photoId) {
        progressPreview.setVisibility(View.VISIBLE);
        txtPhotoError.setVisibility(View.GONE);

        File cacheDir = new File(getCacheDir(), "photo_cache");
        if (!cacheDir.exists()) {
            cacheDir.mkdirs();
        }

        ApiClient apiClient = new ApiClient(this);
        apiClient.fetchReturnPhoto(photoId, cacheDir, new ApiClient.PhotoCallback() {
            @Override
            public void onSuccess(File file) {
                showBitmap(file);
            }

            @Override
            public void onCachedPhoto(File file) {
                showBitmap(file);
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> showError(message));
            }
        });
    }

    private void showBitmap(File file) {
        Bitmap bitmap = decodeForScreen(file);
        runOnUiThread(() -> {
            progressPreview.setVisibility(View.GONE);
            if (bitmap == null) {
                showError(getString(R.string.photo_preview_error_decode));
                return;
            }
            imgPreview.setImageBitmap(bitmap);
            imgPreview.setVisibility(View.VISIBLE);
            txtPhotoError.setVisibility(View.GONE);
        });
    }

    private Bitmap decodeForScreen(File file) {
        BitmapFactory.Options bounds = new BitmapFactory.Options();
        bounds.inJustDecodeBounds = true;
        BitmapFactory.decodeFile(file.getAbsolutePath(), bounds);

        DisplayMetrics metrics = getResources().getDisplayMetrics();
        int reqWidth = Math.max(metrics.widthPixels, 1080);
        int reqHeight = Math.max(metrics.heightPixels, 1920);

        BitmapFactory.Options options = new BitmapFactory.Options();
        options.inSampleSize = calculateInSampleSize(bounds, reqWidth, reqHeight);
        options.inPreferredConfig = Bitmap.Config.RGB_565;
        return BitmapFactory.decodeFile(file.getAbsolutePath(), options);
    }

    private int calculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight) {
        int height = options.outHeight;
        int width = options.outWidth;
        int inSampleSize = 1;

        if (height > reqHeight || width > reqWidth) {
            int halfHeight = height / 2;
            int halfWidth = width / 2;

            while ((halfHeight / inSampleSize) >= reqHeight && (halfWidth / inSampleSize) >= reqWidth) {
                inSampleSize *= 2;
            }
        }
        return Math.max(inSampleSize, 1);
    }

    private void showError(String message) {
        progressPreview.setVisibility(View.GONE);
        imgPreview.setVisibility(View.GONE);
        txtPhotoError.setVisibility(View.VISIBLE);
        txtPhotoError.setText(message);
        Toast.makeText(this, message, Toast.LENGTH_SHORT).show();
    }
}
