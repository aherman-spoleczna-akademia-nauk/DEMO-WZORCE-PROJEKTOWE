namespace WzorceProjektowe.Behavioral;

// COMMAND — żądanie jako obiekt: historia, undo/redo.

// Odbiorca
public class Cart
{
    public List<(string Item, decimal Price)> Items { get; } = new();
    public decimal Discount { get; set; }
    public decimal Total => Items.Sum(i => i.Price) - Discount;
}

// Polecenie
public interface ICommand
{
    string Description { get; }
    void Execute();
    void Undo();
}

public class AddExcursionCommand(Cart cart, string name, decimal price) : ICommand
{
    public string Description => $"Dodaj: {name}";
    public void Execute() => cart.Items.Add((name, price));
    public void Undo() => cart.Items.Remove((name, price));
}

public class ApplyVoucherCommand(Cart cart, decimal amount) : ICommand
{
    private decimal _previous;                     // stan potrzebny do cofnięcia
    public string Description => $"Voucher -{amount:C}";
    public void Execute() { _previous = cart.Discount; cart.Discount = amount; }
    public void Undo() => cart.Discount = _previous;
}

// Wywołujący — nie wie, CO robi polecenie, wie tylko JAK je uruchomić i cofnąć
public class CommandManager
{
    private readonly Stack<ICommand> _undo = new();
    private readonly Stack<ICommand> _redo = new();

    public void Run(ICommand cmd)
    {
        cmd.Execute();
        _undo.Push(cmd);
        _redo.Clear();                 // nowa akcja kasuje możliwość "ponów"
        Console.WriteLine($"  > {cmd.Description}");
    }

    public void Undo()
    {
        if (!_undo.TryPop(out var cmd)) return;
        cmd.Undo();
        _redo.Push(cmd);
        Console.WriteLine($"  < cofnięto: {cmd.Description}");
    }

    public void Redo()
    {
        if (!_redo.TryPop(out var cmd)) return;
        cmd.Execute();
        _undo.Push(cmd);
        Console.WriteLine($"  > ponowiono: {cmd.Description}");
    }
}

public static class CommandDemo
{
    public static void Run()
    {
        var cart = new Cart();
        var manager = new CommandManager();

        manager.Run(new AddExcursionCommand(cart, "Safari na quadach", 260m));
        manager.Run(new AddExcursionCommand(cart, "Nurkowanie", 340m));
        manager.Run(new ApplyVoucherCommand(cart, 100m));
        Console.WriteLine($"Suma: {cart.Total:C}");   // 500,00 zł

        manager.Undo();                                  // zdejmuje voucher
        manager.Undo();                                  // usuwa nurkowanie
        Console.WriteLine($"Suma: {cart.Total:C}");   // 260,00 zł

        manager.Redo();                                  // przywraca nurkowanie
        Console.WriteLine($"Suma: {cart.Total:C}");   // 600,00 zł
    }
}
