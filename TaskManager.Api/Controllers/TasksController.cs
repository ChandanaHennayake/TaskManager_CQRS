using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Tasks.Commands.CreateTask;
using TaskManager.Application.Features.Tasks.Commands.DeleteTask;
using TaskManager.Application.Features.Tasks.Commands.UpdateTask;
using TaskManager.Application.Features.Tasks.Queries.GetAllTasks;
using TaskManager.Application.Features.Tasks.Queries.GetTaskById;

namespace TaskManager.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]

public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskCommand command,
        CancellationToken cancellationToken)
    {
        var taskId = await _mediator.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = taskId },
            new
            {
                Message = "Task created successfully",
                TaskId = taskId
            });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllTasksQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetTaskByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateTaskCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(new
            {
                Message = "Route Id and Request Id do not match."
            });
        }

        await _mediator.Send(
            command,
            cancellationToken);

        return Ok(new
        {
            Message = "Task updated successfully"
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new DeleteTaskCommand(id),
            cancellationToken);

        return Ok(new
        {
            Message = "Task deleted successfully"
        });
    }
}