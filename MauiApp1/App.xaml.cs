using Couchbase.Lite;

namespace MauiApp1;

public partial class App : Application
{
	public MyCouchbaseLiteDatabase? TestDatabase {get; set;}

	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}

    protected override void OnSleep()
    {
		TestDatabase?.Dispose();
        TestDatabase = null;
        base.OnSleep();
    }

    protected override void OnResume()
    {
        TestDatabase = new MyCouchbaseLiteDatabase();
        base.OnResume();
    }
}
