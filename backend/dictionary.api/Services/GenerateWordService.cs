using Dictionary.Data.Messages;
using MassTransit;

namespace Dictionary.Api.Services;

public class GenerateWordService(
  IBus bus,
  ILogger<GenerateWordService> logger
)
{
  public async Task GenerateWordAsync(string word)
  {
    logger.LogInformation("Publishing word generation request for {Word}", word);
    
    var message = new WordGenerateRequest
    {
      Word = word,
      RequestedAt = DateTime.UtcNow
    };
    
    await bus.Publish(message);
    logger.LogInformation("Published word generation request for {Word}", word);
  }
}
