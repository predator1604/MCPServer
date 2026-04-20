namespace MCPServers.Models
{
    public class CreateRepoRequest
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Visibility { get; set; } = "public";

        public bool AutoInit { get; set; } = true;
    }
}
