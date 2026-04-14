using MCPServers.Models;

namespace MCPServers.DB
{
    public interface IMCPServerDBService
    {
        Task<bool> SaveGitHubCredentialsAsync(string username, string token);

        Task<bool> UpsertRepoDetails(RepoDetailsResponse repo);
    }
}
