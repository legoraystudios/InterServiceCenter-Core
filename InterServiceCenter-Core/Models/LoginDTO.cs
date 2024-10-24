using System.ComponentModel.DataAnnotations;

namespace InterServiceCenter_Core.Models;

public class LoginDTO
{
    [Required] public string Email { get; set; }
    [Required] public string Password { get; set; }
    [Required] public bool Remember { get; set; }
}