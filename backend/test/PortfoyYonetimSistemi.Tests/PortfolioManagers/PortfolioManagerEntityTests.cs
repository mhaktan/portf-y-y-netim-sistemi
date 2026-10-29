using System;
using Xunit;
using FluentAssertions;
using PortfoyYonetimSistemi.Entities;

namespace PortfoyYonetimSistemi.Tests.PortfolioManagers
{
    public class PortfolioManagerEntityTests
    {
        [Fact]
        public void PortfolioManager_ShouldBeCreatable()
        {
            // Act
            var entity = new PortfolioManager();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void PortfolioManager_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new PortfolioManager();

            // Assert
            entity.Id.Should().Be(default(long));
            entity.IsActive.Should().Be(false);
        }

        [Fact]
        public void PortfolioManager_FullName_ShouldAcceptValue()
        {
            var entity = new PortfolioManager { FullName = "Test Value" };
            entity.FullName.Should().Be("Test Value");
        }

        [Fact]
        public void PortfolioManager_EmployeeCode_ShouldAcceptValue()
        {
            var entity = new PortfolioManager { EmployeeCode = "Test Value" };
            entity.EmployeeCode.Should().Be("Test Value");
        }

        [Fact]
        public void PortfolioManager_Email_ShouldAcceptValue()
        {
            var entity = new PortfolioManager { Email = "Test Value" };
            entity.Email.Should().Be("Test Value");
        }

    }
}
