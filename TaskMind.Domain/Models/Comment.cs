using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMind.Domain.Identity;

namespace TaskMind.Domain.Models
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset? CreatedAt { get; set; } 
        public Guid TaskId { get; set; }
        public Task? Task { get; set; } 
        public string UserId { get; set; }
        public User? User { get; set; } = null!;
    }
}
