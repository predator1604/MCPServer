namespace MCPServers.Services
{
    public interface IMCPServerService
    {
        Task<IEnumerable<string>> GetAllRepos();

        Task<bool> DeleteRepo(string repoName);
    }
}
