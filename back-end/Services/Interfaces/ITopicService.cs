using back_end.DTOs;
using back_end.DTOs.Topic.Requests;
using back_end.DTOs.Topic.Responses;

namespace back_end.Services.Interfaces
{
    public interface ITopicService
    {
        Task<ApiResponse<TopicResponse?>> GetTopicById(Guid id);
        Task<ApiResponse<object?>> GetTopicsByFolderId(Guid folderId, int pageNumber, int pageSize);
        Task<ApiResponse<TopicResponse>> CreateTopic(CreateTopicRequest req);
        Task<ApiResponse<object?>> UpdateTopic(UpdateTopicRequest req);
        Task<ApiResponse<object?>> SoftDeleteById(Guid id);
    }
}
