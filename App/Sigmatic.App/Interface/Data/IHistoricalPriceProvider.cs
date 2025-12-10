namespace Sigmatic.App.Interface.Data;

using Sigmatic.Core.Model;

public interface IHistoricalPriceProvider
{
    Task<IReadOnlyList<PriceTick>> GetPrices(string symbol);
}
