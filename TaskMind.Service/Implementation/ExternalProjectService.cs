using System.Net.Http.Json;
using TaskMind.Domain.Dto;
using TaskMind.Service.Interface.Dto;

namespace TaskMind.Service.Implementation
{
    public class ExternalProjectService : IExternalProjectService
    {
        private readonly HttpClient _httpClient;

        public ExternalProjectService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ExternalProjectDto>> GetGitHubProjectsAsync(string username)
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TaskMindApp");

            // REST API za public repos (GitHub)
            var response = await _httpClient.GetFromJsonAsync<List<ExternalProjectDto>>(
                $"https://api.github.com/users/{username}/repos");

            return response ?? new List<ExternalProjectDto>();
        }

        public List<ProjectInsight> TransformToInsights(List<ExternalProjectDto> externalProjects)
        {
            var insights = new List<ProjectInsight>();

            foreach (var ext in externalProjects)
            {
                var daysSinceUpdate = ext.Updated_at.HasValue
                                      ? (DateTimeOffset.UtcNow - ext.Updated_at.Value).Days
                                      : -1;

                // Presmetka na slozhenost spored opisot na github
                string complexity;

                if (string.IsNullOrEmpty(ext.Description))
                {
                    complexity = "Unknown"; 
                }
                else if (ext.Description.Length < 50)
                {
                    complexity = "Low";
                }
                else if (ext.Description.Length > 150)
                {
                    complexity = "High";
                }
                else
                {
                    complexity = "Medium";
                }

                insights.Add(new ProjectInsight
                {
                    Title = ext.Name,
                    Description = ext.Description ?? "",
                    IsActive = daysSinceUpdate >= 0 && daysSinceUpdate <= 30,
                    Complexity = complexity,
                    DaysSinceUpdate = daysSinceUpdate
                });
            }

            return insights;
        }
    }
}
