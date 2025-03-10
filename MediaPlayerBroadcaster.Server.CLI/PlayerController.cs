﻿using Microsoft.AspNetCore.Mvc;
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
            if(data.TryGetValue("DiscordImageLink", out var dil))
                PlayerInfoStorage.DiscordImageLink = dil.ToString();

            return Ok(new { Status = "Данные плеера обновлены" });
        }

        [HttpPost("setwebplayerinfo")]
        public async Task<IActionResult> SetWebPlayerInfo([FromBody] JObject data)
        {
            if (data.TryGetValue("Artist", out var artist))
                PlayerInfoStorage.Artist = artist.ToString();
            if (data.TryGetValue("Track", out var track))
                PlayerInfoStorage.Track = track.ToString();
            if (data.TryGetValue("App", out var app))
                PlayerInfoStorage.App = app.ToString();
            if (data.TryGetValue("CoverUrl", out var cover))
                PlayerInfoStorage.CoverUrl = cover.ToString();
            PlayerInfoStorage.PlayerImage = null;

            return Ok(new { Status = "Данные плеера обновлены" });
        }

        [HttpGet("getplayerinfo")]
        public async Task<IActionResult> GetPlayerInfo()
        {
            var response = new Dictionary<string, string>
            {
                { "Artist", PlayerInfoStorage.Artist },
                { "Track", PlayerInfoStorage.Track },
                { "App", PlayerInfoStorage.App },
                { "CoverUrl", PlayerInfoStorage.CoverUrl }
            };
            return Ok(response);
        }

        [HttpPost("setplayerimage")]
        public async Task<IActionResult> SetPlayerImage()
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(memoryStream);
                    PlayerInfoStorage.PlayerImage = memoryStream.ToArray();
                    PlayerInfoStorage.CoverUrl = "";
                }
                return Ok(new { Status = "Изображение плеера обновлено" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = "Ошибка загрузки изображения", Error = ex.Message });
            }
        }

        [HttpGet("getplayerimage")]
        public IActionResult GetPlayerImage()
        {
            if (PlayerInfoStorage.PlayerImage == null || PlayerInfoStorage.PlayerImage.Length == 0)
            {
                return NotFound(new { Status = "Изображение отсутствует" });
            }

            return File(PlayerInfoStorage.PlayerImage, "image/jpeg");
        }
        [HttpGet("getplayerimage/{PlayerInfoStorage.DiscordImageLink}")]
        public IActionResult GetPlayerDiscordImage()
        {
            if (PlayerInfoStorage.PlayerImage == null || PlayerInfoStorage.PlayerImage.Length == 0)
            {
                return NotFound(new { Status = "Изображение отсутствует" });
            }

            return File(PlayerInfoStorage.PlayerImage, "image/jpeg");
        }
    }
}
