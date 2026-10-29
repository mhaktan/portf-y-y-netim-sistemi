using System;
using Xunit;
using FluentAssertions;
using PortfoyYonetimSistemi.Entities;

namespace PortfoyYonetimSistemi.Tests.Customers
{
    public class CustomerEntityTests
    {
        [Fact]
        public void Customer_ShouldBeCreatable()
        {
            // Act
            var entity = new Customer();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void Customer_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new Customer();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void Customer_FullName_ShouldAcceptValue()
        {
            var entity = new Customer { FullName = "Test Value" };
            entity.FullName.Should().Be("Test Value");
        }

        [Fact]
        public void Customer_CustomerCode_ShouldAcceptValue()
        {
            var entity = new Customer { CustomerCode = "Test Value" };
            entity.CustomerCode.Should().Be("Test Value");
        }

    }
}
