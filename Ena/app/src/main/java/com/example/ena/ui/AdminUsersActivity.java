package com.example.ena.ui;

import android.app.AlertDialog;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import com.example.ena.R;
import com.example.ena.api.AdminDtos;
import com.example.ena.api.ApiClient;
import com.google.android.material.floatingactionbutton.FloatingActionButton;

import java.util.ArrayList;
import java.util.List;

public class AdminUsersActivity extends AppCompatActivity {

    private ApiClient apiClient;
    private RecyclerView recycler;
    private SwipeRefreshLayout swipeRefresh;
    private AdminUsersAdapter adapter;
    private FloatingActionButton fabAdd;

    // Lista ról dostępna w systemie
    private final String[] ROLES = {"Admin", "Magazyn", "Handlowiec", "Weryfikacja", "Reklamacje"};
    private final List<AdminDtos.AdminModule> adminModules = new ArrayList<>();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_admin_users);

        apiClient = new ApiClient(this);

        recycler = findViewById(R.id.recyclerUsers);
        swipeRefresh = findViewById(R.id.swipeRefresh);
        fabAdd = findViewById(R.id.fabAddUser);

        recycler.setLayoutManager(new LinearLayoutManager(this));
        adapter = new AdminUsersAdapter(new ArrayList<>(), this::showEditDialog);
        recycler.setAdapter(adapter);

        swipeRefresh.setOnRefreshListener(this::loadUsers);
        fabAdd.setOnClickListener(v -> showAddDialog());

        loadUsers();
    }

    private void loadUsers() {
        swipeRefresh.setRefreshing(true);
        loadModulesThenUsers();
    }

    private void loadModulesThenUsers() {
        apiClient.fetchAdminModules(new ApiClient.ApiCallback<List<AdminDtos.AdminModule>>() {
            @Override
            public void onSuccess(List<AdminDtos.AdminModule> modules) {
                adminModules.clear();
                if (modules != null) {
                    for (AdminDtos.AdminModule module : modules) {
                        if (module != null) {
                            adminModules.add(module);
                        }
                    }
                }
                loadUsersList();
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    Toast.makeText(AdminUsersActivity.this, "Błąd modułów: " + message, Toast.LENGTH_LONG).show();
                    swipeRefresh.setRefreshing(false);
                });
            }
        });
    }

    private void loadUsersList() {
        apiClient.fetchAdminUsers(new ApiClient.ApiCallback<List<AdminDtos.AdminUser>>() {
            @Override
            public void onSuccess(List<AdminDtos.AdminUser> data) {
                runOnUiThread(() -> {
                    adapter.updateData(data);
                    swipeRefresh.setRefreshing(false);
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    Toast.makeText(AdminUsersActivity.this, "Błąd: " + message, Toast.LENGTH_LONG).show();
                    swipeRefresh.setRefreshing(false);
                });
            }
        });
    }

    private void showAddDialog() {
        showUserDialog(null);
    }

    private void showEditDialog(AdminDtos.AdminUser user) {
        showUserDialog(user);
    }

    private void showUserDialog(AdminDtos.AdminUser user) {
        boolean isEdit = (user != null);
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.setTitle(isEdit ? "Edytuj użytkownika" : "Nowy użytkownik");

        View view = LayoutInflater.from(this).inflate(R.layout.dialog_admin_user_edit, null);

        EditText edtLogin = view.findViewById(R.id.edtLogin);
        EditText edtName = view.findViewById(R.id.edtName);
        EditText edtPassword = view.findViewById(R.id.edtPassword);
        TextView txtPassInfo = view.findViewById(R.id.txtPassInfo);
        Spinner spinnerRole = view.findViewById(R.id.spinnerRole);
        LinearLayout layoutModulesContainer = view.findViewById(R.id.layoutModulesContainer);
        CheckBox cbActive = view.findViewById(R.id.cbActive);

        ArrayAdapter<String> roleAdapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_dropdown_item, ROLES);
        spinnerRole.setAdapter(roleAdapter);

        List<CheckBox> moduleCheckboxes = buildModuleCheckboxes(layoutModulesContainer, user != null ? user.moduleIds : null);

        if (isEdit) {
            edtLogin.setText(user.login);
            edtLogin.setEnabled(false);
            edtName.setText(user.nazwaWyswietlana);
            cbActive.setChecked(user.isActive);

            for (int i = 0; i < ROLES.length; i++) {
                if (ROLES[i].equalsIgnoreCase(user.rola)) {
                    spinnerRole.setSelection(i);
                    break;
                }
            }
            txtPassInfo.setVisibility(View.VISIBLE);
        } else {
            txtPassInfo.setVisibility(View.GONE);
            cbActive.setChecked(true);
        }

        builder.setView(view);
        builder.setPositiveButton("Zapisz", null);
        builder.setNegativeButton("Anuluj", (d, w) -> d.dismiss());

        AlertDialog dialog = builder.create();
        dialog.show();

        dialog.getButton(AlertDialog.BUTTON_POSITIVE).setOnClickListener(v -> {
            String login = edtLogin.getText().toString().trim();
            String name = edtName.getText().toString().trim();
            String pass = edtPassword.getText().toString().trim();
            String role = spinnerRole.getSelectedItem().toString();
            boolean active = cbActive.isChecked();
            List<Integer> selectedModuleIds = collectSelectedModuleIds(moduleCheckboxes);

            if (login.isEmpty()) {
                edtLogin.setError("Login wymagany");
                return;
            }

            if (!isEdit && pass.isEmpty()) {
                edtPassword.setError("Hasło wymagane");
                return;
            }

            if (isEdit) {
                AdminDtos.UpdateUserRequest req = new AdminDtos.UpdateUserRequest(name, role, active, selectedModuleIds);
                apiClient.updateAdminUser(user.id, req, new ApiClient.ApiCallback<Void>() {
                    @Override
                    public void onSuccess(Void data) {
                        if (!pass.isEmpty()) {
                            apiClient.resetUserPassword(user.id, pass, new ApiClient.ApiCallback<Void>() {
                                @Override
                                public void onSuccess(Void data) {
                                    finishDialog(dialog, "Zaktualizowano dane i hasło");
                                }

                                @Override
                                public void onError(String msg) {
                                    finishDialog(dialog, "Dane OK, ale błąd hasła: " + msg);
                                }
                            });
                        } else {
                            finishDialog(dialog, "Zaktualizowano dane");
                        }
                    }

                    @Override
                    public void onError(String msg) {
                        runOnUiThread(() -> Toast.makeText(AdminUsersActivity.this, msg, Toast.LENGTH_SHORT).show());
                    }
                });
            } else {
                AdminDtos.CreateUserRequest req = new AdminDtos.CreateUserRequest(login, pass, name, role, selectedModuleIds);
                apiClient.createAdminUser(req, new ApiClient.ApiCallback<Void>() {
                    @Override
                    public void onSuccess(Void data) {
                        finishDialog(dialog, "Utworzono użytkownika");
                    }

                    @Override
                    public void onError(String msg) {
                        runOnUiThread(() -> Toast.makeText(AdminUsersActivity.this, msg, Toast.LENGTH_SHORT).show());
                    }
                });
            }
        });
    }

    private List<CheckBox> buildModuleCheckboxes(LinearLayout container, List<Integer> selectedModuleIds) {
        container.removeAllViews();
        List<CheckBox> checkboxes = new ArrayList<>();

        for (AdminDtos.AdminModule module : adminModules) {
            if (module == null) continue;

            CheckBox checkBox = new CheckBox(this);
            checkBox.setText(module.name != null ? module.name : ("Moduł " + module.id));
            checkBox.setTag(module.id);
            checkBox.setChecked(selectedModuleIds != null && selectedModuleIds.contains(module.id));
            container.addView(checkBox);
            checkboxes.add(checkBox);
        }

        return checkboxes;
    }

    private List<Integer> collectSelectedModuleIds(List<CheckBox> moduleCheckboxes) {
        List<Integer> selected = new ArrayList<>();
        for (CheckBox checkBox : moduleCheckboxes) {
            if (checkBox.isChecked() && checkBox.getTag() instanceof Integer) {
                selected.add((Integer) checkBox.getTag());
            }
        }
        return selected;
    }

    private void finishDialog(AlertDialog dialog, String msg) {
        runOnUiThread(() -> {
            Toast.makeText(AdminUsersActivity.this, msg, Toast.LENGTH_SHORT).show();
            dialog.dismiss();
            loadUsers();
        });
    }
}
