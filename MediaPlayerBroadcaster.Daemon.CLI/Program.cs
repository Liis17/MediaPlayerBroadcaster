using System.Diagnostics;

using Windows.Media.Control;


namespace MediaPlayerBroadcaster.Daemon.CLI
{
    
    class Program
    {
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
                        string appName = session.SourceAppUserModelId;
                        string appNameToLower = appName.ToLower();
                        string trackTitle = mediaProperties.Title;
                        string artistName = mediaProperties.Artist;

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
                await _sender.SendPlayerInfoAsync( "Сейчас никто не играет", "Сейчас ничего не играет", "Silence");
                return null;
            }
            catch (Exception ex)
            {
                return $"Ошибка: {ex.Message}";
            }
        }
    }
}
