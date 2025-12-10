using Sigmatic.Core.Model;

namespace Sigmatic.Core.Interface;

public interface ITradingStrategy
{
    string Name { get; }
    Task<IEnumerable<Signal>> ExecuteAsync();
}

