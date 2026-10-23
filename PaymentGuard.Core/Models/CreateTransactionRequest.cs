namespace PaymentGuard.Core.Models;

// A small request DTO: clients cannot supply decisions, IDs or timestamps.
public sealed record CreateTransactionRequest(
    decimal Amount,
    string? Currency,
    string? MerchantReference);
