using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
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

        internal async Task SendPlayerImageAsync(byte[] imageBytes)
        {
            try
            {
                // Сжимаем изображение до 256x256
                byte[] resizedImage = ResizeImage(imageBytes, 256, 256);

                // Создаем содержимое для отправки
                var content = new ByteArrayContent(resizedImage);
                content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                // Отправляем POST-запрос
                HttpResponseMessage response = await client.PostAsync($"http://{ip}:{port}/player/setplayerimage", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Обложка обновлена");
                }
                else
                {
                    Console.WriteLine($"Ошибка: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки изображения: {ex.Message}");
            }
        }

        private byte[] ResizeImage(byte[] imageBytes, int width, int height)
        {
            using (var inputStream = new MemoryStream(imageBytes))
            using (var outputStream = new MemoryStream())
            {
                var image = Image.FromStream(inputStream);

                // Создаем новое изображение с нужным размером
                var resizedImage = new Bitmap(width, height);

                using (var graphics = Graphics.FromImage(resizedImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.DrawImage(image, 0, 0, width, height);
                }

                // Сохраняем изображение в формате JPEG
                resizedImage.Save(outputStream, ImageFormat.Jpeg);
                return outputStream.ToArray();
            }
        }
    }
}
