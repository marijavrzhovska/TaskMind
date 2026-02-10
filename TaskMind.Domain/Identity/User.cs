using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using TaskMind.Domain.Models;
using Task = TaskMind.Domain.Models.Task;

namespace TaskMind.Domain.Identity
{
    public class User : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public Role Role { get; set; }

        public ICollection<Project>? OwnedProjects { get; set; }
        public ICollection<Task>? AssignedTasks { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }

    public enum Role
    {
        Admin,
        User,
        ProjectManager
    }
}

