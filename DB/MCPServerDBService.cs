using MCPServers.Models;
using MongoDB.Driver;

namespace MCPServers.DB
{
    public class MCPServerDBService: IMCPServerDBService
    {
        private readonly MongoDbContext context;
        private readonly ILogger<MCPServerDBService> logger;

        public MCPServerDBService(MongoDbContext context, ILogger<MCPServerDBService> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<bool> SaveGitHubCredentialsAsync(string username, string token)
        {
            var filter = Builders<GitHubCredential>.Filter.Eq(x => x.Username, username);

            var update = Builders<GitHubCredential>.Update
                .Set(x => x.EncryptedToken, token)
                .Set(x => x.CreatedAt, DateTime.UtcNow);

            var options = new UpdateOptions { IsUpsert = true };

            var result = await context.GitHubCredentials.UpdateOneAsync(filter, update, options);
            return result.IsAcknowledged;
        }

        public async Task<bool> UpsertRepoDetails(RepoDetailsResponse repo)
        {
            try
            {
                var filter = Builders<RepoDetailsResponse>.Filter.Eq(x => x.Id, repo.Id);

                var options = new ReplaceOptions { IsUpsert = true };

                var result = await context.RepoDetails.ReplaceOneAsync(filter, repo, options);

                logger.LogInformation("Successfully persisted repo {RepoName} to MongoDB.", repo.FullName);
                return result.IsAcknowledged;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save repo {RepoName} to MongoDB.", repo.FullName);
                return false;
            }
        }
    }
}
