using System.Diagnostics;
using System.Security.Cryptography;

using Windows.Media.Control;


namespace MediaPlayerBroadcaster.Daemon.CLI
{
    
    class Program
    {
        private static string _lastImageHash = null;
        static Sender _sender;
        static List<string> whiteList = new List<string>();
        static async Task Main(string[] args)
        {
            Console.Title = "MediaPlayerBroadcaster.Daemon.CLI";
            var _ip = File.ReadAllText("ip.data");
            var _port = File.ReadAllText("port.data");
            whiteList = File.ReadAllLines("whitelist.data").ToList();
             _sender = new Sender(_ip, _port);
            while (true)
            {
                var mediaInfo = await GetPlayingMediaTrack();
                Console.Clear();
                Console.WriteLine(mediaInfo ?? "Ничего не воспроизводится.");
                await Task.Delay(5000);
            }
        }

        public static async Task<string> GetPlayingMediaTrack()
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
                        var appName = session.SourceAppUserModelId;
                        var appNameToLower = appName.ToLower();
                        var trackTitle = mediaProperties.Title;
                        var artistName = mediaProperties.Artist;

                        var displayProperties = session.TryGetMediaPropertiesAsync();
                        var thumbnailStream = await mediaProperties.Thumbnail.OpenReadAsync();

                        // Получаем байты обложки и вычисляем хэш
                        byte[] imageBytes;
                        using (var memoryStream = new MemoryStream())
                        {
                            await thumbnailStream.AsStreamForRead().CopyToAsync(memoryStream);
                            imageBytes = memoryStream.ToArray();
                        }

                        string currentImageHash = ComputeHash(imageBytes);

                        // Отправляем обложку, только если она изменилась
                        if (currentImageHash != _lastImageHash)
                        {
                            _lastImageHash = currentImageHash;
                            await _sender.SendPlayerImageAsync(imageBytes);
                        }

                        bool containsMatch = whiteList.Any(item => appNameToLower.Contains(item.ToLower()));

                        if (containsMatch)
                        {
                            await _sender.SendPlayerInfoAsync(artistName, trackTitle, appName);
                            return $"Приложение: {appName}\nТрек: {trackTitle}\nИсполнитель: {artistName}";
                        }
                        else
                        {
                            await _sender.SendPlayerInfoAsync("Сейчас никто не играет", "Сейчас ничего не играет", "Silence");
                            return $"Приложение: {appName}\nТрек: {trackTitle}\nИсполнитель: {artistName}\nЭто приложение скрыто.";
                        }
                    }
                }

                await _sender.SendPlayerInfoAsync("Сейчас никто не играет", "Сейчас ничего не играет", "Silence");
                return null;
            }
            catch (Exception ex)
            {
                return $"Ошибка: {ex.Message}";
            }
        }

        private static string ComputeHash(byte[] data)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(data);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
