using System.Collections.Generic;
using System.Threading.Tasks;
using TaxAlpha.Core.Models;

namespace TaxAlpha.Core.Interfaces
{
    public interface ITradingEngine
    {
        Task RunStrategy(ITradingStrategy strategy);
        Task<IReadOnlyList<PriceTick>> GetPrices(string symbol);
        decimal CalculateRoc(IReadOnlyList<PriceTick> prices, int period);
    }
}
