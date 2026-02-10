package com.example.ena.ui;

import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import com.example.ena.R;
import com.example.ena.api.AdminDtos;
import java.util.List;

public class AdminUsersAdapter extends RecyclerView.Adapter<AdminUsersAdapter.ViewHolder> {

    private List<AdminDtos.AdminUser> users;
    private final OnUserClickListener listener;

    public interface OnUserClickListener {
        void onUserClick(AdminDtos.AdminUser user);
    }

    public AdminUsersAdapter(List<AdminDtos.AdminUser> users, OnUserClickListener listener) {
        this.users = users;
        this.listener = listener;
    }

    public void updateData(List<AdminDtos.AdminUser> newUsers) {
        this.users = newUsers;
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_admin_user, parent, false);
        return new ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
        AdminDtos.AdminUser user = users.get(position);

        String displayName = (user.nazwaWyswietlana != null && !user.nazwaWyswietlana.isEmpty())
                ? user.nazwaWyswietlana : user.login;

        holder.txtName.setText(displayName);
        holder.txtLoginRole.setText(String.format("%s (%s)", user.login, user.rola));

        if (user.isActive) {
            holder.txtStatus.setText("Aktywny");
            holder.txtStatus.setTextColor(Color.parseColor("#4CAF50")); // Green
        } else {
            holder.txtStatus.setText("Zablokowany");
            holder.txtStatus.setTextColor(Color.parseColor("#F44336")); // Red
        }

        holder.itemView.setOnClickListener(v -> listener.onUserClick(user));
    }

    @Override
    public int getItemCount() {
        return users != null ? users.size() : 0;
    }

    static class ViewHolder extends RecyclerView.ViewHolder {
        TextView txtName, txtLoginRole, txtStatus;

        ViewHolder(View itemView) {
            super(itemView);
            txtName = itemView.findViewById(R.id.txtName);
            txtLoginRole = itemView.findViewById(R.id.txtLoginRole);
            txtStatus = itemView.findViewById(R.id.txtStatus);
        }
    }
}