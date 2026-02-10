package com.example.ena.ui;

import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.core.view.GravityCompat;
import androidx.drawerlayout.widget.DrawerLayout;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import com.example.ena.MainActivity;
import com.example.ena.R;
import com.example.ena.api.ApiClient;
import com.example.ena.api.DashboardComplaintDto;
import com.google.android.material.navigation.NavigationView;

import java.util.ArrayList;
import java.util.List;

public class ComplaintsDashboardActivity extends AppCompatActivity
        implements NavigationView.OnNavigationItemSelectedListener {

    // UI Elements
    private RecyclerView recyclerView;
    private SwipeRefreshLayout swipeRefresh;
    private DrawerLayout drawerLayout;
    private NavigationView navigationView;

    // Logic
    private ComplaintsAdapter adapter;
    private ApiClient apiClient;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_complaints_dashboard);

        apiClient = new ApiClient(this);

        setupToolbarAndDrawer();
        setupList();
        loadData();
    }

    private void setupToolbarAndDrawer() {
        // 1. Toolbar
        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        if (getSupportActionBar() != null) {
            getSupportActionBar().setTitle("Reklamacje");
            getSupportActionBar().setSubtitle("Ładowanie...");
        }

        // 2. Drawer (Menu boczne)
        drawerLayout = findViewById(R.id.drawer_layout);
        navigationView = findViewById(R.id.nav_view);
        navigationView.setNavigationItemSelectedListener(this);

        // Przycisk "Hamburger" animowany
        ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(
                this, drawerLayout, toolbar,
                R.string.navigation_drawer_open, R.string.navigation_drawer_close);
        drawerLayout.addDrawerListener(toggle);
        toggle.syncState();
    }

    private void setupList() {
        recyclerView = findViewById(R.id.recyclerReturns);
        swipeRefresh = findViewById(R.id.swipeRefresh);

        recyclerView.setLayoutManager(new LinearLayoutManager(this));

        // Inicjalizacja adaptera
        adapter = new ComplaintsAdapter();
        recyclerView.setAdapter(adapter);

        // Obsługa odświeżania pociągnięciem w dół
        swipeRefresh.setOnRefreshListener(this::loadData);
    }

    private void loadData() {
        swipeRefresh.setRefreshing(true);

        // Pobieramy "Zgłoszenia w toku"
        apiClient.getProcessingComplaints(new ApiClient.ApiCallback<List<DashboardComplaintDto>>() {
            @Override
            public void onSuccess(List<DashboardComplaintDto> data) {
                runOnUiThread(() -> {
                    adapter.setItems(data);
                    swipeRefresh.setRefreshing(false);

                    if (getSupportActionBar() != null) {
                        getSupportActionBar().setSubtitle("W toku: " + (data != null ? data.size() : 0));
                    }
                });
            }

            @Override
            public void onError(String message) {
                runOnUiThread(() -> {
                    Toast.makeText(ComplaintsDashboardActivity.this, "Błąd: " + message, Toast.LENGTH_SHORT).show();
                    swipeRefresh.setRefreshing(false);
                    if (getSupportActionBar() != null) {
                        getSupportActionBar().setSubtitle("Błąd połączenia");
                    }
                });
            }
        });
    }

    // ========================================================================
    // OBSŁUGA MENU BOCZNEGO (NAVIGATION DRAWER)
    // ========================================================================
    @Override
    public boolean onNavigationItemSelected(@NonNull MenuItem item) {
        int id = item.getItemId();

        if (id == R.id.nav_home) {
            loadData(); // Tylko odświeżamy obecny widok
        }
        else if (id == R.id.nav_new_allegro) {
            Toast.makeText(this, "Funkcja: Pobierz z Allegro", Toast.LENGTH_SHORT).show();
            // Tu otworzysz: new FormUniversalWizardV2(WizardSource.Allegro) - odpowiednik w Androidzie
        }
        else if (id == R.id.nav_new_google) {
            Toast.makeText(this, "Funkcja: Pobierz z Google", Toast.LENGTH_SHORT).show();
        }
        else if (id == R.id.nav_new_return) {
            Toast.makeText(this, "Funkcja: Nowe zwroty", Toast.LENGTH_SHORT).show();
        }
        else if (id == R.id.nav_add_manual) {
            // Otwieramy istniejące okno do ręcznego dodawania
            Intent intent = new Intent(this, ManualReturnActivity.class);
            startActivity(intent);
        }
        else if (id == R.id.nav_all_cases) {
            // Otwarcie listy wszystkich zgłoszeń (np. ReturnsListActivity bez filtra statusu)
            Intent intent = new Intent(this, ReturnsListActivity.class);
            intent.putExtra("mode", "all"); // Możesz obsłużyć ten tryb w ReturnsListActivity
            startActivity(intent);
        }
        else if (id == R.id.nav_clients) {
            Toast.makeText(this, "Baza Klientów", Toast.LENGTH_SHORT).show();
        }
        else if (id == R.id.nav_products) {
            Toast.makeText(this, "Baza Produktów", Toast.LENGTH_SHORT).show();
        }
        else if (id == R.id.nav_chat) {
            // Otwarcie listy wiadomości
            Intent intent = new Intent(this, MessagesActivity.class);
            startActivity(intent);
        }
        else if (id == R.id.nav_tracking) {
            Toast.makeText(this, "Śledzenie DPD", Toast.LENGTH_SHORT).show();
        }
        else if (id == R.id.nav_warehouse) {
            // Otwarcie magazynu (ReturnsListActivity w trybie warehouse)
            Intent intent = new Intent(this, ReturnsListActivity.class);
            intent.putExtra("mode", "warehouse");
            startActivity(intent);
        }

        // Zamknij menu po kliknięciu
        drawerLayout.closeDrawer(GravityCompat.START);
        return true;
    }

    @Override
    public void onBackPressed() {
        // Jeśli menu jest otwarte, cofnij zamyka menu. Jeśli nie - zamyka aktywność.
        if (drawerLayout.isDrawerOpen(GravityCompat.START)) {
            drawerLayout.closeDrawer(GravityCompat.START);
        } else {
            super.onBackPressed();
        }
    }

    // ========================================================================
    // WEWNĘTRZNY ADAPTER LISTY
    // ========================================================================
    private class ComplaintsAdapter extends RecyclerView.Adapter<ComplaintsAdapter.ViewHolder> {

        private List<DashboardComplaintDto> items = new ArrayList<>();

        public void setItems(List<DashboardComplaintDto> newItems) {
            this.items = newItems != null ? newItems : new ArrayList<>();
            notifyDataSetChanged();
        }

        @NonNull
        @Override
        public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
            // Używamy layoutu 'item_dashboard_complaint.xml' który stworzyliśmy wcześniej
            View v = LayoutInflater.from(parent.getContext())
                    .inflate(R.layout.item_dashboard_complaint, parent, false);
            return new ViewHolder(v);
        }

        @Override
        public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
            DashboardComplaintDto item = items.get(position);

            holder.txtNr.setText(item.nrZgloszenia);
            holder.txtClient.setText(item.klient != null ? item.klient : "Brak danych klienta");
            holder.txtProduct.setText(item.produkt != null ? item.produkt : "Brak danych produktu");
            holder.txtDays.setText(item.dniPoZgloszeniu + " dni");

            // Logika kolorowania (jak w WinForms: > 14 dni na czerwono)
            if (item.dniPoZgloszeniu > 14) {
                holder.txtDays.setTextColor(Color.RED);
            } else if (item.dniPoZgloszeniu > 7) {
                holder.txtDays.setTextColor(Color.parseColor("#FF9800")); // Pomarańczowy
            } else {
                holder.txtDays.setTextColor(Color.parseColor("#4CAF50")); // Zielony (lub czarny)
            }

            // Kliknięcie w element listy -> Otwarcie szczegółów
            holder.itemView.setOnClickListener(v -> {
                // Jeśli masz ReturnDetailActivity, użyj tego:
                Intent intent = new Intent(ComplaintsDashboardActivity.this, ReturnDetailActivity.class);
                // Przekazujemy ID zgłoszenia lub numer
                intent.putExtra("return_id", item.id);
                intent.putExtra("return_number", item.nrZgloszenia);
                startActivity(intent);
            });
        }

        @Override
        public int getItemCount() {
            return items.size();
        }

        class ViewHolder extends RecyclerView.ViewHolder {
            TextView txtNr, txtClient, txtProduct, txtDays;

            ViewHolder(View itemView) {
                super(itemView);
                txtNr = itemView.findViewById(R.id.txtNr);
                txtClient = itemView.findViewById(R.id.txtClient);
                txtProduct = itemView.findViewById(R.id.txtProduct);
                txtDays = itemView.findViewById(R.id.txtDays);
            }
        }
    }
}