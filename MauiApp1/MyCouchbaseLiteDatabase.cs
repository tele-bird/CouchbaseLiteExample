using System.Diagnostics;
using Couchbase.Lite;
using Couchbase.Lite.Sync;
using MauiApp1.Extensions;

namespace MauiApp1;

public class MyCouchbaseLiteDatabase : IDisposable
{
    private Database? database;
    private Replicator? replicator;
    private ListenerToken? replicatorListenerToken;

    public string Name => database != null ? database.Name : string.Empty;

    public string Path => database != null ? database.Path : string.Empty;

    public MyCouchbaseLiteDatabase()
    {
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
        replicator = new Replicator(replicatorConfiguration);
        replicatorListenerToken = replicator.AddChangeListener(OnReplicatorStatusChanged);
        replicator.Start();
    }

    private void OnReplicatorStatusChanged(object? sender, ReplicatorStatusChangedEventArgs e)
    {
        Trace.WriteLine($"replicator status changed to {e.Status.ToDebugString()}");
    }

    public void Dispose()
    {
        Trace.WriteLine("disposing replicator");
        if(replicatorListenerToken.HasValue)
        {
            replicator?.RemoveChangeListener(replicatorListenerToken.Value);
            replicatorListenerToken = null;
        }
        replicator?.Dispose();
        replicator = null;
        Trace.WriteLine("replicator disposed");
        Trace.WriteLine("closing database");
        DateTime start = DateTime.UtcNow;
        database?.Close();
        TimeSpan duration = DateTime.UtcNow.Subtract(start);
        Trace.WriteLine($"database closed in {duration.Milliseconds} ms");
        database = null;
    }
}
