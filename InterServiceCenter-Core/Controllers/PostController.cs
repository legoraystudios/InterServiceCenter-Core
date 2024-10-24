using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterServiceCenter_Core.Controllers;

[ApiController]
[Route("api/post")]
public class PostController : ControllerBase
{
    public readonly PostService _postService;
    public readonly InterServiceCenterContext _dbContext;
    
    public PostController(PostService postService, InterServiceCenterContext dbContext)
    {
        _postService = postService;
        _dbContext = dbContext;
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
}