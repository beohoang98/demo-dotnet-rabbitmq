using System.ComponentModel.DataAnnotations;

namespace Dictionary.Data.Models;

public class DictionaryWordRequest
{
  [MinLength(1)]
  public string Word { get; set; } = string.Empty;
}
