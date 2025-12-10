using Sigmatic.Core.Model;

namespace Sigmatic.App.Interface.Utility;

public interface IStrategyLogger
{
    void LogStrategyStart(string strategyName);
    void LogTradingSignals(IEnumerable<Signal> signals);
    void LogPortfolioAllocation(Dictionary<string, (int shares, decimal value)> allocation, decimal netLiqValue);
}
