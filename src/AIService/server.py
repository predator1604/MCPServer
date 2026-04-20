import httpx
from mcp.server.fastmcp import FastMCP
from typing import Optional

# Initialize the MCP Server
mcp = FastMCP("GitHub-AI-Bridge")

# Update this to match your local C# API URL
BASE_URL = "https://localhost:44354/mcp/MCPServer"

@mcp.tool()
async def list_all_repositories():
    """List all repositories for the authenticated user."""
    async with httpx.AsyncClient() as client:
        response = await client.get(f"{BASE_URL}/GetAllRepos")
        return response.json()

@mcp.tool()
async def get_repository_details(repo_name: str):
    """Get full details and MongoDB metadata for a specific repository."""
    async with httpx.AsyncClient() as client:
        response = await client.get(f"{BASE_URL}/GetRepoDetails/{repo_name}")
        return response.json()

@mcp.tool()
async def create_repository(name: str, description: Optional[str] = None, is_private: bool = False):
    """Create a new GitHub repository."""
    payload = {"name": name, "description": description, "isPrivate": is_private}
    async with httpx.AsyncClient() as client:
        response = await client.post(f"{BASE_URL}/CreateRepo", json=payload)
        return response.json()

@mcp.tool()
async def delete_repository(repo_name: str):
    """Delete a GitHub repository."""
    async with httpx.AsyncClient() as client:
        response = await client.delete(f"{BASE_URL}/DeleteRepo/{repo_name}")
        return response.text

@mcp.tool()
async def list_branches(repo_name: str):
    """List all branches in a repository."""
    async with httpx.AsyncClient() as client:
        response = await client.get(f"{BASE_URL}/GetBranches/{repo_name}")
        return response.json()

@mcp.tool()
async def get_branch_details(repo_name: str, branch_name: str):
    """Get details of a specific branch."""
    async with httpx.AsyncClient() as client:
        response = await client.get(f"{BASE_URL}/GetBranchDetails/{repo_name}/{branch_name}")
        return response.json()

@mcp.tool()
async def list_pull_requests(repo_name: str, state: str = "Open"):
    """List PRs for a repo. State can be 'Open', 'Closed', or 'All'."""
    async with httpx.AsyncClient() as client:
        response = await client.get(f"{BASE_URL}/GetPullRequests/{repo_name}", params={"state": state})
        return response.json()

@mcp.tool()
async def get_pull_request_details(repo_name: str, pr_number: int):
    """Get details of a specific pull request."""
    async with httpx.AsyncClient() as client:
        response = await client.get(f"{BASE_URL}/GetPullRequestDetails/{repo_name}/{pr_number}")
        return response.json()

@mcp.tool()
async def create_pull_request(repo_name: str, title: str, head: str, base: str, body: str):
    """Create a new pull request."""
    payload = {"title": title, "head": head, "base": base, "body": body}
    async with httpx.AsyncClient() as client:
        response = await client.post(f"{BASE_URL}/CreatePullRequest/{repo_name}", json=payload)
        return response.json()

@mcp.tool()
async def update_pull_request(repo_name: str, pr_number: int, title: Optional[str] = None, body: Optional[str] = None, state: Optional[str] = None):
    """Update an existing pull request."""
    payload = {}
    if title: payload["title"] = title
    if body: payload["body"] = body
    if state: payload["state"] = state
    
    async with httpx.AsyncClient() as client:
        response = await client.patch(f"{BASE_URL}/UpdatePullRequest/{repo_name}/{pr_number}", json=payload)
        return response.json()

@mcp.tool()
async def list_commits(repo_name: str, sha: Optional[str] = None, path: Optional[str] = None):
    """List commits with optional filters for branch/SHA or file path."""
    params = {}
    if sha: params["sha"] = sha
    if path: params["path"] = path
    
    async with httpx.AsyncClient() as client:
        response = await client.get(f"{BASE_URL}/GetCommits/{repo_name}", params=params)
        return response.json()

@mcp.tool()
async def get_commit_details(repo_name: str, sha: str):
    """Get the specific changes (diff) and metadata for a commit SHA."""
    async with httpx.AsyncClient() as client:
        response = await client.get(f"{BASE_URL}/GetCommitDetails/{repo_name}/{sha}")
        return response.json()

@mcp.tool()
async def get_my_user_profile():
    """Get details of the currently authenticated GitHub user."""
    async with httpx.AsyncClient() as client:
        response = await client.get(f"{BASE_URL}/GetCurrentUser")
        return response.json()

@mcp.tool()
async def get_user_profile(username: str):
    """Get details of a specific GitHub user by username."""
    async with httpx.AsyncClient() as client:
        response = await client.get(f"{BASE_URL}/GetUserProfile/{username}")
        return response.json()

if __name__ == "__main__":
    mcp.run()