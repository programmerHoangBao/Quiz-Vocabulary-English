using AutoMapper;
using back_end.DTOs;
using back_end.DTOs.Topic.Requests;
using back_end.DTOs.Topic.Responses;
using back_end.Exceptions;
using back_end.Models;
using back_end.Records;
using back_end.Repositories.Interfaces;
using back_end.Services.Interfaces;

namespace back_end.Services
{
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _topicRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public TopicService(
            ITopicRepository topicRepository, 
            IFolderRepository folderRepository, 
            IMapper mapper, 
            ICurrentUserService currentUserService
        )
        {
            _topicRepository = topicRepository;
            _folderRepository = folderRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<TopicResponse>> CreateTopic(CreateTopicRequest req)
        {
            Guid? currentUserId = _currentUserService.UserId ??
                throw new BusinessException(ErrorRecord.Unauthorized);
            Folder? folder = await _folderRepository.GetFolderById(req.FolderId);
            if (folder == null || folder.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.FolderNotFound);
            }
            if (folder.UserId != currentUserId)
            {
                throw new BusinessException(ErrorRecord.Forbidden);
            }
            Topic newTopic = _mapper.Map<Topic>(req);
            bool isCreated = await _topicRepository.AddAsync(newTopic);
            if (!isCreated)
            {
                throw new BusinessException(ErrorRecord.TopicCreateFailed);
            }
            TopicResponse response = _mapper.Map<TopicResponse>(newTopic);
            return ApiResponse<TopicResponse>.MessageResponse(
                MessageRecord.TopicCreateSuccess,
                response
            );
        }

        public async Task<ApiResponse<TopicResponse?>> GetTopicById(Guid id)
        {
            Guid? viewerId = _currentUserService.UserId ??
                throw new BusinessException(ErrorRecord.Unauthorized);

            Topic? topic = await _topicRepository.GetTopicByIdAsync(id);
            if (topic == null || topic.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.TopicNotFound);
            }
            bool isTopicBelongsToUser = await _topicRepository.IsTopicBelongsToUserAsync(
                id, 
                viewerId.Value
            );

            if (!isTopicBelongsToUser)
            {
                throw new BusinessException(ErrorRecord.Forbidden);
            }
            TopicResponse response = _mapper.Map<TopicResponse>(topic);
            return ApiResponse<TopicResponse?>.MessageResponse(
                MessageRecord.GetTopicByIdSuccess,
                response
            );
        }

        public async Task<ApiResponse<object?>> GetTopicsByFolderId(Guid folderId, int pageNumber, int pageSize)
        {
            Guid? viewerId = _currentUserService.UserId ??
                 throw new BusinessException(ErrorRecord.Unauthorized);

            Folder? folder = await _folderRepository.GetFolderById(folderId);
            if (folder == null || folder.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.FolderNotFound);
            }
            var result = await _topicRepository.GetTopicsByFolderIdAsync(folderId, pageNumber, pageSize);
            List<Topic> topics = result.Topics;
            int totalItems = result.TotalItems;
            if (totalItems <= 0)
            {
                throw new BusinessException(ErrorRecord.NoData);
            }
            List<TopicResponse> responses = new List<TopicResponse>();
            foreach (Topic topic in topics)
            {
                if (topic.Visibility == Enums.Visibility.Public || viewerId == folder.UserId)
                {
                    TopicResponse response = _mapper.Map<TopicResponse>(topic);
                    responses.Add(response);
                }
            }
            int totalPages = (int)Math.Ceiling(
                (double)totalItems / pageSize
            );
            PaginationResponse<TopicResponse> paginationResponse = new()
            {
                Items = responses,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
            return ApiResponse<object?>.MessageResponse(
                MessageRecord.GetTopicsByFolderIdSuccess,
                paginationResponse
            );
        }

        public async Task<ApiResponse<object?>> SoftDeleteById(Guid id)
        {
            Guid? currentUserId = _currentUserService.UserId ??
                 throw new BusinessException(ErrorRecord.Unauthorized);
            Topic? topic = await _topicRepository.GetTopicByIdAsync(id);
            if (topic == null || topic.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.TopicNotFound);
            }
            bool isTopicBelongsToUser = await _topicRepository.IsTopicBelongsToUserAsync(
                id,
                currentUserId.Value
            );

            if (!isTopicBelongsToUser)
            {
                throw new BusinessException(ErrorRecord.Forbidden);
            }
            bool isDeleted = await _topicRepository.SoftDeleteByIdAsync(id);
            if (isDeleted)
            {
                return ApiResponse<object?>.MessageResponse(MessageRecord.TopicDeleteSuccess);
            }
            throw new BusinessException(ErrorRecord.TopicDeleteFailed);
        }

        public async Task<ApiResponse<object?>> UpdateTopic(UpdateTopicRequest req)
        {
            Guid? currentUserId = _currentUserService.UserId ??
                throw new BusinessException(ErrorRecord.Unauthorized);
            Topic? topic = await _topicRepository.GetTopicByIdAsync(req.Id);
            if (topic == null || topic.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.TopicNotFound);
            }
            bool isTopicBelongsToUser = await _topicRepository.IsTopicBelongsToUserAsync(
                req.Id,
                currentUserId.Value
            );
            if (!isTopicBelongsToUser)
            {
                throw new BusinessException(ErrorRecord.Forbidden);
            }
            topic = _mapper.Map<Topic>(req);
            bool isUpdated = await _topicRepository.UpdateAsync(topic);
            if (isUpdated)
            {
                return ApiResponse<object?>.MessageResponse(MessageRecord.TopicUpdateSuccess);
            }
            throw new BusinessException(ErrorRecord.TopicUpdateFailed);
        }
    }
}
