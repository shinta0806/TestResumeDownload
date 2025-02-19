// ============================================================================
// 
// メインページの ViewModel
// 
// ============================================================================

// ----------------------------------------------------------------------------
// 
// ----------------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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
		// コマンド
		ButtonDownloadClickedCommand = new(ButtonDownloadClicked);
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
	/// 中断サイズ [MB]
	/// </summary>
	private Int32 _abortSize = 5;
	public Int32 AbortSize
	{
		get => _abortSize;
		set => SetProperty(ref _abortSize, value);
	}

	/// <summary>
	/// コントロールが有効か
	/// </summary>
	private Boolean _isControlEnabled = true;
	public Boolean IsControlEnabled
	{
		get => _isControlEnabled;
		set => SetProperty(ref _isControlEnabled, value);
	}

	/// <summary>
	/// 進捗（0～1）
	/// </summary>
	private Double _progressValue = 0;
	public Double ProgressValue
	{
		get => _progressValue;
		set => SetProperty(ref _progressValue, value);
	}

	// --------------------------------------------------------------------
	// コマンド
	// --------------------------------------------------------------------

	#region ダウンロードボタンの制御
	public RelayCommand ButtonDownloadClickedCommand
	{
		get;
	}

	private async void ButtonDownloadClicked()
	{
		try
		{
			IsControlEnabled = false;
			CheckInput();
			(Int64 existingSize, Exception? ex) = await DownloadAsync();
			if (ex != null)
			{
				throw ex;
			}

			// ダウンロードした範囲
			String range = existingSize.ToString("#,0") + " ～ ";
			if (File.Exists(DestPartialPath()))
			{
				range += new FileInfo(DestPartialPath()).Length.ToString("#,0") + "（中断）";
			}
			else
			{
				range += new FileInfo(DestPath()).Length.ToString("#,0") + "（完了）";
			}
			await ShowContentDialogAsync("情報", range);
		}
		catch (Exception ex)
		{
			await ShowContentDialogAsync("エラー", ex.Message);
		}
		finally
		{
			IsControlEnabled = true;
		}
	}
	#endregion

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

	// ====================================================================
	// private 変数
	// ====================================================================

	/// <summary>
	/// HTTP クライアント
	/// </summary>
	private static readonly HttpClient _client = new();

	// ====================================================================
	// private 関数
	// ====================================================================

	/// <summary>
	/// 入力チェック
	/// </summary>
	private void CheckInput()
	{
		if (Url.Length <= "https://".Length)
		{
			throw new Exception("URL を入力してください。");
		}
		if (String.IsNullOrEmpty(DestFolder))
		{
			throw new Exception("ダウンロードフォルダーを入力してください。");
		}
		Directory.CreateDirectory(DestFolder);
		if (AbortSize <= 0)
		{
			throw new Exception("ダウンロードサイズを入力してください。");
		}
	}

	/// <summary>
	/// ダウンロード
	/// </summary>
	/// <returns></returns>
	private async Task<(Int64, Exception?)> DownloadAsync()
	{
		return await Task.Run(() =>
		{
			try
			{
				// 総サイズと、既にダウンロード済のサイズ
				Int64 totalSize = TotalSize();
				Int64 existingSize = 0;
				if (File.Exists(DestPartialPath()))
				{
					existingSize = new FileInfo(DestPartialPath()).Length;
				}

				// リクエスト時に Range でレジューム位置（ダウンロード済サイズ）を指定
				using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Url);
				request.Headers.Range = new(existingSize, null);
				using HttpResponseMessage response = _client.Send(request, HttpCompletionOption.ResponseHeadersRead);
				Debug.WriteLine("DownloadAsync() StatusCode: " + response.StatusCode);
				response.EnsureSuccessStatusCode();

				// 読み書きストリームの準備
				using (FileStream destStream = new FileStream(DestPartialPath(), FileMode.Append, FileAccess.Write, FileShare.None))
				{
					using Stream contentStream = response.Content.ReadAsStream();
					Byte[] buffer = new Byte[2048];
					Int32 bytesRead;
					Int64 totalBytesRead = 0;
					Int32 count = 0;

					while ((bytesRead = contentStream.Read(buffer, 0, buffer.Length)) > 0)
					{
						// 応答内容をファイルに書き込む
						destStream.Write(buffer, 0, bytesRead);

						// ダウンロードが中断サイズに達したら中断
						if (totalBytesRead >= AbortSize * 1024 * 1024)
						{
							return (existingSize, (Exception?)null);
						}

						// 一定期間ごとに進捗表示
						totalBytesRead += bytesRead;
						count++;
						if (count % 100 == 0)
						{
							App.MainWindow.DispatcherQueue.TryEnqueue(() =>
							{
								ProgressValue = (Double)(existingSize + totalBytesRead) / totalSize;
							});
						}
					}
				}

				// ダウンロード完了したのでリネーム
				try
				{
					File.Delete(DestPath());
				}
				catch (Exception)
				{
				}
				File.Move(DestPartialPath(), DestPath());

				return (existingSize, null);
			}
			catch (Exception ex)
			{
				return (0, ex);
			}
		});
	}

	/// <summary>
	/// ダウンロード中の一時ファイルパス
	/// </summary>
	/// <returns></returns>
	private String DestPartialPath()
	{
		return DestPath() + ".part";
	}

	/// <summary>
	/// 最終ダウンロードパス
	/// </summary>
	/// <returns></returns>
	private String DestPath()
	{
		return Path.Combine(DestFolder, Path.GetFileName(Url));
	}

	/// <summary>
	/// ContentDialog を表示
	/// </summary>
	/// <param name="title"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	private async Task ShowContentDialogAsync(String title, String message)
	{
		ContentDialog contentDialog = new()
		{
			Title = title,
			Content = message,
			PrimaryButtonText = "OK",
			XamlRoot = App.MainWindow.Content.XamlRoot,
		};
		await contentDialog.ShowAsync();
	}

	/// <summary>
	/// 総ファイルサイズを取得
	/// </summary>
	/// <returns></returns>
	private Int64 TotalSize()
	{
		using HttpRequestMessage request = new(HttpMethod.Head, Url);
		using HttpResponseMessage response = _client.Send(request, HttpCompletionOption.ResponseHeadersRead);
		response.EnsureSuccessStatusCode();
		if (!response.Content.Headers.ContentLength.HasValue)
		{
			throw new Exception("サーバー上のファイルサイズが不明です。");
		}
		return response.Content.Headers.ContentLength.Value;
	}
}
