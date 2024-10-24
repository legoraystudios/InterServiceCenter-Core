using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Services;
using InterServiceCenter_Core.Utilities;
using InterServiceCenter_Core.Utilities.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterServiceCenter_Core.Controllers;

[Authorize]
[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    public readonly InterServiceCenterContext _dbContext;
    public readonly AccountService _accountService;
    public readonly JwtToken _token;

    public AccountController(InterServiceCenterContext dbContext, AccountService accountService, JwtToken token)
    {
        _dbContext = dbContext;
        _accountService = accountService;
        _token = token;
    }
    
    [HttpGet("")]
    public IActionResult GetAccounts()
    {
        List<IscAccount> accounts = _dbContext.IscAccounts.ToList();
        return Ok(accounts);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetAccountById(int id)
    {
        var account = _dbContext.IscAccounts.FindAsync(id);
        return Ok(account);
    }

    [HttpPost("profile-photo")]
    public IActionResult SaveProfilePhoto([FromForm] AddAttachmentDTO attachment)
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);

        Task<JsonResponse> response = _accountService.SaveProfilePhoto(attachment, loggedEmail);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
    
    [HttpPut("profile-photo")]
    public IActionResult ModifyProfilePhoto([FromForm] AddAttachmentDTO attachment)
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);

        Task<JsonResponse> response = _accountService.ModifyProfilePhoto(attachment, loggedEmail);
        return StatusCode(response.Result.StatusCode, new { msg = response.Result.Message });
    }
    
    [HttpDelete("profile-photo/{id}")]
    public IActionResult RemoveProfilePhoto(int id)
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);
        
        JsonResponse response = _accountService.RemoveProfilePhoto(id, loggedEmail);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }
    
    [HttpPut("")]
    public IActionResult ModifyAccount([FromBody] ModifyAccountDTO account)
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);
        
        JsonResponse response = _accountService.ModifyAccount(account, loggedEmail);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }
    
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminRole")]
    public IActionResult DeleteAccount(int id)
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);

        JsonResponse response = _accountService.DeleteAccount(id, loggedEmail);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }
    
}