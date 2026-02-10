package com.example.ena.api;

public class AdminDtos {

    // Do pobierania listy (GET /api/admin/users)
    public static class AdminUser {
        public int id;
        public String login;
        public String nazwaWyswietlana;
        public String rola;
        public boolean isActive;
    }

    // Do tworzenia (POST /api/admin/users)
    public static class CreateUserRequest {
        public String login;
        public String password;
        public String nazwaWyswietlana;
        public String rola;

        public CreateUserRequest(String login, String password, String nazwaWyswietlana, String rola) {
            this.login = login;
            this.password = password;
            this.nazwaWyswietlana = nazwaWyswietlana;
            this.rola = rola;
        }
    }

    // Do edycji (PUT /api/admin/users/{id})
    public static class UpdateUserRequest {
        public String nazwaWyswietlana;
        public String rola;
        public boolean isActive;

        public UpdateUserRequest(String nazwaWyswietlana, String rola, boolean isActive) {
            this.nazwaWyswietlana = nazwaWyswietlana;
            this.rola = rola;
            this.isActive = isActive;
        }
    }

    // Do resetu hasła (POST /api/admin/users/{id}/reset-password)
    public static class ResetPasswordRequest {
        public String newPassword;

        public ResetPasswordRequest(String newPassword) {
            this.newPassword = newPassword;
        }
    }
}