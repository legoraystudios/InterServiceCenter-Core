namespace InterServiceCenter_Core.Models;

public class ResetPasswordDTO
{
    public string Token { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}