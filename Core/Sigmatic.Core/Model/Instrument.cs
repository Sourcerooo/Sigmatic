namespace Sigmatic.Core.Model;

// Immutable Configuration
public record Instrument(
    string Isin,
    string Symbol,
    string Name,
    decimal TfsQuote // "Teilfreistellungsquote" used for german taxes
);
