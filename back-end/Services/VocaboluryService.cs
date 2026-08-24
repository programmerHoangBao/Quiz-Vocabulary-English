using AutoMapper;
using back_end.DTOs;
using back_end.DTOs.Vocabolury.Requests;
using back_end.DTOs.Vocabolury.Responses;
using back_end.Enums;
using back_end.Exceptions;
using back_end.Models;
using back_end.Records;
using back_end.Repositories.Interfaces;
using back_end.Services.Interfaces;

namespace back_end.Services
{
    public class VocaboluryService : IVocaboluryService
    {
        private readonly IVocaboluryRepository _vocaboluryRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly IFileService _fileService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public VocaboluryService(
            IVocaboluryRepository vocaboluryRepository, 
            ITopicRepository topicRepository, 
            IFileService fileService, 
            ICurrentUserService currentUserService,
            IMapper mapper
        )
        {
            _vocaboluryRepository = vocaboluryRepository;
            _topicRepository = topicRepository;
            _fileService = fileService;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<VocaboluryResponse?>> CreateVocabolury(
            CreateVocaboluryRequest req
        )
        {
            Guid? currentUserId = _currentUserService.UserId ??
                throw new BusinessException(ErrorRecord.Unauthorized);
            Topic? topic = await _topicRepository.GetTopicByIdAsync(req.TopicId);
            if (topic == null || topic.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.TopicNotFound);
            }
            bool isTopicBelongsToUser = await _topicRepository.IsTopicBelongsToUserAsync(
                req.TopicId,
                currentUserId.Value
            );

            if (!isTopicBelongsToUser)
            {
                throw new BusinessException(ErrorRecord.Forbidden);
            }
            string? imageUrl = null;
            try
            {
                if (req.Image != null)
                {
                    imageUrl = await _fileService.UploadAsync(req.Image);
                }
                Vocabolury newVocabolury = _mapper.Map<Vocabolury>(req);
                newVocabolury.ImageUrl = imageUrl;
                bool isCreated = await _vocaboluryRepository
                    .AddAsync(newVocabolury);
                if (!isCreated)
                {
                    await _fileService.DeleteAsync(imageUrl);
                    throw new BusinessException(ErrorRecord.VocaboluryCreationFailed);
                }
                VocaboluryResponse response = _mapper.Map<VocaboluryResponse>(newVocabolury);
                return ApiResponse<VocaboluryResponse?>.MessageResponse(
                    MessageRecord.VocaboluryCreationSuccess,
                    response
                );
            }
            catch
            {
                if (imageUrl != null)
                {
                    await _fileService.DeleteAsync(imageUrl);
                }
                throw;
            }
        }

        public async Task<ApiResponse<object?>> GetVocaboluriesByTopicId(
            Guid topicId, 
            int pageNumber, 
            int pageSize)
        {
            Guid? currentUserId = _currentUserService.UserId ??
                throw new BusinessException(ErrorRecord.Unauthorized);
            Topic? topic = await _topicRepository.GetTopicByIdAsync(topicId);
            if (topic == null || topic.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.TopicNotFound);
            }
            bool isTopicBelongsToUser = await _topicRepository.IsTopicBelongsToUserAsync(
                topicId,
                currentUserId.Value
            );

            if (!isTopicBelongsToUser)
            {
                throw new BusinessException(ErrorRecord.Forbidden);
            }  
            var result = await _vocaboluryRepository.GetVocaboluriesByTopicId(topicId, pageNumber, pageSize);
            List<Vocabolury> vocaboluries = result.vocaboluries;
            int totalItems = result.TotalItems;
            if (totalItems <= 0)
            {
                throw new BusinessException(ErrorRecord.NoData);
            }
            List<VocaboluryResponse> responses = vocaboluries
                .Select(v => _mapper.Map<VocaboluryResponse>(v))
                .ToList();
            int totalPages = (int)Math.Ceiling(
                (double)totalItems / pageSize
            );
            PaginationResponse<VocaboluryResponse> pagination = new()
            {
                Items = responses,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
            return ApiResponse<object?>.MessageResponse(
                MessageRecord.GetVocaboluriesByTopicIdSuccess,
                pagination
            );
        }

        public async Task<ApiResponse<VocaboluryResponse?>> GetVocaboluryById(Guid id)
        {
            Guid? currentUserId = _currentUserService.UserId ??
                  throw new BusinessException(ErrorRecord.Unauthorized);
            Vocabolury? vocabolury = await _vocaboluryRepository.GetVocaboluryByIdAsync(id);
            if (vocabolury == null || vocabolury.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.VocaboluryNotFound);
            }
            bool isTopicBelongsToUser = await _topicRepository.IsTopicBelongsToUserAsync(
                vocabolury.TopicId,
                currentUserId.Value
            );

            if (!isTopicBelongsToUser)
            {
                throw new BusinessException(ErrorRecord.Forbidden);
            }
            VocaboluryResponse response = _mapper.Map<VocaboluryResponse>(vocabolury);
            return ApiResponse<VocaboluryResponse?>.MessageResponse(
                MessageRecord.GetVocaboluryByIdSuccess,
                response
            );
        }

        public async Task<ApiResponse<object?>> ImportAsync(Guid topicId, IFormFile file)
        {
            Guid? currentUserId = _currentUserService.UserId ??
                throw new BusinessException(ErrorRecord.Unauthorized);
            Topic? topic = await _topicRepository.GetTopicByIdAsync(topicId);
            if (topic == null || topic.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.TopicNotFound);
            }
            bool isTopicBelongsToUser = await _topicRepository.IsTopicBelongsToUserAsync(
                topicId,
                currentUserId.Value
            );

            if (!isTopicBelongsToUser)
            {
                throw new BusinessException(ErrorRecord.Forbidden);
            }
            var rows = await _fileService.ParseAsync(file);
            var response = new ImportVocabularyResponse{ TotalRows = rows.Count };
            var vocabularies = new List<Vocabolury>();

            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var rowNumber = i + 2;
                if (string.IsNullOrWhiteSpace(row.Word))
                {
                    response.Errors.Add(new ImportVocabularyError
                    {
                        Row = rowNumber,
                        Word = row.Word,
                        Error = "Word is required"
                    });

                    continue;
                }

                if (string.IsNullOrWhiteSpace(row.Meaning))
                {
                    response.Errors.Add(new ImportVocabularyError
                    {
                        Row = rowNumber,
                        Word = row.Word,
                        Error = "Meaning is required"
                    });

                    continue;
                }

                PartOfSpeech? partOfSpeech = null;

                if (!string.IsNullOrWhiteSpace(row.PartOfSpeech))
                {
                    if (!Enum.TryParse<PartOfSpeech>(
                        row.PartOfSpeech,
                        true,
                        out var parsedPartOfSpeech))
                    {
                        response.Errors.Add(new ImportVocabularyError
                        {
                            Row = rowNumber,
                            Word = row.Word,
                            Error = $"Invalid PartOfSpeech: {row.PartOfSpeech}"
                        });

                        continue;
                    }

                    partOfSpeech = parsedPartOfSpeech;
                }

                vocabularies.Add(new Vocabolury
                {
                    Word = row.Word,
                    Meaning = row.Meaning,
                    PartOfSpeech = partOfSpeech,
                    ExampleEn = row.ExampleEn,
                    ExampleVn = row.ExampleVn,
                    IpaUk = row.IpaUk,
                    IpaUs = row.IpaUs,
                    TopicId = topicId
                });
            }

            if (vocabularies.Count > 0)
            {
                await _vocaboluryRepository
                    .AddRangeAsync(vocabularies);
            }

            response.SuccessCount = vocabularies.Count;
            response.FailedCount = response.Errors.Count;

            return ApiResponse<object?>.MessageResponse(
                    MessageRecord.ImportFileSuccesfully,
                    response
                );
        }

        public async Task<ApiResponse<object?>> SoftDeleteById(Guid id)
        {
            Guid? currentUserId = _currentUserService.UserId ??
                  throw new BusinessException(ErrorRecord.Unauthorized);
            Vocabolury? vocabolury = await _vocaboluryRepository.GetVocaboluryByIdAsync(id);
            if (vocabolury == null || vocabolury.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.VocaboluryNotFound);
            }
            bool isTopicBelongsToUser = await _topicRepository.IsTopicBelongsToUserAsync(
                vocabolury.TopicId,
                currentUserId.Value
            );
            if (!isTopicBelongsToUser)
            {
                throw new BusinessException(ErrorRecord.Forbidden);
            }
            bool isDeleted = await _vocaboluryRepository.SoftDeleteByIdAsync(id);
            if (!isDeleted)
            {
                throw new BusinessException(ErrorRecord.VocaboluryDeleteFailed);
            }
            return ApiResponse<object?>.MessageResponse(MessageRecord.VocaboluryDeleteSuccess);
        }

        public async Task<ApiResponse<object?>> UpdateVocabolury(UpdateVocaboluryRequest req)
        {
            Guid? currentUserId = _currentUserService.UserId ??
                  throw new BusinessException(ErrorRecord.Unauthorized);
            Vocabolury? vocabolury = await _vocaboluryRepository.GetVocaboluryByIdAsync(req.Id);
            if (vocabolury == null || vocabolury.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.VocaboluryNotFound);
            }
            bool isTopicBelongsToUser = await _topicRepository.IsTopicBelongsToUserAsync(
                vocabolury.TopicId,
                currentUserId.Value
            );
            if (!isTopicBelongsToUser)
            {
                throw new BusinessException(ErrorRecord.Forbidden);
            }
            string? imageUrl = null;
            try
            {
                if (req.Image != null)
                {
                    imageUrl = await _fileService.UploadAsync(req.Image);
                    vocabolury.ImageUrl = imageUrl;
                }
                vocabolury = _mapper.Map<Vocabolury>(req);
                bool isUdated = await _vocaboluryRepository.UpdateAsync(vocabolury);
                if (!isUdated)
                {
                    throw new BusinessException(ErrorRecord.VocaboluryUpdateFailed);
                }
                return ApiResponse<object?>.MessageResponse(MessageRecord.VocaboluryUpdateSuccess);
            }
            catch
            {
                if (imageUrl != null)
                {
                    await _fileService.DeleteAsync(imageUrl);
                }
                throw;
            }

        }
    }
}
