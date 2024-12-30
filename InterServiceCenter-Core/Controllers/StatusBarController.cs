using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Services;
using InterServiceCenter_Core.Utilities.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterServiceCenter_Core.Controllers;

[ApiController]
[Route("api/statusbar")]
public class StatusBarController : ControllerBase
{
    private readonly StatusBarService _statusBarService;
    private readonly InterServiceCenterContext _dbContext;
    public readonly JwtToken _token;

    public StatusBarController(StatusBarService statusBarService, InterServiceCenterContext dbContext, JwtToken token)
    {
        _statusBarService = statusBarService;
        _dbContext = dbContext;
        _token = token;
    }

    [Authorize]
    [HttpPost("property")]
    public IActionResult SetStatusBarProperties([FromBody] IscStatusbarproperty property)
    {
        var response = _statusBarService.SetStatusBarProperties(property);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }
    
    [HttpGet("property")]
    public IActionResult GetStatusBarProperties()
    {
        var response = _dbContext.IscStatusbarproperties.Select(p => new
            {
                p.Id,
                p.MessageInterval,
                p.StatusBarColor,
                StatusBarColorName = p.StatusBarColorNavigation.ColorName,
            })
            .ToList();
        
        return Ok(response);
    }
    
    [Authorize]
    [HttpGet("messages")]
    public IActionResult GetStatusBarMessages()
    {
        var response = _dbContext.IscStatusbarmessages.Select(p => new
            {
                p.Id,
                p.Message,
                p.Icon,
                p.CreatedBy,
                p.ModifiedBy,
                p.CreatedAt,
                p.ModifiedAt,
                p.ExpiresIn,
                CreatedByName = p.CreatedByNavigation.FirstName + " " + p.CreatedByNavigation.LastName,
                ModifiedByName = p.ModifiedByNavigation.FirstName + " " + p.ModifiedByNavigation.LastName,
                p.IconNavigation.IconName,
            })
            .ToList();
        
        return Ok(response);
    }
    
    [HttpGet("public/messages")]
    public IActionResult GetPublicStatusBarMessages()
    {
        var response = _dbContext.IscStatusbarmessages.Select(p => new
            {
                p.Id,
                p.Message,
                p.Icon,
                p.IconNavigation.IconName,
            })
            .ToList();
        
        return Ok(response);
    }
    
    [Authorize]
    [HttpGet("messages/{id}")]
    public IActionResult GetStatusBarMessagesById(int id)
    {
        var response = _dbContext.IscStatusbarmessages.Where(p => p.Id == id).Select(p => new
            {
                p.Id,
                p.Message,
                p.Icon,
                p.CreatedBy,
                p.ModifiedBy,
                p.CreatedAt,
                p.ModifiedAt,
                p.ExpiresIn,
                CreatedByName = p.CreatedByNavigation.FirstName + " " + p.CreatedByNavigation.LastName,
                ModifiedByName = p.ModifiedByNavigation.FirstName + " " + p.ModifiedByNavigation.LastName,
                p.IconNavigation.IconName,
            })
            .FirstOrDefault();
        
        return Ok(response);
    }
    
    [Authorize]
    [HttpPost("messages")]
    public IActionResult CreateStatusBarMessage([FromBody] IscStatusbarmessage message)
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);

        var response = _statusBarService.SaveStatusBarMessage(message, loggedEmail);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }
    
    [Authorize]
    [HttpPut("messages")]
    public IActionResult ModifyStatusBarMessage([FromBody] IscStatusbarmessage message)
    {
        var loggedEmail = _token.GetLoggedEmail(HttpContext.User);

        var response = _statusBarService.ModifyStatusBarMessage(message, loggedEmail);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }
    
    [Authorize]
    [HttpDelete("messages/{id}")]
    public IActionResult DeleteStatusBarMessage(int id)
    {
        var response = _statusBarService.DeleteStatusBarMessage(id);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }
    
}