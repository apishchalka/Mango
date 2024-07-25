using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dtos;
using Mango.Services.EmailAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class SendCartServiceBusConsumer : IHostedService
    {
        private IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IEmailService _emailService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ServiceBusProcessor _serviceBusProcessor;
        public SendCartServiceBusConsumer(IConfiguration configuration, ILogger<SendCartServiceBusConsumer> logger, IEmailService emailService, IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
            _scopeFactory = scopeFactory;
            
            var connectionString = configuration.GetValue<string>("ServiceBus-ConnectionString");
            var client = new ServiceBusClient(connectionString);
            _serviceBusProcessor = client.CreateProcessor(_configuration.GetValue<string>("ServiceBus-ShoppingCartQueue"));
        }

        private async Task ServiceBusProcessorProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogError(arg.Exception, "Error processing a ServiceBus shopping cart message.");
            await Task.CompletedTask;
        }

        private async Task ServiceBusProcessorProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            try
            {
                var serilaizedMessage = Encoding.UTF8.GetString(arg.Message.Body);
                var cart = JsonConvert.DeserializeObject<ShoppingCartDto>(serilaizedMessage);

                await _emailService.SendCart(cart);

                EmailLogger logger = new()
                {
                    Email = cart.Header.Email!,
                    Name = "Email cart request",
                    Message = $"User requested a card by email. Cart total is {cart.Header.CartTotal.ToString("c")} ",
                    DateTime = DateTime.UtcNow
                };

                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    dbContext.Add(logger);
                    await dbContext.SaveChangesAsync();
                }
                await arg.CompleteMessageAsync(arg.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending a shopping cart.");
                await arg.AbandonMessageAsync(arg.Message);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _serviceBusProcessor.ProcessMessageAsync += ServiceBusProcessorProcessMessageAsync;
            _serviceBusProcessor.ProcessErrorAsync += ServiceBusProcessorProcessErrorAsync;
            await _serviceBusProcessor.StartProcessingAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _serviceBusProcessor.StopProcessingAsync(cancellationToken);
            await _serviceBusProcessor.DisposeAsync();
        }
    }
}
