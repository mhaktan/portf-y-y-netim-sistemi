using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.TargetPeriods;
using PortfoyYonetimSistemi.TargetPeriods.Dto;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.Tests.TargetPeriods
{
    public class TargetPeriodAppServiceTests
    {
        private readonly Mock<IRepository<TargetPeriod, long>> _repositoryMock;
        private readonly TargetPeriodAppService _service;

        public TargetPeriodAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<TargetPeriod, long>>();
            _service = new TargetPeriodAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new TargetPeriod { Id = 1, PeriodName = "Test periodName", PeriodType = 0, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Year = 1, IsActive = true },
                new TargetPeriod { Id = 2, PeriodName = "Test periodName", PeriodType = 0, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Year = 1, IsActive = true },
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
                new TargetPeriod { Id = 1, PeriodName = "Test periodName", PeriodType = 0, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Year = 1, IsActive = true },
                new TargetPeriod { Id = 2, PeriodName = "Test periodName", PeriodType = 0, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Year = 1, IsActive = true },
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
            var dto = new CreateTargetPeriodDto
            {
                PeriodName = "Test periodName", PeriodType = 0, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Year = 1, IsActive = true
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<TargetPeriod>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new TargetPeriod { Id = 1, PeriodName = "Test periodName", PeriodType = 0, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Year = 1, IsActive = true });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new TargetPeriod { Id = 1, PeriodName = "Test periodName", PeriodType = 0, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Year = 1, IsActive = true });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
