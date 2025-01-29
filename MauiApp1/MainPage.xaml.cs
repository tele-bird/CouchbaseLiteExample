using System.Text;
using Couchbase.Lite;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		try
		{
			var db = new Database("TestDatabase");
			ErrorLabel.Text = $"created {db.Name} at {db.Path}";
		}
		catch(Exception exception)
		{
			StringBuilder sbResult = new StringBuilder();
			Exception? exc = exception;
			int level = 0;
			while(exc != null)
			{
				sbResult.AppendLine($"LEVEL: {level} {exc.GetType().FullName}: {exc.Message} {exc.StackTrace}");
				sbResult.AppendLine();
				exc = exc.InnerException;
				++level;
			}
			ErrorLabel.Text = $"{sbResult}";
		}
	}
}
