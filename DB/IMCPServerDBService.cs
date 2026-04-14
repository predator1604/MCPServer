namespace MCPServers.DB
{
    public interface IMCPServerDBService
    {
        Task<bool> SaveGitHubCredentialsAsync(string username, string token);
    }
}
