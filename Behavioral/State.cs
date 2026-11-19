namespace WzorceProjektowe.Behavioral;

// STATE — zachowanie zależne od stanu, bez drabinek if/switch.

// Stan bazowy — domyślnie każda akcja jest niedozwolona
public abstract class ReservationState
{
    public abstract string Name { get; }

    public virtual void Pay(Reservation r)      => Deny(nameof(Pay));
    public virtual void Confirm(Reservation r)  => Deny(nameof(Confirm));
    public virtual void Complete(Reservation r) => Deny(nameof(Complete));
    public virtual void Cancel(Reservation r)   => Deny(nameof(Cancel));

    private void Deny(string action) =>
        throw new InvalidOperationException($"Akcja {action} niedozwolona w stanie '{Name}'.");
}

public class NewState : ReservationState
{
    public override string Name => "Nowa";
    public override void Pay(Reservation r)    => r.TransitionTo(new PaidState());
    public override void Cancel(Reservation r) => r.TransitionTo(new CancelledState(refund: 0m));
}

public class PaidState : ReservationState
{
    public override string Name => "Opłacona";
    public override void Confirm(Reservation r) => r.TransitionTo(new ConfirmedState());
    public override void Cancel(Reservation r)  => r.TransitionTo(new CancelledState(r.Price));
}

public class ConfirmedState : ReservationState
{
    public override string Name => "Potwierdzona";
    public override void Complete(Reservation r) => r.TransitionTo(new CompletedState());
    public override void Cancel(Reservation r)   => r.TransitionTo(new CancelledState(r.Price * 0.7m));
}

public class CompletedState : ReservationState
{
    public override string Name => "Zrealizowana";
}

public class CancelledState(decimal refund) : ReservationState
{
    public override string Name => $"Anulowana (zwrot {refund:C})";
}

// Kontekst
public class Reservation(string client, decimal price)
{
    public string Client { get; } = client;
    public decimal Price { get; } = price;
    public ReservationState State { get; private set; } = new NewState();

    internal void TransitionTo(ReservationState next)
    {
        Console.WriteLine($"  {State.Name} -> {next.Name}");
        State = next;
    }

    public void Pay()      => State.Pay(this);
    public void Confirm()  => State.Confirm(this);
    public void Complete() => State.Complete(this);
    public void Cancel()   => State.Cancel(this);
}

public static class StateDemo
{
    public static void Run()
    {
        var r1 = new Reservation("Kowalski", 5_000m);
        r1.Pay();
        r1.Confirm();
        r1.Cancel();                 // Potwierdzona -> Anulowana (zwrot 3 500,00 zł)

        var r2 = new Reservation("Nowak", 3_000m);
        try { r2.Confirm(); }        // nie można potwierdzić nieopłaconej
        catch (InvalidOperationException ex) { Console.WriteLine($"  BŁĄD: {ex.Message}"); }
    }
}
