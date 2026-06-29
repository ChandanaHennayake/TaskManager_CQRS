
using FluentAssertions;
using Moq;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Features.Tasks.Commands.DeleteTask;
using TaskManager.Domain.Entities;
using Xunit;

namespace TaskManager.Tests.Features.Tasks.Commands;

public class DeleteTaskHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITaskRepository> _taskRepositoryMock;

    private readonly DeleteTaskHandler _handler;

    public DeleteTaskHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _taskRepositoryMock = new Mock<ITaskRepository>();

        _unitOfWorkMock
            .Setup(x => x.Tasks)
            .Returns(_taskRepositoryMock.Object);

        _handler = new DeleteTaskHandler(
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldSoftDeleteTask_WhenTaskExists()
    {
        // Arrange

        var task = new TaskItem
        {
            Id = 1,
            TaskName = "Learn Testing",
            Description = "Write Unit Tests",
            IsDeleted = false
        };

        var command = new DeleteTaskCommand(1);

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(
                command.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act

        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        result.Should().BeTrue();

        task.IsDeleted.Should().BeTrue();

        task.DeletedBy.Should().Be("admin");

        task.DeletedDate.Should().NotBeNull();

        _taskRepositoryMock.Verify(x =>
            x.Update(task),
            Times.Once);

        _unitOfWorkMock.Verify(x =>
            x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
    {
        // Arrange

        var command = new DeleteTaskCommand(100);

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(
                command.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act

        Func<Task> action = async () =>
            await _handler.Handle(
                command,
                CancellationToken.None);

        // Assert

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Task with Id 100 was not found.");

        _taskRepositoryMock.Verify(x =>
            x.Update(It.IsAny<TaskItem>()),
            Times.Never);

        _unitOfWorkMock.Verify(x =>
            x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}

