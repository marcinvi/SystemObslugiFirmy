package com.example.ena.ui;

import android.Manifest;
import android.app.ProgressDialog;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.AutoCompleteTextView;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;
import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.annotation.NonNull;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import androidx.core.content.FileProvider;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.ManualReturnMetaDto;
import com.example.ena.api.ManualReturnRecipientDto;
import com.example.ena.api.ReturnManualCreateRequest;
import com.example.ena.api.ReturnPhotoDto;
import com.example.ena.api.StatusDto;
import com.google.android.material.tabs.TabLayout;
import com.google.android.material.textfield.TextInputEditText;
import com.google.android.material.textfield.TextInputLayout;
import com.journeyapps.barcodescanner.ScanContract;
import com.journeyapps.barcodescanner.ScanIntentResult;
import com.journeyapps.barcodescanner.ScanOptions;
import java.io.File;
import java.util.ArrayList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class ManualReturnActivity extends AppCompatActivity {

    public static final String EXTRA_WAYBILL = "extra_waybill";
    private static final int CAMERA_PERMISSION_REQUEST = 2002;

    // --- UI ELEMENTS ---
    private TabLayout tabLayout;
    private LinearLayout sectionTab1, sectionTab2, sectionTab3;
    private TextView txtTitle;
    private Button btnMainAction;
    private Button btnAddPhoto;
    private View loadingOverlay;

    // STEP 1
    private TextInputEditText etWaybill, etClientName, etContact, etStreet, etZip, etCity;
    private AutoCompleteTextView etCarrier;
    private TextInputLayout inputLayoutWaybill, inputLayoutClient;

    // STEP 2
    private AutoCompleteTextView etProduct;
    private TextInputEditText etNotes;

    // STEP 3
    private Spinner spinnerStanProduktu;
    private CheckBox chkAllSales;
    private LinearLayout layoutHandlowcy;
    private RecyclerView recyclerPhotos;

    // --- DATA ---
    private int currentStep = 0; // 0, 1, 2
    private final List<StatusDto> stanProduktuStatuses = new ArrayList<>();
    private final List<ManualReturnRecipientDto> handlowcy = new ArrayList<>();
    private final List<Uri> selectedPhotos = new ArrayList<>();
    private PhotoAdapter photoAdapter;
    private Uri tempCameraUri;

    // --- SCANNERS & PERMISSIONS ---
    private final ActivityResultLauncher<ScanOptions> scanLauncher =
            registerForActivityResult(new ScanContract(), this::handleScanResult);

    private final ActivityResultLauncher<Intent> cameraLauncher =
            registerForActivityResult(new ActivityResultContracts.StartActivityForResult(), result -> {
                if (result.getResultCode() == RESULT_OK && tempCameraUri != null) {
                    addPhoto(tempCameraUri);
                }
            });

    private final ActivityResultLauncher<String> galleryLauncher =
            registerForActivityResult(new ActivityResultContracts.GetContent(), uri -> {
                if (uri != null) {
                    addPhoto(uri);
                }
            });

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_manual_return);

        initViews();
        setupTabs();
        setupPhotos();
        setupSmartZipCode(); // <--- Zmieniona logika w środku

        String waybill = getIntent().getStringExtra(EXTRA_WAYBILL);
        if (waybill != null) {
            etWaybill.setText(waybill.trim());
        }

        loadStatuses();
        loadManualMeta();

        updateStepUI();
    }

    private void initViews() {
        ImageButton btnBack = findViewById(R.id.btnBack);
        btnBack.setOnClickListener(v -> handleBackNavigation());

        txtTitle = findViewById(R.id.txtTitle);
        tabLayout = findViewById(R.id.tabLayout);

        if (tabLayout.getChildCount() > 0) {
            View tabStrip = tabLayout.getChildAt(0);
            if (tabStrip instanceof ViewGroup) {
                ViewGroup group = (ViewGroup) tabStrip;
                for(int i = 0; i < group.getChildCount(); i++) {
                    group.getChildAt(i).setOnTouchListener((v, event) -> true);
                }
            }
        }

        sectionTab1 = findViewById(R.id.sectionTab1);
        sectionTab2 = findViewById(R.id.sectionTab2);
        sectionTab3 = findViewById(R.id.sectionTab3);

        ImageButton btnScan = findViewById(R.id.btnScan);
        btnScan.setOnClickListener(v -> startScan());

        etWaybill = findViewById(R.id.etWaybill);
        etCarrier = findViewById(R.id.etCarrier);
        etClientName = findViewById(R.id.etClientName);
        etContact = findViewById(R.id.etContact);
        etStreet = findViewById(R.id.etStreet);
        etZip = findViewById(R.id.etZip);
        etCity = findViewById(R.id.etCity);
        inputLayoutWaybill = findViewById(R.id.inputLayoutWaybill);
        inputLayoutClient = findViewById(R.id.inputLayoutClient);

        etProduct = findViewById(R.id.etProduct);
        etNotes = findViewById(R.id.etNotes);

        spinnerStanProduktu = findViewById(R.id.spinnerStanProduktu);
        chkAllSales = findViewById(R.id.chkAllSales);
        layoutHandlowcy = findViewById(R.id.layoutHandlowcy);
        recyclerPhotos = findViewById(R.id.recyclerPhotos);
        btnAddPhoto = findViewById(R.id.btnAddPhoto);
        btnAddPhoto.setOnClickListener(v -> showPhotoSourceDialog());

        loadingOverlay = findViewById(R.id.loadingOverlay);
        btnMainAction = findViewById(R.id.btnMainAction);
        btnMainAction.setOnClickListener(v -> handleNextStep());

        chkAllSales.setOnCheckedChangeListener((buttonView, isChecked) -> setAllRecipientsChecked(isChecked));

        TextWatcher clearErrorWatcher = new TextWatcher() {
            @Override public void beforeTextChanged(CharSequence s, int start, int count, int after) {}
            @Override public void onTextChanged(CharSequence s, int start, int before, int count) {
                inputLayoutWaybill.setError(null);
                inputLayoutClient.setError(null);
            }
            @Override public void afterTextChanged(Editable s) {}
        };
        etWaybill.addTextChangedListener(clearErrorWatcher);
        etClientName.addTextChangedListener(clearErrorWatcher);
    }

    private void setupTabs() {
        tabLayout.addOnTabSelectedListener(new TabLayout.OnTabSelectedListener() {
            @Override public void onTabSelected(TabLayout.Tab tab) {}
            @Override public void onTabUnselected(TabLayout.Tab tab) {}
            @Override public void onTabReselected(TabLayout.Tab tab) {}
        });
    }

    // =========================================================
    // NOWA LOGIKA: ZEWNĘTRZNE API KODÓW POCZTOWYCH
    // =========================================================
    private void setupSmartZipCode() {
        etZip.addTextChangedListener(new TextWatcher() {
            private boolean isFormatting;

            @Override public void beforeTextChanged(CharSequence s, int start, int count, int after) {}
            @Override public void onTextChanged(CharSequence s, int start, int before, int count) {}

            @Override
            public void afterTextChanged(Editable s) {
                if (isFormatting) return;
                isFormatting = true;

                String original = s.toString();
                String digits = original.replaceAll("[^\\d]", "");

                if (digits.length() > 5) digits = digits.substring(0, 5);

                String formatted = digits;
                if (digits.length() > 2) {
                    formatted = digits.substring(0, 2) + "-" + digits.substring(2);
                }

                if (!formatted.equals(original)) {
                    etZip.setText(formatted);
                    try { etZip.setSelection(formatted.length()); } catch (Exception ignored) {}
                }

                // Jak wpisano pełny kod (np. 83-000), pytamy API
                if (formatted.length() == 6) {
                    fetchCityFromApi(formatted);
                }

                isFormatting = false;
            }
        });
    }

    private void fetchCityFromApi(String zipCode) {
        // Czyścimy miasto przed szukaniem, żeby user widział że coś się dzieje
        // (opcjonalnie można dodać mały progressbar w polu tekstowym, ale tutaj wystarczy brak akcji)

        ApiClient client = new ApiClient(this);
        client.fetchExternalZipCode(zipCode, new ApiClient.ApiCallback<List<String>>() {
            @Override
            public void onSuccess(List<String> cities) {
                runOnUiThread(() -> handleZipResponse(cities));
            }

            @Override
            public void onError(String message) {
                // Ignorujemy błędy po cichu (nie przerywamy pracy usera)
            }
        });
    }

    private void handleZipResponse(List<String> cities) {
        if (cities == null || cities.isEmpty()) return;

        if (cities.size() == 1) {
            // Tylko jedna unikalna miejscowość - wpisujemy od razu
            etCity.setText(cities.get(0));
        } else {
            // Kilka miejscowości (np. wsie pod jednym kodem) - dajemy wybór
            String[] citiesArray = cities.toArray(new String[0]);
            new AlertDialog.Builder(ManualReturnActivity.this)
                    .setTitle("Wybierz miejscowość")
                    .setItems(citiesArray, (dialog, which) -> {
                        etCity.setText(citiesArray[which]);
                    })
                    .show();
        }
    }
    // =========================================================

    // --- WIZARD ---
    private void updateStepUI() {
        sectionTab1.setVisibility(currentStep == 0 ? View.VISIBLE : View.GONE);
        sectionTab2.setVisibility(currentStep == 1 ? View.VISIBLE : View.GONE);
        sectionTab3.setVisibility(currentStep == 2 ? View.VISIBLE : View.GONE);

        TabLayout.Tab tab = tabLayout.getTabAt(currentStep);
        if (tab != null) tab.select();

        switch (currentStep) {
            case 0:
                txtTitle.setText("Krok 1/3: Dane");
                btnMainAction.setText("DALEJ >");
                btnMainAction.setBackgroundTintList(ContextCompat.getColorStateList(this, R.color.colorPrimary));
                break;
            case 1:
                txtTitle.setText("Krok 2/3: Produkt");
                btnMainAction.setText("DALEJ >");
                btnMainAction.setBackgroundTintList(ContextCompat.getColorStateList(this, R.color.colorPrimary));
                break;
            case 2:
                txtTitle.setText("Krok 3/3: Decyzja");
                btnMainAction.setText("ZAKOŃCZ I UTWÓRZ");
                btnMainAction.setBackgroundTintList(ContextCompat.getColorStateList(this, android.R.color.holo_green_dark));
                break;
        }
    }

    private void handleNextStep() {
        if (currentStep == 0) {
            if (validateStep1()) { currentStep = 1; updateStepUI(); }
        } else if (currentStep == 1) {
            currentStep = 2; updateStepUI();
        } else if (currentStep == 2) {
            if (validateStep3()) submitManualReturn();
        }
    }

    private void handleBackNavigation() {
        if (currentStep > 0) { currentStep--; updateStepUI(); } else finish();
    }

    @Override public void onBackPressed() { handleBackNavigation(); }

    private boolean validateStep1() {
        boolean valid = true;
        if (etWaybill.getText().toString().trim().isEmpty()) {
            inputLayoutWaybill.setError("Wymagany numer"); valid = false;
        }
        if (etClientName.getText().toString().trim().isEmpty()) {
            inputLayoutClient.setError("Wymagany klient"); valid = false;
        }
        return valid;
    }

    private boolean validateStep3() {
        if (getSelectedStatusId() <= 0) {
            Toast.makeText(this, "Wybierz status (decyzję)!", Toast.LENGTH_SHORT).show(); return false;
        }
        if (getSelectedRecipients().isEmpty()) {
            Toast.makeText(this, "Wybierz handlowca!", Toast.LENGTH_SHORT).show(); return false;
        }
        return true;
    }

    // --- ZDJĘCIA ---
    private void setupPhotos() {
        photoAdapter = new PhotoAdapter(selectedPhotos, this::removePhoto);
        recyclerPhotos.setLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.HORIZONTAL, false));
        recyclerPhotos.setAdapter(photoAdapter);
    }

    private void showPhotoSourceDialog() {
        new AlertDialog.Builder(this).setTitle("Dodaj zdjęcie")
                .setItems(new String[]{"Zrób zdjęcie", "Wybierz z galerii"}, (dialog, which) -> {
                    if (which == 0) startCamera(); else galleryLauncher.launch("image/*");
                }).show();
    }

    private void startCamera() {
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.CAMERA) != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.CAMERA}, CAMERA_PERMISSION_REQUEST);
            return;
        }
        try {
            File photoFile = File.createTempFile("RETURN_", ".jpg", getCacheDir());
            tempCameraUri = FileProvider.getUriForFile(this, getPackageName() + ".fileprovider", photoFile);
            cameraLauncher.launch(new Intent(MediaStore.ACTION_IMAGE_CAPTURE).putExtra(MediaStore.EXTRA_OUTPUT, tempCameraUri));
        } catch (Exception e) { Toast.makeText(this, "Błąd kamery: " + e.getMessage(), Toast.LENGTH_SHORT).show(); }
    }

    private void addPhoto(Uri uri) { selectedPhotos.add(uri); photoAdapter.notifyItemInserted(selectedPhotos.size() - 1); }
    private void removePhoto(int position) { selectedPhotos.remove(position); photoAdapter.notifyItemRemoved(position); }

    // --- WYSYŁKA ---
    private void submitManualReturn() {
        showLoading(true);

        ReturnManualCreateRequest request = new ReturnManualCreateRequest(
                etWaybill.getText().toString().trim(),
                emptyToNull(etProduct.getText().toString().trim()),
                emptyToNull(etCarrier.getText().toString().trim()),
                getSelectedStatusId(),
                emptyToNull(etNotes.getText().toString().trim()),
                etClientName.getText().toString().trim(),
                emptyToNull(etStreet.getText().toString().trim()),
                emptyToNull(etZip.getText().toString().trim()),
                emptyToNull(etCity.getText().toString().trim()),
                emptyToNull(etContact.getText().toString().trim()),
                getSelectedRecipients()
        );

        ApiClient client = new ApiClient(this);
        // 1. Utwórz zwrot (teraz otrzymamy ID!)
        client.createManualReturn(request, new ApiClient.ApiCallback<Integer>() {
            @Override
            public void onSuccess(Integer newReturnId) {
                // 2. Jeśli są zdjęcia, wysyłamy je pod to ID
                if (newReturnId != null && !selectedPhotos.isEmpty()) {
                    uploadPhotos(newReturnId);
                } else {
                    finishSuccess("Zwrot utworzony pomyślnie!");
                }
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    showLoading(false);
                    Toast.makeText(ManualReturnActivity.this, "Błąd zapisu: " + message, Toast.LENGTH_LONG).show();
                });
            }
        });
    }

    private void uploadPhotos(int returnId) {
        runOnUiThread(() -> {
            ProgressDialog pd = new ProgressDialog(this);
            pd.setMessage("Wysyłanie zdjęć...");
            pd.setCancelable(false);
            pd.show();

            ApiClient client = new ApiClient(this);
            client.uploadReturnPhotos(returnId, selectedPhotos, new ApiClient.ApiCallback<List<ReturnPhotoDto>>() {
                @Override
                public void onSuccess(List<ReturnPhotoDto> data) {
                    runOnUiThread(() -> {
                        pd.dismiss();
                        finishSuccess("Zwrot utworzony i dodano " + data.size() + " zdjęć.");
                    });
                }

                @Override
                public void onError(String message) {
                    runOnUiThread(() -> {
                        pd.dismiss();
                        finishSuccess("Zwrot utworzony, ale błąd zdjęć: " + message);
                    });
                }
            });
        });
    }

    private void finishSuccess(String message) {
        runOnUiThread(() -> {
            showLoading(false);
            Toast.makeText(ManualReturnActivity.this, message, Toast.LENGTH_LONG).show();
            finish();
        });
    }

    // --- POMOCNICZE ---
    private void loadStatuses() {
        new ApiClient(this).fetchStatuses("StanProduktu", new ApiClient.ApiCallback<List<StatusDto>>() {
            @Override public void onSuccess(List<StatusDto> data) {
                runOnUiThread(() -> {
                    stanProduktuStatuses.clear();
                    if (data != null) stanProduktuStatuses.addAll(data);
                    List<String> names = new ArrayList<>();
                    for (StatusDto s : stanProduktuStatuses) names.add(s.getNazwa());
                    spinnerStanProduktu.setAdapter(new ArrayAdapter<>(ManualReturnActivity.this, android.R.layout.simple_spinner_item, names));
                });
            }
            @Override public void onError(String m) {}
        });
    }

    private void loadManualMeta() {
        new ApiClient(this).fetchManualReturnMeta(new ApiClient.ApiCallback<ManualReturnMetaDto>() {
            @Override public void onSuccess(ManualReturnMetaDto data) {
                runOnUiThread(() -> {
                    if (data != null) {
                        setupRecipients(data.getHandlowcy());
                        if (data.getPrzewoznicy() != null) etCarrier.setAdapter(new ArrayAdapter<>(ManualReturnActivity.this, android.R.layout.simple_dropdown_item_1line, data.getPrzewoznicy()));
                        if (data.getProdukty() != null) etProduct.setAdapter(new ArrayAdapter<>(ManualReturnActivity.this, android.R.layout.simple_dropdown_item_1line, data.getProdukty()));
                    }
                });
            }
            @Override public void onError(String m) {}
        });
    }

    private void setupRecipients(List<ManualReturnRecipientDto> items) {
        layoutHandlowcy.removeAllViews();
        handlowcy.clear();
        if (items != null) handlowcy.addAll(items);
        for (ManualReturnRecipientDto r : handlowcy) {
            CheckBox cb = new CheckBox(this);
            cb.setText(r.getNazwaWyswietlana());
            cb.setTag(r.getId());
            layoutHandlowcy.addView(cb);
        }
    }

    private void setAllRecipientsChecked(boolean checked) {
        for(int i=0; i<layoutHandlowcy.getChildCount(); i++) {
            ((CheckBox)layoutHandlowcy.getChildAt(i)).setChecked(checked);
        }
    }

    private int getSelectedStatusId() {
        int idx = spinnerStanProduktu.getSelectedItemPosition();
        return (idx >= 0 && idx < stanProduktuStatuses.size()) ? stanProduktuStatuses.get(idx).getId() : 0;
    }

    private List<Integer> getSelectedRecipients() {
        List<Integer> ids = new ArrayList<>();
        for (int i = 0; i < layoutHandlowcy.getChildCount(); i++) {
            CheckBox cb = (CheckBox) layoutHandlowcy.getChildAt(i);
            if (cb.isChecked()) ids.add((Integer) cb.getTag());
        }
        return ids;
    }

    private String emptyToNull(String s) { return s.isEmpty() ? null : s; }

    private void showLoading(boolean show) {
        loadingOverlay.setVisibility(show ? View.VISIBLE : View.GONE);
        btnMainAction.setEnabled(!show);
    }

    private void startScan() {
        ScanOptions options = new ScanOptions();
        options.setPrompt("Skanuj etykietę");
        options.setBeepEnabled(true);
        options.setOrientationLocked(true);
        scanLauncher.launch(options);
    }

    private void handleScanResult(ScanIntentResult result) {
        if (result.getContents() != null) {
            String scanned = result.getContents().trim();
            Matcher m = Pattern.compile("^%.{7}([a-zA-Z0-9]{14})").matcher(scanned);
            if (m.find()) etWaybill.setText(m.group(1));
            else etWaybill.setText(scanned.replaceAll("[^a-zA-Z0-9]", ""));
        }
    }

    private static class PhotoAdapter extends RecyclerView.Adapter<PhotoAdapter.Holder> {
        private final List<Uri> uris;
        private final OnRemoveListener listener;
        interface OnRemoveListener { void onRemove(int pos); }
        PhotoAdapter(List<Uri> uris, OnRemoveListener listener) { this.uris = uris; this.listener = listener; }
        @NonNull @Override public Holder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
            ImageView iv = new ImageView(parent.getContext());
            iv.setLayoutParams(new ViewGroup.LayoutParams(200, 200));
            iv.setScaleType(ImageView.ScaleType.CENTER_CROP);
            iv.setPadding(8,8,8,8);
            return new Holder(iv);
        }
        @Override public void onBindViewHolder(@NonNull Holder holder, int position) {
            ((ImageView)holder.itemView).setImageURI(uris.get(position));
            holder.itemView.setOnClickListener(v -> listener.onRemove(position));
        }
        @Override public int getItemCount() { return uris.size(); }
        static class Holder extends RecyclerView.ViewHolder { Holder(View v) { super(v); } }
    }
}