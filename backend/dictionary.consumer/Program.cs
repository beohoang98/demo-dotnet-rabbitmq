using System.Net.Mime;
using Dictionary.Consumer.Consumers;
using Dictionary.Data.Context;
using Dictionary.Data.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateApplicationBuilder();
Console.WriteLine(host.Environment.EnvironmentName);

host.Logging.AddConsole();
host.Logging.SetMinimumLevel(LogLevel.Information);

host.Services.AddLogging();
host.Services.AddDbContext<AppDbContext>(options =>
  options.UseSqlite(host.Configuration.GetConnectionString("Database"))
);
host.Services.AddScoped<DictionaryService>();

// Configure MassTransit with RabbitMQ
host.Services.AddMassTransit(x =>
{
  x.AddConsumer<WordGenerateConsumer>();

  x.UsingRabbitMq((context, cfg) =>
  {
    var configuration = context.GetRequiredService<IConfiguration>();
    cfg.Host($"rabbitmq://{configuration["RabbitMQ:Host"]}:{configuration["RabbitMQ:Port"]}", h =>
    {
      h.Username(configuration["RabbitMQ:Username"] ?? "guest");
      h.Password(configuration["RabbitMQ:Password"] ?? "guest");
    });

    cfg.SerializerContentType = new ContentType("application/json");
    cfg.ClearSerialization();
    cfg.UseRawJsonDeserializer();
    cfg.UseRawJsonSerializer();
    
    cfg.ConfigureEndpoints(context);
  });
});

var app = host.Build();
await app.RunAsync();
