using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PaymentGuard.Api.Data;
using PaymentGuard.Core.Models;
using Xunit;

namespace PaymentGuard.Tests;

public class SqliteTransactionRepositoryTests
{
    // Exercise real SQLite on disk, including closing and reopening DbContext.
    // Disable pooling only for isolated test databases so Windows releases files
    // before the finally block removes them.
    private static DbContextOptions<PaymentGuardDbContext> TestDatabaseOptions(string path)
    {
        var connection = new SqliteConnectionStringBuilder
        {
            DataSource = path,
            Pooling = false
        };

        return new DbContextOptionsBuilder<PaymentGuardDbContext>()
            .UseSqlite(connection.ToString())
            .Options;
    }

    [Fact]
    public async Task Transaction_PersistsAfterDbContextIsDisposed()
    {
        var path = Path.Combine(Path.GetTempPath(), $"paymentguard-test-{Guid.NewGuid():N}.db");

        try
        {
            var options = TestDatabaseOptions(path);
            var id = Guid.NewGuid();
            var createdAt = new DateTimeOffset(2026, 10, 23, 13, 27, 39, TimeSpan.Zero);
            var record = new TransactionRecord(
                id, 1450m, "EUR", "DEMO-ORDER-002", "REVIEW", "HIGH_AMOUNT", createdAt);

            await using (var db = new PaymentGuardDbContext(options))
            {
                await db.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
                await new SqliteTransactionRepository(db).SaveAsync(
                    record, TestContext.Current.CancellationToken);
            }

            await using (var db = new PaymentGuardDbContext(options))
            {
                var stored = await new SqliteTransactionRepository(db).FindAsync(
                    id, TestContext.Current.CancellationToken);
                Assert.NotNull(stored);
                Assert.Equal(1450m, stored.Amount);
                Assert.Equal("REVIEW", stored.Decision);
                Assert.Equal("HIGH_AMOUNT", stored.ReasonCode);
                Assert.Equal("DEMO-ORDER-002", stored.MerchantReference);
                Assert.Equal(createdAt, stored.CreatedAtUtc);
            }
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    [Fact]
    public async Task MissingId_ReturnsNull()
    {
        var path = Path.Combine(Path.GetTempPath(), $"paymentguard-test-{Guid.NewGuid():N}.db");

        try
        {
            var options = TestDatabaseOptions(path);

            // Dispose before the finally block: otherwise Windows can keep the file locked.
            await using (var db = new PaymentGuardDbContext(options))
            {
                await db.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
                var stored = await new SqliteTransactionRepository(db).FindAsync(
                    Guid.NewGuid(), TestContext.Current.CancellationToken);
                Assert.Null(stored);
            }
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
