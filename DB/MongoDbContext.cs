using MongoDB.Driver;
using MCPServers.Models;

namespace MCPServers.DB
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("MongoDB:ConnectionString").Value;
            var databaseName = configuration.GetSection("MongoDB:DatabaseName").Value;

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<GitHubCredential> GitHubCredentials =>  _database.GetCollection<GitHubCredential>("GitHubCredentials");
    }
}