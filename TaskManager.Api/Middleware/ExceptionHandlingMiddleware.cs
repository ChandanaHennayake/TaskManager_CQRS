using FluentValidation;
using System.Net;
using System.Text.Json;
using TaskManager.Application.Exceptions;

namespace TaskManager.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(
                ex,
                "Validation error occurred");

            await HandleValidationException(
                context,
                ex);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(
                ex,
                "Resource not found");

            await HandleNotFoundException(
                context,
                ex);
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(
                ex,
                "Bad request occurred");

            await HandleBadRequestException(
                context,
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception occurred");

            await HandleException(
                context,
                ex);
        }
    }

    private static async Task HandleValidationException(
        HttpContext context,
        ValidationException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode =
            (int)HttpStatusCode.BadRequest;

        var response = new
        {
            Success = false,
            Message = "Validation failed",
            Errors = ex.Errors
                .Select(x => x.ErrorMessage)
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }

    private static async Task HandleNotFoundException(
        HttpContext context,
        NotFoundException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode =
            (int)HttpStatusCode.NotFound;

        var response = new
        {
            Success = false,
            Message = ex.Message
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }

    private static async Task HandleBadRequestException(
        HttpContext context,
        BadRequestException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode =
            (int)HttpStatusCode.BadRequest;

        var response = new
        {
            Success = false,
            Message = ex.Message
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }

    private static async Task HandleException(
        HttpContext context,
        Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode =
            (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            Success = false,
            Message = "An unexpected error occurred."
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}