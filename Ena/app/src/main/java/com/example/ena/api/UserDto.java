package com.example.ena.api;

public class UserDto {
    private int id;
    private String login;
    private String nazwaWyswietlana;
    private String email;
    private boolean aktywny;

    public int getId() {
        return id;
    }

    public String getLogin() {
        return login;
    }

    public String getNazwaWyswietlana() {
        return nazwaWyswietlana;
    }

    public String getEmail() {
        return email;
    }

    public boolean isAktywny() {
        return aktywny;
    }
}
