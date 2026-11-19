namespace WzorceProjektowe.Creational;

// FACTORY METHOD — podklasy decydują, który obiekt utworzyć.

// Produkt
public interface ITransport
{
    string Name { get; }
    decimal PricePerPerson { get; }
    void Book(int persons);
}

public class Flight : ITransport
{
    public string Name => "Samolot";
    public decimal PricePerPerson => 899m;
    public void Book(int persons) =>
        Console.WriteLine($"  Zarezerwowano {persons} miejsc w samolocie (bagaż 20 kg).");
}

public class Coach : ITransport
{
    public string Name => "Autokar";
    public decimal PricePerPerson => 349m;
    public void Book(int persons) =>
        Console.WriteLine($"  Zarezerwowano {persons} miejsc w autokarze (2 kierowców).");
}

// Twórca — zawiera logikę biznesową, która NIE zna konkretnych klas
public abstract class TripPlanner
{
    protected abstract ITransport CreateTransport();   // <- metoda fabrykująca

    public decimal PlanTrip(string destination, int persons)
    {
        ITransport transport = CreateTransport();
        Console.WriteLine($"Wycieczka do: {destination}, transport: {transport.Name}");
        transport.Book(persons);
        decimal total = transport.PricePerPerson * persons;
        Console.WriteLine($"  Koszt transportu: {total:C}");
        return total;
    }
}

// Twórcy konkretni — decydują, CO powstaje
public class AirTripPlanner : TripPlanner
{
    protected override ITransport CreateTransport() => new Flight();
}

public class CoachTripPlanner : TripPlanner
{
    protected override ITransport CreateTransport() => new Coach();
}

public static class FactoryMethodDemo
{
    public static void Run()
    {
        TripPlanner[] planners = [new AirTripPlanner(), new CoachTripPlanner()];
        foreach (var planner in planners)
            planner.PlanTrip("Kreta", 4);
    }
}
