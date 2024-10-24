using System.ComponentModel.DataAnnotations;

namespace InterServiceCenter_Core.Models;

public class AddAttachmentDTO
{
    [Required]
    public int Id { get; set; }
    [Required]
    public IFormFile? ImageFile { get; set; }
}