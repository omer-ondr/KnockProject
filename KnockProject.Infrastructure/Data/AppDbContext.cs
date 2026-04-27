using KnockProject.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace KnockProject.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<HistoricalMemory> HistoricalMemories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PostgreSQL'e vector eklentisini kullanacağını söylüyoruz
        modelBuilder.HasPostgresExtension("vector");

        // Embedding kolonunun kaç boyutlu olacağını belirliyoruz. 
        // OpenAI'ın text-embedding-3-small modeli standart 1536 boyut döner.
        modelBuilder.Entity<HistoricalMemory>()
            .Property(h => h.Embedding)
            .HasColumnType("vector(1536)");
    }
}