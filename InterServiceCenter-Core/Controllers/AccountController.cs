using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Services;
using InterServiceCenter_Core.Utilities.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterServiceCenter_Core.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;
    private readonly InterServiceCenterContext _dbContext;
    private readonly FileService _fileService;
    private readonly JwtToken _token;


    public AccountController(InterServiceCenterContext dbContext, AccountService accountService, JwtToken token,
        FileService fileService)
    {
        _dbContext = dbContext;
        _accountService = accountService;
        _token = token;
        _fileService = fileService;
    }
    
    [HttpGet("")]
    [Authorize(Policy = "AdminRole")]
    public async Task<ActionResult<PagedResponse<IscAccount>>> GetAccounts([FromQuery] int page = 0, [FromQuery] int pageSize = 0)
    {
        if (page == 0 && pageSize == 0)
        {
            var accounts = _dbContext.IscAccounts.Select(p => new
                {
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    p.Email,
                    p.Role,
                    p.ProfilePhotoFile,
                    p.CreatedAt,
                    Posts = p.IscPosts.Select(p => new
                    {
                        p.Id,
                        p.Title,
                        p.Content,
                        p.PublishedAt,
                        p.PublishedBy,
                        p.FrontBannerFile
                    })

                })
                .ToList();

            return Ok(accounts);
        }
        else
        {
            if (page == 0 && pageSize > 0)
            {
                return BadRequest("Invalid Page Number");
            }
            
            var totalItems = await _dbContext.IscPosts.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            
            var accounts = await _dbContext.IscAccounts.Select(p => new
                {
                    p.Id,
                    p.FirstName,
                    p.LastName,
                    p.Email,
                    p.Role,
                    p.ProfilePhotoFile,
                    p.CreatedAt,
                    Posts = p.IscPosts.Select(p => new
                    {
                        p.Id,
                        p.Title,
                        p.Content,
                        p.PublishedAt,
                        p.PublishedBy,
                        p.FrontBannerFile
                    })

                })
            .Skip((page - 1) * pageSize)
            .Take(pageSize).ToListAsync();
            
            var response = new PagedResponse<object>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalItems,
                TotalPages = totalPages,
                Items = accounts
            };
        
            return Ok(response);
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    [Authorize(Policy = "AdminRole")]
    public IActionResult GetAccountById(int id)
    {
        var account = _dbContext.IscAccounts.Where(p => p.Id == id).Select(p => new
        {
            p.Id,
            p.FirstName,
            p.LastName,
            p.Email,
            p.Role,
            p.ProfilePhotoFile,
            p.CreatedAt,
            Posts = p.IscPosts.Select(p => new
            {
                p.Id,
                p.Title,
                p.Content,
                p.PublishedAt,
                p.PublishedBy,
                p.FrontBannerFile
            })

        }).FirstOrDefault();
        
        return Ok(account);
    }
    
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetLoggedAccountInfo()
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);
        var account = _dbContext.IscAccounts.Where(p => p.Email == loggedEmail).Select(p => new
        {
            p.Id,
            p.FirstName,
            p.LastName,
            p.Email,
            p.Role,
            p.ProfilePhotoFile,
            p.CreatedAt,
            Posts = p.IscPosts.Select(p => new
            {
                p.Id,
                p.Title,
                p.Content,
                p.PublishedAt,
                p.PublishedBy,
                p.FrontBannerFile
            })

        }).FirstOrDefault();

        if (account == null)
        {
            return NotFound(new { msg = "ERROR: Account doesn't exist in our records." });
        }

        return Ok(account);
    }

    [Authorize]
    [HttpPost("profile-photo")]
    public IActionResult SaveProfilePhoto([FromForm] AddAttachmentDTO attachment)
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);

        var response = _accountService.SaveProfilePhoto(attachment, loggedEmail);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }

    [Authorize]
    [HttpPut("profile-photo")]
    public IActionResult ModifyProfilePhoto([FromForm] AddAttachmentDTO attachment)
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);

        var response = _accountService.ModifyProfilePhoto(attachment, loggedEmail);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }

    [Authorize]
    [HttpDelete("{id}/profile-photo")]
    public IActionResult RemoveProfilePhoto(int id)
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);

        var response = _accountService.RemoveProfilePhoto(id, loggedEmail);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }

    [HttpGet("{id}/profile-photo")]
    public async Task<IActionResult> GetProfilePhoto(int id)
    {
        var response = _accountService.GetProfilePhoto(id);

        if (response.Result.StatusCode != 200)
            return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });

        var path = _fileService.GetProfilePhotoPath(response.Result.Message);
        return PhysicalFile(path, "image/jpeg");
    }

    [Authorize]
    [HttpPut("")]
    public IActionResult ModifyAccount([FromBody] ModifyAccountDTO account)
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);

        var response = _accountService.ModifyAccount(account, loggedEmail);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }

    [Authorize]
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminRole")]
    public IActionResult DeleteAccount(int id)
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);

        var response = _accountService.DeleteAccount(id, loggedEmail);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }
}