using JournalIQ.Core;
using Microsoft.EntityFrameworkCore;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TradeTag>()
            .HasKey(tt => new { tt.TradeId, tt.TagId });

        modelBuilder.Entity<TradeTag>()
            .HasOne(tt => tt.Trade)
            .WithMany(t => t.TradeTags)
            .HasForeignKey(tt => tt.TradeId);

        modelBuilder.Entity<TradeTag>()
            .HasOne(tt => tt.Tag)
            .WithMany(t => t.TradeTags)
            .HasForeignKey(tt => tt.TagId);
    }

    public DbSet<Trade> Trades { get; set; }
    public DbSet<Tag> Tags { get; set; }
}
