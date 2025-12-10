namespace Sigmatic.App.Interface.Data;

using Sigmatic.Core.Model;

public interface IInstrumentProvider
{
    Instrument GetInstrument(string isin, string fallbackSymbol);
}