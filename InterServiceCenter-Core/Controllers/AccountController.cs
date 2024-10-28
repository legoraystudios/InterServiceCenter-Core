using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Services;
using InterServiceCenter_Core.Utilities.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [Authorize]
    [HttpGet("")]
    public IActionResult GetAccounts()
    {
        var accounts = _dbContext.IscAccounts.ToList();
        return Ok(accounts);
    }

    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetAccountById(int id)
    {
        var account = _dbContext.IscAccounts.FindAsync(id);
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