using Sigmatic.App.Interface.Data;
using Sigmatic.App.Interface.Engine;
using Sigmatic.App.Interface.Strategy;
using Sigmatic.App.Interface.Utility;
using Sigmatic.Core.Model;

namespace Sigmatic.App.Engine;

public class TradingEngine : ITradingEngine
{
    private readonly IHistoricalPriceProvider _historicalPriceProvider;
    private readonly IPortfolio _portfolio;
    private readonly IStrategyLogger _logger;

    public TradingEngine(IHistoricalPriceProvider historicalPriceProvider, IPortfolio portfolio, IStrategyLogger logger)
    {
        _historicalPriceProvider = historicalPriceProvider;
        _portfolio = portfolio;
        _logger = logger;
    }

    public async Task RunStrategy(ITradingStrategy strategy)
    {
        _logger.LogStrategyStart(strategy.Name);
        var signals = await strategy.ExecuteAsync();
        _logger.LogTradingSignals(signals);

        var symbols = signals.Select(s => s.Symbol).Distinct();
        var prices = new Dictionary<string, decimal>();
        foreach (var symbol in symbols)
        {
            var priceTicks = await _historicalPriceProvider.GetPrices(symbol);
            if (priceTicks.Any())
            {
                prices[symbol] = priceTicks.Last().Close;
            }
        }

        var allocation = _portfolio.CalculateAllocation(signals, prices);
        _logger.LogPortfolioAllocation(allocation, _portfolio.NetLiqValue);
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
