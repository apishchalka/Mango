namespace Mango.MessageBus
{
    public interface IMessageBus
    {
        Task SendMessageAsync(string message, string queueName);
    }
}
