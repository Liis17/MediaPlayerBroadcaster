using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerBroadcaster.Daemon.CLI
{
    public class Sender
    {
        private static readonly HttpClient client = new HttpClient();
        private string ip = "127.0.0.1";
        private string port = "1025";

        public Sender(string _ip, string _port)
        {
            ip = _ip;
            port = _port;
        }
        public async Task SendPlayerInfoAsync(string artist, string track, string app)
        {
            var playerInfo = new
            {
                Artist = artist,
                Track = track,
                App = app
            };
            var jsonContent = JsonConvert.SerializeObject(playerInfo);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"http://{ip}:{port}/player/setplayerinfo", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Данные обновлены");
            }
            else
            {
                Console.WriteLine($"Ошибка: {response.StatusCode}");
            }
        }
    }
}
