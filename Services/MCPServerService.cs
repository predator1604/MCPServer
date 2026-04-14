using MCPServers.DB;
using Octokit;

namespace MCPServers.Services
{
    public class MCPServerService : IMCPServerService
    {
        private readonly IMCPServerDBService mcpServerDBService;
        private readonly ILogger<MCPServerService> logger;
        private readonly GitHubClient gitHubClient;
        private readonly string username;

        public MCPServerService(IMCPServerDBService mcpServerDBService, ILogger<MCPServerService> logger, IConfiguration config)
        {
            this.mcpServerDBService = mcpServerDBService;
            this.logger = logger;
            var gitHubToken = config["GitHubSettings:PAT"];
            this.username = config["GitHubSettings:UserName"] ?? string.Empty;

            this.gitHubClient = new GitHubClient(new ProductHeaderValue("MCPServerService"))
            {
                Credentials = new Credentials(gitHubToken)
            };
        }

        private async Task<bool> StoreCredentialsInDB(string token, string username)
        {
            bool success = await mcpServerDBService.SaveGitHubCredentialsAsync(username, token);
            if (success)
                logger.LogInformation("Credentials for {Username} stored successfully.", username);
            else
                logger.LogWarning("Failed to store credentials for {Username}.", username);

            return success;
        }

        public async Task<IEnumerable<string>> GetRepos()
        {
            logger.LogInformation("Fetching repos from the github.");
            var repos = await gitHubClient.Repository.GetAllForUser(username);
            await StoreCredentialsInDB(gitHubClient.Credentials.GetToken(), username);
            return repos.Select(r => r.Name).ToList();
        }

        public async Task<bool> DeleteRepo(string repoName)
        {
            try
            {
                logger.LogInformation("Attempting to delete {Owner}/{Repo} via Octokit", username, repoName);
                await gitHubClient.Repository.Delete(username, repoName);

                return true;
            }
            catch (Octokit.NotFoundException)
            {
                logger.LogWarning("Repository {Repo} not found.", repoName);
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete repository from GitHub");
                throw;
            }
        }
    }
}
