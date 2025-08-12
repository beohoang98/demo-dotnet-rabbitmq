using Dictionary.Api.Services;
using Dictionary.Data.Models;
using Dictionary.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dictionary.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WordController : ControllerBase
{
  private readonly DictionaryService _dictionaryService;
  private readonly GenerateWordService _generateWordService;

  public WordController(
    DictionaryService dictionaryService,
    GenerateWordService generateWordService
  )
  {
    _dictionaryService = dictionaryService;
    _generateWordService = generateWordService;
  }

  [HttpGet("")]
  public async Task<IActionResult> GetAllWords()
  {
    var result = await _dictionaryService.GetAllWordsAsync();
    return Ok(result);
  }

  [HttpGet("{word}")]
  public async Task<IActionResult> GetWord(string word)
  {
    var result = await _dictionaryService.GetWordAsync(word);
    if (result == null)
      return NotFound();
    return Ok(result);
  }

  [HttpPost("generate")]
  public async Task<IActionResult> GenerateWord([FromForm] string word)
  {
    if (string.IsNullOrWhiteSpace(word))
      return BadRequest();

    // add skeleton word
    await _dictionaryService.AddWordAsync(new DictionaryWord { Word = word });

    await _generateWordService.GenerateWordAsync(word);
    return Accepted();
  }
}
