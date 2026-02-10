using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Security.Claims;
using TaskMind.Domain.Dto;
using TaskMind.Domain.Identity;
using TaskMind.Domain.Models;
using TaskMind.Repository.Interface;
using TaskMind.Service.Interface;
using TaskMind.Service.Interface.Dto;
using TaskMind.Web.Extensions;
using Task = TaskMind.Domain.Models.Task;
using TaskStatus = TaskMind.Domain.Models.TaskStatus;



namespace TaskMind.Web.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ITaskService _taskService;
        private readonly UserManager<User> _userManager; 
        private readonly IExternalProjectService _externalService;

        public ProjectsController(
            IProjectService projectService,
            ITaskService taskService,
            UserManager<User> userManager,
            IExternalProjectService externalProjectService)
        {
            _projectService = projectService;
            _taskService = taskService;
            _userManager = userManager;
            _externalService = externalProjectService;
        }


        [HttpPost]
        public async Task<IActionResult> ImportFromGitHub(string username)
        {

            if (string.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("", "Username cannot be empty");
                return RedirectToAction(nameof(Index)); 
            }

            var externalProjects = await _externalService.GetGitHubProjectsAsync(username);
            var savedProjects = new List<Project>();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
            {
                return Unauthorized(); 
            }
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null ||
                (currentUser.Role != Role.Admin && currentUser.Role != Role.ProjectManager))
            {
                
                return Forbid();
            }

            foreach (var ext in externalProjects)
            {
                
                var existing = _projectService.GetAll()
                    .FirstOrDefault(p => p.Title == ext.Name);

                if (existing != null)
                {
                    savedProjects.Add(existing);
                    continue;
                }

                var project = new Project
                {
                    Title = ext.Name,
                    Description = ext.Description ?? "",
                    Deadline = ext.Updated_at,  
                    OwnerId = currentUserId,    
                    Tasks = new List<Task>()
                };

                savedProjects.Add(_projectService.Insert(project));
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> ViewGitHubInsights(string? username)
        {
            List<ProjectInsight> insights = new List<ProjectInsight>();

            if (!string.IsNullOrWhiteSpace(username))
            {
                var externalProjects = await _externalService.GetGitHubProjectsAsync(username);
                insights = _externalService.TransformToInsights(externalProjects);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string currentRole = "User"; 

            if (currentUserId != null)
            {
                var user = await _userManager.FindByIdAsync(currentUserId);
                if (user != null)
                {
                    currentRole = user.Role.ToString(); 
                }
            }

            ViewBag.CurrentRole = currentRole;

            return View("GitHubProjectsInsights", insights);
        }


        // GET: Projects
        public IActionResult Index()
        {
            var projects = _projectService.GetAll();

            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var currentUser = _userManager.FindByIdAsync(userId).Result;
                ViewBag.CurrentRole = currentUser?.Role.ToString(); 
            }
            else
            {
                ViewBag.CurrentRole = "Guest";
            }

            return View(projects);
        }

        // GET: Projects/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null) return NotFound();

            var project = _projectService.GetById(id.Value);
            if (project == null) return NotFound();

            var tasks = _projectService.GetProjectTasks(project.Id);

            ViewData["Tasks"] = tasks;

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            var users = _userManager.Users.ToList();
            ViewBag.OwnerId = new SelectList(users, "Id", "FirstName");
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public IActionResult Create(Project project)
        {
            if (project.Deadline.HasValue)
            {
                project.Deadline = project.Deadline.Value.ToUniversalTime();
            }

            if (!ModelState.IsValid)
            {
                var users = _userManager.Users.ToList();
                ViewBag.OwnerId = new SelectList(users, "Id", "FirstName", project.OwnerId);
                return View(project);
            }

            project.Id = Guid.NewGuid();
            _projectService.Insert(project);
            return RedirectToAction(nameof(Index));
        }



        // GET: Projects/Edit/5
        public IActionResult Edit(Guid? id)
        {

            if (id == null)
                return NotFound();

            var project = _projectService.GetById(id.Value);
            if (project == null)
                return NotFound();

            var users = _userManager.Users.ToList();
            ViewBag.OwnerId = new SelectList(users, "Id", "FirstName", project.OwnerId);
            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Title,Description,Deadline,OwnerId,Id")] Project project)
        {
            if (project.Deadline.HasValue)
            {
                project.Deadline = project.Deadline.Value.ToUniversalTime();
            }
            if (id != project.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _projectService.Update(project);
                }
                catch
                {
                    if (_projectService.GetById(project.Id) == null)
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var users = _userManager.Users.ToList();
            ViewBag.OwnerId = new SelectList(users, "Id", "FirstName", project.OwnerId);
            return View(project);
        }

        // GET: Projects/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var project = _projectService.GetById(id.Value);
            if (project == null)
                return NotFound();

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _projectService.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult MyProjects()
        {
            var user = _userManager.GetUserAsync(User).Result; 
            if (user == null)
                return Challenge(); 

            var projects = _projectService.GetProjectsByOwnerId(user.Id);
            return View(projects);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTask(Guid projectId, [Bind("Title,Description,DueDate")] Task task)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Details", new { id = projectId });

            _projectService.AddTaskToProject(projectId, task);

            return RedirectToAction("Details", new { id = projectId });
        }
        
        private bool ProjectExists(Guid id)
        {
            return _projectService.GetById(id) != null;
        }
    }
}
