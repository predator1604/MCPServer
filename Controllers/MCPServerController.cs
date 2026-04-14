using MCPServers.Models;
using MCPServers.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Octokit;

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

        [HttpGet("GetAllRepos")]
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

        [HttpGet("GetRepoDetails/{repoName}")]
        public async Task<IActionResult> GetRepoDetails(string repoName)
        {
            logger.LogInformation("GetRepoDetails API called for {RepoName}", repoName);

            try
            {
                var repo = await mcpServerService.GetRepoDetails(repoName);
                return Ok(repo);
            }
            catch (NotFoundException)
            {
                return NotFound($"Repository '{repoName}' not found.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching repo details for {RepoName}",repoName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("CreateRepo")]
        public async Task<IActionResult> CreateRepo([FromBody] CreateRepoRequest request)
        {
            logger.LogInformation("CreateRepo API called for repo: {RepoName}", request.Name);

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Repository name is required.");
            }

            try
            {
                var createdRepo = await mcpServerService.CreateRepo(request);
                return Created(createdRepo.Url, createdRepo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating repository {RepoName}", request.Name);
                return StatusCode(500, $"An error occurred while creating the repository: {ex.Message}");
            }
        }

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