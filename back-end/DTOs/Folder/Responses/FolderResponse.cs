using back_end.Enums;

namespace back_end.DTOs.Folder.Responses
{
    public class FolderResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Visibility Visibility { get; set; }
        public DateTime Created {  get; set; }
    }
}
