using System.Globalization;
using System.Text;
using WzorceProjektowe.Creational;
using WzorceProjektowe.Structural;
using WzorceProjektowe.Behavioral;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pl-PL");

var demos = new (string Name, Action Run)[]
{
    ("Singleton",      SingletonDemo.Run),
    ("Factory Method", FactoryMethodDemo.Run),
    ("Builder",        BuilderDemo.Run),
    ("Adapter",        AdapterDemo.Run),
    ("Decorator",      DecoratorDemo.Run),
    ("Composite",      CompositeDemo.Run),
    ("Strategy",       StrategyDemo.Run),
    ("Observer",       ObserverDemo.Run),
    ("Command",        CommandDemo.Run),
    ("State",          StateDemo.Run),
};

// Uruchomienie: dotnet run            -> menu
//               dotnet run -- all     -> wszystkie przykłady po kolei
//               dotnet run -- 7       -> tylko przykład nr 7
if (args.Length > 0)
{
    if (args[0] == "all") { for (int i = 0; i < demos.Length; i++) RunDemo(i); }
    else if (int.TryParse(args[0], out int n) && n >= 1 && n <= demos.Length) RunDemo(n - 1);
    return;
}

while (true)
{
    Console.WriteLine("\n=== Wzorce projektowe w C# — system rezerwacji wycieczek ===");
    for (int i = 0; i < demos.Length; i++)
        Console.WriteLine($"{i + 1,2}. {demos[i].Name}");
    Console.WriteLine(" 0. Wyjście");
    Console.Write("Wybierz przykład: ");

    if (!int.TryParse(Console.ReadLine(), out int choice) || choice == 0) break;
    if (choice >= 1 && choice <= demos.Length) RunDemo(choice - 1);
}

void RunDemo(int index)
{
    Console.WriteLine($"\n----- {demos[index].Name} -----");
    demos[index].Run();
}
