using AutoMapper;
using Mango.MessageBus;
using Microsoft.EntityFrameworkCore;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Extentions;
using Mango.Services.OrderAPI.Mapping;
using Mango.Services.OrderAPI.Utilities;
using Mango.Services.OrderAPI.Services;
using Stripe;
using Azure.Identity;

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
    options.UseSqlServer(builder.Configuration.GetValue<string>("OrdersConnectionString"));
});

IMapper mapper = MappingConfig.Create().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddSingleton<IMessageBus>(provider =>
{
    string connectionString = builder.Configuration.GetValue<string>("ServiceBus:ConnectionString")!;
    return new MessageBus(connectionString);
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthenticationHandler>();

builder.Services.AddHttpClient<IProductService, Mango.Services.OrderAPI.Services.ProductService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServicesUrl:ProductAPI"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

StripeConfiguration.ApiKey = builder.Configuration.GetValue<string>("StripeAPIKey");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddAppAuthorization();

builder.AddAppAuthentication();

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
