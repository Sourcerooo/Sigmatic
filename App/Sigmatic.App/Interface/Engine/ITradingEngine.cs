using Sigmatic.App.Interface.Strategy;
using Sigmatic.Core.Model;

namespace Sigmatic.App.Interface.Engine;

/// <summary>
/// Represents the core trading engine used to execute strategies and provide market data and calculations.
/// Implementations are responsible for running strategies, retrieving historical price ticks, and computing performance metrics.
/// </summary>
public interface ITradingEngine
{
    /// <summary>
    /// Executes the supplied trading strategy using this engine.
    /// The method runs asynchronously and completes when the strategy has finished its execution.
    /// Implementations should coordinate order execution, portfolio updates and logging as required by the strategy.
    /// </summary>
    /// <param name="strategy">The trading strategy to execute. The engine may call back into the strategy during execution.</param>
    /// <returns>A task that completes when the strategy run has finished.</returns>
    Task RunStrategy(ITradingStrategy strategy);

    /// <summary>
    /// Retrieves historical price ticks for the given symbol.
    /// The returned list should be ordered chronologically (oldest first) and be read-only to callers.
    /// </summary>
    /// <param name="symbol">The instrument symbol to fetch price history for (e.g. ticker).</param>
    /// <returns>
    /// A task that resolves to a read-only list of <see cref="PriceTick"/> objects containing historical prices.
    /// Returns an empty list if no data is available for the symbol.
    /// </returns>
    Task<IReadOnlyList<PriceTick>> GetPrices(string symbol);

    /// <summary>
    /// Calculates the rate of change (ROC) for the provided price series over the specified period.
    /// The calculation is based on the price ticks supplied and the number of periods to look back.
    /// </summary>
    /// <param name="prices">A read-only list of <see cref="PriceTick"/> representing the time series to compute ROC from.</param>
    /// <param name="period">The number of ticks (periods) to use for the ROC calculation.</param>
    /// <returns>
    /// The ROC value as a decimal. The numeric meaning (absolute vs. percentage) should be documented by the concrete implementation;
    /// typically this returns a percentage change (e.g. 0.05 for +5%) or a raw ratio depending on project conventions.
    /// </returns>
    decimal CalculateRoc(IReadOnlyList<PriceTick> prices, int period);
}
