using System.Diagnostics;
using Foundation;
using UIKit;

namespace MauiApp1;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	private MyCouchbaseLiteDatabase? testDatabase = null;
	private nint? taskId = null;

	public AppDelegate()
	{
        Console.WriteLine($"{GetType().Name}.ctor >> creating {nameof(MyCouchbaseLiteDatabase)}");
        testDatabase = new MyCouchbaseLiteDatabase();
        Console.WriteLine($"{GetType().Name}.ctor >> created: {testDatabase?.Name}");
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override void DidEnterBackground(UIApplication application)
    {
		if(testDatabase != null)
		{
			CancellationTokenSource cts = new CancellationTokenSource();
			var backgroundTaskName = "DatabaseCloseTask";
			taskId = UIApplication.SharedApplication.BeginBackgroundTask(backgroundTaskName,() =>
			{
				Console.WriteLine($"{GetType().Name}.{backgroundTaskName} - background time expired.  time remaining: {UIApplication.SharedApplication.BackgroundTimeRemaining}. ending background task {taskId.Value}");
				cts.Cancel();
			});
			Task.Factory.StartNew(async (state) => 
			{
				CancellationToken cancellationToken = (CancellationToken)state;
				try
				{
					await testDatabase.DisposeAsync(cancellationToken, () => UIApplication.SharedApplication.BackgroundTimeRemaining);
					testDatabase = null;
				}
				catch(AggregateException e) // thrown from Task.Delay(500, cancellationToken);
				{
					var innerException = e.InnerExceptions[0];
					Console.WriteLine($"{GetType().Name}.{backgroundTaskName} - caught exception: {innerException.GetType().Name}:{innerException.Message}");
				}
				catch(OperationCanceledException e) // thrown from cancellationToken.ThrowIfCancellationRequested();
				{
					Console.WriteLine($"{GetType().Name}.{backgroundTaskName} - caught exception: {e.GetType().Name}:{e.Message}");
				}
				finally
				{
					Console.WriteLine($"{GetType().Name}.{backgroundTaskName} - ending background task {taskId.Value}: {backgroundTaskName}");
					UIApplication.SharedApplication.EndBackgroundTask(taskId.Value);
				}
			}, cts.Token);
		}
		base.DidEnterBackground(application);
    }

    public override void WillEnterForeground(UIApplication application)
    {
		if(testDatabase == null)
		{
			Console.WriteLine($"{GetType().Name}.{nameof(WillEnterForeground)} >> creating {nameof(MyCouchbaseLiteDatabase)}");
			testDatabase = new MyCouchbaseLiteDatabase();
			Console.WriteLine($"{GetType().Name}.{nameof(WillEnterForeground)} << created: {testDatabase?.Name}");
		}
		else
		{
			Console.WriteLine($"{GetType().Name}.{nameof(WillEnterForeground)} >> {nameof(MyCouchbaseLiteDatabase)} is not null: {testDatabase.Name}");
		}
        base.WillEnterForeground(application);
    }

    public override void WillTerminate(UIApplication application)
    {
		Console.WriteLine($"{GetType().Name}.{nameof(WillTerminate)} >>");
        base.WillTerminate(application);
		Console.WriteLine($"{GetType().Name}.{nameof(WillTerminate)} <<");
    }
}
