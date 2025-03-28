using System.Diagnostics;
using System.Text;
using Couchbase.Lite.Query;
using Couchbase.Lite.Sync;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
	private MyCouchbaseLiteDatabase? testDatabase;

	private const string notInitializedText = "new MyCouchbaseLiteDatabase()";
	private const string stopText = $"replicator.Stop()";
	private const string startText = $"replicator.Start()";
	private const string metaIdAlias = "metaId";

	public MainPage()
	{
		InitializeComponent();
		Button.Text = notInitializedText;
	}

    private void OnReplicatorStatusChanged(object? sender, ReplicatorStatusChangedEventArgs e)
    {
		Trace.WriteLine($"{GetType().Name}.{nameof(OnReplicatorStatusChanged)} >> {e.Status.Activity}");
		MainThread.BeginInvokeOnMainThread(()=> {
			StatusLabel.Text = e.Status.Activity.ToString();
			// QueryButton.IsEnabled = true;
			switch(e.Status.Activity)
			{
				case ReplicatorActivityLevel.Busy:
				{
					Button.IsEnabled = false;
					break;
				}
				case ReplicatorActivityLevel.Connecting:
				{
					Button.IsEnabled = false;
					break;
				}
				case ReplicatorActivityLevel.Idle:
				{
					Button.Text = stopText;
					Button.IsEnabled = true;
					break;
				}
				case ReplicatorActivityLevel.Offline:
				{
					Button.Text = stopText;
					Button.IsEnabled = true;
					break;
				}
				case ReplicatorActivityLevel.Stopped:
				{
					Button.Text = startText;
					Button.IsEnabled = true;
					QueryResultsLabel.Text = string.Empty;
					break;
				}
				default:
				{
					throw new Exception($"Unexpected {nameof(ReplicatorActivityLevel)}: {e.Status.Activity}");
				}
			}
		});
    }

	private void OnButtonClicked(object sender, EventArgs e)
	{
		Trace.WriteLine($"{nameof(OnButtonClicked)} >> {Button.Text}");
		try
		{
			switch(Button.Text)
			{
				case notInitializedText:
				{
					testDatabase = new MyCouchbaseLiteDatabase(OnReplicatorStatusChanged);
					PerformQuery();
					break;
				}
				case stopText:
				{
					testDatabase!.StopReplicator();
					break;
				}
				case startText:
				{
					testDatabase!.StartReplicator();
					PerformQuery();
					break;
				}
				default:
				{
					throw new Exception($"Unexpected button state: {Button.Text}");
				}
			}
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
			DisplayAlert($"{nameof(OnButtonClicked)} Error", sbResult.ToString(), "OK");
		}
	}

	private void OnQueryButtonClicked(object sender, EventArgs e)
	{
		Trace.WriteLine($"{nameof(OnQueryButtonClicked)}");
		PerformQuery();
	}

    private void PerformQuery()
    {
		try
		{
			MainThread.BeginInvokeOnMainThread(()=> {
				QueryResultsLabel.Text = string.Empty;
			});
			var results = testDatabase!.QueryForEvents(metaIdAlias, (sender, e) => {
				SetQueryResults("updated results", e.Results, true);
			}, out string explain);
			SetQueryResults(explain, results, false);
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
			DisplayAlert($"{nameof(OnQueryButtonClicked)} Error", sbResult.ToString(), "OK");
		}
    }

    public void SetQueryResults(string explain, IResultSet resultSet, bool updated)
	{
		MainThread.BeginInvokeOnMainThread(()=> {
			var sbResultsText = new StringBuilder();
			sbResultsText.AppendLine(explain);
			var allResults = resultSet.AllResults();
			sbResultsText.AppendLine($"{allResults.Count} documents as of {DateTime.Now}:");
			sbResultsText.AppendLine(string.Join(Environment.NewLine, allResults.Select(r => r.GetString(metaIdAlias))));
			if(updated)
			{
				sbResultsText.AppendLine();
				QueryResultsLabel.Text += sbResultsText.ToString();
			}
			else
			{
				QueryResultsLabel.Text = sbResultsText.ToString();
			}
		});
	}
}
