using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using TaskMind.Domain.Models;
using TaskMind.Repository.Interface;
using TaskMind.Service.Interface;
using TaskMind.Service.Interface.Dto;
using Task = TaskMind.Domain.Models.Task;

namespace TaskMind.Service.Implementation
{
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> projectRepository;
        private readonly ITaskService taskService;
        private readonly IExternalProjectService _externalService;
        public ProjectService(IRepository<Project> projectRepository, ITaskService taskService)
        {
            this.projectRepository = projectRepository;
            this.taskService = taskService;
        }

        public async Task<List<Project>> ImportExternalProjects(string username)
        {
            var externalProjects = await _externalService.GetGitHubProjectsAsync(username);
            var savedProjects = new List<Project>();

            foreach (var ext in externalProjects)
            {
                var existing = projectRepository.GetAll(
                    selector: x => x
                ).FirstOrDefault(p => p.Title == ext.Name);

                if (existing != null)
                {
                    savedProjects.Add(existing);
                    continue;
                }

                var project = new Project
                {
                    Title = ext.Name,
                    Description = ext.Description,
                    Deadline = ext.Updated_at,
                    OwnerId = null,
                    Tasks = new List<Task>()
                };

                var inserted = projectRepository.Insert(project);
                savedProjects.Add(inserted);
            }

            return savedProjects;
        }




        public void AddTaskToProject(Guid projectId, Task task)
        {
            var project = GetById(projectId);
            if (project == null)
                throw new InvalidOperationException("Project not found");

            if (project.Tasks == null)
                project.Tasks = new List<Task>();

            project.Tasks.Add(task);
            taskService.Insert(task);

            projectRepository.Update(project);
        }

        public Project DeleteById(Guid id)
        {
            var item = GetById(id);
            if (item == null)
            {
                throw new ArgumentNullException("Item is null");
            }
            return projectRepository.Delete(item);
        }

        public List<Project> GetAll()
        {
            return projectRepository.GetAll(
                selector: x => x,
                include: x => x.Include(p => p.Owner).Include(p => p.Tasks).ThenInclude(z=>z.Comments)
            ).ToList();
        }

        public Project? GetById(Guid id)
        {
            return projectRepository.Get(selector: x => x, predicate: x => x.Id == id,
                include: x => x.Include(y => y.Owner).Include(y=>y.Tasks).ThenInclude(z=>z.Comments));
        }

        public List<Project> GetProjectsByOwnerId(string ownerId)
        {
            return projectRepository.GetAll(selector: x => x, predicate: x => x.OwnerId == ownerId,
                include: x => x.Include(y => y.Owner).Include(y => y.Tasks)).ToList();
        }

        public List<Task> GetProjectTasks(Guid projectId)
        {
            var project = GetById(projectId);
            if (project == null)
                throw new InvalidOperationException("Project not found");

            return project.Tasks?.ToList() ?? new List<Task>();
        }
        

        public Project Insert(Project project)
        {
            return projectRepository.Insert(project);
        }

        public Project Update(Project project)
        {
            return projectRepository.Update(project);
        }
    }
}
