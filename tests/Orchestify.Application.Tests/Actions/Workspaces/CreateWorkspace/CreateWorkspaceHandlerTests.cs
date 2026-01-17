using FluentAssertions;
using Moq;
using Orchestify.Application.Actions.Workspaces.CreateWorkspace;
using Orchestify.Application.Common.Interfaces;
using Orchestify.Contracts.Workspaces;
using Orchestify.Domain.Entities;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace Orchestify.Application.Tests.Actions.Workspaces.CreateWorkspace;

public class CreateWorkspaceHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly CreateWorkspaceHandler _handler;

    public CreateWorkspaceHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        var mockSet = new Mock<DbSet<WorkspaceEntity>>();
        _mockContext.Setup(m => m.Workspaces).Returns(mockSet.Object);

        _handler = new CreateWorkspaceHandler(_mockContext.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateWorkspace_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateWorkspaceRequestDto
        {
            Name = "Test Workspace",
            RepositoryPath = "/tmp/repo",
            DefaultBranch = "main"
        };
        var command = new CreateWorkspaceCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Workspace.Name.Should().Be(request.Name);
        
        _mockContext.Verify(m => m.Workspaces.Add(It.IsAny<WorkspaceEntity>()), Times.Once);
        _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
