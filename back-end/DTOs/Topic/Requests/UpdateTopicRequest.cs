using back_end.Enums;
using System.ComponentModel.DataAnnotations;

namespace back_end.DTOs.Topic.Requests
{
    public class UpdateTopicRequest
    {
        [Required(ErrorMessage = "Id is required!")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name is required!")]
        [MaxLength(ErrorMessage = "Name must not exceed 255 characters.")]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public Visibility Visibility { get; set; } = Visibility.Private;
    }
}
