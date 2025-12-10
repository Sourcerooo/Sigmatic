using Sigmatic.Core.Model;

namespace Sigmatic.App.Interface.Engine;

public interface IPortfolio
{
    decimal NetLiqValue { get; set; }
    Dictionary<string, (int shares, decimal value)> CalculateAllocation(IEnumerable<Signal> signals, IReadOnlyDictionary<string, decimal> prices);
}
