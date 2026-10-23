# Architecture

```text
PowerShell / HTTP client
         |
         v
ASP.NET Core Minimal API  (PaymentGuard.Api/Program.cs)
         |         |              |
         |         v              v
         |    Validator      RiskEvaluator (PaymentGuard.Core)
         |         |              |
         +---------+--------------+
                   |
                   v
          TransactionRecord
                   |
                   v
       ITransactionRepository
                   |
                   v
      SqliteTransactionRepository
                   |
                   v
         EF Core DbContext
                   |
                   v
       SQLite: App_Data/paymentguard.db
```

## POST request flow

1. ASP.NET Core binds `CreateTransactionRequest` (`amount`, `currency`, `merchantReference`).
2. `TransactionValidator` checks business constraints and produces HTTP 400 with validation errors on invalid input.
3. `RiskEvaluator` calculates a purely educational threshold classification: `APPROVED` below EUR 1,000, otherwise `REVIEW`.
4. The API generates a `Guid` ID and UTC timestamp. Clients cannot set decision fields.
5. The repository maps the domain record to a storage entity, storing money as **integer cents** and timestamps as UTC `DateTime`.
6. EF Core stores the record in local SQLite. The API returns HTTP 201 and a created-resource location.

## Read paths

- `GET /api/transactions/{guid}` uses the repository to look up the stored entity and maps it back to the domain record; 404 when missing.
- `GET /api/transactions` lists records sorted by UTC creation time (not paginated, demo only).

## Tests and build

- `PaymentGuard.Checks`: twelve dependency-free validation/evaluator assertions.
- `PaymentGuard.Tests`: nine xUnit tests, including on-disk SQLite creation, disposal, reopen, retrieval and missing-ID behaviour. Test-only SQLite connection pooling disabled for Windows file cleanup.
- GitHub Actions: build, run behaviour checks and xUnit tests.

## Security / operational limits

**Not a bank-ready service:** no authentication, authorisation, rate limiting, idempotency, migration strategy, threat monitoring, or production audit trail. Run on loopback only. Use synthetic records and never deploy this demo with public write access.
