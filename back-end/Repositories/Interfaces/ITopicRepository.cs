using back_end.Models;

namespace back_end.Repositories.Interfaces
{
    public interface ITopicRepository
    {
        Task<bool> AddAsync(Topic topic);
        Task<bool> UpdateAsync(Topic topic);
        Task<bool> SoftDeleteByIdAsync(Guid id);
        Task<Topic?> GetTopicByIdAsync(Guid id);
        Task<(List<Topic> Topics, int TotalItems)> GetTopicsByFolderIdAsync(
            Guid folderId, 
            int pageNumber, 
            int pageSize
        );
        Task<Guid?> GetOwnerIdByIdAsync(Guid id);
    }
}
