namespace MCPServers.Services
{
    public interface IMCPServerService
    {
        Task<IEnumerable<string>> GetRepos();
    }
}
