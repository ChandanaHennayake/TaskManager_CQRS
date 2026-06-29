
using FluentAssertions;
using Moq;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Features.Tasks.Queries.GetTaskById;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using Xunit;

namespace TaskManager.Tests.Features.Tasks.Queries;

public class GetTaskByIdHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly GetTaskByIdHandler _handler;

    public GetTaskByIdHandlerTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();

        _handler = new GetTaskByIdHandler(
            _taskRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange

        var task = new TaskItem
        {
            Id = 1,
            TaskName = "Learn Unit Testing",
            Description = "Using xUnit and Moq",
            Status = (TaskStatus)TaskItemStatus.InProgress,
            CreatedDate = DateTime.UtcNow
        };

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(
                task.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        // Act

        var result = await _handler.Handle(
            new GetTaskByIdQuery(task.Id),
            CancellationToken.None);

        // Assert

        result.Should().NotBeNull();

        result.Id.Should().Be(task.Id);

        result.Title.Should().Be(task.TaskName);

        result.Description.Should().Be(task.Description);

        result.Status.Should().Be(task.Status);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
    {
        // Arrange

        _taskRepositoryMock
            .Setup(x => x.GetByIdAsync(
                100,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act

        Func<Task> action = async () =>
            await _handler.Handle(
                new GetTaskByIdQuery(100),
                CancellationToken.None);

        // Assert

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Task with Id 100 was not found.");
    }
}

