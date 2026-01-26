package com.example.ena.api;

import java.time.OffsetDateTime;

public class MessageDto {
    private int id;
    private String sender;    // Było np. 'nadawca' lub brak gettera
    private String content;   // Było np. 'tresc'
    private OffsetDateTime createdAt; // Było np. 'data'

    // Gettery wymagane przez MessageAdapter
    public String getSender() {
        return sender;
    }

    public String getContent() {
        return content;
    }

    public OffsetDateTime getCreatedAt() {
        return createdAt;
    }

    // Settery (opcjonalne, ale przydatne dla Gson)
    public void setSender(String sender) { this.sender = sender; }
    public void setContent(String content) { this.content = content; }
    public void setCreatedAt(OffsetDateTime createdAt) { this.createdAt = createdAt; }
}