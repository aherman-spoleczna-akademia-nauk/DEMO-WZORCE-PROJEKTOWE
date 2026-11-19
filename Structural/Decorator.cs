namespace WzorceProjektowe.Structural;

// DECORATOR — dokładanie zachowań w czasie działania, bez eksplozji podklas.

public interface IOffer
{
    string Description { get; }
    decimal Price { get; }
}

// Komponent konkretny
public class BaseOffer(string destination, decimal price) : IOffer
{
    public string Description => $"Wycieczka: {destination}";
    public decimal Price => price;
}

// Dekorator bazowy — przechowuje opakowany obiekt i deleguje do niego
public abstract class OfferDecorator(IOffer inner) : IOffer
{
    protected IOffer Inner { get; } = inner;
    public virtual string Description => Inner.Description;
    public virtual decimal Price => Inner.Price;
}

// Dekoratory konkretne
public class InsuranceDecorator(IOffer inner) : OfferDecorator(inner)
{
    public override string Description => Inner.Description + " + ubezpieczenie";
    public override decimal Price => Inner.Price + 129m;
}

public class SeaViewDecorator(IOffer inner) : OfferDecorator(inner)
{
    public override string Description => Inner.Description + " + widok na morze";
    public override decimal Price => Inner.Price * 1.15m;      // +15%
}

public class AirportTransferDecorator(IOffer inner) : OfferDecorator(inner)
{
    public override string Description => Inner.Description + " + transfer";
    public override decimal Price => Inner.Price + 80m;
}

public static class DecoratorDemo
{
    public static void Run()
    {
        // Składamy dodatki jak klocki, w dowolnej kolejności i liczbie
        IOffer offer = new BaseOffer("Malta", 2_400m);
        offer = new SeaViewDecorator(offer);
        offer = new InsuranceDecorator(offer);
        offer = new AirportTransferDecorator(offer);

        Console.WriteLine(offer.Description);
        Console.WriteLine(offer.Price.ToString("C"));   // 2400*1.15 + 129 + 80 = 2 969,00 zł

        // Kolejność ma znaczenie: +15% liczone także od ubezpieczenia
        IOffer other = new SeaViewDecorator(new InsuranceDecorator(new BaseOffer("Malta", 2_400m)));
        Console.WriteLine($"Inna kolejność: {other.Description} = {other.Price:C}");
    }
}
