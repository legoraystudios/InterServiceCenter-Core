using System.ComponentModel.DataAnnotations;

namespace InterServiceCenter_Core.Models;

public class ModifyAccountDTO
{
    [Required] public int Id { get; set; }
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    [Required] public string Email { get; set; }
    [Required] public string Role { get; set; }
}