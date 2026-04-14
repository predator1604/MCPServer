using MCPServers.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCPServers.Controllers
{
    [ApiController]
    [Route("mcp/[controller]")]
    public class MCPServerController : ControllerBase
    {
        private readonly IMCPServerService mcpServerService;
        private readonly ILogger<MCPServerController> logger;

        public MCPServerController(IMCPServerService mcpServerService, ILogger<MCPServerController> logger)
        {
            this.mcpServerService = mcpServerService;
            this.logger = logger;
        }

        // Define all the GET APIs here
        [HttpGet("GetRepos")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllRepos()
        {
            logger.LogInformation("GetRepos API called.");
            try
            {
                var repos = await mcpServerService.GetAllRepos();
                if (repos == null || !repos.Any())
                {
                    return NotFound("No repositories found.");
                }
                return Ok(repos);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching repositories from GitHub: {ex.Message}");
            }
        }

        // Define all the POST APIs here

        // Define all the PATCH APIs here

        // Define all the DELETE APIs here
        [HttpDelete("DeleteRepo/{repoName}")]
        public async Task<IActionResult> DeleteRepo(string repoName) {
            logger.LogInformation("DeleteRepo API called with id: {repoName}", repoName);
            try
            {
                var result = await mcpServerService.DeleteRepo(repoName);
                if (!result)
                {
                    return NotFound("Repository not found.");
                }
                return Ok("Repository deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the repository: {ex.Message}");
            }
        }
    }
}