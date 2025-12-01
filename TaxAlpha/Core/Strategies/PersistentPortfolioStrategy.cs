using System;
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

        public async Task ExecuteAsync()
        {
            var symbols = new[] { "SPY", "TLT", "GLD" };
            var allocation = new Dictionary<string, decimal>
            {
                { "SPY", 0.50m },
                { "TLT", 0.35m },
                { "GLD", 0.15m }
            };
            var netLiqValue = 100000m; // Example Net Liq Value

            var signals = await GetTradingSignals(symbols);

            Console.WriteLine($"\nTrading Signals (SMA 150):");
            foreach (var (symbol, (isBuySignal, movingAverage, lastClose)) in signals)
            {
                Console.WriteLine($"- {symbol}: Last Close: {lastClose:C}, SMA: {movingAverage:C}, Signal: {(isBuySignal ? "Buy" : "Hold/Sell")}");
            }

            var portfolioAllocation = CalculatePortfolioAllocation(netLiqValue, signals, allocation);

            Console.WriteLine($"\nPortfolio Allocation (Net Liq: {netLiqValue:C}):");
            if (portfolioAllocation.Any())
            {
                foreach (var (symbol, (shares, value)) in portfolioAllocation)
                {
                    Console.WriteLine($"- {symbol}: Buy {shares} shares for a total of {value:C}");
                }
            }
            else
            {
                Console.WriteLine("No buy signals at the moment.");
            }
        }

        private async Task<Dictionary<string, (bool isBuySignal, decimal movingAverage, decimal lastClose)>> GetTradingSignals(params string[] symbols)
        {
            var signals = new Dictionary<string, (bool isBuySignal, decimal movingAverage, decimal lastClose)>();

            foreach (var symbol in symbols)
            {
                var prices = await _tradingEngine.GetPrices(symbol);
                if (prices.Count >= MovingAverageDays)
                {
                    var movingAverage = prices.TakeLast(MovingAverageDays).Average(p => p.Close);
                    var lastClose = prices.Last().Close;
                    var isBuySignal = lastClose > movingAverage;
                    signals.Add(symbol, (isBuySignal, movingAverage, lastClose));
                }
            }

            return signals;
        }

        private Dictionary<string, (int shares, decimal value)> CalculatePortfolioAllocation(
            decimal netLiqValue,
            Dictionary<string, (bool isBuySignal, decimal movingAverage, decimal lastClose)> signals,
            Dictionary<string, decimal> allocation)
        {
            var portfolio = new Dictionary<string, (int shares, decimal value)>();

            foreach (var (symbol, (isBuySignal, _, lastClose)) in signals)
            {
                if (isBuySignal && allocation.TryGetValue(symbol, out var targetAllocation))
                {
                    var targetValue = netLiqValue * targetAllocation;
                    if (lastClose > 0)
                    {
                        var shares = (int)(targetValue / lastClose);
                        var value = shares * lastClose;
                        portfolio.Add(symbol, (shares, value));
                    }
                }
            }

            return portfolio;
        }
    }
}
