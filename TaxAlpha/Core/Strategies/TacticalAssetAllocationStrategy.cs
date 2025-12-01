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

        public async Task<IEnumerable<Signal>> ExecuteAsync()
        {
            var symbols = new[] { "SPY", "TIP", "BIL", "IEF" };
            
            var momentum = new Dictionary<string, decimal>();
            foreach (var symbol in symbols)
            {
                var prices = await _tradingEngine.GetPrices(symbol);
                if (prices.Count == 0)
                {
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
                return new List<Signal>();
            }

            var isAggressive = momentum["SPY"] > 0 && momentum["TIP"] > 0;
            var signals = new List<Signal>();

            if (isAggressive)
            {
                signals.Add(new Signal("SPY", 1.0m, $"Aggressive environment: SPY Momentum {momentum["SPY"]:P2}, TIP Momentum {momentum["TIP"]:P2}"));
            }
            else
            {
                if (momentum.ContainsKey("BIL") && momentum.ContainsKey("IEF"))
                {
                    var defensiveSymbols = new[] { "BIL", "IEF" };
                    var defensiveMomentum = defensiveSymbols
                        .Where(s => momentum.ContainsKey(s) && momentum[s] > 0)
                        .OrderByDescending(s => momentum[s])
                        .ToList();

                    if (defensiveMomentum.Any())
                    {
                        var symbolToBuy = defensiveMomentum.First();
                        signals.Add(new Signal(symbolToBuy, 1.0m, $"Defensive environment: {symbolToBuy} Momentum {momentum[symbolToBuy]:P2}"));
                    }
                }
            }

            return signals;
        }
    }
}