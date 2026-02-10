using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMind.Domain.Identity;

namespace TaskMind.Domain.Models
{
    public class Task : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;
        public DateTimeOffset? DueDate { get; set; }
        public Guid Projectid { get; set; }
        public Project? Project { get; set; } 
        public string? AssignedUserId { get; set; }
        public User? AssignedUser { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<FileAttachement>? Files { get; set; }
        public ICollection<Tag>? Tags { get; set; } = new List<Tag>();
    }

    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Done
    }
}
