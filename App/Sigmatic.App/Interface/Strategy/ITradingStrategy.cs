using Sigmatic.Core.Model;

namespace Sigmatic.App.Interface.Strategy;

public interface ITradingStrategy
{
    string Name { get; }
    Task<IEnumerable<Signal>> ExecuteAsync();
}
