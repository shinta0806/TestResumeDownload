using Microsoft.UI.Xaml.Controls;

using TestResumeDownload.ViewModels;

namespace TestResumeDownload.Views.MainWindows;

public sealed partial class MainPage : Page
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
