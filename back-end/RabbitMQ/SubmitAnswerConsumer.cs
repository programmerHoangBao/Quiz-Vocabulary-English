using back_end.Configurations.Settings;
using back_end.Enums;
using back_end.Exceptions;
using back_end.Models;
using back_end.Records;
using back_end.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace back_end.RabbitMQ
{
    public class SubmitAnswerConsumer : BackgroundService
    {
        private readonly ILogger<SubmitAnswerConsumer> _logger;
        private readonly RabbitMQSetting _rabbitMqSetting;
        private readonly IServiceScopeFactory _scopeFactory;

        public SubmitAnswerConsumer(
            ILogger<SubmitAnswerConsumer> logger,
            IOptions<RabbitMQSetting> options,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _rabbitMqSetting = options.Value;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _rabbitMqSetting.HostName,
                    Port = _rabbitMqSetting.Port,
                    UserName = _rabbitMqSetting.UserName,
                    Password = _rabbitMqSetting.Password
                };

                await using var connection =
                    await factory.CreateConnectionAsync(stoppingToken);

                await using var channel =
                    await connection.CreateChannelAsync(
                        cancellationToken: stoppingToken);

                await channel.QueueDeclareAsync(
                    queue: "submit-answer",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: stoppingToken
                );

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (sender, args) =>
                {
                    try
                    {
                        var json = Encoding.UTF8.GetString(
                            args.Body.ToArray());

                        var message =
                            JsonSerializer.Deserialize<SendSubmitReviewMessage>(
                                json);

                        if (message == null)
                        {
                            _logger.LogWarning(
                                "SendSubmitReviewMessage is NULL.");

                            await channel.BasicNackAsync(
                                args.DeliveryTag,
                                multiple: false,
                                requeue: false);

                            return;
                        }


                        using var scope = _scopeFactory.CreateScope();

                        var repository =
                            scope.ServiceProvider
                                .GetRequiredService<
                                    IVocabularyProgressRepository>();

                        var existingVP =
                            await repository
                                .GetVocabularyProgressByUserIdAndVocabularyIdAsync(
                                    message.userId,
                                    message.vocabularyId);


                        var earnedScore = message.isCorrect
                            ? message.method.GetScore()
                            : 0;

                        if (existingVP == null)
                        {
                            await CreateVocabularyProgressAsync(
                                repository,
                                message.userId,
                                message.vocabularyId,
                                earnedScore);
                        }
                        else
                        {
                            int reduce = 0;
                            if (existingVP.Status == VocabularyStatus.Learning)
                                reduce = 2; 
                            else if (existingVP.Status == VocabularyStatus.Learned)
                                reduce = 20;
                            else
                                reduce = 50;
                                await UpdateVocabularyProgressAsync(
                                    repository,
                                    existingVP,
                                    earnedScore - reduce);
                        }

                        await channel.BasicAckAsync(
                            deliveryTag: args.DeliveryTag,
                            multiple: false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Error while processing submit-answer message.");

                        // Requeue message
                        await channel.BasicNackAsync(
                            deliveryTag: args.DeliveryTag,
                            multiple: false,
                            requeue: true);
                    }
                };

                await channel.BasicConsumeAsync(
                    queue: "submit-answer",
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: stoppingToken
                );

                // Keep BackgroundService alive
                await Task.Delay(
                    Timeout.Infinite,
                    stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation(
                    "SubmitAnswerConsumer is stopping.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error in SubmitAnswerConsumer.");
            }
        }

        private async Task CreateVocabularyProgressAsync(
            IVocabularyProgressRepository repository,
            Guid userId,
            Guid vocabularyId,
            int score)
        {
            var newVP = new VocabularyProgress
            {
                UserId = userId,
                VocabularyId = vocabularyId,

                NextReviewAt = DateTime.UtcNow.AddHours(24),

                ReviewCount = 1,

                Score = score,

                CorrectCount = score > 0
                    ? 1
                    : 0,

                IncorrectCount = score <= 0
                    ? 1
                    : 0,

                Status = VocabularyStatus.Learning,

                IsDeleted = false
            };

            var isCreated =
                await repository.AddVocabularyProgressAsync(newVP);

            if (!isCreated)
            {
                _logger.LogError(
                    "Failed to create VocabularyProgress " +
                    "for UserId: {UserId}, VocabularyId: {VocabularyId}",
                    userId,
                    vocabularyId);
            }
        }

        private async Task UpdateVocabularyProgressAsync(
            IVocabularyProgressRepository repository,
            VocabularyProgress existingVP,
            int score)
        {
            existingVP.ReviewCount += 1;

            existingVP.Score += score;

            existingVP.CorrectCount += score > 0
                ? 1
                : 0;

            existingVP.IncorrectCount += score <= 0
                ? 1
                : 0;

            existingVP.NextReviewAt =
                CalculateNextReviewAt(existingVP.Score);

            existingVP.Status =
                CalculateStatus(existingVP.Score);

            var isUpdated =
                await repository
                    .UpdateVocabularyProgressAsync(existingVP);

            if (!isUpdated)
            {
                _logger.LogError(
                    "Failed to update VocabularyProgress " +
                    "for UserId: {UserId}, VocabularyId: {VocabularyId}",
                    existingVP.UserId,
                    existingVP.VocabularyId);
            }
        }

        private DateTime CalculateNextReviewAt(int score)
        {
            var now = DateTime.UtcNow;

            if (score <= 0)
                return now.AddHours(12);

            if (score < 100)
                return now.AddHours(24);

            if (score < 150)
                return now.AddHours(48);

            if (score < 200)
                return now.AddHours(72);

            return now.AddHours(96);
        }

        private VocabularyStatus CalculateStatus(int score)
        {
            if (score >= 200)
                return VocabularyStatus.Mastered;

            if (score >= 100)
                return VocabularyStatus.Learned;

            return VocabularyStatus.Learning;
        }
    }
}