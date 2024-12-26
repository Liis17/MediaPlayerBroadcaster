namespace MediaPlayerBroadcaster.Server.CLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://0.0.0.0:37474");
                    webBuilder.UseStartup<Startup>();
                })
                .Build()
                .Run();
        }
    }
}
