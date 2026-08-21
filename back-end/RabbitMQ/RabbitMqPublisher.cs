using back_end.Configurations.Settings;
using back_end.RabbitMQ.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text.Json;

namespace back_end.RabbitMQ
{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly RabbitMQSetting _rabbitMqSetting;
        private readonly ILogger<RabbitMqPublisher> _logger;

        public RabbitMqPublisher(IOptions<RabbitMQSetting> options, ILogger<RabbitMqPublisher> logger)
        {
            _rabbitMqSetting = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task PublishAsync<T>(T message, string queueName)
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

                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                var json = JsonSerializer.Serialize(message);
                var body = System.Text.Encoding.UTF8.GetBytes(json);

                await channel.BasicPublishAsync<BasicProperties>(
                    exchange: "",
                    routingKey: queueName,
                    mandatory: false,
                    basicProperties: new BasicProperties(),
                    body: body
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing message to RabbitMQ");
                throw;
            }
        }
    }
}
