using System;
using System.Collections.Generic;
using TaskMind.Domain.Models;

namespace TaskMind.Service.Interface
{
    public interface IFileAttachmentService
    {
        List<FileAttachement> GetAll();
        FileAttachement? GetById(Guid id);
        FileAttachement Insert(FileAttachement file);
        FileAttachement Update(FileAttachement file);
        FileAttachement DeleteById(Guid id);

        List<FileAttachement> GetFilesByTaskId(Guid taskId);
    }
}
