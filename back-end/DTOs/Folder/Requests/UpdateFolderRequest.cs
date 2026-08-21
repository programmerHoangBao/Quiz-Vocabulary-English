using back_end.Enums;
using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs.Folder.Requests
{
    public class UpdateFolderRequest
    {
        [Required]
        public Guid Id {  get; set; }
        [Required]
        [MaxLength(ErrorMessage = "Name must not exceed 255 characters.")]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        [Required]
        public Visibility Visibility { get; set; } = Visibility.Private;
    }
}
