# DODAJ DO .CSPROJ JEŚLI JEST PROBLEM

Jeśli Visual Studio nie widzi MessageBubble.cs, dodaj ręcznie do pliku .csproj:

## Znajdź sekcję z plikami (ItemGroup):

```xml
<ItemGroup>
    <Compile Include="FormWiadomosci.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="FormWiadomosci.Designer.cs">
      <DependentUpon>FormWiadomosci.cs</DependentUpon>
    </Compile>
    
    <!-- DODAJ TE 2 LINIJKI PONIŻEJ: -->
    <Compile Include="MessageBubble.cs">
      <SubType>Component</SubType>
    </Compile>
    
    <!-- reszta plików... -->
</ItemGroup>
```

## Albo PROŚCIEJ:

1. W Visual Studio: **Solution Explorer**
2. Prawy przycisk na projekcie → **Add** → **Existing Item**
3. Wybierz `MessageBubble.cs`
4. Kliknij **Add**

Visual Studio automatycznie doda do .csproj!

## Rebuild:
```
Build → Clean Solution
Build → Rebuild Solution
```

GOTOWE! 🎉
