using Mango.MessageBus;
using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetValue<string>("IdentityConnectionString"));
});

builder.Services.Configure((JwtOptions w) =>
{
    w.Secret = builder.Configuration.GetValue<string>("Jwt-Secret")!;
    w.Issuer = builder.Configuration.GetValue<string>("Jwt-Issuer")!;
    w.Audience = builder.Configuration.GetValue<string>("Jwt-Audience")!;
});
builder.Services.AddIdentity<ApplicationUser,IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddSingleton<IMessageBus>(provider =>
{
    string connectionString = builder.Configuration.GetValue<string>("ServiceBus-ConnectionString")!;
    return new MessageBus(connectionString);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
