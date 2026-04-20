using Dal.Abstractions.Entities;
using Dal.Options;
using MongoDB.Driver;

namespace Dal.Context;

public sealed class NotificationDbContext
{
    public NotificationDbContext(MongoOptions options)
    {
        var client = new MongoClient(options.ConnectionString);
        var database = client.GetDatabase(options.DatabaseName);

        Notifications = database.GetCollection<Notification>("Notifications");
    }
    
    public IMongoCollection<Notification> Notifications { get; }
}