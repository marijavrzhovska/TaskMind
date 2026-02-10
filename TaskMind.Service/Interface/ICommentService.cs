using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMind.Domain.Models;

namespace TaskMind.Service.Interface
{
    public interface ICommentService
    {
        List<Comment> GetAll();
        Comment? GetById(Guid id);
        Comment Insert(Comment comment);
        Comment Update(Comment comment);
        Comment DeleteById(Guid id);

        List<Comment> GetCommentsByTaskId(Guid taskId);
        List<Comment> GetCommentsByUserId(string userId);
    }
}
