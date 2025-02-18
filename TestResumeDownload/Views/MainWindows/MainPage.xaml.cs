using Microsoft.UI.Xaml.Controls;

using TestResumeDownload.ViewModels.MainWindows;

namespace TestResumeDownload.Views.MainWindows;

internal sealed partial class MainPage : Page
{
	public MainPageViewModel ViewModel
	{
		get;
	}

	public MainPage()
	{
		ViewModel = new();
		InitializeComponent();
	}
}
