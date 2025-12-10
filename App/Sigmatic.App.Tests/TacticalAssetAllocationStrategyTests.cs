using Moq;
using Sigmatic.App.Interface.Engine;
using Sigmatic.App.Strategy;
using Sigmatic.Core.Model;
using Xunit;

namespace Sigmatic.App.Tests;

public class TacticalAssetAllocationStrategyTests
{
    private readonly Mock<ITradingEngine> _tradingEngineMock;
    private readonly TacticalAssetAllocationStrategy _strategy;

    public TacticalAssetAllocationStrategyTests()
    {
        _tradingEngineMock = new Mock<ITradingEngine>();
        _strategy = new TacticalAssetAllocationStrategy(_tradingEngineMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnSpySignal_InAggressiveEnvironment()
    {
        // Arrange
        var prices = new List<PriceTick>();
        for (int i = 0; i < 273; i++)
        {
            prices.Add(new PriceTick { Close = 100m + i * 0.1m });
        }

        _tradingEngineMock.Setup(e => e.GetPrices(It.IsAny<string>())).ReturnsAsync(prices);
        _tradingEngineMock.Setup(e => e.CalculateRoc(It.IsAny<IReadOnlyList<PriceTick>>(), It.IsAny<int>())).Returns(0.1m);


        // Act
        var signals = await _strategy.ExecuteAsync();

        // Assert
        Assert.Single(signals);
        Assert.Equal("SPY", signals.First().Symbol);
        Assert.Equal(1.0m, signals.First().Allocation);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnDefensiveSignal_InDefensiveEnvironment()
    {
        // Arrange
        // Prices for SPY and TIP (falling to ensure defensive environment)
        var fallingPrices = new List<PriceTick>();
        for (int i = 0; i < 273; i++)
        {
            fallingPrices.Add(new PriceTick { Close = 100m - i * 0.1m });
        }

        // Prices for BIL/IEF (rising to ensure positive momentum for defensive assets)
        var risingPrices = new List<PriceTick>();
        for (int i = 0; i < 273; i++)
        {
            risingPrices.Add(new PriceTick { Close = 100m + i * 0.1m });
        }

        _tradingEngineMock.Setup(e => e.GetPrices("SPY")).ReturnsAsync(fallingPrices);
        _tradingEngineMock.Setup(e => e.GetPrices("TIP")).ReturnsAsync(fallingPrices);
        _tradingEngineMock.Setup(e => e.GetPrices("BIL")).ReturnsAsync(risingPrices);
        _tradingEngineMock.Setup(e => e.GetPrices("IEF")).ReturnsAsync(fallingPrices); // Can be falling or rising, as long as one defensive asset is rising

        // SPY and TIP will have negative ROC from fallingPrices
        _tradingEngineMock.Setup(e => e.CalculateRoc(fallingPrices, It.IsAny<int>())).Returns(-0.1m);
        // BIL will have positive ROC from risingPrices
        _tradingEngineMock.Setup(e => e.CalculateRoc(risingPrices, It.IsAny<int>())).Returns(0.1m);

        // Act
        var signals = await _strategy.ExecuteAsync();

        // Assert
        Assert.Single(signals);
        Assert.True(signals.First().Symbol == "BIL" || signals.First().Symbol == "IEF");
        Assert.Equal(1.0m, signals.First().Allocation);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNoSignal_InDefensiveEnvironment_WithNegativeMomentum()
    {
        // Arrange
        var prices = new List<PriceTick>();
        for (int i = 0; i < 273; i++)
        {
            prices.Add(new PriceTick { Close = 100m - i * 0.1m });
        }
        _tradingEngineMock.Setup(e => e.GetPrices(It.IsAny<string>())).ReturnsAsync(prices);
        _tradingEngineMock.Setup(e => e.CalculateRoc(It.IsAny<IReadOnlyList<PriceTick>>(), It.IsAny<int>())).Returns(-0.1m);

        // Act
        var signals = await _strategy.ExecuteAsync();

        // Assert
        Assert.Empty(signals);
    }
}
