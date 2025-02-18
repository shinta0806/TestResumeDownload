using Microsoft.UI.Xaml.Controls;

using TestResumeDownload.ViewModels;

namespace TestResumeDownload.Views;

public sealed partial class MainPage : Page
{
	public MainViewModel ViewModel
	{
		get;
	}

	public MainPage()
	{
		ViewModel = new();
		InitializeComponent();
	}
}
