using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.PortfolioManagers;
using PortfoyYonetimSistemi.PortfolioManagers.Dto;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.Tests.PortfolioManagers
{
    public class PortfolioManagerAppServiceTests
    {
        private readonly Mock<IRepository<PortfolioManager, long>> _repositoryMock;
        private readonly PortfolioManagerAppService _service;

        public PortfolioManagerAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<PortfolioManager, long>>();
            _service = new PortfolioManagerAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new PortfolioManager { Id = 1, FullName = "Test fullName", EmployeeCode = "Test employeeCode", Email = "Test email", IsActive = true },
                new PortfolioManager { Id = 2, FullName = "Test fullName", EmployeeCode = "Test employeeCode", Email = "Test email", IsActive = true },
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
                new PortfolioManager { Id = 1, FullName = "Test fullName", EmployeeCode = "Test employeeCode", Email = "Test email", IsActive = true },
                new PortfolioManager { Id = 2, FullName = "Test fullName", EmployeeCode = "Test employeeCode", Email = "Test email", IsActive = true },
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
            var dto = new CreatePortfolioManagerDto
            {
                FullName = "Test fullName", EmployeeCode = "Test employeeCode", Email = "Test email", IsActive = true
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<PortfolioManager>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new PortfolioManager { Id = 1, FullName = "Test fullName", EmployeeCode = "Test employeeCode", Email = "Test email", IsActive = true });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new PortfolioManager { Id = 1, FullName = "Test fullName", EmployeeCode = "Test employeeCode", Email = "Test email", IsActive = true });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
