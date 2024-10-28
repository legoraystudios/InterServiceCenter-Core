using System.ComponentModel.DataAnnotations;

namespace InterServiceCenter_Core.Models;

public class RegisterDTO
{
    [Required] public string FirstName { get; set; }

    [Required] public string LastName { get; set; }

    [Required] public string Email { get; set; }

    [Required] public string Password { get; set; }

    [Required] public string ConfirmPassword { get; set; }
}