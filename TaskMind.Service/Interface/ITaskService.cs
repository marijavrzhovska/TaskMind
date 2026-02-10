using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskMind.Domain.Models;
using Task = TaskMind.Domain.Models.Task;
using TaskStatus = TaskMind.Domain.Models.TaskStatus;

namespace TaskMind.Service.Interface
{
    public interface ITaskService
    {
        List<Task> GetAll();
        Task? GetById(Guid id);
        Task Insert(Task task);
        Task Update(Task task);
        Task DeleteById(Guid id);
        ICollection<Task> InsertMany(ICollection<Task> tasks);

        List<Task> GetByProjectId(Guid projectId);
        List<Task> GetByAssignedUserId(string userId);
        List<Task> GetByStatus(TaskStatus status);
        void AssignUser(Guid taskId, string userId);
        void UpdateStatus(Guid taskId, TaskStatus status);
        void AddTag(Guid taskId, Tag tag);
    }
}
