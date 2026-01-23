package com.example.ena.api;

public class LoginResponse {
    private String token;
    private String refreshToken;
    private String expiresAt;
    private UserDto user;

    public String getToken() {
        return token;
    }

    public String getRefreshToken() {
        return refreshToken;
    }

    public String getExpiresAt() {
        return expiresAt;
    }

    public UserDto getUser() {
        return user;
    }
}
