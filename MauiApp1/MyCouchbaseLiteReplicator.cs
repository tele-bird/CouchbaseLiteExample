using Couchbase.Lite;
using Couchbase.Lite.Sync;
using MauiApp1.Extensions;
using System.Diagnostics;

namespace MauiApp1;

public class MyCouchbaseLiteReplicator
{
    private Replicator? replicator;
    private ListenerToken? replicatorListenerToken;

    public MyCouchbaseLiteReplicator(ReplicatorConfiguration replicatorConfiguration)
    {
        replicator = new Replicator(replicatorConfiguration);
        replicatorListenerToken = replicator.AddChangeListener(OnReplicatorStatusChanged);
        replicator.Start();
    }

    private void OnReplicatorStatusChanged(object? sender, ReplicatorStatusChangedEventArgs e)
    {
        Trace.WriteLine($"{GetType().Name}.{nameof(OnReplicatorStatusChanged)} - replicator status changed to {e.Status.ToDebugString()}");
    }

    public async Task DisposeAsync(CancellationToken cancellationToken, Func<double> timeRemainingFunction)
    {
        if(replicator != null)
        {
            Trace.WriteLine($"{GetType().Name}.{nameof(DisposeAsync)} - stopping replicator");
            if(!await StopAsync(cancellationToken, timeRemainingFunction))
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            Trace.WriteLine($"{GetType().Name}.{nameof(DisposeAsync)} - replicator stopped");
            if(replicatorListenerToken.HasValue)
            {
                Trace.WriteLine($"{GetType().Name}.{nameof(DisposeAsync)} - removing listener");
                replicator.RemoveChangeListener(replicatorListenerToken.Value);
                Trace.WriteLine($"{GetType().Name}.{nameof(DisposeAsync)} - listener removed");
                replicatorListenerToken = null;
            }
            Trace.WriteLine($"{GetType().Name}.{nameof(DisposeAsync)} - disposing replicator");
            replicator?.Dispose();
            Trace.WriteLine($"{GetType().Name}.{nameof(DisposeAsync)} - replicator disposed");
            replicator = null;
        }
    }

    private async Task<bool> StopAsync(CancellationToken cancellationToken, Func<double> timeRemainingFunction)
    {
        ArgumentNullException.ThrowIfNull(replicator);
        Exception? exception = null;
        bool? result = null;
        var listenerToken = replicator.AddChangeListener((sender, e) =>
        {
            Trace.WriteLine($"{GetType().Name}.{nameof(StopAsync)} - replicator status changed to {e.Status.Activity}");
            if(e.Status.Activity == ReplicatorActivityLevel.Stopped)
            {
                result = true;
            }
        });
        replicator.Stop();
        try
        {
            while(!result.HasValue)
            {
				Trace.WriteLine($"{GetType().Name}.{nameof(StopAsync)} - waiting for replicator to stop.  time remaining: {timeRemainingFunction.Invoke()}");
                await Task.Delay(50, cancellationToken);
            }
            Trace.WriteLine($"{GetType().Name}.{nameof(StopAsync)} - result changed to {result}");
        }
        catch(TaskCanceledException e)
        {
            exception = e;
            result = false;
        }
        catch(Exception e)
        {
            exception = e;
            result = false;
        }
        finally
        {
            Trace.WriteLine($"{GetType().Name}.{nameof(StopAsync)} - removing listener");
            replicator.RemoveChangeListener(listenerToken);
            Trace.WriteLine($"{GetType().Name}.{nameof(StopAsync)} - listener removed");
        }
        Trace.WriteLine($"{GetType().Name}.{nameof(StopAsync)} - returning result: {result.Value} with exception: {exception?.Message}");
        return result.Value;
    }
}