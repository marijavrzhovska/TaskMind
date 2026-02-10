using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMind.Domain.Models;
using Task = TaskMind.Domain.Models.Task;

namespace TaskMind.Service.Interface
{
    public interface IProjectService
    {
        List<Project> GetAll();
        Project? GetById(Guid id);
        Project Insert(Project project);
        Project Update(Project project);
        Project DeleteById(Guid id);

        List<Project> GetProjectsByOwnerId(string ownerId);
        List<Task> GetProjectTasks(Guid projectId);
        void AddTaskToProject(Guid projectId, Task task);
    }
}
