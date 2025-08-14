using System.Text.Json;
using System.Text.Json.Serialization;
using Dictionary.Api.Models;
using Dictionary.Api.Services;
using Dictionary.Data.Context;
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

// Configure MassTransit with RabbitMQ
builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddMassTransit(x =>
{
  x.UsingRabbitMq((context, cfg) =>
  {
    var rabbitMqConfig = context.GetRequiredService<IOptions<RabbitMqConfig>>().Value;
    cfg.Host(rabbitMqConfig.Host, (ushort)rabbitMqConfig.Port, "/", h =>
    {
      h.Username(rabbitMqConfig.UserName);
      h.Password(rabbitMqConfig.Password);
    });
    
    cfg.ConfigureEndpoints(context);
  });
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
