using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMind.Domain.Models
{
    public class FileAttachement : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public Guid TaskId { get; set; }
        public Task Task { get; set; } = null!;
    }
}
