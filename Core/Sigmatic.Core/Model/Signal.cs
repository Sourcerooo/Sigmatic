namespace Sigmatic.Core.Model;

public class Signal
{
    public string Symbol { get; }
    public decimal Allocation { get; }
    public string? Description { get; }

    public Signal(string symbol, decimal allocation, string? description = null)
    {
        Symbol = symbol;
        Allocation = allocation;
        Description = description;
    }
}

