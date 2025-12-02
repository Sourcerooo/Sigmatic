using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using TaxAlpha.Core.Engine;
using TaxAlpha.Core.Interfaces;
using TaxAlpha.Core.Models;
using TaxAlpha.Core.Strategies;
using Xunit;

namespace TaxAlpha.Tests
{
    public class PersistentPortfolioStrategyTests
    {
        private readonly Mock<ITradingEngine> _tradingEngineMock;
        private readonly PersistentPortfolioStrategy _strategy;

        public PersistentPortfolioStrategyTests()
        {
            _tradingEngineMock = new Mock<ITradingEngine>();
            _strategy = new PersistentPortfolioStrategy(_tradingEngineMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnBuySignals_WhenPriceIsAboveSma()
        {
            // Arrange
            var buyPrices = new List<PriceTick>();
            for (int i = 0; i < 150; i++)
            {
                buyPrices.Add(new PriceTick { Close = 100m });
            }
            buyPrices.Add(new PriceTick { Close = 110m }); // Last price > SMA (100) -> Buy

            var holdPrices = new List<PriceTick>();
            for (int i = 0; i < 150; i++)
            {
                holdPrices.Add(new PriceTick { Close = 100m });
            }
            holdPrices.Add(new PriceTick { Close = 90m }); // Last price < SMA (100) -> Hold/Sell

            _tradingEngineMock.Setup(e => e.GetPrices("SPY")).ReturnsAsync(buyPrices);
            _tradingEngineMock.Setup(e => e.GetPrices("TLT")).ReturnsAsync(holdPrices);
            _tradingEngineMock.Setup(e => e.GetPrices("GLD")).ReturnsAsync(holdPrices);

            // Act
            var signals = await _strategy.ExecuteAsync();

            // Assert
            Assert.Single(signals, s => s.Allocation > 0);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnHoldSignals_WhenPriceIsBelowSma()
        {
            // Arrange
            var prices = new List<PriceTick>();
            for (int i = 0; i < 150; i++)
            {
                prices.Add(new PriceTick { Close = 100m });
            }
            prices.Add(new PriceTick { Close = 90m });

            _tradingEngineMock.Setup(e => e.GetPrices(It.IsAny<string>())).ReturnsAsync(prices);

            // Act
            var signals = await _strategy.ExecuteAsync();

            // Assert
            Assert.DoesNotContain(signals, s => s.Allocation > 0);
        }
    }
}
