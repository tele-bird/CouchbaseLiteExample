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
        Console.WriteLine($"{GetType().Name}.{nameof(OnStart)} >>");
        base.OnStart();
        Console.WriteLine($"{GetType().Name}.{nameof(OnStart)} <<");
    }

    protected override void OnSleep()
    {
        Console.WriteLine($"{GetType().Name}.{nameof(OnSleep)} >>");
        base.OnSleep();
        Console.WriteLine($"{GetType().Name}.{nameof(OnSleep)} <<");
    }

    protected override void OnResume()
    {
        Console.WriteLine($"{GetType().Name}.{nameof(OnResume)} >>");
        base.OnResume();
        Console.WriteLine($"{GetType().Name}.{nameof(OnResume)} <<");
    }
}
