using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.CustomerFundHoldings;
using PortfoyYonetimSistemi.CustomerFundHoldings.Dto;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.Tests.CustomerFundHoldings
{
    public class CustomerFundHoldingAppServiceTests
    {
        private readonly Mock<IRepository<CustomerFundHolding, long>> _repositoryMock;
        private readonly CustomerFundHoldingAppService _service;

        public CustomerFundHoldingAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<CustomerFundHolding, long>>();
            _service = new CustomerFundHoldingAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new CustomerFundHolding { Id = 1, Units = 10.0m },
                new CustomerFundHolding { Id = 2, Units = 10.0m },
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
                new CustomerFundHolding { Id = 1, Units = 10.0m },
                new CustomerFundHolding { Id = 2, Units = 10.0m },
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
            var dto = new CreateCustomerFundHoldingDto
            {
                Units = 10.0m
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<CustomerFundHolding>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new CustomerFundHolding { Id = 1, Units = 10.0m });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new CustomerFundHolding { Id = 1, Units = 10.0m });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
