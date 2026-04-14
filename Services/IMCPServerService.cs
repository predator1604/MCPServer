using MCPServers.Models;

namespace MCPServers.Services
{
    public interface IMCPServerService
    {
        Task<IEnumerable<string>> GetAllRepos();

        Task<bool> DeleteRepo(string repoName);

        Task<RepoResponse> CreateRepo(CreateRepoRequest request);

        Task<RepoDetailsResponse> GetRepoDetails(string repoName);
    }
}
