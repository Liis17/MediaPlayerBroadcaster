using DiscordRPC;
using DiscordRPC.Logging;

using MediaPlayerBroadcaster.Daemon.CLI;

using System;
using System.Threading.Tasks;

using Windows.Media.Control;

public class DiscordService : IDisposable
{
    private DiscordRpcClient _client;
    private string _applicationId;
    private bool _enable;
    private bool _showButtons;
    private bool _overrideDeepLinksExperiment;
    private bool _showGitHubButton;
    private int _afkTimeout;
    private bool _showAlbum;
    private bool _showSmallIcon;

    private string server = string.Empty;

    public DiscordService(string applicationId, string _server, bool enable = true, bool showButtons = true, bool overrideDeepLinksExperiment = true, bool showGitHubButton = true, int afkTimeout = 15, bool showAlbum = true, bool showSmallIcon = true)
    {
        _applicationId = applicationId;
        _enable = enable;
        _showButtons = showButtons;
        _overrideDeepLinksExperiment = overrideDeepLinksExperiment;
        _showGitHubButton = showGitHubButton;
        _afkTimeout = afkTimeout;
        _showAlbum = showAlbum;
        _showSmallIcon = showSmallIcon;
        server = _server;

        if (_enable)
        {
            _client = new DiscordRpcClient(_applicationId)
            {
                Logger = new ConsoleLogger(LogLevel.Warning, true)
            };
        }
    }

    public async Task InitializeAsync()
    {
        if (_enable)
        {
            _client.Initialize();
            _client.RegisterUriScheme();
            _client.SetSubscription(EventType.Join);
            await Task.Delay(5000); 
        }
    }

    public async Task UpdatePresenceAsync(GlobalSystemMediaTransportControlsSessionMediaProperties mediaProperties, (string totalTime, string currentPosition) trackTimeInfo, string app)
    {
        if (!_enable) return;

        var artist = mediaProperties.Artist;
        var trackName = mediaProperties.Title;
        var totalTime = TimeSpan.Parse(trackTimeInfo.totalTime).TotalSeconds;
        var currentPosition = TimeSpan.Parse(trackTimeInfo.currentPosition).TotalSeconds;

        var timestamp = DateTime.UtcNow;
        var startTime = timestamp.AddSeconds(-currentPosition);
        var endTime = timestamp.AddSeconds(totalTime - currentPosition);

        var presence = new RichPresence
        {
            Details = $"{trackName}",
            State = $"{artist}",
            Assets = new Assets
            {
                LargeImageKey = $"{server}/player/getplayerimage/{Program.DiscordImageGuid}",
                LargeImageText = app,
                SmallImageKey = /*_showSmallIcon ? $"http://{ip}:{port}/player/getplayerimage/{Program.DiscordImageGuid}" : */null,
                SmallImageText = /*_showSmallIcon ? "Small Image Text" :*/ null
            },
            Timestamps = new Timestamps
            {
                Start = startTime,
                End = endTime
            }
        };

        _client.SetPresence(presence);
        _client.Invoke(); 
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
