using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaxAlpha.Core.Engine;
using TaxAlpha.Core.Interfaces;
using TaxAlpha.Core.Models;

namespace TaxAlpha.Core.Strategies
{
    public class TacticalAssetAllocationStrategy : ITradingStrategy
    {
        private readonly TradingEngine _tradingEngine;

        public TacticalAssetAllocationStrategy(TradingEngine tradingEngine)
        {
            _tradingEngine = tradingEngine;
        }

        public string Name => "Tactical Asset Allocation";

        public async Task ExecuteAsync()
        {
            var symbols = new[] { "SPY", "TIP", "BIL", "IEF" };
            var netLiqValue = 100000m; // Example Net Liq Value

            var momentum = new Dictionary<string, decimal>();
            foreach (var symbol in symbols)
            {
                var prices = await _tradingEngine.GetPrices(symbol);
                if (prices.Count == 0)
                {
                    Console.WriteLine($"No prices found for {symbol}. Skipping...");
                    continue;
                }
                var roc1 = _tradingEngine.CalculateRoc(prices, 21);
                var roc3 = _tradingEngine.CalculateRoc(prices, 63);
                var roc6 = _tradingEngine.CalculateRoc(prices, 126);
                var roc13 = _tradingEngine.CalculateRoc(prices, 273);
                var symbolMomentum = (roc1 + roc3 + roc6 + roc13) / 4;
                momentum.Add(symbol, symbolMomentum);
            }
            if (!momentum.ContainsKey("SPY") || !momentum.ContainsKey("TIP"))
            {
                Console.WriteLine("Could not calculate momentum for SPY or TIP. Exiting strategy.");
                return;
            }


            var isAggressive = momentum["SPY"] > 0 && momentum["TIP"] > 0;

            Console.WriteLine("\nEnvironment: {0}", isAggressive ? "Aggressive" : "Defensive");
            Console.WriteLine("SPY Momentum: {0:P2}", momentum["SPY"]);
            Console.WriteLine("TIP Momentum: {0:P2}", momentum["TIP"]);


            if (isAggressive)
            {
                Console.WriteLine("\nAction: Buy 100% SPY");
                var prices = await _tradingEngine.GetPrices("SPY");
                var lastClose = prices.Last().Close;
                if (lastClose > 0)
                {
                    var shares = (int)(netLiqValue / lastClose);
                    var value = shares * lastClose;
                    Console.WriteLine("- SPY: Buy {0} shares for a total of {1:C}", shares, value);
                }
            }
            else
            {
                Console.WriteLine("\n--- Defensive Environment ---");
                if (momentum.ContainsKey("BIL") && momentum.ContainsKey("IEF"))
                {
                    Console.WriteLine("BIL Momentum: {0:P2}", momentum["BIL"]);
                    Console.WriteLine("IEF Momentum: {0:P2}", momentum["IEF"]);

                    var defensiveSymbols = new[] { "BIL", "IEF" };
                    var defensiveMomentum = defensiveSymbols
                        .Where(s => momentum[s] > 0)
                        .OrderByDescending(s => momentum[s])
                        .ToList();

                    if (defensiveMomentum.Any())
                    {
                        var symbolToBuy = defensiveMomentum.First();
                        Console.WriteLine("\nAction: Buy 100% {0}", symbolToBuy);
                        var prices = await _tradingEngine.GetPrices(symbolToBuy);
                        var lastClose = prices.Last().Close;
                        if (lastClose > 0)
                        {
                            var shares = (int)(netLiqValue / lastClose);
                            var value = shares * lastClose;
                            Console.WriteLine("- {0}: Buy {1} shares for a total of {2:C}", symbolToBuy, shares, value);
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nAction: No buy signals at the moment.");
                    }
                }
                else
                {
                    Console.WriteLine("Could not calculate momentum for BIL or IEF. Exiting strategy.");
                }
            }
        }
    }
}