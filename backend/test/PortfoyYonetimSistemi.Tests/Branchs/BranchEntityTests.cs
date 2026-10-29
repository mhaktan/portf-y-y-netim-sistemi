using System;
using Xunit;
using FluentAssertions;
using PortfoyYonetimSistemi.Entities;

namespace PortfoyYonetimSistemi.Tests.Branchs
{
    public class BranchEntityTests
    {
        [Fact]
        public void Branch_ShouldBeCreatable()
        {
            // Act
            var entity = new Branch();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void Branch_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new Branch();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void Branch_Name_ShouldAcceptValue()
        {
            var entity = new Branch { Name = "Test Value" };
            entity.Name.Should().Be("Test Value");
        }

        [Fact]
        public void Branch_Code_ShouldAcceptValue()
        {
            var entity = new Branch { Code = "Test Value" };
            entity.Code.Should().Be("Test Value");
        }

    }
}
