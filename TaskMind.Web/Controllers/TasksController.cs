using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using TaskMind.Domain.Identity;
using TaskMind.Domain.Models;
using TaskMind.Service.Implementation;
using TaskMind.Service.Interface;
using TaskMind.Web.Extensions;
using Task = TaskMind.Domain.Models.Task;
using TaskStatus = TaskMind.Domain.Models.TaskStatus;

namespace TaskMind.Web.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService taskService;
        private readonly UserManager<User> userManager;
        private readonly IProjectService projectService;
        private readonly IFileAttachmentService fileAttachmentService;
        private readonly ITagService tagService;

        public TasksController(
            ITaskService taskService,
            UserManager<User> userManager,
            IProjectService projectService,
            IFileAttachmentService fileAttachmentService,
            ITagService  tagService)
        {
            this.taskService = taskService;
            this.userManager = userManager;
            this.projectService = projectService;
            this.fileAttachmentService = fileAttachmentService;
            this.tagService = tagService;
        }

       
        // GET: Tasks
        public IActionResult Index(Guid? tagId) 
        {
            IEnumerable<Task> tasks;

            if (tagId.HasValue)
            {
                tasks = tagService.GetTasksByTagId(tagId.Value);
                ViewBag.FilterTag = tagService.GetById(tagId.Value)?.Name ?? "Unknown";
            }
            else
            {
                tasks = taskService.GetAll();
                ViewBag.FilterTag = null;
            }

            ViewBag.Projects = projectService.GetAll().ToDictionary(p => p.Id, p => p.Title);
            ViewBag.Users = userManager.Users.ToDictionary(u => u.Id, u => u.Email);

            ViewBag.Tags = tagService.GetAll();

            return View(tasks);
        }

        // GET: Tasks/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null) return NotFound();

            var task = taskService.GetById(id.Value);
            if (task == null) return NotFound();

            var attachments = fileAttachmentService.GetFilesByTaskId(task.Id);
            ViewData["Attachments"] = attachments;

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            var projects = projectService.GetAll();
            ViewData["AssignedUserId"] = new SelectList(userManager.Users.ToList(), "Id", "Email");
            ViewData["Projectid"] = new SelectList(projects, "Id", "Title");
            ViewBag.Status = new SelectList(Enum.GetValues(typeof(TaskStatus)));
            return View();
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Title,Description,Status,DueDate,Projectid,AssignedUserId,Id")] Task task)
        {
            if (task.DueDate.HasValue)
            {
                task.DueDate = task.DueDate.Value.ToUniversalTime();
            }
            if (ModelState.IsValid)
            {
                task.Id = Guid.NewGuid();
                taskService.Insert(task);
                return RedirectToAction(nameof(Index));
            }

            var projects = projectService.GetAll();
            ViewData["AssignedUserId"] = new SelectList(userManager.Users.ToList(), "Id", "Email", task.AssignedUserId);
            ViewData["Projectid"] = new SelectList(projects, "Id", "Title", task.Projectid);
            ViewBag.Status = new SelectList(Enum.GetValues(typeof(TaskStatus)));
            return View(task);
        }

        // GET: Tasks/Edit/5
        public IActionResult Edit(Guid? id)
        {

            if (id == null) return NotFound();

            var task = taskService.GetById(id.Value);
            if (task == null) return NotFound();

            var projects = projectService.GetAll();
            ViewData["AssignedUserId"] = new SelectList(userManager.Users.ToList(), "Id", "Email", task.AssignedUserId);
            ViewData["Projectid"] = new SelectList(projects, "Id", "Title", task.Projectid);
            ViewBag.Status = new SelectList(Enum.GetValues(typeof(TaskStatus)));
            ViewBag.Tags = tagService.GetAll();
            return View(task);
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Title,Description,Status,DueDate,Projectid,AssignedUserId,Id")] Task task)
        {
            if (task.DueDate.HasValue)
            {
                task.DueDate = task.DueDate.Value.ToUniversalTime();
            }
            if (id != task.Id) return NotFound();

            if (ModelState.IsValid)
            {
                taskService.Update(task);
                return RedirectToAction(nameof(Index));
            }

            var projects = projectService.GetAll();
            ViewData["AssignedUserId"] = new SelectList(userManager.Users.ToList(), "Id", "Email", task.AssignedUserId);
            ViewData["Projectid"] = new SelectList(projects, "Id", "Title", task.Projectid);
            ViewBag.Tags = tagService.GetAll();
            return View(task);
        }

        // GET: Tasks/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var task = taskService.GetById(id.Value);
            if (task == null) return NotFound();

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            taskService.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Tasks/ByProject/5
        public IActionResult ByProject(Guid projectId)
        {
            var tasks = taskService.GetByProjectId(projectId);
            return View("Index", tasks);
        }

        // GET: Tasks/ByUser/5
        public IActionResult ByUser(string userId)
        {
            var tasks = taskService.GetByAssignedUserId(userId);
            return View("Index", tasks);
        }

        // POST: Tasks/AssignUser
        [HttpPost]
        public IActionResult AssignUser(Guid taskId, string userId)
        {
            taskService.AssignUser(taskId, userId);
            return RedirectToAction(nameof(Index));
        }

        // POST: Tasks/UpdateStatus
        [HttpPost]
        public IActionResult UpdateStatus(Guid taskId, TaskStatus status)
        {
            taskService.UpdateStatus(taskId, status);
            return RedirectToAction(nameof(Index));
        }
        // GET: Tasks/CreateTaskWithFiles
        public IActionResult CreateTaskWithFiles(Guid? projectId)
        {
            ViewBag.AssignedUserId = new SelectList(userManager.Users.ToList(), "Id", "Email");

            if (projectId.HasValue)
            {
                var project = projectService.GetById(projectId.Value);
                ViewBag.ProjectId = new SelectList(new[] { project }, "Id", "Title", project.Id);
            }
            else
            {
                var projects = projectService.GetAll();
                ViewBag.ProjectId = new SelectList(projects, "Id", "Title");
            }

            return View(new Task());
        }

        // POST: Tasks/CreateTaskWithFiles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTaskWithFiles(Task task, List<IFormFile> files)
        {
            if (task.DueDate.HasValue)
            {
                task.DueDate = task.DueDate.Value.ToUniversalTime(); 
            }

            if (ModelState.IsValid)
            {
                task.Id = Guid.NewGuid();
                taskService.Insert(task);

                foreach (var file in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        var fileAttachment = new FileAttachement
                        {
                            Id = Guid.NewGuid(),
                            TaskId = task.Id,
                            FullName = file.FileName,
                            FileUrl = "/uploads/" + uniqueFileName
                        };

                        fileAttachmentService.Insert(fileAttachment);
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            var projects = projectService.GetAll();
            ViewBag.ProjectId = new SelectList(projects, "Id", "Title", task.Projectid);
            ViewBag.AssignedUserId = new SelectList(userManager.Users.ToList(), "Id", "Email", task.AssignedUserId);

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTag(Guid taskId, Guid selectedTagId)
        {
            var tag = tagService.GetById(selectedTagId);
            if (tag == null) return NotFound();

            taskService.AddTag(taskId, tag);

            return RedirectToAction("Edit", new { id = taskId });
        }


        private bool TaskExists(Guid id)
        {
            return taskService.GetById(id) != null;
        }
    }
}