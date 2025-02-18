using Microsoft.UI.Xaml;

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
		// TODO: Log and handle exceptions as appropriate.
		// https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
	}

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		base.OnLaunched(args);
		MainWindow.Activate();
	}
}
