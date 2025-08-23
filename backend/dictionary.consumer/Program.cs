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
host.Services.AddMassTransit(
  (x) =>
  {
    x.UsingRabbitMq(
      (context, cfg) =>
      {
        cfg.Host(
          $"rabbitmq://{host.Configuration["RabbitMQ:Host"]}:{host.Configuration["RabbitMQ:Port"]}",
          h =>
          {
            h.Username(host.Configuration["RabbitMQ:Username"]);
            h.Password(host.Configuration["RabbitMQ:Password"]);
          }
        );
        cfg.SerializerContentType = new ContentType("application/json");
        cfg.ReceiveEndpoint(
          "dictionary-word",
          e =>
          {
            e.ConfigureConsumer<WordCreatedConsumer>(context);
          }
        );
        cfg.ClearSerialization();
        cfg.UseRawJsonDeserializer();
        cfg.UseRawJsonSerializer();
      }
    );
    x.AddConsumer<WordCreatedConsumer>();
  }
);

var app = host.Build();
await app.RunAsync();
