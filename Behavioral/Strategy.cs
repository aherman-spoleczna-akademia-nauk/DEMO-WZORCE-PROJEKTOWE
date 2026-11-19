namespace WzorceProjektowe.Behavioral;

// STRATEGY — wymienne algorytmy wycen.

public interface IPricingStrategy
{
    string Name { get; }
    decimal Calculate(decimal basePrice, DateOnly departure, DateOnly today);
}

public class StandardPricing : IPricingStrategy
{
    public string Name => "Cena standardowa";
    public decimal Calculate(decimal basePrice, DateOnly departure, DateOnly today) => basePrice;
}

public class FirstMinutePricing : IPricingStrategy
{
    public string Name => "First minute (-10%)";
    public decimal Calculate(decimal basePrice, DateOnly departure, DateOnly today) =>
        departure.DayNumber - today.DayNumber >= 120 ? basePrice * 0.90m : basePrice;
}

public class LastMinutePricing : IPricingStrategy
{
    public string Name => "Last minute (-30%)";
    public decimal Calculate(decimal basePrice, DateOnly departure, DateOnly today) =>
        departure.DayNumber - today.DayNumber <= 7 ? basePrice * 0.70m : basePrice;
}

public class LoyaltyPricing(int tripsSoFar) : IPricingStrategy
{
    public string Name => $"Lojalnościowa ({tripsSoFar} wyjazdów)";
    public decimal Calculate(decimal basePrice, DateOnly departure, DateOnly today) =>
        basePrice * (1 - Math.Min(tripsSoFar, 10) * 0.02m);   // 2% za wyjazd, max 20%
}

// Kontekst
public class Booking(decimal basePrice, DateOnly departure)
{
    public IPricingStrategy Pricing { get; set; } = new StandardPricing();

    public decimal FinalPrice(DateOnly today) => Pricing.Calculate(basePrice, departure, today);
}

public static class StrategyDemo
{
    public static void Run()
    {
        var today = new DateOnly(2027, 1, 10);
        var departure = new DateOnly(2027, 1, 15);
        var booking = new Booking(4_000m, departure);

        IPricingStrategy[] strategies =
            [new StandardPricing(), new FirstMinutePricing(), new LastMinutePricing(), new LoyaltyPricing(4)];

        foreach (var s in strategies)
        {
            booking.Pricing = s;      // strategię zmieniamy w czasie działania
            Console.WriteLine($"{s.Name,-28} {booking.FinalPrice(today),12:C}");
        }

        var best = strategies.MinBy(s => s.Calculate(4_000m, departure, today))!;
        Console.WriteLine($"Najkorzystniejsza: {best.Name}");

        // Wariant funkcyjny: strategia jako delegat
        Func<decimal, decimal> studentDiscount = p => p * 0.85m;
        Console.WriteLine($"Strategia jako Func<>: {studentDiscount(4_000m):C}");
    }
}
