using System.ComponentModel.Design;
using System.Text;
using System.Text.Json;
using Dictionary.Data.Models;
using Dictionary.Data.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Dictionary.Consumer.Consumers;

public class WordCreatedConsumer : AsyncEventingBasicConsumer
{
  private readonly IServiceScopeFactory _serviceScopeFactory;

  public WordCreatedConsumer(IChannel channel, IServiceScopeFactory serviceScopeFactory)
    : base(channel)
  {
    _serviceScopeFactory = serviceScopeFactory;

    ReceivedAsync += async (model, ea) =>
    {
      var body = ea.Body.ToArray();
      var message = Encoding.UTF8.GetString(body);
      await Consume(message);
      await channel.BasicAckAsync(ea.DeliveryTag, false);
    };
  }

  public async Task Consume(string message)
  {
    using var scope = _serviceScopeFactory.CreateScope();
    var _dictionaryService = scope.ServiceProvider.GetRequiredService<DictionaryService>();
    var _logger = scope.ServiceProvider.GetRequiredService<ILogger<WordCreatedConsumer>>();
    try
    {
      _logger.LogInformation("Received message: {Message}", message);
      var json = JsonSerializer.Deserialize<DictionaryWord>(
        message,
        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
      );

      if (json is not null)
      {
        _logger.LogInformation("Deserialized message to DictionaryWord: {Word}", json.Word);
        await _dictionaryService.AddWordAsync(json);
        _logger.LogInformation(
          "Added new word to dictionary: {Word} {Definitions}",
          json.Word,
          json.Definitions
        );
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error processing message: {Message}", ex.Message);
      throw;
    }
  }
}
