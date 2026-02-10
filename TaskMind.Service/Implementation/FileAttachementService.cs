using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskMind.Domain.Models;
using TaskMind.Repository.Interface;
using TaskMind.Service.Interface;

namespace TaskMind.Service.Implementation
{
    public class FileAttachmentService : IFileAttachmentService
    {
        private readonly IRepository<FileAttachement> fileRepository;

        public FileAttachmentService(IRepository<FileAttachement> fileRepository)
        {
            this.fileRepository = fileRepository;
        }

        public List<FileAttachement> GetAll()
        {
            return fileRepository.GetAll(selector:x=>x,
                include: x=>x.Include(y=>y.Task)).ToList();
        }

        public FileAttachement? GetById(Guid id)
        {
            return fileRepository.Get(x => x, x => x.Id == id, include: q => q.Include(f => f.Task));
        }

        public FileAttachement Insert(FileAttachement file)
        {
            return fileRepository.Insert(file);
        }

        public FileAttachement Update(FileAttachement file)
        {
            return fileRepository.Update(file);
        }

        public FileAttachement DeleteById(Guid id)
        {
            var file = GetById(id);
            if (file == null)
                throw new ArgumentNullException("File not found");

            return fileRepository.Delete(file);
        }

        public List<FileAttachement> GetFilesByTaskId(Guid taskId)
        {
            return fileRepository.GetAll(x => x, x => x.TaskId == taskId).ToList();
        }
    }
}
