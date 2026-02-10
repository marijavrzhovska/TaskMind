using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMind.Domain.Models;
using Task = TaskMind.Domain.Models.Task;

namespace TaskMind.Service.Interface
{
    public interface ITagService
    {
        List<Tag> GetAll();
        Tag? GetById(Guid id);
        Tag Insert(Tag tag);
        Tag Update(Tag tag);
        Tag DeleteById(Guid id);

        List<Task> GetTasksByTagId(Guid tagId);
    }
}
