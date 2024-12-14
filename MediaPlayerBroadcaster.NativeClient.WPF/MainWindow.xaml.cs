using MicaWPF.Controls;

using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

using Windows.Media.Control;

namespace MediaPlayerBroadcaster.NativeClient.WPF
{
    public partial class MainWindow : MicaWindow
    {
        private CancellationTokenSource _cancellationTokenSource;
        public static List<string> whiteList = new List<string>();
        public static ForScreenCapture FCW;
        public TrackData CurrentTrackData;
        public MainWindow()
        {
            PreloadInitialize();
            InitializeComponent();
            PostloadInitialize();
            StartBackgroundTask();

        }
        private void PreloadInitialize()
        {
            CurrentTrackData = new TrackData();
            if (File.Exists("whitelist.data"))
            {
                whiteList = File.ReadAllLines("whitelist.data").ToList();
            }
            else
            {
                whiteList = new List<string>();
            }
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            FCW.Close();
        }

        private void PostloadInitialize()
        {
            this.Title = "MPB.NativeClient.WPF";

            FCW = new ForScreenCapture();
            FCW.Show();
        }
        private void StartBackgroundTask()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(async () =>
            {
                var cancellationToken = _cancellationTokenSource.Token;
                while (!cancellationToken.IsCancellationRequested)
                {
                    var (mediaInfo, albumArt) = await GetPlayingMediaTrackWithArt();
                    Dispatcher.Invoke(() =>
                    {
                        TrackPreview.Text = mediaInfo ?? "Ничего не воспроизводится.";
                        FCW.UpdateText(CurrentTrackData.Name, CurrentTrackData.Artist);

                        if (albumArt != null)
                        {
                            using (var stream = new MemoryStream(albumArt))
                            {
                                var image = new BitmapImage();
                                image.BeginInit();
                                image.StreamSource = stream;
                                image.CacheOption = BitmapCacheOption.OnLoad;
                                image.EndInit();
                                TrackImage.Source = image;
                                FCW.UpdateImage(image);
                                FCW.UpdateBackground(image, (bool)BlurEnabler.IsChecked, (int)BlurRadius.Value);
                            }
                        }
                        else
                        {
                            CurrentTrackData = new TrackData();
                            FCW.UpdateText(CurrentTrackData.Name, CurrentTrackData.Artist);
                            TrackImage.Source = CreateTransparentImage();
                            FCW.UpdateImage(CreateTransparentImage());
                            FCW.UpdateBackground(CreateTransparentImage(), (bool)BlurEnabler.IsChecked, (int)BlurRadius.Value);
                        }
                    });

                    try
                    {
                        await Task.Delay(5000, cancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                }
            });
        }
        private BitmapImage CreateTransparentImage()
        {
            var transparentBitmap = new BitmapImage();
            using (var memoryStream = new MemoryStream())
            {
                var bitmap = new System.Drawing.Bitmap(1, 1);
                bitmap.SetPixel(0, 0, System.Drawing.Color.Black);
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);

                transparentBitmap.BeginInit();
                transparentBitmap.StreamSource = memoryStream;
                transparentBitmap.CacheOption = BitmapCacheOption.OnLoad;
                transparentBitmap.EndInit();
            }
            return transparentBitmap;
        }
        public async Task<(string trackInfo, byte[] albumArt)> GetPlayingMediaTrackWithArt()
        {
            try
            {
                var sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                var sessions = sessionManager.GetSessions();

                foreach (var session in sessions)
                {
                    var playbackInfo = session.GetPlaybackInfo();
                    if (playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
                    {
                        var mediaProperties = await session.TryGetMediaPropertiesAsync();
                        string appName = session.SourceAppUserModelId;
                        string appNameToLower = appName.ToLower();
                        string trackTitle = mediaProperties.Title;
                        string artistName = mediaProperties.Artist;

                        bool containsMatch = whiteList.Any(item => appNameToLower.Contains(item.ToLower()));

                        byte[] albumArt = null;
                        if (mediaProperties.Thumbnail != null)
                        {
                            using (var thumbnailStream = await mediaProperties.Thumbnail.OpenReadAsync())
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await thumbnailStream.AsStreamForRead().CopyToAsync(memoryStream);
                                    albumArt = memoryStream.ToArray();
                                }
                            }
                        }

                        string trackInfo = containsMatch
                            ? $"Приложение: {appName}\nТрек: {trackTitle}\nИсполнитель: {artistName}"
                            : $"Приложение: {appName}\nТрек: {trackTitle}\nИсполнитель: {artistName}\nЭто приложение скрыто.";
                        CurrentTrackData = new TrackData { Artist = artistName, Name = trackTitle };
                        return (trackInfo, albumArt);
                    }
                }
                return (null, null);
            }
            catch (Exception ex)
            {
                return ($"Ошибка: {ex.Message}", null);
            }
        }
    }
}