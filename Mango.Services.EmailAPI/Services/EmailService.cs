using Azure.Communication.Email;
using Mango.Services.EmailAPI.Dto;
using Mango.Services.EmailAPI.Models.Dtos;

namespace Mango.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _connectionString;
        private readonly string _fromEmail;
        private readonly ILogger<EmailService> _logger;
        private readonly IRazorTemplateService _razorTemplateService;
        private readonly EmailClient _emailClient;

        public EmailService(string connectionString, string fromEmail, ILogger<EmailService> logger, IRazorTemplateService razorTemplateService)
        {
            _connectionString = connectionString;
            _fromEmail = fromEmail;
            _logger = logger;
            _razorTemplateService = razorTemplateService;
            _emailClient = new EmailClient(connectionString);
        }

        public async Task SendCart(ShoppingCartDto cart)
        {
            try
            {
                var htmlContent = await _razorTemplateService.RenderTemplateAsync("Templates\\ShoppingCart.cshtml", cart);

                var emailMessage = new EmailMessage(_fromEmail,
                    new EmailRecipients([new EmailAddress(cart.Header.Email)]),
                    new EmailContent("Mango Shopping - Your Cart")
                    {
                        Html = htmlContent
                    });

                var response = await _emailClient.SendAsync(Azure.WaitUntil.Started, emailMessage);
                _logger.LogInformation($"Shopping cart has been sent. MessageId = {response.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send shopping cart email: {ex.Message}");
            }
        }

        public async Task SendRegistrationEmail(UserRegisterRequestDTO user)
        {

            try
            {
                var htmlContent = await _razorTemplateService.RenderTemplateAsync("Templates\\UserRegistered.cshtml", user);

                var emailMessage = new EmailMessage(_fromEmail,
                    new EmailRecipients([new EmailAddress(user.Email)]),
                    new EmailContent("Mango Shopping - Registration")
                    {
                        Html = htmlContent
                    });

                var response = await _emailClient.SendAsync(Azure.WaitUntil.Started, emailMessage);
                _logger.LogInformation($"New user registration email has been sent. MessageId = {response.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send a new user registration email: {ex.Message}");
            }
        }
    }
}
