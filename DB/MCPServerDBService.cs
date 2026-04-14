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
            // In MongoDB, we use Filters and "Upsert" logic
            var filter = Builders<GitHubCredential>.Filter.Eq(x => x.Username, username);

            var update = Builders<GitHubCredential>.Update
                .Set(x => x.EncryptedToken, token)
                .Set(x => x.CreatedAt, DateTime.UtcNow);

            var options = new UpdateOptions { IsUpsert = true };

            var result = await context.GitHubCredentials.UpdateOneAsync(filter, update, options);
            return result.IsAcknowledged;
        }
    }
}
