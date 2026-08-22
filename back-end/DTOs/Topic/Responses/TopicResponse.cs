namespace back_end.DTOs.Topic.Responses
{
    public class TopicResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public DateTime Created { get; set; }
    }
}
