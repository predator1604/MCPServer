namespace MCPServers.Services
{
    public interface IMCPServerService
    {
        Task<IEnumerable<string>> GetRepos();

        Task<bool> DeleteRepo(string repoName);
    }
}
