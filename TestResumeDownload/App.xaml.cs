using Microsoft.UI.Xaml;

using TestResumeDownload.Views.MainWindows;

namespace TestResumeDownload;

public partial class App : Application
{
	public static WindowEx MainWindow { get; } = new MainWindow();

	public App()
	{
		InitializeComponent();
		UnhandledException += App_UnhandledException;
	}

	private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
	{
	}

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		base.OnLaunched(args);
		MainWindow.Activate();
	}
}
