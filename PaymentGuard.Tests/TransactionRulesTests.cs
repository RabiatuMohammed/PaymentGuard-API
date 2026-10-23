using PaymentGuard.Core.Models;
using PaymentGuard.Core.Services;
using Xunit;

namespace PaymentGuard.Tests;

public class TransactionRulesTests
{
    [Fact]
    public void BelowThreshold_IsApproved()
    {
        var result = RiskEvaluator.Evaluate(49.95m);
        Assert.Equal("APPROVED", result.Decision);
        Assert.Equal("BELOW_REVIEW_THRESHOLD", result.ReasonCode);
    }

    [Fact]
    public void ExactThreshold_RequiresReview()
    {
        var result = RiskEvaluator.Evaluate(1000m);
        Assert.Equal("REVIEW", result.Decision);
        Assert.Equal("HIGH_AMOUNT", result.ReasonCode);
    }

    [Fact]
    public void HigherAmount_RequiresReview()
    {
        var result = RiskEvaluator.Evaluate(1450m);
        Assert.Equal("REVIEW", result.Decision);
    }

    [Fact]
    public void ValidRequest_HasNoErrors()
    {
        var errors = TransactionValidator.Validate(new(49.95m, "eur", "DEMO-001"));
        Assert.Empty(errors);
    }

    [Fact]
    public void FractionalCent_IsRejected()
    {
        var errors = TransactionValidator.Validate(new(10.999m, "EUR", "DEMO-001"));
        Assert.True(errors.ContainsKey("amount"));
    }

    [Fact]
    public void ZeroAmount_IsRejected()
    {
        var errors = TransactionValidator.Validate(new(0m, "EUR", "DEMO-001"));
        Assert.True(errors.ContainsKey("amount"));
    }

    [Fact]
    public void UnsupportedCurrency_IsRejected()
    {
        var errors = TransactionValidator.Validate(new(10m, "USD", "DEMO-001"));
        Assert.True(errors.ContainsKey("currency"));
    }
}
