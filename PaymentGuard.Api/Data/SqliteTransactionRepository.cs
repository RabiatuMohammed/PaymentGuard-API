using Microsoft.EntityFrameworkCore;
using PaymentGuard.Core.Models;

namespace PaymentGuard.Api.Data;

public sealed class SqliteTransactionRepository(PaymentGuardDbContext database)
    : ITransactionRepository
{
    public async Task SaveAsync(TransactionRecord record, CancellationToken cancellationToken = default)
    {
        await database.Transactions.AddAsync(ToEntity(record), cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
    }

    public async Task<TransactionRecord?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await database.Transactions.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity is null ? null : ToRecord(entity);
    }

    public async Task<IReadOnlyList<TransactionRecord>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await database.Transactions.AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return rows.Select(ToRecord).ToArray();
    }

    private static TransactionEntity ToEntity(TransactionRecord record) => new()
    {
        Id = record.Id,
        AmountCents = checked((long)(record.Amount * 100m)),
        Currency = record.Currency,
        MerchantReference = record.MerchantReference,
        Decision = record.Decision,
        ReasonCode = record.ReasonCode,
        CreatedAtUtc = record.CreatedAtUtc.UtcDateTime
    };

    private static TransactionRecord ToRecord(TransactionEntity entity) => new(
        entity.Id,
        entity.AmountCents / 100m,
        entity.Currency,
        entity.MerchantReference,
        entity.Decision,
        entity.ReasonCode,
        new DateTimeOffset(DateTime.SpecifyKind(entity.CreatedAtUtc, DateTimeKind.Utc)));
}
