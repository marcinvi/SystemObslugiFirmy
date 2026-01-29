package com.example.ena.api;

public class ReturnPhotoDto {
    private int id;
    private int returnId;
    private String fileName;
    private String contentType;
    private Long size;
    private String addedAt;
    private String addedByName;
    private String url;

    public int getId() {
        return id;
    }

    public int getReturnId() {
        return returnId;
    }

    public String getFileName() {
        return fileName;
    }

    public String getContentType() {
        return contentType;
    }

    public Long getSize() {
        return size;
    }

    public String getAddedAt() {
        return addedAt;
    }

    public String getAddedByName() {
        return addedByName;
    }

    public String getUrl() {
        return url;
    }
}
