using System.Diagnostics;
using Couchbase.Lite;

namespace MauiApp1;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}

    protected override void OnStart()
    {
        Trace.WriteLine($"{GetType().Name}.{nameof(OnStart)} >>");
        base.OnStart();
        Trace.WriteLine($"{GetType().Name}.{nameof(OnStart)} <<");
    }

    protected override void OnSleep()
    {
        Trace.WriteLine($"{GetType().Name}.{nameof(OnSleep)} >>");
        base.OnSleep();
        Trace.WriteLine($"{GetType().Name}.{nameof(OnSleep)} <<");
    }

    protected override void OnResume()
    {
        Trace.WriteLine($"{GetType().Name}.{nameof(OnResume)} >>");
        base.OnResume();
        Trace.WriteLine($"{GetType().Name}.{nameof(OnResume)} <<");
    }
}
