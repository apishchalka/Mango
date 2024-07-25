using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Dto;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class UserRegisteredServiceBusConsumer : IHostedService
    {
        private IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IEmailService _emailService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ServiceBusProcessor _serviceBusProcessor;
        public UserRegisteredServiceBusConsumer(IConfiguration configuration, ILogger<UserRegisteredServiceBusConsumer> logger, IEmailService emailService, IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
            _scopeFactory = scopeFactory;
            
            var connectionString = configuration.GetValue<string>("ServiceBus-ConnectionString");
            var client = new ServiceBusClient(connectionString);
            _serviceBusProcessor = client.CreateProcessor(_configuration.GetValue<string>("ServiceBus-UserRegisteredQueue"));
        }

        private async Task ServiceBusProcessorProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogError(arg.Exception, "Error processing a ServiceBus user registration message.");
            await Task.CompletedTask;
        }

        private async Task ServiceBusProcessorProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            try
            {
                var serilaizedMessage = Encoding.UTF8.GetString(arg.Message.Body);
                var user = JsonConvert.DeserializeObject<UserRegisterRequestDTO>(serilaizedMessage);

                await _emailService.SendRegistrationEmail(user);

                EmailLogger logger = new()
                {
                    Email = user.Email!,
                    Name = "User registration",
                    Message = $"New user with email {user.Email} registered.",
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
                _logger.LogError(ex, $"Error sending a new registration email.");
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
