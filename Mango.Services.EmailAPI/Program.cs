using Azure.Identity;
using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Messaging;
using Mango.Services.EmailAPI.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Retrieve KeyVaultName from configuration
var keyVaultName = builder.Configuration["KeyVaultName"];
var keyVaultUri = $"https://{keyVaultName}.vault.azure.net/";

// Configure to use Azure Key Vault with DefaultAzureCredential
builder.Configuration.AddAzureKeyVault(
    new Uri(keyVaultUri),
    new DefaultAzureCredential()
);

// Load environment-specific appsettings.{Environment}.json files.
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetValue<string>("EmailConnectionString");
    options.UseSqlServer(connectionString);
});

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Host.UseSerilog();

builder.Services.AddSingleton<IRazorTemplateService, RazorTemplateService>();

builder.Services.AddSingleton<IEmailService>(provider =>
{
    string connectionString = builder.Configuration.GetValue<string>("EmailConnectionString")!;
    string from = builder.Configuration.GetValue<string>("Email-From")!;
    return new EmailService(connectionString, from, provider.GetRequiredService<ILogger<EmailService>>(), provider.GetRequiredService<IRazorTemplateService>());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register ServiceBusConsumer
builder.Services.AddHostedService<SendCartServiceBusConsumer>();
builder.Services.AddHostedService<UserRegisteredServiceBusConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
