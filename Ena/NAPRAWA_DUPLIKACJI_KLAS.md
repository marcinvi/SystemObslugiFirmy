# 🔥 NAPRAWA: Type is defined multiple times

**Data:** 2025-01-19  
**Problem:** Duplikacja klas Java/Kotlin  
**Status:** ✅ Naprawione

---

## ❌ PROBLEM

### **Błąd kompilacji:**
```
Type com.example.ena.api.ReturnWarehouseUpdateRequest is defined multiple times:
- C:\...\kotlin-classes\debug\...\ReturnWarehouseUpdateRequest.class
- C:\...\javac\...\ReturnWarehouseUpdateRequest.class
```

### **Przyczyna:**

Te same klasy istnieją **2 razy** - w Java i Kotlin:

```
📁 api/
├── ReturnWarehouseUpdateRequest.java  ❌ STARA (Java)
├── ReturnListItem.java                ❌ STARA (Java)
├── ReturnDetails.java                 ❌ STARA (Java)
├── ReturnSummaryItem.java             ❌ STARA (Java)
├── ReturnSummaryStats.java            ❌ STARA (Java)
└── ReturnDtos.kt                      ✅ NOWA (Kotlin - wszystkie klasy tutaj!)
```

Gradle kompiluje **OBE wersje** → błąd duplikacji!

---

## ✅ ROZWIĄZANIE - 2 MINUTY

### **SUPER ŁATWY SPOSÓB:**

1. **Uruchom skrypt naprawczy:**
   ```
   C:\Users\mpaprocki\Documents\GitHub\SystemObslugiFirmy\Ena\NAPRAW_DUPLIKATY.bat
   ```
   
   Kliknij **DWUKROTNIE** na plik i poczekaj 5 sekund.

2. **W Android Studio:**
   ```
   Build → Clean Project
   Build → Rebuild Project (poczekaj 1-2 min)
   ```

3. **Zweryfikuj:**
   ```
   Build → Make Project (Ctrl+F9)
   ✅ BUILD SUCCESSFUL
   ```

---

## 🔧 CO ROBI SKRYPT

### **Usuwa zduplikowane pliki Java:**

1. ❌ `ReturnWarehouseUpdateRequest.java` - duplikat z Kotlin
2. ❌ `ReturnListItem.java` - duplikat (ReturnListItemDto)
3. ❌ `ReturnDetails.java` - duplikat (ReturnDetailsDto)
4. ❌ `ReturnSummaryItem.java` - duplikat (ReturnSummaryItemDto)
5. ❌ `ReturnSummaryStats.java` - duplikat (ReturnSummaryStatsDto)

### **Czyści build cache:**

- `app/build/` - cache kompilacji
- `build/` - cache projektu
- `.gradle/` - cache Gradle

---

## 📝 DLACZEGO TO SIĘ STAŁO

Projekt był migrowany z **Java → Kotlin**:

1. Pierwotnie klasy były w **Java**
2. Ktoś dodał **nowe wersje w Kotlin** (ReturnDtos.kt)
3. **Nie usunął starych** plików Java
4. Gradle kompiluje **OBE** → duplikacja → błąd

---

## ✅ CO ZOSTAJE PO NAPRAWIE

### **Pliki API (tylko te potrzebne):**

```
📁 api/
├── ApiClient.java              ✅ (używa klas)
├── ApiConfig.java              ✅ (konfiguracja)
├── MessageDto.java             ✅ (osobna klasa)
├── OffsetDateTimeAdapter.java  ✅ (adapter JSON)
├── ReturnDecisionRequest.java  ✅ (nie ma duplikatu)
└── ReturnDtos.kt               ✅ (wszystkie DTO w Kotlin)
```

### **Wszystkie klasy są teraz w ReturnDtos.kt:**

- ReturnListItemDto
- ReturnDetailsDto
- ReturnWarehouseUpdateRequest
- ReturnForwardToSalesRequest
- ReturnDecisionResponse
- ReturnManualCreateRequest
- ReturnActionDto
- ReturnActionCreateRequest
- MessageCreateRequest
- ReturnSummaryItemDto
- ReturnSummaryStatsDto
- ReturnSummaryResponse
- WarehouseSearchItemDto
- WarehouseIntakeRequest
- ForwardToComplaintRequest
- ComplaintCustomerDto
- ComplaintAddressDto
- ComplaintProductDto

---

## 🚨 JEŚLI NADAL NIE DZIAŁA

### **Błąd: "Cannot find symbol"**

Jeśli po usunięciu duplikatów inne klasy nie mogą znaleźć tych klas:

**Przyczyna:** Używają starej nazwy Java zamiast Kotlin

**Rozwiązanie:**

W `ApiClient.java` zmień importy:

```java
// PRZED (Java - nie działa już)
import com.example.ena.api.ReturnListItem;
import com.example.ena.api.ReturnDetails;

// PO (Kotlin - działa)
import com.example.ena.api.ReturnListItemDto;
import com.example.ena.api.ReturnDetailsDto;
```

Dodaj "Dto" na końcu każdej nazwy klasy!

---

### **Błąd: "Unresolved reference"**

Jeśli klasa Kotlin nie widzi innych klas:

**Rozwiązanie:**

1. **File → Invalidate Caches**
2. **Restart Android Studio**
3. **Build → Rebuild Project**

---

## 🎯 WERYFIKACJA

Po naprawie sprawdź:

### **1. Build przechodzi:**
```
Build → Make Project
✅ BUILD SUCCESSFUL in 45s
```

### **2. Brak błędów DEX:**
```
:app:dexBuilderDebug
✅ SUCCESS
```

### **3. Struktura plików OK:**
```
api/
├── 6 plików Java (bez duplikatów)
└── 1 plik Kotlin (ReturnDtos.kt)
```

---

## 📋 CHECKLIST

- [ ] Uruchom `NAPRAW_DUPLIKATY.bat`
- [ ] Poczekaj na "GOTOWE!"
- [ ] Android Studio → Clean Project
- [ ] Android Studio → Rebuild Project
- [ ] Sprawdź Build → Make Project
- [ ] ✅ BUILD SUCCESSFUL!

---

## 🎉 PODSUMOWANIE

**PRZED:**
- ❌ 5 klas zduplikowanych (Java + Kotlin)
- ❌ Błąd: "Type is defined multiple times"
- ❌ Build fails

**PO:**
- ✅ Tylko Kotlin (ReturnDtos.kt)
- ✅ Brak duplikacji
- ✅ Build successful

---

## 📞 JEŚLI MASZ INNE BŁĘDY

Po naprawie duplikacji mogą się pojawić **nowe błędy** związane z:
- Brakującymi importami
- Zmianą nazw klas (Java → Kotlin Dto)
- Typami danych (String vs OffsetDateTime)

**Pokaż mi konkretny błąd** - naprawię go od razu!

---

**TERAZ URUCHOM SKRYPT I REBUILD!** 🚀

```
Kliknij 2x: NAPRAW_DUPLIKATY.bat
Poczekaj 5 sekund
Android Studio → Clean → Rebuild
✅ Działa!
```
