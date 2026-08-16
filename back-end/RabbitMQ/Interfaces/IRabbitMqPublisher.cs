namespace back_end.RabbitMQ.Interfaces
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync<T>(T message, string queueName);
    }
}
