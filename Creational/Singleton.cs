namespace WzorceProjektowe.Creational;

// SINGLETON — dokładnie jedna instancja i globalny punkt dostępu.
public sealed class AppConfig
{
    // Lazy<T> domyślnie używa LazyThreadSafetyMode.ExecutionAndPublication
    private static readonly Lazy<AppConfig> _instance = new(() => new AppConfig());

    public static AppConfig Instance => _instance.Value;

    public string Currency { get; }
    public decimal VatRate { get; }
    public DateTime CreatedAt { get; }

    private AppConfig()   // nikt z zewnątrz nie wywoła "new AppConfig()"
    {
        Currency = "PLN";
        VatRate = 0.23m;
        CreatedAt = DateTime.Now;
        Console.WriteLine("[AppConfig] Utworzono jedyną instancję");
    }
}

// Dla porównania: wersja naiwna — NIE jest bezpieczna wątkowo.
public class NaiveConfig
{
    private static NaiveConfig? _instance;
    public static NaiveConfig Instance => _instance ??= new NaiveConfig(); // wyścig!
    private NaiveConfig() { }
}

public static class SingletonDemo
{
    public static void Run()
    {
        var a = AppConfig.Instance;
        var b = AppConfig.Instance;
        Console.WriteLine($"Ta sama instancja? {ReferenceEquals(a, b)}");   // True
        Console.WriteLine($"{a.Currency}, VAT {a.VatRate:P0}");

        // Eksperyment: 10 wątków jednocześnie — instancja nadal jest jedna
        var instances = new System.Collections.Concurrent.ConcurrentBag<AppConfig>();
        Parallel.For(0, 10, _ => instances.Add(AppConfig.Instance));
        Console.WriteLine($"Różnych instancji z 10 wątków: {instances.Distinct().Count()}");
    }
}
