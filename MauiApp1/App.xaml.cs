using Couchbase.Lite;
using Page = Microsoft.Maui.Controls.PlatformConfiguration.macOSSpecific.Page;

namespace MauiApp1;

public partial class App : Application
{
	MainPage mainPage => (MainPage)((AppShell)Windows[0].Page!).CurrentPage;
	
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}

	protected override void OnSleep()
	{
		mainPage.Pause();
		base.OnSleep();
	}

	protected override void OnResume()
	{
		mainPage.Resume();
		base.OnResume();
	}
}
