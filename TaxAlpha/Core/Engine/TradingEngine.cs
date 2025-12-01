using TaxAlpha.Core.Interfaces;
using TaxAlpha.Core.Models;

namespace TaxAlpha.Core.Engine
{
    public class TradingEngine
    {
        private readonly IHistoricalPriceProvider _historicalPriceProvider;

        public TradingEngine(IHistoricalPriceProvider historicalPriceProvider)
        {
            _historicalPriceProvider = historicalPriceProvider;
        }

        public async Task<IReadOnlyList<PriceTick>> GetPrices(string symbol)
        {
            return await _historicalPriceProvider.GetPrices(symbol);
        }

        public decimal CalculateRoc(IReadOnlyList<PriceTick> prices, int period)
        {
            if (prices.Count < period)
            {
                return 0;
            }

            var lastPrice = prices.Last().Close;
            var earlierPrice = prices[prices.Count - period].Close;

            if (earlierPrice == 0)
            {
                return 0;
            }

            return (lastPrice - earlierPrice) / earlierPrice;
        }
    }
}
