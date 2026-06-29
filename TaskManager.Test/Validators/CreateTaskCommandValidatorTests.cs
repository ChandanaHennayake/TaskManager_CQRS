
using FluentAssertions;
using FluentValidation.TestHelper;
using TaskManager.Application.Features.Tasks.Commands.CreateTask;
using TaskManager.Domain.Enums;
using Xunit;

namespace TaskManager.Tests.Validators;

public class CreateTaskCommandValidatorTests
{
    private readonly CreateTaskCommandValidator _validator;

    public CreateTaskCommandValidatorTests()
    {
        _validator = new CreateTaskCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var command = new CreateTaskCommand(
            "",
            "Description",
            (TaskStatus)TaskItemStatus.Pending);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Exceeds_Max_Length()
    {
        var command = new CreateTaskCommand(
            new string('A', 201),
            "Description",
            (TaskStatus)TaskItemStatus.Pending);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Is_Empty()
    {
        var command = new CreateTaskCommand(
            "Title",
            "",
           (TaskStatus)TaskItemStatus.Pending);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Have_Error_When_Description_Exceeds_Max_Length()
    {
        var command = new CreateTaskCommand(
            "Title",
            new string('A', 1001),
           (TaskStatus)TaskItemStatus.Pending);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Not_Have_Validation_Error_When_Command_Is_Valid()
    {
        var command = new CreateTaskCommand(
            "Learn CQRS",
            "Study MediatR",
            (TaskStatus)TaskItemStatus.Pending);

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
    }
}

