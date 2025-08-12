using Dictionary.Consumer;
using Dictionary.Consumer.Consumers;
using Dictionary.Data.Context;
using Dictionary.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

var host = Host.CreateApplicationBuilder();
Console.WriteLine(host.Environment.EnvironmentName);

host.Logging.AddConsole();
host.Logging.SetMinimumLevel(LogLevel.Information);

host.Services.AddLogging();
host.Services.AddDbContext<AppDbContext>(options =>
  options.UseSqlite(host.Configuration.GetConnectionString("Database"))
);
host.Services.AddScoped<DictionaryService>();

host.Services.AddScoped<IConnectionFactory>(serviceProvider =>
{
  var configuration = serviceProvider.GetRequiredService<IConfiguration>();
  return new ConnectionFactory()
  {
    HostName = configuration["RabbitMQ:Host"],
    Port = int.Parse(configuration["RabbitMQ:Port"]),
    UserName = configuration["RabbitMQ:Username"],
    Password = configuration["RabbitMQ:Password"],
  };
});
host.Services.AddHostedService<AppHostedService>();

var app = host.Build();

await app.RunAsync();
