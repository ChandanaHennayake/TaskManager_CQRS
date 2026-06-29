
using FluentAssertions;
using Moq;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Features.Tasks.Commands.CreateTask;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using Xunit;

namespace TaskManager.Tests.Features.Tasks.Commands;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITaskRepository> _taskRepositoryMock;

    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _taskRepositoryMock = new Mock<ITaskRepository>();

        _unitOfWorkMock
            .Setup(x => x.Tasks)
            .Returns(_taskRepositoryMock.Object);

        _handler = new CreateTaskCommandHandler(
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateTask_WhenRequestIsValid()
    {
        // Arrange

        var command = new CreateTaskCommand(
            "Learn CQRS",
            "Study MediatR and CQRS pattern",
            (TaskStatus)TaskItemStatus.Pending);

        TaskItem? addedTask = null;

        _taskRepositoryMock
            .Setup(x => x.AddAsync(
                It.IsAny<TaskItem>(),
                It.IsAny<CancellationToken>()))
            .Callback<TaskItem, CancellationToken>((task, _) =>
            {
                addedTask = task;
                task.Id = 1;
            })
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act

        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        result.Should().Be(1);

        addedTask.Should().NotBeNull();

        addedTask!.TaskName.Should().Be("Learn CQRS");

        addedTask.Description.Should().Be("Study MediatR and CQRS pattern");

        addedTask.Status.Should().Be((TaskStatus)TaskItemStatus.Pending);

        _taskRepositoryMock.Verify(x =>
            x.AddAsync(
                It.IsAny<TaskItem>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(x =>
            x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldTrim_Title_And_Description()
    {
        // Arrange

        var command = new CreateTaskCommand(
            "  Learn .NET  ",
            "  Clean Architecture  ",
            (TaskStatus)TaskItemStatus.Pending);

        TaskItem? addedTask = null;

        _taskRepositoryMock
            .Setup(x => x.AddAsync(
                It.IsAny<TaskItem>(),
                It.IsAny<CancellationToken>()))
            .Callback<TaskItem, CancellationToken>((task, _) =>
            {
                addedTask = task;
                task.Id = 10;
            })
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act

        await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        addedTask.Should().NotBeNull();

        addedTask!.TaskName.Should().Be("Learn .NET");

        addedTask.Description.Should().Be("Clean Architecture");
    }
}
