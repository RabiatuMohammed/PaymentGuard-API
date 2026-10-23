using PaymentGuard.Core.Models;
using PaymentGuard.Core.Services;

var checks = new (string Name, Action Execute)[]
{
    ("Small transaction approved", () => Equal("APPROVED", RiskEvaluator.Evaluate(49.95m).Decision)),
    ("Amount just below threshold approved", () => Equal("APPROVED", RiskEvaluator.Evaluate(999.99m).Decision)),
    ("Exact threshold flagged", () => Equal("REVIEW", RiskEvaluator.Evaluate(1_000m).Decision)),
    ("Large transaction flagged", () => Equal("HIGH_AMOUNT", RiskEvaluator.Evaluate(5_000m).ReasonCode)),
    ("Valid synthetic transaction accepted", () => NoErrors(new(79.90m, "EUR", "DEMO-ORDER-001"))),
    ("Lowercase EUR accepted", () => NoErrors(new(79.90m, "eur", "SHOP_005"))),
    ("Zero amount rejected", () => HasError(new(0m, "EUR", "SHOP-005"), "amount")),
    ("Negative amount rejected", () => HasError(new(-1m, "EUR", "SHOP-005"), "amount")),
    ("Unsupported currency rejected", () => HasError(new(10m, "USD", "SHOP-005"), "currency")),
    ("Blank merchant reference rejected", () => HasError(new(10m, "EUR", ""), "merchantReference")),
    ("Very large transaction rejected", () => HasError(new(100_000.01m, "EUR", "SHOP-005"), "amount")),
    ("Punctuation in merchant reference rejected", () => HasError(new(10m, "EUR", "X;<script>"), "merchantReference"))
};

var failures = 0;
foreach (var (name, execute) in checks)
{
    try
    {
        execute();
        Console.WriteLine($"PASS: {name}");
    }
    catch (Exception exception)
    {
        failures++;
        Console.Error.WriteLine($"FAIL: {name} ({exception.Message})");
    }
}

Console.WriteLine($"{checks.Length - failures}/{checks.Length} checks passed.");
return failures == 0 ? 0 : 1;

static void Equal(string expected, string actual)
{
    if (expected != actual)
    {
        throw new InvalidOperationException($"Expected {expected}; got {actual}.");
    }
}

static void NoErrors(CreateTransactionRequest request)
{
    if (TransactionValidator.Validate(request).Count != 0)
    {
        throw new InvalidOperationException("Unexpected validation errors.");
    }
}

static void HasError(CreateTransactionRequest request, string field)
{
    if (!TransactionValidator.Validate(request).ContainsKey(field))
    {
        throw new InvalidOperationException($"Missing error on {field}.");
    }
}
