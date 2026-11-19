namespace WzorceProjektowe.Structural;

// COMPOSITE — jednolita obsługa pojedynczych elementów i całych drzew.

// Komponent — wspólny interfejs liści i kompozytów
public abstract class ItineraryItem(string name)
{
    public string Name { get; } = name;
    public abstract decimal Cost { get; }
    public abstract TimeSpan Duration { get; }
    public abstract void Print(int indent = 0);
}

// Liść
public class Activity(string name, decimal cost, TimeSpan duration) : ItineraryItem(name)
{
    public override decimal Cost => cost;
    public override TimeSpan Duration => duration;
    public override void Print(int indent = 0) =>
        Console.WriteLine($"{new string(' ', indent)}- {Name} ({Duration:h\\:mm} h, {Cost:C})");
}

// Kompozyt — sam jest elementem i zawiera elementy
public class ItineraryGroup(string name) : ItineraryItem(name)
{
    private readonly List<ItineraryItem> _children = new();

    public ItineraryGroup Add(ItineraryItem item) { _children.Add(item); return this; }

    public override decimal Cost => _children.Sum(c => c.Cost);
    public override TimeSpan Duration =>
        _children.Aggregate(TimeSpan.Zero, (sum, c) => sum + c.Duration);

    public override void Print(int indent = 0)
    {
        Console.WriteLine($"{new string(' ', indent)}+ {Name} [razem: {Cost:C}]");
        foreach (var child in _children)
            child.Print(indent + 2);       // rekurencja — klient nie widzi różnicy
    }
}

public static class CompositeDemo
{
    public static void Run()
    {
        var rome = new ItineraryGroup("Zwiedzanie Rzymu")
            .Add(new Activity("Koloseum", 120m, TimeSpan.FromHours(2)))
            .Add(new Activity("Forum Romanum", 0m, TimeSpan.FromHours(1.5)));

        var tour = new ItineraryGroup("Włochy objazdowo")
            .Add(new ItineraryGroup("Dzień 1 — Rzym")
                .Add(rome)
                .Add(new Activity("Kolacja w Trastevere", 150m, TimeSpan.FromHours(2))))
            .Add(new ItineraryGroup("Dzień 2 — Watykan")
                .Add(new Activity("Muzea Watykańskie", 95m, TimeSpan.FromHours(3))));

        tour.Print();
        Console.WriteLine($"Koszt całkowity: {tour.Cost:C}, czas: {tour.Duration.TotalHours} h");
    }
}
