using Dictionary.Data.Context;
using Dictionary.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.Data.Services;

public class DictionaryService
{
  private readonly AppDbContext _dbContext;

  public DictionaryService(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<DictionaryWord?> GetWordAsync(string word)
  {
    return await _dbContext.DictionaryWords.AsNoTracking().FirstOrDefaultAsync(w => w.Word == word);
  }

  public async Task<DictionaryWord> AddWordAsync(DictionaryWord word)
  {
    if (string.IsNullOrWhiteSpace(word.Word))
    {
      throw new ArgumentException("Word cannot be empty", nameof(word));
    }

    var existingWord = await GetWordAsync(word.Word);
    if (existingWord != null)
    {
      existingWord.Definitions.Clear();
      word.Definitions.ToList().ForEach(d => existingWord.Definitions.Add(d));

      _dbContext.DictionaryWords.Update(existingWord);
      await _dbContext.SaveChangesAsync();
      return existingWord;
    }

    await _dbContext.DictionaryWords.AddAsync(word);
    await _dbContext.SaveChangesAsync();
    return word;
  }

  public async Task<DictionaryWord> AddWordDefinitionAsync(
    string word,
    DictionaryWordDefinition definition
  )
  {
    var dictionaryWord = await GetWordAsync(word);
    if (dictionaryWord is null)
    {
      dictionaryWord = new DictionaryWord
      {
        Word = word,
        Definitions = new List<DictionaryWordDefinition>(),
      };
      dictionaryWord.Definitions.Add(definition);
      await _dbContext.DictionaryWords.AddAsync(dictionaryWord);
      await _dbContext.SaveChangesAsync();
      return dictionaryWord;
    }

    dictionaryWord.Definitions.Add(definition);
    await _dbContext.SaveChangesAsync();

    return dictionaryWord;
  }

  public async Task<IEnumerable<DictionaryWord>> GetAllWordsAsync()
  {
    return await _dbContext
      .DictionaryWords.Include(w => w.Definitions)
      .AsNoTracking()
      .ToListAsync();
  }
}
