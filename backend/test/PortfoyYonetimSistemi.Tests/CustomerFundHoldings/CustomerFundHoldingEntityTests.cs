using System;
using Xunit;
using FluentAssertions;
using PortfoyYonetimSistemi.Entities;

namespace PortfoyYonetimSistemi.Tests.CustomerFundHoldings
{
    public class CustomerFundHoldingEntityTests
    {
        [Fact]
        public void CustomerFundHolding_ShouldBeCreatable()
        {
            // Act
            var entity = new CustomerFundHolding();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void CustomerFundHolding_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new CustomerFundHolding();

            // Assert
            entity.Id.Should().Be(default(long));

        }


    }
}
