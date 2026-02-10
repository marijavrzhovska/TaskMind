using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMind.Domain.Models;
using TaskMind.Repository.Interface;
using TaskMind.Service.Interface;

namespace TaskMind.Service.Implementation
{
    public class CommentService : ICommentService
    {
        private readonly IRepository<Comment> commentRepository;
        public CommentService(IRepository<Comment> commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        public Comment DeleteById(Guid id)
        {
            var item = GetById(id);
            if(item == null)
            {
                throw new ArgumentNullException("Item is null"); 
            }
            return commentRepository.Delete(item);
        }

        public List<Comment> GetAll()
        {
            return commentRepository.GetAll(
                selector: x => x,
                include: x => x.Include(c => c.Task).Include(c => c.User)
            ).ToList();
        }

        public Comment? GetById(Guid id)
        {
            return commentRepository.Get(
                selector: x => x,
                predicate: x => x.Id == id,
                include: x => x.Include(c => c.Task).Include(c => c.User)
            );
        }

        public List<Comment> GetCommentsByTaskId(Guid taskId)
        {
            return commentRepository.GetAll(
                    selector: x => x,
                    predicate: x => x.TaskId == taskId,
                    include: x => x.Include(y => y.User).Include(y => y.Task)
             ).ToList();
        }

        public List<Comment> GetCommentsByUserId(string userId)
        {
            return commentRepository.GetAll(selector: x => x, predicate: x => x.UserId == userId,
                include: x => x.Include(y => y.Task).Include(y => y.User)).ToList();
        }

        public Comment Insert(Comment comment)
        {
            if (comment.CreatedAt.HasValue)
                comment.CreatedAt = comment.CreatedAt.Value.UtcDateTime;
            else
                comment.CreatedAt = DateTimeOffset.UtcNow;
            return commentRepository.Insert(comment);  
        }

        public Comment Update(Comment comment)
        {
            if (comment.CreatedAt.HasValue)
                comment.CreatedAt = comment.CreatedAt.Value.UtcDateTime;
            return commentRepository.Update(comment);
        }
    }
}
