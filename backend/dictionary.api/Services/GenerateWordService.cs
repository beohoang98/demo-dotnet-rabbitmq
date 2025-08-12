using System.Text;
using RabbitMQ.Client;

namespace Dictionary.Api.Services;

public class GenerateWordService(
  IConnectionFactory connectionFactory,
  ILogger<GenerateWordService> logger
)
{
  private async Task PublishWordAsync(string word)
  {
    logger.LogInformation("Publishing word to RabbitMQ {Word}", word);
    logger.LogInformation("Send to RabbitMQ {Uri}", connectionFactory.Uri);
    using var connection = await connectionFactory.CreateConnectionAsync();
    using var channel = await connection.CreateChannelAsync();
    await channel.QueueDeclareAsync("words", durable: false, exclusive: false, autoDelete: false);
    var body = Encoding.UTF8.GetBytes(word);
    await channel.BasicPublishAsync(exchange: "", routingKey: "words", body: body);
  }

  public async Task GenerateWordAsync(string word)
  {
    await PublishWordAsync(word);
  }
}
