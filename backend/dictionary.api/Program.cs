using System.Text.Json;
using System.Text.Json.Serialization;
using Dictionary.Api.Models;
using Dictionary.Api.Services;
using Dictionary.Data.Context;
using Dictionary.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder
  .Services.AddControllers()
  .AddJsonOptions(options =>
  {
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
  });

builder.Services.AddDbContext<AppDbContext>(options =>
{
  options.UseSqlite(builder.Configuration.GetConnectionString("Database"));
});
builder.Services.AddScoped<DictionaryService>();
builder.Services.AddScoped<GenerateWordService>();
builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
  var config = sp.GetRequiredService<IOptions<RabbitMqConfig>>().Value;
  var logger = sp.GetRequiredService<ILogger<ConnectionFactory>>();
  logger.LogInformation(
    "Creating RabbitMQ connection factory: {HostName}:{Port}",
    config.Host,
    config.Port
  );
  return new ConnectionFactory()
  {
    HostName = config.Host,
    Port = config.Port,
    UserName = config.UserName,
    Password = config.Password,
  };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
  var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  await dbContext.Database.EnsureCreatedAsync();
  await dbContext.Database.MigrateAsync();
}

app.Run();
