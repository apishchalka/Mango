using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Extentions;
using Mango.Services.CouponAPI.Mapping;
using Microsoft.EntityFrameworkCore;
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
    options.UseSqlServer(builder.Configuration.GetValue<string>("CouponConnectionString"));
});

IMapper mapper = MappingConfig.Create().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

StripeConfiguration.ApiKey = builder.Configuration.GetValue<string>("StripeAPIKey");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
