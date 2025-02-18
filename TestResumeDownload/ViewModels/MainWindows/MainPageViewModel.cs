// ============================================================================
// 
// メインページの ViewModel
// 
// ============================================================================

// ----------------------------------------------------------------------------
// 
// ----------------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml;

namespace TestResumeDownload.ViewModels.MainWindows;

internal partial class MainPageViewModel : ObservableRecipient
{
	// ====================================================================
	// コンストラクター
	// ====================================================================

	/// <summary>
	/// メインコンストラクター
	/// </summary>
	public MainPageViewModel()
	{
	}

	// ====================================================================
	// public プロパティー
	// ====================================================================

	// --------------------------------------------------------------------
	// View 通信用のプロパティー
	// --------------------------------------------------------------------

	/// <summary>
	/// URL
	/// </summary>
	private String _url = "https://";
	public String Url
	{
		get => _url;
		set => SetProperty(ref _url, value);
	}

	/// <summary>
	/// ダウンロードフォルダー
	/// </summary>
	private String _destFolder = @"C:\Temp";
	public String DestFolder
	{
		get => _destFolder;
		set => SetProperty(ref _destFolder, value);
	}

	/// <summary>
	/// ダウンロードサイズ [MB]
	/// </summary>
	private Int32 _downloadSize;
	public Int32 DownloadSize
	{
		get => _downloadSize;
		set => SetProperty(ref _downloadSize, value);
	}

	// ====================================================================
	// public 関数
	// ====================================================================

	/// <summary>
	/// イベントハンドラー
	/// </summary>
	/// <param name="_1"></param>
	/// <param name="_2"></param>
	public void PageLoaded(Object _1, RoutedEventArgs _2)
	{
		App.MainWindow.Width = 590;
		App.MainWindow.Height = 240;
	}

}
