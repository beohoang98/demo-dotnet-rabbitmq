namespace Dictionary.Data.Models;

public class DictionaryWord
{
  public int Id { get; set; }
  public string Word { get; set; } = string.Empty;

  public ICollection<DictionaryWordDefinition> Definitions { get; set; } = [];
}

public class DictionaryWordDefinition
{
  public int Id { get; set; }
  public int WordId { get; set; }

  // Example: noun, verb, adjective, adverb, ...
  public string PartOfSpeech { get; set; } = string.Empty;
  public string Ipa { get; set; } = string.Empty; // International Phonetic Alphabet representation
  public string Definition { get; set; } = string.Empty;

  public string CerfLevel { get; set; } = string.Empty;

  public List<string> Synonyms { get; set; } = new();
  public List<string> Antonyms { get; set; } = new();

  public DictionaryWord Word { get; set; } = null!;
}
