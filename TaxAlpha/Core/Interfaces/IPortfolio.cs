using System.Collections.Generic;
using TaxAlpha.Core.Models;

namespace TaxAlpha.Core.Interfaces
{
    public interface IPortfolio
    {
        decimal NetLiqValue { get; set; }
        Dictionary<string, (int shares, decimal value)> CalculateAllocation(IEnumerable<Signal> signals, IReadOnlyDictionary<string, decimal> prices);
    }
}
