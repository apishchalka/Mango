using Azure.Messaging.ServiceBus;

namespace Mango.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private readonly string _connectionString;
        
        public MessageBus(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));            
        }

        public async Task SendMessageAsync(string message, string queueName)
        {
            await using (var client = new ServiceBusClient(_connectionString))
            {
                ServiceBusSender sender = client.CreateSender(queueName);
                ServiceBusMessage serviceBusMessage = new ServiceBusMessage(message)
                {
                    CorrelationId = Guid.NewGuid().ToString()
                };
                await sender.SendMessageAsync(serviceBusMessage);
            }
        }
    }
}
