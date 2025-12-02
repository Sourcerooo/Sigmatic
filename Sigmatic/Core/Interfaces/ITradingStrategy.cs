using System.Collections.Generic;
using System.Threading.Tasks;
using TaxAlpha.Core.Models;

namespace TaxAlpha.Core.Interfaces
{
    public interface ITradingStrategy
    {
        string Name { get; }
        Task<IEnumerable<Signal>> ExecuteAsync();
    }
}
