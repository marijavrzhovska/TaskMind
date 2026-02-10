using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMind.Domain.Models;
using TaskMind.Repository.Interface;
using TaskMind.Service.Interface;
using Task = TaskMind.Domain.Models.Task;
using TaskStatus = TaskMind.Domain.Models.TaskStatus;

namespace TaskMind.Service.Implementation
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<Task> taskRepository;
        public TaskService(IRepository<Task> taskRepository)
        {
            this.taskRepository = taskRepository;
        }

        public void AssignUser(Guid taskId, string userId)
        {
            var task = GetById(taskId);
            if (task == null)
                throw new InvalidOperationException("Task not found");

            task.AssignedUserId = userId;
            taskRepository.Update(task);
        }

        public Task DeleteById(Guid id)
        {
            var item = GetById(id);
            if (item == null)
            {
                throw new ArgumentNullException("Item is null");
            }
            return taskRepository.Delete(item); 
        }

        public List<Task> GetAll()
        {
            return taskRepository.GetAll(
                selector: x => x,
                include: x => x.Include(t => t.Tags)
            ).ToList();
        }

        public List<Task> GetByAssignedUserId(string userId)
        {
            return taskRepository.GetAll(
                selector: x => x,
                predicate: x => x.AssignedUserId == userId,
                include: x => x.Include(t => t.Comments).Include(t => t.Project).Include(t => t.AssignedUser)
            ).ToList();
        }

        public Task? GetById(Guid id)
        {
                        return taskRepository.Get(
                selector: x => x,
                predicate: x => x.Id == id,
                include: x => x.Include(y => y.Comments)
                               .Include(y => y.Project)
                               .Include(y => y.AssignedUser)
            );

        }

        public List<Task> GetByProjectId(Guid projectId)
        {
            return taskRepository.GetAll(
                selector: x => x,
                predicate: x => x.Projectid == projectId,
                include: x => x.Include(t => t.Comments).Include(t => t.Project).Include(t => t.AssignedUser)
            ).ToList();
        }

        public List<Task> GetByStatus(TaskStatus status)
        {
            return taskRepository.GetAll(
                selector: x => x,
                predicate: x => x.Status == status,
                include: x => x.Include(t => t.Comments).Include(t => t.Project).Include(t => t.AssignedUser)
            ).ToList();
        }

        public Task Insert(Task task)
        {
            return taskRepository.Insert(task); 
        }

        public ICollection<Task> InsertMany(ICollection<Task> tasks)
        {
            return taskRepository.InsertMany(tasks);    
        }

        public Task Update(Task task)
        {
            return taskRepository.Update(task);
        }

        public void UpdateStatus(Guid taskId, TaskStatus status)
        {
            var task = GetById(taskId);
            if (task == null)
                throw new InvalidOperationException("Task not found");

            task.Status = status;
            taskRepository.Update(task);
        }
        public void AddTag(Guid taskId, Tag tag)
        {
            var task = GetById(taskId);
            if (task == null)
                throw new InvalidOperationException("Task not found");

            if (task.Tags == null)
                task.Tags = new List<Tag>();

            if (!task.Tags.Any(t => t.Id == tag.Id))
            {
                task.Tags.Add(tag);
                Update(task); 
            }
        }
    }
}
