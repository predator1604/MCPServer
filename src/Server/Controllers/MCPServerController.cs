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

        [HttpGet("GetBranches/{repoName}")]
        public async Task<IActionResult> GetBranches(string repoName)
        {
            logger.LogInformation("GetBranches API called for repo: {RepoName}", repoName);
            try
            {
                var branches = await mcpServerService.GetBranches(repoName);
                return Ok(branches);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching branches for {RepoName}", repoName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetBranchDetails/{repoName}/{branch}")]
        public async Task<IActionResult> GetBranchDetails(string repoName, string branch)
        {
            logger.LogInformation("GetBranchDetails API called for {RepoName} branch {Branch}", repoName, branch);
            try
            {
                var branchDetails = await mcpServerService.GetBranchDetails(repoName, branch);
                return Ok(branchDetails);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching details for branch {Branch} in {RepoName}", branch, repoName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetPullRequests/{repoName}")]
        public async Task<IActionResult> GetPullRequests(string repoName, [FromQuery] ItemStateFilter state = ItemStateFilter.Open)
        {
            logger.LogInformation("GetPullRequests called for {RepoName} with state {State}", repoName, state);
            try
            {
                var pulls = await mcpServerService.GetPullRequests(repoName, state);
                return Ok(pulls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching PRs: {ex.Message}");
            }
        }

        [HttpGet("GetPullRequestDetails/{repoName}/{number}")]
        public async Task<IActionResult> GetPullRequestDetails(string repoName, int number)
        {
            try
            {
                var pr = await mcpServerService.GetPullRequestDetails(repoName, number);
                return Ok(pr);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("CreatePullRequest/{repoName}")]
        public async Task<IActionResult> CreatePullRequest(string repoName, [FromBody] NewPullRequest request)
        {
            try
            {
                var pr = await mcpServerService.CreatePullRequest(repoName, request);
                return Created(pr.HtmlUrl, pr);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("UpdatePullRequest/{repoName}/{number}")]
        public async Task<IActionResult> UpdatePullRequest(string repoName, int number, [FromBody] PullRequestUpdate request)
        {
            try
            {
                var pr = await mcpServerService.UpdatePullRequest(repoName, number, request);
                return Ok(pr);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            logger.LogInformation("GetCurrentUser API called.");
            try
            {
                var user = await mcpServerService.GetCurrentUser();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching authenticated user: {ex.Message}");
            }
        }

        [HttpGet("GetUserProfile/{username}")]
        public async Task<IActionResult> GetUserProfile(string username)
        {
            logger.LogInformation("GetUserProfile called for: {Username}", username);
            try
            {
                var user = await mcpServerService.GetUserProfile(username);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound($"User '{username}' not found or error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetCommits/{repoName}")]
        public async Task<IActionResult> GetCommits(string repoName, [FromQuery] string? sha = null, [FromQuery] string? path = null)
        {
            logger.LogInformation("GetCommits called for {RepoName}. Filter SHA: {Sha}, Path: {Path}", repoName, sha, path);
            try
            {
                var commits = await mcpServerService.GetCommits(repoName, sha, path);
                return Ok(commits);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching commits for {RepoName}", repoName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetCommitDetails/{repoName}/{sha}")]
        public async Task<IActionResult> GetCommitDetails(string repoName, string sha)
        {
            logger.LogInformation("GetCommitDetails called for {RepoName} SHA: {Sha}", repoName, sha);
            try
            {
                var commit = await mcpServerService.GetCommitDetails(repoName, sha);
                return Ok(commit);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching commit {Sha} for {RepoName}", sha, repoName);
                return NotFound($"Commit not found: {ex.Message}");
            }
        }
    }
}