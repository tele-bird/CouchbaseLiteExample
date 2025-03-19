namespace MauiApp1;

public partial class MainPage : ContentPage
{
	private int numTimesButtonClicked;

	public MainPage()
	{
		InitializeComponent();
		ErrorLabel.Text = $"button was clicked {numTimesButtonClicked} times";
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		++numTimesButtonClicked;
		ErrorLabel.Text = $"button was clicked {numTimesButtonClicked} times";
	}
}
