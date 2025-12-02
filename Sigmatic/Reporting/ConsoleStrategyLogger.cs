using System;
using System.Collections.Generic;
using System.Linq;
using TaxAlpha.Core.Interfaces;
using TaxAlpha.Core.Models;

namespace TaxAlpha.Reporting
{
    public class ConsoleStrategyLogger : IStrategyLogger
    {
        public void LogStrategyStart(string strategyName)
        {
            Console.WriteLine($"\n--- Running Strategy: {strategyName} ---");
        }

        public void LogTradingSignals(IEnumerable<Signal> signals)
        {
            Console.WriteLine("\nTrading Signals:");
            foreach (var signal in signals)
            {
                Console.WriteLine($"- {signal.Symbol}: Allocation: {signal.Allocation:P2}, Description: {signal.Description}");
            }
        }

        public void LogPortfolioAllocation(Dictionary<string, (int shares, decimal value)> allocation, decimal netLiqValue)
        {
            Console.WriteLine($"\nPortfolio Allocation (Net Liq: {netLiqValue:C}):");
            if (allocation.Any())
            {
                foreach (var (symbol, (shares, value)) in allocation)
                {
                    Console.WriteLine($"- {symbol}: Buy {shares} shares for a total of {value:C}");
                }
            }
            else
            {
                Console.WriteLine("No buy signals at the moment.");
            }
        }
    }
}