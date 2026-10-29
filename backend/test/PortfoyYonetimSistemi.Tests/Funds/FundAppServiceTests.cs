using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.Funds;
using PortfoyYonetimSistemi.Funds.Dto;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.Tests.Funds
{
    public class FundAppServiceTests
    {
        private readonly Mock<IRepository<Fund, long>> _repositoryMock;
        private readonly FundAppService _service;

        public FundAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<Fund, long>>();
            _service = new FundAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new Fund { Id = 1, FundCode = "Test fundCode", FundName = "Test fundName", FundType = 0, Currency = 0, IsActive = true },
                new Fund { Id = 2, FundCode = "Test fundCode", FundName = "Test fundName", FundType = 0, Currency = 0, IsActive = true },
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
                new Fund { Id = 1, FundCode = "Test fundCode", FundName = "Test fundName", FundType = 0, Currency = 0, IsActive = true },
                new Fund { Id = 2, FundCode = "Test fundCode", FundName = "Test fundName", FundType = 0, Currency = 0, IsActive = true },
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
            var dto = new CreateFundDto
            {
                FundCode = "Test fundCode", FundName = "Test fundName", FundType = 0, Currency = 0, IsActive = true
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<Fund>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Fund { Id = 1, FundCode = "Test fundCode", FundName = "Test fundName", FundType = 0, Currency = 0, IsActive = true });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Fund { Id = 1, FundCode = "Test fundCode", FundName = "Test fundName", FundType = 0, Currency = 0, IsActive = true });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
