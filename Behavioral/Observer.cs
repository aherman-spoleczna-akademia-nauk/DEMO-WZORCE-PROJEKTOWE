namespace WzorceProjektowe.Behavioral;

// OBSERVER — powiadamianie wielu odbiorców o zmianie stanu.

// ---------- Wersja klasyczna (GoF) ----------
public interface IPriceObserver
{
    void OnPriceChanged(string offer, decimal oldPrice, decimal newPrice);
}

public class ObservableOffer(string name, decimal price)
{
    private readonly List<IPriceObserver> _observers = new();

    public void Subscribe(IPriceObserver o) => _observers.Add(o);
    public void Unsubscribe(IPriceObserver o) => _observers.Remove(o);

    public decimal Price
    {
        get => price;
        set
        {
            if (value == price) return;
            var old = price;
            price = value;
            foreach (var o in _observers.ToList())   // kopia: obserwator może się wypisać w trakcie
                o.OnPriceChanged(name, old, value);
        }
    }
}

public class ConsoleObserver : IPriceObserver
{
    public void OnPriceChanged(string offer, decimal oldPrice, decimal newPrice) =>
        Console.WriteLine($"  [Klasyczny] {offer}: {oldPrice:C} -> {newPrice:C}");
}

// ---------- Wersja idiomatyczna C# (zdarzenia) ----------
public class PriceChangedEventArgs(decimal oldPrice, decimal newPrice) : EventArgs
{
    public decimal OldPrice { get; } = oldPrice;
    public decimal NewPrice { get; } = newPrice;
    public decimal ChangePercent => (NewPrice - OldPrice) / OldPrice * 100;
}

public class Offer(string name, decimal price)
{
    public string Name { get; } = name;
    public event EventHandler<PriceChangedEventArgs>? PriceChanged;

    public decimal Price
    {
        get => price;
        set
        {
            if (value == price) return;
            var args = new PriceChangedEventArgs(price, value);
            price = value;
            PriceChanged?.Invoke(this, args);
        }
    }
}

// Obserwatorzy — nic o sobie nie wiedzą
public class EmailNotifier
{
    public void Handle(object? sender, PriceChangedEventArgs e)
    {
        if (e.NewPrice < e.OldPrice && sender is Offer offer)
            Console.WriteLine($"  [E-mail] {offer.Name} tanieje o {-e.ChangePercent:F1}%!");
    }
}

public class PriceHistory
{
    public List<(DateTime At, decimal Price)> Entries { get; } = new();
    public void Handle(object? sender, PriceChangedEventArgs e) =>
        Entries.Add((DateTime.Now, e.NewPrice));
}

public static class ObserverDemo
{
    public static void Run()
    {
        Console.WriteLine("Wersja klasyczna:");
        var classic = new ObservableOffer("Cypr 7 dni", 2_500m);
        classic.Subscribe(new ConsoleObserver());
        classic.Price = 2_300m;

        Console.WriteLine("Wersja ze zdarzeniami:");
        var offer = new Offer("Egipt, Hurghada 7 dni", 3_200m);
        var mailer = new EmailNotifier();
        var history = new PriceHistory();

        offer.PriceChanged += mailer.Handle;
        offer.PriceChanged += history.Handle;
        offer.PriceChanged += (_, e) => Console.WriteLine($"  [WWW] Nowa cena: {e.NewPrice:C}");

        offer.Price = 2_880m;   // powiadamia trzech obserwatorów
        offer.PriceChanged -= mailer.Handle;
        offer.Price = 2_700m;   // już tylko dwóch
        Console.WriteLine($"Wpisów w historii: {history.Entries.Count}");
    }
}
