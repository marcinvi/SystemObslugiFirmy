package com.example.ena.api;

public class SimpleUserDto {
    public int id;
    public String name;

    @Override
    public String toString() {
        return name; // To ważne dla Spinnera w Androidzie (wyświetla tekst z toString)
    }
}