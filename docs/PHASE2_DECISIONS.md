# Engineering decisions

1. **SQLite via EF Core:** local file persistence without an external database server.
2. **AmountCents:** store amounts as 64-bit integer cents after validating no more than two decimal places.
3. **UTC timestamps:** store UTC `DateTime` in SQLite, surface `DateTimeOffset` in API records.
4. **Repository interface:** isolate persistence from validation and evaluation logic.
5. **`EnsureCreated`:** appropriate only to initialise a new local demo; prefer versioned EF Core migrations in real applications.
6. **Test pooling disabled:** `Pooling=false` only on temporary SQLite test connections to permit reliable deletion on Windows.
7. **Cancellation tokens:** test code passes `TestContext.Current.CancellationToken` to asynchronous operations.
8. **No external services:** no banking API, user PII, credentials or payment processing.
9. **No HTTP integration-test suite yet:** xUnit exercises the domain logic and SQLite persistence, not end-to-end HTTP hosting.
10. **Explicit constraints:** EUR-only, max EUR 100,000, synthetic references only, deterministic EUR 1,000 review rule.
