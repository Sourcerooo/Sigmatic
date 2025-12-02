using System.Collections.Generic;
using TaxAlpha.Core.Models;
using Xunit;

namespace TaxAlpha.Tests
{
    public class PortfolioTests
    {
        [Fact]
        public void CalculateAllocation_ShouldCalculateCorrectAllocation()
        {
            // Arrange
            var portfolio = new Portfolio(100000m);
            var signals = new List<Signal>
            {
                new("SPY", 0.5m),
                new("TLT", 0.5m)
            };
            var prices = new Dictionary<string, decimal>
            {
                { "SPY", 100m },
                { "TLT", 50m }
            };

            // Act
            var allocation = portfolio.CalculateAllocation(signals, prices);

            // Assert
            Assert.Equal(2, allocation.Count);
            Assert.Equal(500, allocation["SPY"].shares);
            Assert.Equal(50000m, allocation["SPY"].value);
            Assert.Equal(1000, allocation["TLT"].shares);
            Assert.Equal(50000m, allocation["TLT"].value);
        }
    }
}
