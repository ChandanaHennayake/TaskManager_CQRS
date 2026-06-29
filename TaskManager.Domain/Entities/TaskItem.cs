using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities
{
    public class TaskItem : BaseEntity
    {
        public string TaskName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public TaskStatus Status { get; set; }

        public string? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}
