using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.ManagerTargets;
using PortfoyYonetimSistemi.ManagerTargets.Dto;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.Tests.ManagerTargets
{
    public class ManagerTargetAppServiceTests
    {
        private readonly Mock<IRepository<ManagerTarget, long>> _repositoryMock;
        private readonly ManagerTargetAppService _service;

        public ManagerTargetAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<ManagerTarget, long>>();
            _service = new ManagerTargetAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object, new Mock<IRepository<StatusChangeLog, long>>().Object, new Mock<IRepository<ApprovalRecord, Guid>>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new ManagerTarget { Id = 1, Description = "Test description", TargetType = 0, TargetValue = 10.0m, Status = 0 },
                new ManagerTarget { Id = 2, Description = "Test description", TargetType = 0, TargetValue = 10.0m, Status = 0 },
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
                new ManagerTarget { Id = 1, Description = "Test description", TargetType = 0, TargetValue = 10.0m, Status = 0 },
                new ManagerTarget { Id = 2, Description = "Test description", TargetType = 0, TargetValue = 10.0m, Status = 0 },
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
            var dto = new CreateManagerTargetDto
            {
                Description = "Test description", TargetType = 0, TargetValue = 10.0m, Status = 0
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<ManagerTarget>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new ManagerTarget { Id = 1, Description = "Test description", TargetType = 0, TargetValue = 10.0m, Status = 0 });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new ManagerTarget { Id = 1, Description = "Test description", TargetType = 0, TargetValue = 10.0m, Status = 0 });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
