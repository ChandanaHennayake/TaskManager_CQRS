
using FluentAssertions;
using FluentValidation.TestHelper;
using TaskManager.Application.Features.Tasks.Commands.UpdateTask;
using TaskManager.Domain.Enums;
using Xunit;

namespace TaskManager.Tests.Validators;

public class UpdateTaskCommandValidatorTests
{
    private readonly UpdateTaskCommandValidator _validator;

    public UpdateTaskCommandValidatorTests()
    {
        _validator = new UpdateTaskCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Less_Than_Or_Equal_To_Zero()
    {
        // Arrange
        var command = new UpdateTaskCommand(
            0,
            "Title",
            "Description",
            (TaskStatus)TaskItemStatus.Pending);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        // Arrange
        var command = new UpdateTaskCommand(
            1,
            string.Empty,
            "Description",
             (TaskStatus)TaskItemStatus.Pending);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Exceeds_Max_Length()
    {
        // Arrange
        var command = new UpdateTaskCommand(
            1,
            new string('A', 201),
            "Description",
             (TaskStatus)TaskItemStatus.Pending);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Exceeds_Max_Length()
    {
        // Arrange
        var command = new UpdateTaskCommand(
            1,
            "Title",
            new string('A', 1001),
             (TaskStatus)TaskItemStatus.Pending);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new UpdateTaskCommand(
            1,
            "Learn Clean Architecture",
            "Implement CQRS using MediatR",
             (TaskStatus)TaskItemStatus.Completed);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
