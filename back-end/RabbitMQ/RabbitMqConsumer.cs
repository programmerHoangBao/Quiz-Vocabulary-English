using back_end.Configurations.Settings;
using back_end.RabbitMQ.Interfaces;
using back_end.Records;
using back_end.Services.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace back_end.RabbitMQ
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly ILogger<RabbitMqConsumer> _logger;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;
        private readonly RabbitMQSetting _rabbitMqSetting;
        private readonly IEmailService _emailService;

        public RabbitMqConsumer(ILogger<RabbitMqConsumer> logger, IRabbitMqPublisher rabbitMqPublisher, IOptions<RabbitMQSetting> rabbitMqSetting, IEmailService emailService)
        {
            _logger = logger;
            _rabbitMqPublisher = rabbitMqPublisher;
            _rabbitMqSetting = rabbitMqSetting.Value;
            _emailService = emailService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
                var connection = await factory.CreateConnectionAsync();
                var channel = await connection.CreateChannelAsync();
                await channel.QueueDeclareAsync(queue: "send_otp_email", durable: true, exclusive: false, autoDelete: false, arguments: null);
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (sender, args) =>
                {
                    try
                    {
                        var json = Encoding.UTF8.GetString(args.Body.ToArray());

                        var message =
                            JsonSerializer.Deserialize<SendOtpMessage>(json);

                        if (message == null)
                        {
                            _logger.LogWarning("SendOtpMessage is NULL.");

                            // Invalid message -> discard message.
                            await channel.BasicNackAsync(
                                args.DeliveryTag,
                                multiple: false,
                                requeue: false);

                            return;
                        }

                        // send email
                        await _emailService.SendOtpEmailAsync(
                            message.Email,
                            message.Name,
                            message.OtpCode,
                            message.OtpExpiryMinutes,
                            message.AppName
                        );

                        // Only acknowledge after the email has been sent successfully.
                        await channel.BasicAckAsync(
                            args.DeliveryTag,
                            multiple: false);
                        _logger.LogInformation("OTP email sent successfully to {Email}.", message.Email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,"Error while sending OTP email.");
                        // Request RabbitMQ to return the message to the queue for reprocessing.
                        await channel.BasicNackAsync(
                            args.DeliveryTag,
                            multiple: false,
                            requeue: true);
                    }
                };
                await channel.BasicConsumeAsync(
                    queue: "send_otp_email",
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: stoppingToken
                );
                await Task.Delay(
                    Timeout.Infinite,
                    stoppingToken
                );
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("RabbitMqConsumer is stopping.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RabbitMqConsumer");
            }
        }
    }
}
