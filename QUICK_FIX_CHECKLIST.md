# ✅ QUICK FIX CHECKLIST - 5 MINUT

## 🔧 WINDOWS FORMS - NullReferenceException

- [ ] Otwórz Visual Studio
- [ ] **Build → Rebuild Solution**
- [ ] Uruchom aplikację (F5)
- [ ] ✅ Błąd naprawiony!

**Naprawiłem:** NetworkAutoDiscovery.cs (TcpClient disposal)

---

## 📱 ANDROID STUDIO - Nie uruchamia się

### **KROK 1:** Uruchom skrypt (10 sekund)

- [ ] Idź do: `C:\Users\mpaprocki\Documents\GitHub\SystemObslugiFirmy\Ena`
- [ ] **Kliknij 2x:** `NAPRAW_PROJEKT.bat`
- [ ] Poczekaj na komunikat: **"✅ PROJEKT GOTOWY!"**
- [ ] Naciśnij dowolny klawisz

### **KROK 2:** Otwórz w Android Studio (2-5 min)

- [ ] Uruchom Android Studio
- [ ] **File → Close Project** (jeśli coś otwarte)
- [ ] **File → Open**
- [ ] Wybierz folder: `Ena`
- [ ] Kliknij **OK**
- [ ] **CZEKAJ** na Gradle sync (2-5 min pierwszym razem)
- [ ] Sprawdź dolny pasek: **"Gradle sync finished"**
- [ ] ✅ Gotowe!

---

## 🎯 WERYFIKACJA

### **Android Studio działa jeśli:**
- ✅ Widzisz strukturę projektu po lewej (app/java/res)
- ✅ Brak czerwonych błędów w Build Output
- ✅ Build → Make Project - przechodzi bez błędów

### **Windows Forms działa jeśli:**
- ✅ Aplikacja uruchamia się (F5)
- ✅ Brak NullReferenceException
- ✅ Auto-konfiguracja działa (lub można pominąć)

---

## 🚨 JEŚLI GRADLE SYNC FAILS:

```
File → Settings → Build Tools → Gradle
→ Gradle JDK: Embedded JDK (17)
→ OK
→ File → Sync Project with Gradle Files
```

**Lub:**

```
Tools → SDK Manager
→ SDK Platforms → ☑ Android 14.0 (API 34)
→ Apply → OK
```

---

## 📞 POMOC

Jeśli nadal nie działa, pokaż mi:
- Screenshot Android Studio (cały ekran)
- Komunikat błędu (dokładny tekst)
- Output window z Visual Studio

---

**TERAZ SPRÓBUJ!** 🚀

1. Rebuild Solution (Windows Forms)
2. Uruchom NAPRAW_PROJEKT.bat
3. Otwórz projekt w Android Studio
4. ✅ Działa!
