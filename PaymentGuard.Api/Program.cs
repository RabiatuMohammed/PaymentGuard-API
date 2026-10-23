using Microsoft.EntityFrameworkCore;
using PaymentGuard.Api.Data;
using PaymentGuard.Core.Models;
using PaymentGuard.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// A local file under the API project directory. No bank credentials or external server.
var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(dataDirectory);
var databaseFile = Path.Combine(dataDirectory, "paymentguard.db");

builder.Services.AddDbContext<PaymentGuardDbContext>(options =>
    options.UseSqlite($"Data Source={databaseFile}"));
builder.Services.AddScoped<ITransactionRepository, SqliteTransactionRepository>();

var app = builder.Build();

// Demo only. This initializes an empty schema; future schema changes need EF migrations.
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<PaymentGuardDbContext>();
    database.Database.EnsureCreated();
}

app.MapGet("/", () => Results.Ok(new
{
    name = "PaymentGuard API",
    stage = "Phase 2: SQLite persistence and xUnit tests",
    disclaimer = "Synthetic transactions only; not a real payment processor."
}));

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

var transactions = app.MapGroup("/api/transactions");

transactions.MapPost("/", async (CreateTransactionRequest request,
    ITransactionRepository repository, CancellationToken cancellationToken) =>
{
    var errors = TransactionValidator.Validate(request);
    if (errors.Count != 0)
        return Results.ValidationProblem(errors);

    var assessment = RiskEvaluator.Evaluate(request.Amount);
    var record = new TransactionRecord(
        Guid.NewGuid(),
        request.Amount,
        "EUR",
        request.MerchantReference!,
        assessment.Decision,
        assessment.ReasonCode,
        DateTimeOffset.UtcNow);

    await repository.SaveAsync(record, cancellationToken);
    return Results.Created($"/api/transactions/{record.Id}", record);
});

transactions.MapGet("/", async (ITransactionRepository repository,
    CancellationToken cancellationToken) =>
    Results.Ok(await repository.ListAsync(cancellationToken)));

transactions.MapGet("/{id:guid}", async (Guid id,
    ITransactionRepository repository, CancellationToken cancellationToken) =>
{
    var record = await repository.FindAsync(id, cancellationToken);
    return record is null
        ? Results.NotFound(new { error = "Transaction not found." })
        : Results.Ok(record);
});

app.Run();
