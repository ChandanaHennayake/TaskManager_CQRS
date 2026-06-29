
using FluentAssertions;
using Moq;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Features.Tasks.Commands.UpdateTask;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using Xunit;

namespace TaskManager.Tests.Features.Tasks.Commands;

public class UpdateTaskHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITaskRepository> _taskRepositoryMock;

    private readonly UpdateTaskHandler _handler;

    public UpdateTaskHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _taskRepositoryMock = new Mock<ITaskRepository>();

        _unitOfWorkMock
            .Setup(x => x.Tasks)
            .Returns(_taskRepositoryMock.Object);

        _handler = new UpdateTaskHandler(
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateTask_WhenTaskExists()
    {
        // Arrange

        var task = new TaskItem
        {
            Id = 1,
            TaskName = "Old Title",
            Description = "Old Description",
            Status = (TaskStatus)TaskItemStatus.Pending
        };

        var command = new UpdateTaskCommand(
            1,
            "New Title",
            "New Description",
            (TaskStatus)TaskItemStatus.Completed);

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

        task.TaskName.Should().Be("New Title");

        task.Description.Should().Be("New Description");

        task.Status.Should().Be((TaskStatus)TaskItemStatus.Completed);

        task.UpdatedDate.Should().NotBeNull();

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

        var command = new UpdateTaskCommand(
            100,
            "Title",
            "Description",
            (TaskStatus)TaskItemStatus.Pending);

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

