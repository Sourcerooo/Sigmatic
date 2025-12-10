using Sigmatic.App.Interface.Engine;
using Sigmatic.Core.Model;

namespace Sigmatic.App.Model;

public class Portfolio : IPortfolio
{
    public decimal NetLiqValue { get; set; }

    public Portfolio(decimal netLiqValue)
    {
        NetLiqValue = netLiqValue;
    }

    public Dictionary<string, (int shares, decimal value)> CalculateAllocation(IEnumerable<Signal> signals, IReadOnlyDictionary<string, decimal> prices)
    {
        var allocation = new Dictionary<string, (int shares, decimal value)>();

        foreach (var signal in signals)
        {
            if (prices.TryGetValue(signal.Symbol, out var price) && price > 0)
            {
                var targetValue = NetLiqValue * signal.Allocation;
                var shares = (int)(targetValue / price);
                var value = shares * price;
                allocation.Add(signal.Symbol, (shares, value));
            }
        }

        return allocation;
    }
}
