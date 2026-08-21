using back_end.DTOs;
using back_end.DTOs.Folder.Requests;
using back_end.DTOs.Folder.Responses;

namespace back_end.Services.Interfaces
{
    public interface IFolderService
    {
        Task<ApiResponse<FolderResponse>> CreateFolder(CreateFolderRequest req);
        Task<ApiResponse<object?>> UpdateFolder(UpdateFolderRequest req);
        Task<ApiResponse<object?>> SoftDeleteFolder(Guid folderId);
        Task<ApiResponse<FolderResponse>> GetFolderById(Guid folderId);
        Task<ApiResponse<PaginationResponse<FolderResponse>>> GetFoldersByUserId(
            Guid userId, 
            int pageNumber, 
            int pageSize
        );
    }
}
