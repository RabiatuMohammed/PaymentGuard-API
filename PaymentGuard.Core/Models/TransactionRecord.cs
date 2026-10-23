namespace PaymentGuard.Core.Models;

// Illustrative decisions, not real fraud or credit decisions.
public sealed record TransactionRecord(
    Guid Id,
    decimal Amount,
    string Currency,
    string MerchantReference,
    string Decision,
    string ReasonCode,
    DateTimeOffset CreatedAtUtc);
