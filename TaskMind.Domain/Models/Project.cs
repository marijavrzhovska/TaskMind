using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMind.Domain.Identity;

namespace TaskMind.Domain.Models
{
    public class Project : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTimeOffset? Deadline { get; set; } 
        public string OwnerId { get; set; }
        public User? Owner { get; set; }
        public ICollection<Task>? Tasks { get; set; }
    }
}
