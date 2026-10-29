using System;
using Xunit;
using FluentAssertions;
using PortfoyYonetimSistemi.Entities;

namespace PortfoyYonetimSistemi.Tests.MarketDatas
{
    public class MarketDataEntityTests
    {
        [Fact]
        public void MarketData_ShouldBeCreatable()
        {
            // Act
            var entity = new MarketData();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void MarketData_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new MarketData();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void MarketData_InstrumentName_ShouldAcceptValue()
        {
            var entity = new MarketData { InstrumentName = "Test Value" };
            entity.InstrumentName.Should().Be("Test Value");
        }

        [Fact]
        public void MarketData_InstrumentCode_ShouldAcceptValue()
        {
            var entity = new MarketData { InstrumentCode = "Test Value" };
            entity.InstrumentCode.Should().Be("Test Value");
        }

    }
}
