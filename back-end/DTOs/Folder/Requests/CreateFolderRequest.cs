using back_end.Enums;
using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs.Folder.Requests
{
    public class CreateFolderRequest
    {
        [Required]
        [MaxLength(ErrorMessage = "Name must not exceed 255 characters.")]
        public string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public Visibility Visibility { get; set; } = Visibility.Private;
    }
}
