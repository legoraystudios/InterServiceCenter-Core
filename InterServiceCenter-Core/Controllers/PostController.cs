using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Services;
using InterServiceCenter_Core.Utilities.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterServiceCenter_Core.Controllers;

[ApiController]
[Route("api/post")]
public class PostController : ControllerBase
{
    private readonly InterServiceCenterContext _dbContext;
    private readonly PostService _postService;
    private readonly FileService _fileService;
    private readonly JwtToken _token;

    public PostController(PostService postService, InterServiceCenterContext dbContext, JwtToken token, FileService fileService)
    {
        _postService = postService;
        _dbContext = dbContext;
        _fileService = fileService;
        _token = token;
    }

    [HttpGet("")]
    public async Task<ActionResult<PagedResponse<IscPost>>> GetPosts([FromQuery] int page = 0, [FromQuery] int pageSize = 0)
    {
        
        if (page == 0 && pageSize == 0)
        {
            // Fetch all posts without pagination
            var posts = await _dbContext.IscPosts.OrderByDescending(p => p.PublishedAt)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Content,
                    p.PublishedAt,
                    p.PublishedBy,
                    p.FrontBannerFile,
                    AuthorName = p.PublishedByNavigation.FirstName + " " + p.PublishedByNavigation.LastName
                })
                .ToListAsync();

            return Ok(posts);
        }
        else
        {
            if (page == 0 && pageSize > 0)
            {
                return BadRequest("Invalid Page Number");
            }
            
            var totalItems = await _dbContext.IscPosts.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        
            var posts = await _dbContext.IscPosts.OrderByDescending(p => p.PublishedAt).Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Content,
                    p.PublishedAt,
                    p.PublishedBy,
                    p.FrontBannerFile,
                    AuthorName = p.PublishedByNavigation.FirstName + " " + p.PublishedByNavigation.LastName
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();
        
            var response = new PagedResponse<object>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalItems,
                TotalPages = totalPages,
                Items = posts
            };
        
            return Ok(response);
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetPostsById(int id)
    {
        var posts = _dbContext.IscPosts.Where(p => p.Id == id).Select(p => new
            {
                p.Id,
                p.Title,
                p.Content,
                p.PublishedAt,
                p.PublishedBy,
                p.FrontBannerFile,
                AuthorName = p.PublishedByNavigation.FirstName + " " + p.PublishedByNavigation.LastName
            })
            .FirstOrDefault();
        
        return Ok(posts);
    }
    
    [HttpGet("{id}/banner")]
    public IActionResult GetFrontBanner(int id)
    {
        var response = _postService.GetFrontBanner(id);

        if (response.Result.StatusCode != 200)
            return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });

        var path = _fileService.GetFrontBannerPath(response.Result.Message);
        return PhysicalFile(path, "image/jpeg");
    }
    
    [Authorize]
    [HttpDelete("{id}/banner")]
    public IActionResult RemoveFrontBanner(int id)
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);

        var response = _postService.RemoveFrontBanner(id);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }

    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> CreatePost([FromForm] IscPost post, [FromForm] IFormFile? frontBanner)
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);

        var response = _postService.SavePost(post, frontBanner, loggedEmail);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
    
    [Authorize]
    [HttpPut("")]
    public async Task<IActionResult> ModifyPost([FromForm] IscPost post, [FromForm] IFormFile? frontBanner)
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);

        var response = _postService.ModifyPost(post, frontBanner, loggedEmail);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult DeletePost(int id)
    {
        var response = _postService.DeletePost(id);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }
}