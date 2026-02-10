using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMind.Domain.Dto;
using TaskMind.Domain.Models;

namespace TaskMind.Service.Interface.Dto
{
    public interface IExternalProjectService
    {
        Task<List<ExternalProjectDto>> GetGitHubProjectsAsync(string username);
        List<ProjectInsight> TransformToInsights(List<ExternalProjectDto> externalProjects);
    }
}
