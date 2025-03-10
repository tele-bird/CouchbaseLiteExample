using System.Text;
using Couchbase.Lite.Sync;

namespace MauiApp1.Extensions;

public static class ReplicatorStatusExtensions
{
    public static string ToDebugString(this ReplicatorStatus replicatorStatus)
    {
        StringBuilder sbStatus = new StringBuilder();
        sbStatus.Append("{");
        sbStatus.Append($"Activity: {replicatorStatus.Activity} Progress: {replicatorStatus.Progress.ToDebugString()} Error: {replicatorStatus.Error}");
        sbStatus.Append("}");
        return sbStatus.ToString();
    }

    public static string ToDebugString(this ReplicatorProgress replicatorProgress)
    {
        StringBuilder sbStatus = new StringBuilder();
        sbStatus.Append("{");
        sbStatus.Append($"Completed: {replicatorProgress.Completed} Total: {replicatorProgress.Total}");
        sbStatus.Append("}");
        return sbStatus.ToString();
    }
}
