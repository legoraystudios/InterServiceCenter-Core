using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Services;
using InterServiceCenter_Core.Utilities;
using InterServiceCenter_Core.Utilities.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterServiceCenter_Core.Controllers;

[ApiController]
[Route("api/post")]
public class PostController : ControllerBase
{
    public readonly PostService _postService;
    public readonly InterServiceCenterContext _dbContext;
    public readonly JwtToken _token;
    
    public PostController(PostService postService, InterServiceCenterContext dbContext, JwtToken token)
    {
        _postService = postService;
        _dbContext = dbContext;
        _token = token;
    }
    
    [HttpGet("")]
    public IActionResult GetPosts()
    {
        List<IscPost> posts = _dbContext.IscPosts.ToList();
        return Ok(posts);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetPosts(int id)
    {
        var posts = _dbContext.IscPosts.FindAsync(id);
        return Ok(posts);
    }
    
    [Authorize]
    [HttpPost("")]
    public IActionResult CreatePost([FromBody] IscPost post, [FromForm] IFormFile? frontBanner)
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);

        var publishedBy = _dbContext.IscAccounts.FirstOrDefault(acct => acct.Email == loggedEmail);

        if (publishedBy == null)
        {
            return StatusCode(404, new { msg = "ERROR: Signed In user doesn't exist in our records." });
        }

        post.PublishedByNavigation = publishedBy;
        
        Task<JsonResponse> response = _postService.SavePost(post, frontBanner, loggedEmail);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
}