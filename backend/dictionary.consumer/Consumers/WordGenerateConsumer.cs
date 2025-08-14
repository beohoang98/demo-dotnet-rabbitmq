using Dictionary.Data.Messages;
using Dictionary.Data.Models;
using Dictionary.Data.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Dictionary.Consumer.Consumers;

public class WordGenerateConsumer : IConsumer<WordGenerateRequest>
{
  private readonly DictionaryService _dictionaryService;
  private readonly ILogger<WordGenerateConsumer> _logger;

  public WordGenerateConsumer(DictionaryService dictionaryService, ILogger<WordGenerateConsumer> logger)
  {
    _dictionaryService = dictionaryService;
    _logger = logger;
  }

  public async Task Consume(ConsumeContext<WordGenerateRequest> context)
  {
    var message = context.Message;
    _logger.LogInformation("Processing word generation request for: {Word} (requested at {RequestedAt})", 
      message.Word, message.RequestedAt);

    try
    {
      // Create a basic dictionary word entry
      var dictionaryWord = new DictionaryWord
      {
        Word = message.Word
      };

      await _dictionaryService.AddWordAsync(dictionaryWord);
      
      _logger.LogInformation("Successfully processed word generation request for: {Word}", message.Word);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error processing word generation request for: {Word}", message.Word);
      throw; // This will cause MassTransit to retry or move to error queue
    }
  }
}
