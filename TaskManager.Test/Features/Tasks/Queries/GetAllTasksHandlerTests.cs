
using FluentAssertions;
using Moq;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Features.Tasks.Queries.GetAllTasks;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using Xunit;

namespace TaskManager.Tests.Features.Tasks.Queries;

public class GetAllTasksHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly GetAllTasksHandler _handler;

    public GetAllTasksHandlerTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();

        _handler = new GetAllTasksHandler(
            _taskRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTasks_WhenTasksExist()
    {
        // Arrange

        var tasks = new List<TaskItem>
        {
            new()
            {
                Id = 1,
                TaskName = "Task One",
                Description = "Description One",
                Status = (TaskStatus)TaskItemStatus.Pending,
                CreatedDate = DateTime.UtcNow
            },
            new()
            {
                Id = 2,
                TaskName = "Task Two",
                Description = "Description Two",
                Status = (TaskStatus)TaskItemStatus.Completed,
                CreatedDate = DateTime.UtcNow
            }
        };

        var query = new GetAllTasksQuery(
            1,
            10,
            null,
            null,
            null);

        _taskRepositoryMock
            .Setup(x => x.GetPagedTasksAsync(
                query.PageNumber,
                query.PageSize,
                query.Search,
                query.Status,
                query.SortBy,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act

        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert

        result.Should().NotBeNull();

        result.Should().HaveCount(2);

        result.First().Title.Should().Be("Task One");

        result.Last().Title.Should().Be("Task Two");

        _taskRepositoryMock.Verify(x =>
            x.GetPagedTasksAsync(
                query.PageNumber,
                query.PageSize,
                query.Search,
                query.Status,
                query.SortBy,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoTasksExist()
    {
        // Arrange

        var query = new GetAllTasksQuery(
            1,
            10,
            null,
            null,
            null);

        _taskRepositoryMock
            .Setup(x => x.GetPagedTasksAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                (TaskStatus?)It.IsAny<TaskItemStatus?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem>());

        // Act

        var result = await _handler.Handle(
            query,
            CancellationToken.None);

        // Assert

        result.Should().NotBeNull();

        result.Should().BeEmpty();
    }
}
