using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

namespace MediaPlayerBroadcaster.Server.CLI
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        [HttpPost("setplayerinfo")]
        public async Task<IActionResult> SetPlayerInfo([FromBody] JObject data)
        {
            if (data.TryGetValue("Artist", out var artist))
                PlayerInfoStorage.Artist = artist.ToString();
            if (data.TryGetValue("Track", out var track))
                PlayerInfoStorage.Track = track.ToString();
            if (data.TryGetValue("App", out var app))
                PlayerInfoStorage.App = app.ToString();

            return Ok(new { Status = "Данные плеера обновлены" });
        }

        [HttpGet("getplayerinfo")]
        public IActionResult GetPlayerInfo()
        {
            var response = new Dictionary<string, string>
            {
                { "Artist", PlayerInfoStorage.Artist },
                { "Track", PlayerInfoStorage.Track },
                { "App", PlayerInfoStorage.App }
            };
            return Ok(response);
        }
    }
}
