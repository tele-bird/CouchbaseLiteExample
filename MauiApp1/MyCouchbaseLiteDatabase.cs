using System.Diagnostics;
using Couchbase.Lite;
using Couchbase.Lite.Query;
using Couchbase.Lite.Sync;
using MauiApp1.Extensions;

namespace MauiApp1;

public class MyCouchbaseLiteDatabase : IDisposable
{
    private Database? database;
    private Replicator? replicator;
    private ListenerToken? replicatorListenerToken;
    private ListenerToken? externalReplicatorListenerToken;
    private Collection eventsCollection;
    private ListenerToken? queryListenerToken;

    public string Name => database != null ? database.Name : string.Empty;

    public string Path => database != null ? database.Path : string.Empty;

    public MyCouchbaseLiteDatabase(EventHandler<ReplicatorStatusChangedEventArgs> eventHandler)
    {
        database = new Database("TestDatabase");
        eventsCollection = database.CreateCollection("events", "event");
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
        externalReplicatorListenerToken = replicator.AddChangeListener(eventHandler);
        replicator.Start();
    }

    private void OnReplicatorStatusChanged(object? sender, ReplicatorStatusChangedEventArgs e)
    {
        Trace.WriteLine($"replicator status changed to {e.Status.ToDebugString()}");
    }

    public void Dispose()
    {
        RemoveReplicatorListeners();
        Trace.WriteLine("disposing replicator");
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

    private void RemoveReplicatorListeners()
    {
        Trace.WriteLine("removing replicator listeners");
        RemoveReplicatorQueryListeners();
        if(replicatorListenerToken.HasValue)
        {
            replicator?.RemoveChangeListener(replicatorListenerToken.Value);
            replicatorListenerToken = null;
        }
        if(externalReplicatorListenerToken.HasValue)
        {
            replicator?.RemoveChangeListener(externalReplicatorListenerToken.Value);
            externalReplicatorListenerToken = null;
        }
    }

    private void RemoveReplicatorQueryListeners()
    {
        queryListenerToken?.Remove();
        queryListenerToken = null;
    }

    public void StopReplicator()
    {
        ArgumentNullException.ThrowIfNull(replicator);
        // RemoveReplicatorListeners();
        replicator.Stop();
    }

    public void StartReplicator()
    {
        ArgumentNullException.ThrowIfNull(replicator);

        replicator.Start();
    }

    public IResultSet QueryForEvents(string metaIdAlias, EventHandler<QueryChangedEventArgs> eventHandler, out string explain)
    {
        ArgumentNullException.ThrowIfNull(metaIdAlias);
        RemoveReplicatorQueryListeners();
        var eventsAlias = "e";
        var query = QueryBuilder.Select(SelectResult.Expression(Meta.ID.From(eventsAlias)).As(metaIdAlias))
            .From(DataSource.Collection(eventsCollection).As(eventsAlias));
        explain = query.Explain();
        queryListenerToken = query.AddChangeListener(eventHandler);
        return query.Execute();
    }
}
