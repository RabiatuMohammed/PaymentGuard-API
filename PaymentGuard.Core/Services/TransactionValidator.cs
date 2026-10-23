using System.Text.RegularExpressions;
using PaymentGuard.Core.Models;

namespace PaymentGuard.Core.Services;

public static partial class TransactionValidator
{
    // Restrict demonstration references to non-PII, plain identifiers.
    [GeneratedRegex("^[A-Za-z0-9][A-Za-z0-9_-]{2,39}$")]
    private static partial Regex MerchantReferencePattern();

    public static Dictionary<string, string[]> Validate(CreateTransactionRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.Amount <= 0m || request.Amount > 100_000m)
        {
            errors["amount"] = ["Amount must be greater than 0 and no more than 100000."];
        }

        // Exact cents only: reject fractions of a cent before converting to integer cents.
        if (request.Amount != decimal.Truncate(request.Amount * 100m) / 100m)
        {
            errors["amount"] = ["Amount must contain no more than two decimal places."];
        }

        // EUR-only by design: this demo performs no currency conversion.
        if (!string.Equals(request.Currency, "EUR", StringComparison.OrdinalIgnoreCase))
        {
            errors["currency"] = ["Only EUR is supported in this demonstration."];
        }

        if (string.IsNullOrWhiteSpace(request.MerchantReference)
            || !MerchantReferencePattern().IsMatch(request.MerchantReference))
        {
            errors["merchantReference"] =
                ["Use a synthetic reference of 3-40 letters, digits, hyphens or underscores; start with a letter or digit."];
        }

        return errors;
    }
}
