using System;
using Xunit;
using FluentAssertions;
using PortfoyYonetimSistemi.Entities;

namespace PortfoyYonetimSistemi.Tests.CustomerManagerTransfers
{
    public class CustomerManagerTransferEntityTests
    {
        [Fact]
        public void CustomerManagerTransfer_ShouldBeCreatable()
        {
            // Act
            var entity = new CustomerManagerTransfer();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void CustomerManagerTransfer_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new CustomerManagerTransfer();

            // Assert
            entity.Id.Should().Be(default(long));

        }


    }
}
