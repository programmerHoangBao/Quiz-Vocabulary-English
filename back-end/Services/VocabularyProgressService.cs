using AutoMapper;
using back_end.DTOs;
using back_end.DTOs.Vocabolury.Responses;
using back_end.DTOs.VocabularyProgress.Requests;
using back_end.DTOs.VocabularyProgress.Responses;
using back_end.Exceptions;
using back_end.Models;
using back_end.RabbitMQ.Interfaces;
using back_end.Records;
using back_end.Repositories.Interfaces;
using back_end.Services.Interfaces;

namespace back_end.Services
{
    public class VocabularyProgressService : IVocabularyProgressService
    {
        private readonly IVocabularyProgressRepository _vocabularyProgressRepository;
        private readonly IVocaboluryRepository _vocaboluryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;

        public VocabularyProgressService(
            IVocabularyProgressRepository vocabularyProgressRepository, 
            IVocaboluryRepository vocaboluryRepository, 
            IUserRepository userRepository,
            IMapper mapper,
            IRabbitMqPublisher rabbitMqPublisher
        )
        {
            _vocabularyProgressRepository = vocabularyProgressRepository;
            _vocaboluryRepository = vocaboluryRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        public async Task<ApiResponse<object?>> CreateVocabularyProgressAsync(CreateVocabularyProgressRequest req)
        {
            User? user = await _userRepository.GetUserByIdAndIsDeleteFalse(req.UserId);
            Vocabolury? vocabolury = await _vocaboluryRepository.GetVocaboluryByIdAsync(req.VocabularyId);
            if (user == null || user.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.UserNotFound);
            }
            if (vocabolury == null || vocabolury.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.VocaboluryNotFound);
            }
            VocabularyProgress? existingProgress = await _vocabularyProgressRepository.GetVocabularyProgressByUserIdAndVocabularyIdAsync(req.UserId, req.VocabularyId);
            if (existingProgress != null)
            {
                throw new BusinessException(ErrorRecord.VocabularyProgressAlreadyExists);
            }
            VocabularyProgress newVocabularyProgress = _mapper.Map<VocabularyProgress>(req);
            bool isCreated = await _vocabularyProgressRepository.AddVocabularyProgressAsync(newVocabularyProgress);
            if (!isCreated)
            {
                throw new BusinessException(ErrorRecord.CreateVocabularyProgressFailed);
            }
            VocabularyProgressResponse response = _mapper.Map<VocabularyProgressResponse>(newVocabularyProgress);
            return ApiResponse<object?>.MessageResponse(
                MessageRecord.CreateVocabularyProgressSuccess,
                response
            );
        }

        public async Task<ApiResponse<object?>> GetDueVocabulariesAsync(Guid userId)
        {
            User? user = await _userRepository.GetUserByIdAndIsDeleteFalse(userId);
            if (user == null || user.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.UserNotFound);
            }
            List<VocabularyProgress> dueVocabularies = await _vocabularyProgressRepository.GetDueVocabulariesAsync(userId);
            if (dueVocabularies == null || dueVocabularies.Count == 0)
            {
                throw new BusinessException(ErrorRecord.NoData);
            }
            List<VocaboluryResponse> responses = new List<VocaboluryResponse>();
            foreach (var progress in dueVocabularies)
            {
                VocaboluryResponse vocaboluryResponse = _mapper.Map<VocaboluryResponse>(progress.Vocabulary);
                responses.Add(vocaboluryResponse);
            }
            return ApiResponse<object?>.MessageResponse(
                MessageRecord.GetDueVocabulariesSuccess,
                responses
            );
        }

        public async Task<ApiResponse<object?>> SubmitReviewAsync(SubmitReviewRequest req)
        {
            User? user = await _userRepository.GetUserByIdAndIsDeleteFalse(req.UserId);
            if (user == null)
            {
                throw new BusinessException(ErrorRecord.UserNotFound);
            }
            Vocabolury? vocabulary = await _vocaboluryRepository.GetVocaboluryByIdAsync(req.VocabularyId);
            if (vocabulary == null || vocabulary.IsDeleted)
            {
                throw new BusinessException(ErrorRecord.VocaboluryNotFound);
            }
            var message = new SendSubmitReviewMessage(
                userId: req.UserId,
                vocabularyId: req.VocabularyId,
                method: req.Method,
                answer: req.Answer,
                isCorrect: req.IsCorrect
            );
            await _rabbitMqPublisher.PublishAsync(message, "submit-answer");
            if (!req.IsCorrect)
            {
                throw new BusinessException(ErrorRecord.SubmitAnswerFailed);
            }
            return ApiResponse<object?>.MessageResponse(MessageRecord.SubmitAnswerSuccess);
        }
    }
}
