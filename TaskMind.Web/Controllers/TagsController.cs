using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMind.Service.Interface;

namespace TaskMind.Web.Controllers
{
    [Authorize]
    public class TagsController : Controller
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        // GET: Tags/Tasks/5
        public IActionResult Tasks(Guid id)
        {
            var tasks = _tagService.GetTasksByTagId(id);
            return View(tasks);
        }
    }
}
