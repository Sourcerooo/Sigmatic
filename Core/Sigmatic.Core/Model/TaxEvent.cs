using Sigmatic.Core.Enumeration;

namespace Sigmatic.Core.Model;

// Immutable Tax Result Event
public record TaxEvent(
    int Year,
    DateOnly Date,
    TaxEventType Type,
    string Symbol,
    string Isin,
    decimal RawProfit,
    decimal TaxableProfit,
    decimal UsedVap = 0m,
    decimal ForeignWht = 0m,
    // Debug info
    decimal QuantitySold = 0m,
    decimal SaleProceeds = 0m,
    decimal AcquisitionCosts = 0m
);
