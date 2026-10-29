using System;
using Xunit;
using FluentAssertions;
using PortfoyYonetimSistemi.Entities;

namespace PortfoyYonetimSistemi.Tests.WorkAssignments
{
    public class WorkAssignmentEntityTests
    {
        [Fact]
        public void WorkAssignment_ShouldBeCreatable()
        {
            // Act
            var entity = new WorkAssignment();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void WorkAssignment_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new WorkAssignment();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void WorkAssignment_Title_ShouldAcceptValue()
        {
            var entity = new WorkAssignment { Title = "Test Value" };
            entity.Title.Should().Be("Test Value");
        }

    }
}
