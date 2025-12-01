using System.Collections.Generic;
using TaxAlpha.Core.Models;

namespace TaxAlpha.Core.Interfaces
{
    public interface IStrategyLogger
    {
        void LogStrategyStart(string strategyName);
        void LogTradingSignals(IEnumerable<Signal> signals);
        void LogPortfolioAllocation(Dictionary<string, (int shares, decimal value)> allocation, decimal netLiqValue);
    }
}
