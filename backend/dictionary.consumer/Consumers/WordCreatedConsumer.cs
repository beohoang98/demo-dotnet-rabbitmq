using Dictionary.Data.Models;
using Dictionary.Data.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Dictionary.Consumer.Consumers;

public class WordCreatedConsumer(
  DictionaryService dictionaryService,
  ILogger<WordCreatedConsumer> logger
) : IConsumer<DictionaryWord>
{
  public async Task Consume(ConsumeContext<DictionaryWord> context)
  {
    try
    {
      var json = context.Message;
      logger.LogInformation("Deserialized message to DictionaryWord: {Word}", json.Word);
      await dictionaryService.AddWordAsync(json);
      logger.LogInformation(
        "Added new word to dictionary: {Word} {Definitions}",
        json.Word,
        json.Definitions
      );
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error processing message: {Message}", ex.Message);
      throw;
    }
  }
}
