using MongoDB.Bson.Serialization.Attributes;

namespace MCPServers.Models
{
    public class RepoDetailsResponse
    {
        [BsonId]
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Url { get; set; } = string.Empty;

        public bool IsPrivate { get; set; }

        public string DefaultBranch { get; set; } = "main";

        public int StargazersCount { get; set; }

        public int ForksCount { get; set; }

        public int OpenIssuesCount { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
