using System;
using Xunit;
using FluentAssertions;
using PortfoyYonetimSistemi.Entities;

namespace PortfoyYonetimSistemi.Tests.PerformanceReports
{
    public class PerformanceReportEntityTests
    {
        [Fact]
        public void PerformanceReport_ShouldBeCreatable()
        {
            // Act
            var entity = new PerformanceReport();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void PerformanceReport_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new PerformanceReport();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void PerformanceReport_ReportName_ShouldAcceptValue()
        {
            var entity = new PerformanceReport { ReportName = "Test Value" };
            entity.ReportName.Should().Be("Test Value");
        }

    }
}
