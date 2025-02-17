using InterServiceCenter_Core.Models;
using InterServiceCenter_Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterServiceCenter_Core.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    public readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO login)
    {
        var response = _authService.LoginAccount(login.Email, login.Password, login.Remember);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }
    
    [HttpPost("register")]
    [Authorize(Policy = "AdminRole")]
    public IActionResult Register([FromBody] RegisterDTO register)
    {
        var response = _authService.SaveAccount(register.FirstName, register.LastName, register.Email,
            register.Password, register.ConfirmPassword);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }
    
    [HttpPost("forgot-password/{email}")]
    public IActionResult ForgotPassword(string email)
    {
        var response = _authService.ForgotPassword(email);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }
    
    [HttpPost("forgot-password/check-token/{token}")]
    public IActionResult CheckToken(string token)
    {
        var response = _authService.CheckToken(token);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }
    
    [HttpPost("forgot-password/reset/")]
    public IActionResult CheckToken([FromBody] ResetPasswordDTO content)
    {
        var response = _authService.ResetPassword(content.Token, content.Password, content.ConfirmPassword);
        return StatusCode(response.StatusCode, new { msg = response.Message });
    }
}