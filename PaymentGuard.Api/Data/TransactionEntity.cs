namespace PaymentGuard.Api.Data;

// Database-only representation. Money is stored as integer cents, not floating point.
public sealed class TransactionEntity
{
    public Guid Id { get; set; }
    public long AmountCents { get; set; }
    public string Currency { get; set; } = "EUR";
    public string MerchantReference { get; set; } = string.Empty;
    public string Decision { get; set; } = string.Empty;
    public string ReasonCode { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
