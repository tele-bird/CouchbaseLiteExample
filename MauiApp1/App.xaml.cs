using Couchbase.Lite;

namespace MauiApp1;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
        Database.Log.Console.Level = Couchbase.Lite.Logging.LogLevel.Verbose;
        Database.Log.Console.Domains = Couchbase.Lite.Logging.LogDomain.Couchbase | Couchbase.Lite.Logging.LogDomain.Database;
		MainPage = new AppShell();
	}
}
