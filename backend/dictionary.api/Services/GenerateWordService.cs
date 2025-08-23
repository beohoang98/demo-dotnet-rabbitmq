using Dictionary.Data.Models;
using MassTransit;

namespace Dictionary.Api.Services;

public class GenerateWordService(ISendEndpointProvider client, ILogger<GenerateWordService> logger)
{
  private async Task PublishWordAsync(string word)
  {
    logger.LogInformation("Publishing word to RabbitMQ {Word}", word);
    var wordRequest = new DictionaryWordRequest { Word = word };
    var endpoint = await client.GetSendEndpoint(new Uri("queue:words"));
    await endpoint.Send(wordRequest);
  }

  public async Task GenerateWordAsync(string word)
  {
    await PublishWordAsync(word);
  }
}
