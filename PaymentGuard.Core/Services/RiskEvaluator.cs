using PaymentGuard.Core.Models;

namespace PaymentGuard.Core.Services;

public static class RiskEvaluator
{
    // Deliberately transparent educational rule, not a bank's risk model.
    public const decimal ReviewThresholdEur = 1_000m;

    public static RiskAssessment Evaluate(decimal amountEur) =>
        amountEur >= ReviewThresholdEur
            ? new RiskAssessment("REVIEW", "HIGH_AMOUNT")
            : new RiskAssessment("APPROVED", "BELOW_REVIEW_THRESHOLD");
}
