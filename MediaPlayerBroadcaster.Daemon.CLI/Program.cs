using System.Security.Cryptography;
using Windows.Media;
using Windows.Media.Control;


namespace MediaPlayerBroadcaster.Daemon.CLI
{

    class Program
    {
        private static string _lastImageHash = null;
        static Sender _sender;
        static List<string> whiteList = new List<string>();
        public static TrackData CurrentTrackData;
        static DiscordService _discordService;
        public static string DiscordImageGuid = string.Empty;
        private static bool _discord = false;
        static async Task Main(string[] args)
        {
            
            CurrentTrackData = new TrackData();
            Console.Title = "MediaPlayerBroadcaster.Daemon.CLI";
            var _ip = File.ReadAllText("ip.data");
            var _port = File.ReadAllText("port.data");
            whiteList = File.ReadAllLines("whitelist.data").ToList();
            _sender = new Sender(_ip, _port);
            var discordStart = false;
            if (File.Exists("discord"))
            {
                discordStart = true;
                _discord = true;
                _discordService = new DiscordService("1255752860189196380", _ip, _port, enable: discordStart);
                await _discordService.InitializeAsync();
            }
            


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
                        var trackTitle = mediaProperties?.Title;
                        var artistName = mediaProperties?.Artist;

                        var displayProperties = session.TryGetMediaPropertiesAsync();
                        var thumbnailStream = mediaProperties?.Thumbnail != null ? await mediaProperties.Thumbnail.OpenReadAsync() : null;

                        byte[] imageBytes = null;
                        if (thumbnailStream != null)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await thumbnailStream.AsStreamForRead().CopyToAsync(memoryStream);
                                imageBytes = memoryStream.ToArray();
                            }
                        }

                        string currentImageHash = imageBytes != null ? ComputeHash(imageBytes) : null;

                        

                        bool containsMatch = whiteList.Any(item => appNameToLower.Contains(item.ToLower()));

                        if (containsMatch)
                        {
                            var trackTimeInfo = await GetTrackTimeInfo(session);
                            await _sender.SendPlayerInfoAsync(artistName, trackTitle, appName);
                            if (_discord)
                            {
                                await _discordService.UpdatePresenceAsync(mediaProperties, trackTimeInfo, appName);
                            }

                            if (currentImageHash != _lastImageHash)
                            {
                                _lastImageHash = currentImageHash;
                                if (imageBytes != null)
                                {
                                    await _sender.SendPlayerImageAsync(imageBytes);
                                    if (_discord)
                                    {
                                        DiscordImageGuid = Guid.NewGuid().ToString().Replace("-", "");
                                    }

                                }
                            }
                            return $"Приложение: {appName}\nТрек: {trackTitle}\nИсполнитель: {artistName}\nОбщее время: {trackTimeInfo.totalTime}\nТекущая позиция: {trackTimeInfo.currentPosition}";
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

        private static async Task<(string totalTime, string currentPosition)> GetTrackTimeInfo(GlobalSystemMediaTransportControlsSession session)
        {
            var timelineProperties = session.GetTimelineProperties();
            if (timelineProperties == null)
            {
                return (TimeSpan.Zero.ToString(@"hh\:mm\:ss"), TimeSpan.Zero.ToString(@"hh\:mm\:ss"));
            }

            var totalTime = timelineProperties.EndTime.TotalSeconds;
            var currentPosition = timelineProperties.Position.TotalSeconds;

            var totalTimeString = TimeSpan.FromSeconds(totalTime).ToString(@"hh\:mm\:ss");
            var currentPositionString = TimeSpan.FromSeconds(currentPosition).ToString(@"hh\:mm\:ss");

            return (totalTimeString, currentPositionString);
        }


    }
}
