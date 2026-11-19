namespace WzorceProjektowe.Creational;

// BUILDER — składanie złożonego obiektu krok po kroku.

public enum Board { RoomOnly, BedAndBreakfast, HalfBoard, AllInclusive }

// Produkt — niezmienny (record), tworzony tylko przez budowniczego
public sealed record TravelPackage(
    string Destination,
    DateOnly From,
    DateOnly To,
    string Hotel,
    Board Board,
    bool Insurance,
    IReadOnlyList<string> Excursions)
{
    public int Nights => To.DayNumber - From.DayNumber;
}

public class TravelPackageBuilder
{
    private string? _destination;
    private DateOnly _from, _to;
    private string _hotel = "Hotel 3*";
    private Board _board = Board.BedAndBreakfast;
    private bool _insurance;
    private readonly List<string> _excursions = new();

    public TravelPackageBuilder To(string destination) { _destination = destination; return this; }
    public TravelPackageBuilder Dates(DateOnly from, DateOnly to) { _from = from; _to = to; return this; }
    public TravelPackageBuilder InHotel(string hotel) { _hotel = hotel; return this; }
    public TravelPackageBuilder WithBoard(Board board) { _board = board; return this; }
    public TravelPackageBuilder WithInsurance() { _insurance = true; return this; }
    public TravelPackageBuilder AddExcursion(string name) { _excursions.Add(name); return this; }

    public TravelPackage Build()
    {
        if (string.IsNullOrWhiteSpace(_destination))
            throw new InvalidOperationException("Nie podano kierunku.");
        if (_to <= _from)
            throw new InvalidOperationException("Data powrotu musi być późniejsza niż wyjazdu.");

        return new TravelPackage(_destination, _from, _to, _hotel, _board,
                                 _insurance, _excursions.ToList());
    }
}

public static class BuilderDemo
{
    public static void Run()
    {
        var package = new TravelPackageBuilder()
            .To("Teneryfa")
            .Dates(new DateOnly(2027, 2, 10), new DateOnly(2027, 2, 17))
            .InHotel("Iberostar 4*")
            .WithBoard(Board.AllInclusive)
            .WithInsurance()
            .AddExcursion("Wulkan Teide")
            .AddExcursion("Rejs z delfinami")
            .Build();

        Console.WriteLine($"{package.Destination}: {package.Nights} nocy, {package.Board}, " +
                          $"wycieczki: {string.Join(", ", package.Excursions)}");

        // Walidacja w Build() — obiekt w niespójnym stanie nie powstanie
        try
        {
            new TravelPackageBuilder().To("Rodos")
                .Dates(new DateOnly(2027, 6, 10), new DateOnly(2027, 6, 1))
                .Build();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"  BŁĄD: {ex.Message}");
        }
    }
}
