using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dictionary.Api.Models;
using Dictionary.Api.Services;
using Dictionary.Data.Context;
using Dictionary.Data.Models;
using Dictionary.Data.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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
builder.Services.AddMassTransit(x =>
{
  var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMqConfig>();
  if (rabbitMqConfig == null)
  {
    throw new ArgumentNullException(nameof(rabbitMqConfig), "RabbitMQ configuration is missing");
  }
  var hostAddress = $"rabbitmq://{rabbitMqConfig.Host}:{rabbitMqConfig.Port}";

  x.UsingRabbitMq(
    (context, cfg) =>
    {
      cfg.Host(
        hostAddress,
        h =>
        {
          h.Username(rabbitMqConfig.UserName);
          h.Password(rabbitMqConfig.Password);
        }
      );
      cfg.SerializerContentType = new ContentType("application/json");
      cfg.ClearSerialization();
      cfg.UseRawJsonDeserializer();
      cfg.UseRawJsonSerializer();
    }
  );
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
