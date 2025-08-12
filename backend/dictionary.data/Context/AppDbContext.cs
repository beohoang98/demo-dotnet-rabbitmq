namespace Dictionary.Data.Context;

using Dictionary.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

[DbContext(typeof(AppDbContext))]
public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options) { }

  public DbSet<DictionaryWord> DictionaryWords { get; set; } = null!;
  public DbSet<DictionaryWordDefinition> DictionaryWordDefinitions { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder
      .Entity<DictionaryWord>()
      .HasMany(dw => dw.Definitions)
      .WithOne(d => d.Word)
      .HasForeignKey(d => d.WordId);
    modelBuilder
      .Entity<DictionaryWordDefinition>()
      .HasOne(d => d.Word)
      .WithMany(w => w.Definitions)
      .HasForeignKey(d => d.WordId);

    modelBuilder
      .Entity<DictionaryWordDefinition>()
      .Property(d => d.CerfLevel)
      .HasDefaultValue("A1")
      .HasMaxLength(50);
    modelBuilder
      .Entity<DictionaryWordDefinition>()
      .Property(d => d.Ipa)
      .HasDefaultValue(string.Empty)
      .HasMaxLength(255);

    modelBuilder
      .Entity<DictionaryWordDefinition>()
      .Property(d => d.Synonyms)
      .HasDefaultValueSql("''")
      .HasConversion(
        v => string.Join(',', v),
        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
      );

    modelBuilder
      .Entity<DictionaryWordDefinition>()
      .Property(d => d.Antonyms)
      .HasDefaultValueSql("''")
      .HasConversion(
        v => string.Join(',', v),
        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
      );
  }
}
