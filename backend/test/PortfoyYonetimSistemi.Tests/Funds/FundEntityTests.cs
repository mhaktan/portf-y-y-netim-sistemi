using System;
using Xunit;
using FluentAssertions;
using PortfoyYonetimSistemi.Entities;

namespace PortfoyYonetimSistemi.Tests.Funds
{
    public class FundEntityTests
    {
        [Fact]
        public void Fund_ShouldBeCreatable()
        {
            // Act
            var entity = new Fund();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void Fund_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new Fund();

            // Assert
            entity.Id.Should().Be(default(long));
            entity.IsActive.Should().Be(false);
        }

        [Fact]
        public void Fund_FundCode_ShouldAcceptValue()
        {
            var entity = new Fund { FundCode = "Test Value" };
            entity.FundCode.Should().Be("Test Value");
        }

        [Fact]
        public void Fund_FundName_ShouldAcceptValue()
        {
            var entity = new Fund { FundName = "Test Value" };
            entity.FundName.Should().Be("Test Value");
        }

    }
}
