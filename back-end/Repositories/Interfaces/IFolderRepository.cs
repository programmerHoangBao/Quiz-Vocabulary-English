using back_end.Models;

namespace back_end.Repositories.Interfaces
{
    public interface IFolderRepository
    {
        Task<bool> AddAsync(Folder folder);
        Task<bool> Update(Folder folder);
        Task<bool> SoftDelete(Guid folderId);
        Task<Folder?> GetFolderById(Guid folderId);
        Task<(List<Folder> Folders, int TotalItems)> GetFoldersByUserId(Guid userId, int pageNumber, int pageSize);
    }
}
