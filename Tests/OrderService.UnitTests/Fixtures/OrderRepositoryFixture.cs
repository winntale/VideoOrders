
using Dal.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace OrderService.UnitTests.Fixtures;

public sealed class OrderRepositoryFixture : IDisposable
{

    private readonly SqliteConnection _connection;
    public OrderDbContext DbContext { get; }

    public OrderRepositoryFixture()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseSqlite(_connection)
            .Options;

        DbContext = new OrderDbContext(options);
        DbContext.Database.EnsureCreated();
    }
    
    public void Dispose()
    {
        DbContext.Dispose();
        _connection.Dispose();
    }
}