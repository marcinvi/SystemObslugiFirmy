package com.example.ena.ui;

import android.app.DatePickerDialog;
import android.content.Intent; // WAŻNE: Dodaj import Intent
import android.os.Bundle;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.RadioButton;
import android.widget.RadioGroup;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.ena.R;
import com.example.ena.SendLinkActivity; // WAŻNE: Import Twojej aktywności wysyłania
import com.example.ena.api.ApiClient;
import com.example.ena.api.DelegacjaDto;
import com.example.ena.api.SimpleUserDto;
import com.example.ena.api.UserProfileDto;
import com.google.android.material.textfield.TextInputEditText;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;
import java.util.Locale;

public class UserProfileActivity extends AppCompatActivity {

    private TextInputEditText editEmail, editPhone;
    private EditText editOldPass, editNewPass;
    private Button btnSaveContact, btnChangePass, btnDateFrom, btnDateTo, btnAddAbsence;
    // NOWY PRZYCISK
    private Button btnOpenSendLink;

    private RadioGroup radioGroupType;
    private Spinner spinnerReplacement;
    private LinearLayout layoutAbsencesList;
    private TextView txtProfileHeader;

    private Calendar calendarFrom = Calendar.getInstance();
    private Calendar calendarTo = Calendar.getInstance();
    private SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd", Locale.getDefault());

    private List<SimpleUserDto> replacementUsers = new ArrayList<>();
    private ApiClient apiClient;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_user_profile);

        apiClient = new ApiClient(this);

        initViews();
        setupListeners();
        loadReplacementUsers();
    }

    private void initViews() {
        txtProfileHeader = findViewById(R.id.txtProfileHeader);

        // Inicjalizacja nowego przycisku
        btnOpenSendLink = findViewById(R.id.btnOpenSendLink);

        editEmail = findViewById(R.id.editEmail);
        editPhone = findViewById(R.id.editPhone);
        btnSaveContact = findViewById(R.id.btnSaveContact);

        editOldPass = findViewById(R.id.editOldPass);
        editNewPass = findViewById(R.id.editNewPass);
        btnChangePass = findViewById(R.id.btnChangePass);

        btnDateFrom = findViewById(R.id.btnDateFrom);
        btnDateTo = findViewById(R.id.btnDateTo);
        radioGroupType = findViewById(R.id.radioGroupType);
        spinnerReplacement = findViewById(R.id.spinnerReplacement);
        btnAddAbsence = findViewById(R.id.btnAddAbsence);
        layoutAbsencesList = findViewById(R.id.layoutAbsencesList);

        updateDateButton(btnDateFrom, calendarFrom);
        updateDateButton(btnDateTo, calendarTo);
    }

    private void setupListeners() {
        // OBSŁUGA NOWEGO PRZYCISKU
        btnOpenSendLink.setOnClickListener(v -> {
            Intent intent = new Intent(UserProfileActivity.this, SendLinkActivity.class);
            startActivity(intent);
        });

        btnSaveContact.setOnClickListener(v -> saveContactInfo());
        btnChangePass.setOnClickListener(v -> changePassword());

        btnDateFrom.setOnClickListener(v -> showDatePicker(calendarFrom, btnDateFrom));
        btnDateTo.setOnClickListener(v -> showDatePicker(calendarTo, btnDateTo));

        btnAddAbsence.setOnClickListener(v -> addAbsence());
    }

    // ... RESZTA KODU BEZ ZMIAN (loadProfileData, saveContactInfo, itp.) ...

    private void loadProfileData() {
        apiClient.getProfile(new ApiClient.ApiCallback<UserProfileDto>() {
            @Override
            public void onSuccess(UserProfileDto data) {
                runOnUiThread(() -> {
                    try {
                        txtProfileHeader.setText("Witaj, " + (data.nazwaWyswietlana != null ? data.nazwaWyswietlana : data.login));
                        editEmail.setText(data.email);
                        editPhone.setText(data.telefon);

                        layoutAbsencesList.removeAllViews();
                        if (data.delegacje != null) {
                            for (DelegacjaDto d : data.delegacje) {
                                addAbsenceView(d);
                            }
                        }
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> Toast.makeText(UserProfileActivity.this, "Błąd profilu: " + message, Toast.LENGTH_SHORT).show());
            }
        });
    }

    private void addAbsenceView(DelegacjaDto d) {
        TextView tv = new TextView(this);
        String from = d.dataOd != null ? d.dataOd.split("T")[0] : "";
        String to = d.dataDo != null ? d.dataDo.split("T")[0] : "";
        String zast = d.zastepcaNazwa != null ? d.zastepcaNazwa : "Brak";

        tv.setText(String.format("• %s (%s do %s)\n  Zastępstwo: %s", d.typ, from, to, zast));
        tv.setPadding(0, 0, 0, 16);
        layoutAbsencesList.addView(tv);
    }

    private void loadReplacementUsers() {
        apiClient.getReplacementUsers(new ApiClient.ApiCallback<List<SimpleUserDto>>() {
            @Override
            public void onSuccess(List<SimpleUserDto> data) {
                runOnUiThread(() -> {
                    replacementUsers.clear();
                    SimpleUserDto none = new SimpleUserDto();
                    none.id = 0;
                    none.name = "Brak zastępstwa";
                    replacementUsers.add(none);

                    if (data != null) {
                        replacementUsers.addAll(data);
                    }

                    ArrayAdapter<SimpleUserDto> adapter = new ArrayAdapter<>(
                            UserProfileActivity.this,
                            android.R.layout.simple_spinner_dropdown_item,
                            replacementUsers
                    );
                    spinnerReplacement.setAdapter(adapter);
                    loadProfileData();
                });
            }
            @Override
            public void onError(String message) {
                runOnUiThread(() -> loadProfileData());
            }
        });
    }

    private void saveContactInfo() {
        String email = editEmail.getText().toString();
        String phone = editPhone.getText().toString();

        apiClient.updateContact(email, phone, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> Toast.makeText(UserProfileActivity.this, "Zapisano dane kontaktowe", Toast.LENGTH_SHORT).show());
            }
            @Override
            public void onError(String message) {
                runOnUiThread(() -> Toast.makeText(UserProfileActivity.this, "Błąd: " + message, Toast.LENGTH_SHORT).show());
            }
        });
    }

    private void changePassword() {
        String oldP = editOldPass.getText().toString();
        String newP = editNewPass.getText().toString();

        if (newP.length() < 4) {
            Toast.makeText(this, "Hasło za krótkie", Toast.LENGTH_SHORT).show();
            return;
        }

        apiClient.changePassword(oldP, newP, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    Toast.makeText(UserProfileActivity.this, "Hasło zmienione!", Toast.LENGTH_LONG).show();
                    editOldPass.setText("");
                    editNewPass.setText("");
                });
            }
            @Override
            public void onError(String message) {
                runOnUiThread(() -> Toast.makeText(UserProfileActivity.this, "Błąd zmiany hasła: " + message, Toast.LENGTH_LONG).show());
            }
        });
    }

    private void addAbsence() {
        String type = "Urlop";
        int checkedId = radioGroupType.getCheckedRadioButtonId();
        if (checkedId != -1) {
            RadioButton rb = findViewById(checkedId);
            if (rb != null) type = rb.getText().toString();
        }

        SimpleUserDto selectedUser = (SimpleUserDto) spinnerReplacement.getSelectedItem();
        int zastepcaId = selectedUser != null ? selectedUser.id : 0;

        String dateOd = sdf.format(calendarFrom.getTime());
        String dateDo = sdf.format(calendarTo.getTime());

        apiClient.addAbsence(dateOd, dateDo, type, zastepcaId, new ApiClient.ApiCallback<Void>() {
            @Override
            public void onSuccess(Void data) {
                runOnUiThread(() -> {
                    Toast.makeText(UserProfileActivity.this, "Dodano nieobecność", Toast.LENGTH_SHORT).show();
                    loadProfileData();
                });
            }
            @Override
            public void onError(String message) {
                runOnUiThread(() -> Toast.makeText(UserProfileActivity.this, "Błąd: " + message, Toast.LENGTH_SHORT).show());
            }
        });
    }

    private void showDatePicker(Calendar calendar, Button btn) {
        new DatePickerDialog(this, (view, year, month, dayOfMonth) -> {
            calendar.set(year, month, dayOfMonth);
            updateDateButton(btn, calendar);
        }, calendar.get(Calendar.YEAR), calendar.get(Calendar.MONTH), calendar.get(Calendar.DAY_OF_MONTH)).show();
    }

    private void updateDateButton(Button btn, Calendar cal) {
        btn.setText(sdf.format(cal.getTime()));
    }
}