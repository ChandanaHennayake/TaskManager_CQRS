using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Domain.Common
{
   
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
