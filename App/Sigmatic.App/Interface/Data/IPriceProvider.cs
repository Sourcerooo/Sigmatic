namespace Sigmatic.App.Interface.Data;

public interface IPriceProvider
{
    decimal? GetPrice(string isin, int year);
}
