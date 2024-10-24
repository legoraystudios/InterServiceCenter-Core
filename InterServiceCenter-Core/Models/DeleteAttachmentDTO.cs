using System.ComponentModel.DataAnnotations;

namespace InterServiceCenter_Core.Models;

public class DeleteAttachmentDTO
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string FileUID { get; set; }
}