using System;
using Xunit;
using FluentAssertions;
using PortfoyYonetimSistemi.Entities;

namespace PortfoyYonetimSistemi.Tests.TargetPeriods
{
    public class TargetPeriodEntityTests
    {
        [Fact]
        public void TargetPeriod_ShouldBeCreatable()
        {
            // Act
            var entity = new TargetPeriod();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void TargetPeriod_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new TargetPeriod();

            // Assert
            entity.Id.Should().Be(default(long));
            entity.IsActive.Should().Be(false);
        }

        [Fact]
        public void TargetPeriod_PeriodName_ShouldAcceptValue()
        {
            var entity = new TargetPeriod { PeriodName = "Test Value" };
            entity.PeriodName.Should().Be("Test Value");
        }

    }
}
