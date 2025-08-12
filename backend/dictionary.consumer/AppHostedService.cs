using System.ComponentModel.Design;
using Dictionary.Consumer.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Dictionary.Consumer;

public class AppHostedService : IHostedService
{
  private readonly IServiceScopeFactory _serviceScopeFactory;
  private readonly ILogger<AppHostedService> _logger;

  public AppHostedService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<AppHostedService> logger
  )
  {
    _serviceScopeFactory = serviceScopeFactory;
    _logger = logger;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Starting the consumer service...");
    using var scope = _serviceScopeFactory.CreateScope();
    var connectionFactory = scope.ServiceProvider.GetRequiredService<IConnectionFactory>();
    var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
    var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

    var consumer = new WordCreatedConsumer(channel, _serviceScopeFactory);

    await channel.QueueDeclareAsync("dictionary-word", cancellationToken: cancellationToken);
    await channel.BasicConsumeAsync("dictionary-word", false, consumer, cancellationToken);
    _logger.LogInformation("Consumer started and listening to 'dictionary-word' queue.");
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Stopping the consumer service...");
    return Task.CompletedTask;
  }
}
