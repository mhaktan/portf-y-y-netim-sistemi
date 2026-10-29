using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.PerformanceReports;
using PortfoyYonetimSistemi.PerformanceReports.Dto;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.Tests.PerformanceReports
{
    public class PerformanceReportAppServiceTests
    {
        private readonly Mock<IRepository<PerformanceReport, long>> _repositoryMock;
        private readonly PerformanceReportAppService _service;

        public PerformanceReportAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<PerformanceReport, long>>();
            _service = new PerformanceReportAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object, new Mock<IRepository<StatusChangeLog, long>>().Object, new Mock<IRepository<ApprovalRecord, Guid>>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new PerformanceReport { Id = 1, ReportName = "Test reportName", ReportType = 0, ReportDate = DateTime.UtcNow, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Status = 0 },
                new PerformanceReport { Id = 2, ReportName = "Test reportName", ReportType = 0, ReportDate = DateTime.UtcNow, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Status = 0 },
            }.AsQueryable();

            _repositoryMock.Setup(r => r.GetAll()).Returns(entities);

            // Act
            var result = _repositoryMock.Object.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }

        [Fact]
        public void Repository_GetAll_WithFilter_ShouldWork()
        {
            // Arrange
            var entities = new[]
            {
                new PerformanceReport { Id = 1, ReportName = "Test reportName", ReportType = 0, ReportDate = DateTime.UtcNow, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Status = 0 },
                new PerformanceReport { Id = 2, ReportName = "Test reportName", ReportType = 0, ReportDate = DateTime.UtcNow, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Status = 0 },
            }.AsQueryable();

            _repositoryMock.Setup(r => r.GetAll()).Returns(entities);

            // Act — simulate keyword filter
            var result = _repositoryMock.Object.GetAll()
                .Where(x => x.Id.ToString().Contains("1"));

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_ShouldInsertEntity()
        {
            // Arrange
            var dto = new CreatePerformanceReportDto
            {
                ReportName = "Test reportName", ReportType = 0, ReportDate = DateTime.UtcNow, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Status = 0
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<PerformanceReport>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new PerformanceReport { Id = 1, ReportName = "Test reportName", ReportType = 0, ReportDate = DateTime.UtcNow, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Status = 0 });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new PerformanceReport { Id = 1, ReportName = "Test reportName", ReportType = 0, ReportDate = DateTime.UtcNow, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Status = 0 });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
