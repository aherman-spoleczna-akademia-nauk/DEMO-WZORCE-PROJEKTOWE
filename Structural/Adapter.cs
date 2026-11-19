namespace WzorceProjektowe.Structural;

// ADAPTER — dopasowanie obcego interfejsu do oczekiwanego przez nasz system.

// Interfejs docelowy — tego oczekuje nasz system
public record HotelOffer(string Name, decimal PricePln, int Stars);

public interface IHotelProvider
{
    IEnumerable<HotelOffer> Search(string city, DateOnly checkIn, int nights);
}

// Klasa adaptowana — zewnętrzna biblioteka, której nie możemy modyfikować
public class GlobalHotelsApi
{
    public record GhResult(string HotelTitle, long PriceUsdCents, string Category);

    public List<GhResult> FindRooms(string cityCode, string dateYyyyMmDd, int lengthOfStay) =>
    [
        new("Sunset Resort", 12_500 * lengthOfStay, "****"),
        new("Budget Inn",     4_900 * lengthOfStay, "**")
    ];
}

// Adapter (obiektowy — przez kompozycję)
public class GlobalHotelsAdapter : IHotelProvider
{
    private readonly GlobalHotelsApi _api;
    private readonly decimal _usdToPln;

    public GlobalHotelsAdapter(GlobalHotelsApi api, decimal usdToPln)
    {
        _api = api;
        _usdToPln = usdToPln;
    }

    public IEnumerable<HotelOffer> Search(string city, DateOnly checkIn, int nights)
    {
        string cityCode = city.ToUpperInvariant()[..3];        // "Lizbona" -> "LIZ"
        string date = checkIn.ToString("yyyyMMdd");

        return _api.FindRooms(cityCode, date, nights)
            .Select(r => new HotelOffer(
                Name: r.HotelTitle,
                PricePln: Math.Round(r.PriceUsdCents / 100m * _usdToPln, 2),
                Stars: r.Category.Length));
    }
}

public static class AdapterDemo
{
    public static void Run()
    {
        // Klient widzi wyłącznie IHotelProvider
        IHotelProvider provider = new GlobalHotelsAdapter(new GlobalHotelsApi(), usdToPln: 3.95m);
        foreach (var offer in provider.Search("Lizbona", new DateOnly(2027, 5, 1), nights: 3))
            Console.WriteLine($"{offer.Name} ({offer.Stars}*) — {offer.PricePln:C}");
    }
}
