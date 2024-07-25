using AutoMapper;
using Mango.MessageBus;
using Mango.Services.CouponAPI.Mapping;
using Mango.Services.ShoppnigCartAPI.Data;
using Mango.Services.ShoppnigCartAPI.Extentions;
using Mango.Services.ShoppnigCartAPI.Services;
using Mango.Services.ShoppnigCartAPI.Utilities;
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
    options.UseSqlServer(builder.Configuration.GetValue<String>("ShoppingCartConnectionString"));
});

IMapper mapper = MappingConfig.Create().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddSingleton<IMessageBus>(provider => 
{
    string connectionString = builder.Configuration.GetValue<string>("ServiceBus-ConnectionString")!;
    return new MessageBus(connectionString);
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthenticationHandler>();

builder.Services.AddHttpClient<IProductService, ProductService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServicesUrl:ProductAPI"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


builder.Services.AddHttpClient<ICouponService, CouponService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServicesUrl:CouponAPI"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<BackendApiAuthenticationHandler>();


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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
