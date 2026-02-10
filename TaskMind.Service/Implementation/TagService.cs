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

namespace TaskMind.Service.Implementation
{
    public class TagService : ITagService
    {
        private readonly IRepository<Tag> tagRepository;
        public TagService(IRepository<Tag> tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        public Tag DeleteById(Guid id)
        {
            var item = GetById(id);
            if(item == null)
            {
                throw new ArgumentNullException("id");
            }
            return tagRepository.Delete(item);  
        }

        public List<Tag> GetAll()
        {
            return tagRepository.GetAll(selector: x => x).ToList();
        }

        public Tag? GetById(Guid id)
        {
            return tagRepository.Get(
                selector: x => x,
                predicate: x => x.Id == id,
                include: x => x.Include(y => y.Tasks)
                               .ThenInclude(t => t.Comments)
                               .Include(y => y.Tasks)
                               .ThenInclude(t => t.AssignedUser)
            );
        }

        public List<Task> GetTasksByTagId(Guid tagId)
        {
            var tag = tagRepository.Get(
                selector: x => x,
                predicate: x => x.Id == tagId,
                include: x => x.Include(t => t.Tasks)
                               .ThenInclude(task => task.AssignedUser)
                               .Include(t => t.Tasks)
                               .ThenInclude(task => task.Comments)
            );

            if (tag == null)
                throw new InvalidOperationException("Tag not found");

            return tag.Tasks?.ToList() ?? new List<Task>();
        }

        public Tag Insert(Tag tag)
        {
            return tagRepository.Insert(tag);
        }

        public Tag Update(Tag tag)
        {
            return tagRepository.Update(tag);
        }
    }
}
