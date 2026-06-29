using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Application.Features.Tasks.DTOs
{
    public sealed class TaskFilterDto
    {
        public int PageNumber { get; init; } = 1;

        public int PageSize { get; init; } = 10;

        public string? Search { get; init; }

        public TaskStatus? Status { get; init; }

        public string? SortBy { get; init; }
    }
}
