namespace MCPServers.Models
{
    public class GitHubCredential
    {
        public int Id { get; set; }

        public required string Username { get; set; } = string.Empty;

        public required string EncryptedToken { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
