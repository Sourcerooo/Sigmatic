using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using TaxAlpha.Core.Engine;
using TaxAlpha.Core.Interfaces;
using TaxAlpha.Core.Models;
using Xunit;

namespace TaxAlpha.Tests
{
    public class TradingEngineTests
    {
        private readonly Mock<IHistoricalPriceProvider> _historicalPriceProviderMock;
        private readonly Mock<IPortfolio> _portfolioMock;
        private readonly Mock<IStrategyLogger> _loggerMock;
        private readonly TradingEngine _tradingEngine;

        public TradingEngineTests()
        {
            _historicalPriceProviderMock = new Mock<IHistoricalPriceProvider>();
            _portfolioMock = new Mock<IPortfolio>();
            _loggerMock = new Mock<IStrategyLogger>();
            _tradingEngine = new TradingEngine(_historicalPriceProviderMock.Object, _portfolioMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task RunStrategy_ShouldExecuteStrategyAndLogResults()
        {
            // Arrange
            var strategyMock = new Mock<ITradingStrategy>();
            var signals = new List<Signal> { new("SPY", 1.0m) };
            var prices = new Dictionary<string, decimal> { { "SPY", 100m } };
            var allocation = new Dictionary<string, (int shares, decimal value)> { { "SPY", (10, 1000m) } };

            strategyMock.Setup(s => s.ExecuteAsync()).ReturnsAsync(signals);
            _historicalPriceProviderMock.Setup(p => p.GetPrices("SPY")).ReturnsAsync(new List<PriceTick> { new() { Close = 100m } });
            _portfolioMock.Setup(p => p.CalculateAllocation(signals, It.IsAny<IReadOnlyDictionary<string, decimal>>())).Returns(allocation);

            // Act
            await _tradingEngine.RunStrategy(strategyMock.Object);

            // Assert
            strategyMock.Verify(s => s.ExecuteAsync(), Times.Once);
            _loggerMock.Verify(l => l.LogStrategyStart(It.IsAny<string>()), Times.Once);
            _loggerMock.Verify(l => l.LogTradingSignals(signals), Times.Once);
            _portfolioMock.Verify(p => p.CalculateAllocation(signals, It.IsAny<IReadOnlyDictionary<string, decimal>>()), Times.Once);
            _loggerMock.Verify(l => l.LogPortfolioAllocation(allocation, It.IsAny<decimal>()), Times.Once);
        }

        [Fact]
        public async Task GetPrices_ShouldReturnPricesFromProvider()
        {
            // Arrange
            var expectedPrices = new List<PriceTick> { new() { Close = 100m } };
            _historicalPriceProviderMock.Setup(p => p.GetPrices("SPY")).ReturnsAsync(expectedPrices);

            // Act
            var actualPrices = await _tradingEngine.GetPrices("SPY");

            // Assert
            Assert.Equal(expectedPrices, actualPrices);
        }

        [Fact]
        public void CalculateRoc_ShouldCalculateCorrectly()
        {
            // Arrange
            var prices = new List<PriceTick>
            {
                new() { Close = 100m },
                new() { Close = 110m },
                new() { Close = 120m },
                new() { Close = 130m },
                new() { Close = 140m },
                new() { Close = 150m },
            };

            // Act
            var roc = _tradingEngine.CalculateRoc(prices, 5);

            // Assert
            Assert.Equal(40m / 110m, roc);
        }
    }
}
