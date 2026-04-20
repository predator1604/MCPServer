using MCPServers.Models;
using Octokit;

namespace MCPServers.Services
{
    public interface IMCPServerService
    {
        Task<IEnumerable<string>> GetAllRepos();

        Task<bool> DeleteRepo(string repoName);

        Task<RepoResponse> CreateRepo(CreateRepoRequest request);

        Task<RepoDetailsResponse> GetRepoDetails(string repoName);

        Task<IEnumerable<object>> GetBranches(string repoName);

        Task<object> GetBranchDetails(string repoName, string branch);

        Task<IReadOnlyList<PullRequest>> GetPullRequests(string repoName, ItemStateFilter state);

        Task<PullRequest> GetPullRequestDetails(string repoName, int number);

        Task<PullRequest> CreatePullRequest(string repoName, NewPullRequest request);

        Task<PullRequest> UpdatePullRequest(string repoName, int number, PullRequestUpdate request);

        Task<User> GetCurrentUser();

        Task<User> GetUserProfile(string username);

        Task<IReadOnlyList<GitHubCommit>> GetCommits(string repoName, string? sha = null, string? path = null);

        Task<GitHubCommit> GetCommitDetails(string repoName, string sha);
    }
}
