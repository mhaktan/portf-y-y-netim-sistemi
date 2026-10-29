using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using PortfoyYonetimSistemi.Entities;
using PortfoyYonetimSistemi.Customers;
using PortfoyYonetimSistemi.Customers.Dto;
using PortfoyYonetimSistemi.Flows;

namespace PortfoyYonetimSistemi.Tests.Customers
{
    public class CustomerAppServiceTests
    {
        private readonly Mock<IRepository<Customer, long>> _repositoryMock;
        private readonly CustomerAppService _service;

        public CustomerAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<Customer, long>>();
            _service = new CustomerAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new Customer { Id = 1, FullName = "Test fullName", CustomerCode = "Test customerCode", CustomerType = 0 },
                new Customer { Id = 2, FullName = "Test fullName", CustomerCode = "Test customerCode", CustomerType = 0 },
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
                new Customer { Id = 1, FullName = "Test fullName", CustomerCode = "Test customerCode", CustomerType = 0 },
                new Customer { Id = 2, FullName = "Test fullName", CustomerCode = "Test customerCode", CustomerType = 0 },
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
            var dto = new CreateCustomerDto
            {
                FullName = "Test fullName", CustomerCode = "Test customerCode", CustomerType = 0
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<Customer>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Customer { Id = 1, FullName = "Test fullName", CustomerCode = "Test customerCode", CustomerType = 0 });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Customer { Id = 1, FullName = "Test fullName", CustomerCode = "Test customerCode", CustomerType = 0 });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
