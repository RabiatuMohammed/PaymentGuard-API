using PaymentGuard.Core.Models;

namespace PaymentGuard.Api.Data;

public interface ITransactionRepository
{
    Task SaveAsync(TransactionRecord record, CancellationToken cancellationToken = default);
    Task<TransactionRecord?> FindAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransactionRecord>> ListAsync(CancellationToken cancellationToken = default);
}
