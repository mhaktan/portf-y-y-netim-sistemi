using System;
using Xunit;
using FluentAssertions;
using PortfoyYonetimSistemi.Entities;

namespace PortfoyYonetimSistemi.Tests.ManagerTargets
{
    public class ManagerTargetEntityTests
    {
        [Fact]
        public void ManagerTarget_ShouldBeCreatable()
        {
            // Act
            var entity = new ManagerTarget();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void ManagerTarget_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new ManagerTarget();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void ManagerTarget_Description_ShouldAcceptValue()
        {
            var entity = new ManagerTarget { Description = "Test Value" };
            entity.Description.Should().Be("Test Value");
        }

    }
}
