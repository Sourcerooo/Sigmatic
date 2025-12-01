using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaxAlpha.Core.Engine;
using TaxAlpha.Core.Interfaces;
using TaxAlpha.Core.Models;

namespace TaxAlpha.Core.Strategies
{
    public class PersistentPortfolioStrategy : ITradingStrategy
    {
        private readonly TradingEngine _tradingEngine;
        private const int MovingAverageDays = 150;

        public PersistentPortfolioStrategy(TradingEngine tradingEngine)
        {
            _tradingEngine = tradingEngine;
        }

        public string Name => "Persistent Portfolio";

        public async Task<IEnumerable<Signal>> ExecuteAsync()
        {
            var symbols = new[] { "SPY", "TLT", "GLD" };
            var allocation = new Dictionary<string, decimal>
            {
                { "SPY", 0.50m },
                { "TLT", 0.35m },
                { "GLD", 0.15m }
            };
            
            var signals = new List<Signal>();

            foreach (var symbol in symbols)
            {
                var prices = await _tradingEngine.GetPrices(symbol);
                if (prices.Count >= MovingAverageDays)
                {
                    var movingAverage = prices.TakeLast(MovingAverageDays).Average(p => p.Close);
                    var lastClose = prices.Last().Close;
                    var isBuySignal = lastClose > movingAverage;
                    var description = $"Last Close: {lastClose:C}, SMA: {movingAverage:C}, Signal: {(isBuySignal ? "Buy" : "Hold/Sell")}";
                    if (isBuySignal)
                    {
                        signals.Add(new Signal(symbol, allocation[symbol], description));
                    }
                    else
                    {
                        signals.Add(new Signal(symbol, 0, description));
                    }
                }
            }
            return signals;
        }
    }
}
