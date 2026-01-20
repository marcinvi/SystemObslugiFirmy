package com.example.ena.api;

import java.util.ArrayList;
import java.util.List;

public class PaginatedResponse<T> {
    private List<T> items = new ArrayList<>();
    private int page;
    private int pageSize;
    private int totalItems;

    public List<T> getItems() {
        return items;
    }

    public int getPage() {
        return page;
    }

    public int getPageSize() {
        return pageSize;
    }

    public int getTotalItems() {
        return totalItems;
    }
}
