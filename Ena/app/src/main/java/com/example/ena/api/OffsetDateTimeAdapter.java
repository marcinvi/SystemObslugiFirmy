package com.example.ena.api;

import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonParseException;
import com.google.gson.JsonPrimitive;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;
import java.lang.reflect.Type;
import java.time.OffsetDateTime;
import java.time.format.DateTimeFormatter;

public class OffsetDateTimeAdapter implements JsonSerializer<OffsetDateTime>, JsonDeserializer<OffsetDateTime> {
    @Override
    public JsonElement serialize(OffsetDateTime src, Type typeOfSrc, JsonSerializationContext context) {
        if (src == null) {
            return null;
        }
        return new JsonPrimitive(src.format(DateTimeFormatter.ISO_OFFSET_DATE_TIME));
    }

    @Override
    public OffsetDateTime deserialize(JsonElement json, Type typeOfT, JsonDeserializationContext context)
            throws JsonParseException {
        if (json == null || json.isJsonNull()) {
            return null;
        }
        return OffsetDateTime.parse(json.getAsString(), DateTimeFormatter.ISO_OFFSET_DATE_TIME);
    }
}
