# Interview preparation: PaymentGuard API

## A short introduction

"I wanted to learn the C#/.NET stack relevant to the role, so I built PaymentGuard, a local training API that accepts synthetic euro transactions, validates requests and applies a transparent amount-based review rule. I used ASP.NET Core Minimal APIs, separated the domain rules from the web layer, persisted records using EF Core and SQLite, then added xUnit and GitHub Actions testing. It's deliberately not a payment processing or fraud detection system."

## Questions I can answer from actual code

**Why separate API and Core projects?** HTTP binding and persistence are infrastructure concerns; validation and threshold logic can be exercised without starting an HTTP server.

**Why decimal amounts but integer cents in storage?** The public DTO uses `decimal` and limits scale to two decimal places, while the SQLite entity stores integer cents to avoid floating-point amount conversion problems. A real multi-currency product needs explicit currency scale, auditability and rounding policy.

**Why EF Core and SQLite?** A local file database enables learning relational persistence and automated reopen checks without setting up a database service. The code uses `EnsureCreated` as a demo convenience, not production migrations.

**What does REVIEW mean?** It means the demonstration threshold of EUR 1,000 was reached. It does not mean fraud and does not authorise or decline a real payment.

**What was the Windows testing failure?** SQLite pooled connections initially retained temporary test files, so the tests could not delete them. I examined the error, used `Pooling=false` for test-only connections, ensured DbContexts were disposed before cleanup, and reran all nine xUnit tests successfully.

**Why HTTP 201, 400 and 404?** Created record, invalid input and missing resource respectively.

**How did you use AI?** AI-assisted code suggestions were reviewed and checked with actual builds, test output and HTTP requests. The real evidence is the code and recorded test outcomes, not the suggestion itself.

**What would you change for production?** Authentication and authorisation, idempotency constraints, audit trails, pagination, versioned migrations, safe observability, rate limits, operational monitoring and independent security review, before connecting to real financial infrastructure.

## Verified evidence from development

- 23 Oct 2026: `.NET 10.0.402`, API build success.
- 23 Oct 2026: 12/12 core checks passed.
- 23 Oct 2026: 9/9 xUnit tests passed after test-connection cleanup fix.
- 23 Oct 2026: HTTP POST of synthetic EUR 250 returned `APPROVED`; HTTP GET retrieved the same record while an API instance was still listening.
- Cross-restart proof should only be claimed once the active server is stopped and restarted cleanly.

## Accurate CV line

"Developed a local learning API in C#/.NET 10 using ASP.NET Core, EF Core/SQLite, REST endpoints, input validation, nine xUnit tests, twelve behaviour checks and CI automation; handled synthetic transaction data only."
