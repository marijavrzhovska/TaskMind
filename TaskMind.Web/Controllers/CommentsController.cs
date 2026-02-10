using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskMind.Domain.Identity;
using TaskMind.Domain.Models;
using TaskMind.Service.Interface;

namespace TaskMind.Web.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly ITaskService _taskService;
        private readonly UserManager<User> _userManager;

        public CommentsController(ICommentService commentService, ITaskService taskService, UserManager<User> userManager)
        {
            _commentService = commentService;
            _taskService = taskService;
            _userManager = userManager;
        }

        // GET: Comments
        public IActionResult Index()
        {
            var comments = _commentService.GetAll();
            return View(comments);
        }

        // GET: Comments/Details/5
        public IActionResult Details(Guid id)
        {
            var comment = _commentService.GetById(id);
            if (comment == null)
                return NotFound();

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            ViewData["TaskId"] = new SelectList(_taskService.GetAll(), "Id", "Title");
            ViewData["UserId"] = new SelectList(_userManager.Users.ToList(), "Id", "FirstName");
            return View();
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Content,CreatedAt,TaskId,UserId,Id")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.Id = Guid.NewGuid();
                comment.CreatedAt = DateTimeOffset.UtcNow;
                _commentService.Insert(comment);
                return RedirectToAction(nameof(Index));
            }

            ViewData["TaskId"] = new SelectList(_taskService.GetAll(), "Id", "Title", comment.TaskId);
            ViewData["UserId"] = new SelectList(_userManager.Users.ToList(), "Id", "FirstName", comment.UserId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public IActionResult Edit(Guid id)
        {
            var comment = _commentService.GetById(id);
            if (comment == null)
                return NotFound();

            ViewData["TaskId"] = new SelectList(_taskService.GetAll(), "Id", "Title", comment.TaskId);
            ViewData["UserId"] = new SelectList(_userManager.Users.ToList(), "Id", "FirstName", comment.UserId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Content,CreatedAt,TaskId,UserId,Id")] Comment comment)
        {
            
            if (id != comment.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _commentService.Update(comment);
                return RedirectToAction(nameof(Index));
            }

            ViewData["TaskId"] = new SelectList(_taskService.GetAll(), "Id", "Title", comment.TaskId);
            ViewData["UserId"] = new SelectList(_userManager.Users.ToList(), "Id", "FirstName", comment.UserId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public IActionResult Delete(Guid id)
        {
            var comment = _commentService.GetById(id);
            if (comment == null)
                return NotFound();

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _commentService.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }

        
        // GET: Comments/ByTask/5
        public IActionResult ByTask(Guid taskId)
        {
            var task = _taskService.GetById(taskId);
            if (task == null)
                return NotFound();

            var comments = _commentService.GetCommentsByTaskId(taskId);

            ViewBag.TaskTitle = task.Title;

            return View(comments);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public IActionResult AddComment(Guid taskId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return RedirectToAction("Details", "Tasks", new { id = taskId });

            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
                return Challenge(); 

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = content,
                CreatedAt = DateTimeOffset.UtcNow,
                TaskId = taskId,
                UserId = user.Id
            };

            _commentService.Insert(comment);

            return RedirectToAction("Details", "Tasks", new { id = taskId });
        }


        // POST: Comments/ByTask/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ByTask(Guid taskId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                ModelState.AddModelError("Content", "Comment cannot be empty.");
            }
            else
            {
                var user = _userManager.GetUserAsync(User).Result;
                if (user == null)
                {
                    return Challenge(); 
                }

                var comment = new Comment
                {
                    Id = Guid.NewGuid(),
                    TaskId = taskId,
                    UserId = user.Id,
                    Content = content,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                _commentService.Insert(comment);
            }

            return RedirectToAction(nameof(ByTask), new { taskId });
        }

        // GET: Comments/ByUser/5
        public IActionResult ByUser(string userId)
        {
            var comments = _commentService.GetCommentsByUserId(userId);
            var user = _userManager.FindByIdAsync(userId).Result;
            ViewBag.UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown";
            return View(comments);
        }
    }
}
