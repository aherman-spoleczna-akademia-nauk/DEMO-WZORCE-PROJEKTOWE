# Wzorce projektowe w C# — przykłady do zajęć

Projekt konsolowy .NET 10 (C# 14) z 10 wzorcami GoF na wspólnej dziedzinie: **system rezerwacji wycieczek**.
Kod jest identyczny z przykładami w skrypcie do przedmiotu.

## Uruchomienie

```bash
cd WzorceProjektowe
dotnet run            # menu wyboru wzorca
dotnet run -- all     # wszystkie przykłady po kolei
dotnet run -- 7       # tylko przykład nr 7 (Strategy)
```

Projekt można też otworzyć w Visual Studio 2026 (lub 2022 17.14+) / Rider przez `WzorceProjektowe.sln`.

## Struktura

| Folder | Wzorce |
| --- | --- |
| `Creational/` | Singleton, Factory Method, Builder |
| `Structural/` | Adapter, Decorator, Composite |
| `Behavioral/` | Strategy, Observer, Command, State |

Każdy plik zawiera implementację wzorca oraz statyczną klasę `XxxDemo` z metodą `Run()`, wywoływaną z menu w `Program.cs`.
