package com.example.ena.api;

import java.time.OffsetDateTime;

public class NotificationDto {
    private int id;
    private int nadawcaId;
    private int odbiorcaId;
    private String nadawcaNazwa;
    private String odbiorcaNazwa;
    private String tytul;
    private String tresc;
    private OffsetDateTime dataWyslania;
    private Integer dotyczyZwrotuId;
    private boolean czyPrzeczytana;

    public int getId() {
        return id;
    }

    public int getNadawcaId() {
        return nadawcaId;
    }

    public int getOdbiorcaId() {
        return odbiorcaId;
    }

    public String getNadawcaNazwa() {
        return nadawcaNazwa;
    }

    public String getOdbiorcaNazwa() {
        return odbiorcaNazwa;
    }

    public String getTytul() {
        return tytul;
    }

    public String getTresc() {
        return tresc;
    }

    public OffsetDateTime getDataWyslania() {
        return dataWyslania;
    }

    public Integer getDotyczyZwrotuId() {
        return dotyczyZwrotuId;
    }

    public boolean isCzyPrzeczytana() {
        return czyPrzeczytana;
    }
}
