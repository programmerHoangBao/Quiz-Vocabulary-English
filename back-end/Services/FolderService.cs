using AutoMapper;
using back_end.DTOs;
using back_end.DTOs.Folder.Requests;
using back_end.DTOs.Folder.Responses;
using back_end.Exceptions;
using back_end.Models;
using back_end.Records;
using back_end.Repositories.Interfaces;
using back_end.Services.Interfaces;

namespace back_end.Services
{
    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public FolderService(
            IFolderRepository folderRepository, 
            IUserRepository userRepository, 
            ILogger<FolderService> logger,
            IMapper mapper,
            ICurrentUserService currentUserService
        )
        {
            _folderRepository = folderRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<FolderResponse>> CreateFolder(CreateFolderRequest req)
        {
            Guid? userId = _currentUserService.UserId;
            if (userId == null)
            {
                throw new BusinessException(ErrorRecord.Unauthorized);
            }
            User? userEntity = await _userRepository.GetUserByIdAndIsDeleteFalse(userId.Value);
            if (userEntity == null) 
            {
                throw new BusinessException(ErrorRecord.UserNotFound);
            }
            Folder newFolder = _mapper.Map<Folder>(req);
            newFolder.UserId = userId.Value;
            bool isCreated = await _folderRepository.AddAsync(newFolder);
            if (!isCreated)
            {
                throw new BusinessException(ErrorRecord.FolderCreateFailed);
            }
            FolderResponse response = _mapper.Map<FolderResponse>(newFolder);
            return ApiResponse<FolderResponse>.MessageResponse(MessageRecord.FolderCreateSuccess, response);
        }

        public async Task<ApiResponse<FolderResponse>> GetFolderById(Guid folderId)
        {
            Guid? currentUserId = _currentUserService.UserId;
            if (currentUserId == null)
            {
                throw new BusinessException(ErrorRecord.Unauthorized);
            }
            Folder? folder = await _folderRepository.GetFolderById(folderId);
            if (folder == null || folder.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.FolderNotFound);
            }
            else if (folder.UserId != currentUserId && folder.Visibility == Enums.Visibility.Private)
            {
                throw new BusinessException(ErrorRecord.Forbidden);
            }
            FolderResponse response = _mapper.Map<FolderResponse>(folder);
            return ApiResponse<FolderResponse>.MessageResponse(MessageRecord.GetFolderByIdSuccess, response);
        }

        public async Task<ApiResponse<PaginationResponse<FolderResponse>>> GetFoldersByUserId(
            Guid userId, 
            int pageNumber, 
            int pageSize
        )
        {
            Guid? currentUserId = _currentUserService.UserId;
            if (currentUserId == null)
            {
                throw new BusinessException(ErrorRecord.Unauthorized);
            }
            User? userEntity = await _userRepository.GetUserByIdAndIsDeleteFalse(userId);
            if (userEntity == null)
            {
                throw new BusinessException(ErrorRecord.UserNotFound);
            }
            else if (currentUserId != userId)
            {
                throw new BusinessException(ErrorRecord.Forbidden);
            }
            var result = await _folderRepository.GetFoldersByUserId(userId, pageNumber, pageSize);
            List<Folder> folders = result.Folders;
            int totalItems = result.TotalItems;

            if (totalItems == 0)
            {
                throw new BusinessException(ErrorRecord.NoData);
            }
            List<FolderResponse> responses = folders
                .Select(f => _mapper.Map<FolderResponse>(f))
                .ToList();
            int totalPages = (int)Math.Ceiling(
                (double)totalItems / pageSize
            );
            PaginationResponse<FolderResponse> pagination = new()
            {
                Items = responses,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
            return ApiResponse<PaginationResponse<FolderResponse>>.MessageResponse(
                MessageRecord.GetFolderOfUserSuccess,
                pagination
            );
        }

        public async Task<ApiResponse<object?>> SoftDeleteFolder(Guid folderId)
        {
            Guid? userId = _currentUserService.UserId;
            if (userId == null)
            {
                throw new BusinessException(ErrorRecord.Unauthorized);
            }
            Folder? folder = await _folderRepository.GetFolderById(folderId);
            if (folder == null || folder.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.FolderNotFound);
            }
            if (folder.UserId != userId)
            {
                throw new BusinessException(ErrorRecord.Forbidden);
            }
            bool isSoftDelete = await _folderRepository.SoftDelete(folderId);
            if (!isSoftDelete)
            {
                throw new BusinessException(ErrorRecord.FolderDeleteFailed);
            }
            return ApiResponse<object?>.MessageResponse(MessageRecord.FolderDeleteSuccess);
        }

        public async Task<ApiResponse<object?>> UpdateFolder(UpdateFolderRequest req)
        {
            Guid? userId = _currentUserService.UserId;
            if (userId == null)
            {
                throw new BusinessException(ErrorRecord.Unauthorized);
            }
            Folder? folder = await _folderRepository.GetFolderById(req.Id);
            if (folder == null || folder.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.FolderNotFound);
            }
            if (folder.UserId != userId)
            {
                throw new BusinessException(ErrorRecord.Forbidden);
            }
            _mapper.Map(req, folder);
            bool isUpdated = await _folderRepository.Update(folder);
            if (!isUpdated)
            {
                throw new BusinessException(ErrorRecord.FolderUpdateFailed);
            }
            return ApiResponse<object?>.MessageResponse(MessageRecord.FolderUpdateSuccess);
        }
    }
}
