using MCPServers.DB;
using MCPServers.Models;
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

        public async Task<IEnumerable<string>> GetAllRepos()
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
            catch (NotFoundException)
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

        public async Task<RepoResponse> CreateRepo(CreateRepoRequest request)
        {
            var newRepo = new NewRepository(request.Name)
            {
                Description = request.Description,
                Private = request.Visibility.Equals("private", StringComparison.OrdinalIgnoreCase),
                AutoInit = request.AutoInit
            };
            try
            {
                var createdRepo = await gitHubClient.Repository.Create(newRepo);
                return new RepoResponse
                {
                    Id = createdRepo.Id,
                    Name = createdRepo.Name,
                    Url = createdRepo.HtmlUrl
                };
            }
            catch (ApiValidationException ex) when (ex.ApiError.Errors.Any(e => e.Code == "already_exists"))
            {
                logger.LogWarning("Repository {Repo} already exists.", request.Name);
                throw new ArgumentException($"A repository with the name '{request.Name}' already exists.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create repository on GitHub");
                throw;
            }
        }

        public async Task<RepoDetailsResponse> GetRepoDetails(string repoName)
        {
            try
            {
                var repo = await gitHubClient.Repository.Get(username, repoName);

                var response =new RepoDetailsResponse
                {
                    Id = repo.Id,
                    Name = repo.Name,
                    FullName = repo.FullName,
                    Description = repo.Description,
                    Url = repo.HtmlUrl,
                    IsPrivate = repo.Private,
                    DefaultBranch = repo.DefaultBranch,
                    StargazersCount = repo.StargazersCount,
                    ForksCount = repo.ForksCount,
                    OpenIssuesCount = repo.OpenIssuesCount,
                    CreatedAt = repo.CreatedAt
                };

                await mcpServerDBService.UpsertRepoDetails(response);
                return response;
            }
            catch 
            {
                logger.LogWarning("Repository {Owner}/{RepoName} was not found.", username, repoName);
                throw;
            }
        }
    }
}
