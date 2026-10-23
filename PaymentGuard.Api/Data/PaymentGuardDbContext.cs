using Microsoft.EntityFrameworkCore;

namespace PaymentGuard.Api.Data;

public sealed class PaymentGuardDbContext(DbContextOptions<PaymentGuardDbContext> options)
    : DbContext(options)
{
    public DbSet<TransactionEntity> Transactions => Set<TransactionEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<TransactionEntity>();
        entity.ToTable("Transactions");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Currency).IsRequired().HasMaxLength(3);
        entity.Property(x => x.MerchantReference).IsRequired().HasMaxLength(40);
        entity.Property(x => x.Decision).IsRequired().HasMaxLength(20);
        entity.Property(x => x.ReasonCode).IsRequired().HasMaxLength(40);
        entity.HasIndex(x => x.CreatedAtUtc);
    }
}
