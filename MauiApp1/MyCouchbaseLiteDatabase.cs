using System.Diagnostics;
using Couchbase.Lite;
using Couchbase.Lite.Sync;

namespace MauiApp1;

public class MyCouchbaseLiteDatabase
{
    private Database? database;
    private MyCouchbaseLiteReplicator? myCouchbaseLiteReplicator;

    public string Name => database != null ? database.Name : string.Empty;

    public string Path => database != null ? database.Path : string.Empty;

    public MyCouchbaseLiteDatabase()
    {
        Database.Log.Console.Level = Couchbase.Lite.Logging.LogLevel.Verbose;
        Database.Log.Console.Domains = Couchbase.Lite.Logging.LogDomain.Couchbase | Couchbase.Lite.Logging.LogDomain.Database;
        database = new Database("TestDatabase");
        var eventsCollection = database.CreateCollection("events", "event");
        var replicatorConfiguration = new ReplicatorConfiguration(new URLEndpoint(new Uri("wss://k4bz2uxpwsjwp4mg.apps.cloud.couchbase.com:4984/event-endpoint/")))
        {
            ReplicatorType = ReplicatorType.Pull,
            Continuous = true,
            Authenticator = new BasicAuthenticator("guest", "Miami123!")
        };
        replicatorConfiguration.AddCollection(eventsCollection, new CollectionConfiguration
        {
            Channels = new List<string> { "*" }
        });
        myCouchbaseLiteReplicator = new MyCouchbaseLiteReplicator(replicatorConfiguration);
    }

    public async Task DisposeAsync(CancellationToken cancellationToken, Func<double> timeRemainingFunction)
    {
        // if(myCouchbaseLiteReplicator != null)
        // {
        //     Console.WriteLine($"{GetType().Name}.{nameof(DisposeAsync)} - disposing {nameof(MyCouchbaseLiteReplicator)}");
        //     await myCouchbaseLiteReplicator.DisposeAsync(cancellationToken, timeRemainingFunction);
        //     Console.WriteLine($"{GetType().Name}.{nameof(DisposeAsync)} - {nameof(MyCouchbaseLiteReplicator)} disposed");
        //     myCouchbaseLiteReplicator = null;
        // }
        if(database != null)
        {
            Console.WriteLine($"{GetType().Name}.{nameof(DisposeAsync)} - closing database");
            DateTime start = DateTime.UtcNow;
            database.Close();
            TimeSpan duration = DateTime.UtcNow.Subtract(start);
            Console.WriteLine($"{GetType().Name}.{nameof(DisposeAsync)} - database was closed in {duration.Milliseconds} ms");
            database = null;
        }
    }
}
