package com.example.ena.ui;

import android.content.Context;
import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.cardview.widget.CardView;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.R;
import java.util.List;

public class ModulesAdapter extends RecyclerView.Adapter<ModulesAdapter.ModuleViewHolder> {

    public interface OnModuleClickListener {
        void onModuleClick(String moduleName);
    }

    private final List<String> modules;
    private final OnModuleClickListener listener;

    public ModulesAdapter(List<String> modules, OnModuleClickListener listener) {
        this.modules = modules;
        this.listener = listener;
    }

    @NonNull
    @Override
    public ModuleViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_module_card, parent, false);
        return new ModuleViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ModuleViewHolder holder, int position) {
        String moduleName = modules.get(position);
        holder.bind(moduleName, listener);
    }

    @Override
    public int getItemCount() {
        return modules != null ? modules.size() : 0;
    }

    static class ModuleViewHolder extends RecyclerView.ViewHolder {
        final TextView txtName;
        final ImageView imgIcon;
        final View iconBg;
        final CardView cardView;

        ModuleViewHolder(@NonNull View itemView) {
            super(itemView);
            txtName = itemView.findViewById(R.id.txtModuleName);
            imgIcon = itemView.findViewById(R.id.imgModuleIcon);
            iconBg = itemView.findViewById(R.id.iconBg);
            cardView = (CardView) itemView;
        }

        void bind(String name, OnModuleClickListener listener) {
            txtName.setText(name);
            setupAppearance(name);
            itemView.setOnClickListener(v -> listener.onModuleClick(name));
        }

        private void setupAppearance(String name) {
            String lower = name.toLowerCase();
            int color;
            int iconRes;

            // Logika ikon i kolorów dla konkretnych modułów
            if (lower.contains("magazyn")) {
                color = Color.parseColor("#1976D2"); // Niebieski
                iconRes = android.R.drawable.ic_menu_agenda; // Ikona pudełka/listy
            } else if (lower.contains("handlowiec") || lower.contains("sprzedaż")) {
                color = Color.parseColor("#43A047"); // Zielony
                iconRes = android.R.drawable.ic_menu_my_calendar; // Ikona osoby/kalendarza
            } else if (lower.contains("zwroty") || lower.contains("podsumowanie")) {
                color = Color.parseColor("#FB8C00"); // Pomarańczowy
                iconRes = android.R.drawable.ic_menu_sort_by_size; // Ikona wykresu/listy
            } else if (lower.contains("wiadomości")) {
                color = Color.parseColor("#5E35B1"); // Fioletowy
                iconRes = android.R.drawable.ic_menu_send; // Ikona wiadomości
            } else if (lower.contains("ustawienia")) {
                color = Color.parseColor("#757575"); // Szary
                iconRes = android.R.drawable.ic_menu_preferences; // Ikona trybika
            } else {
                color = Color.parseColor("#00BCD4"); // Cyan domyślny
                iconRes = android.R.drawable.ic_menu_info_details;
            }

            // Ustawienie kolorów (tło ikony jasne, ikona ciemna)
            // Przyjmujemy uproszczoną metodę zmiany odcienia dla tła
            iconBg.getBackground().setTint(manipulateColor(color, 0.2f)); // Bardzo jasne tło
            imgIcon.setColorFilter(color);
            imgIcon.setImageResource(iconRes);
        }

        // Pomocnicza metoda do rozjaśniania koloru dla tła
        private int manipulateColor(int color, float factor) {
            int a = Color.alpha(color);
            int r = Math.round(Color.red(color) + (255 - Color.red(color)) * (1 - factor));
            int g = Math.round(Color.green(color) + (255 - Color.green(color)) * (1 - factor));
            int b = Math.round(Color.blue(color) + (255 - Color.blue(color)) * (1 - factor));
            // Wymuszamy pełną przezroczystość tła jeśli trzeba, ale tu chcemy jasny pastel
            return Color.argb(50, Color.red(color), Color.green(color), Color.blue(color));
        }
    }
}