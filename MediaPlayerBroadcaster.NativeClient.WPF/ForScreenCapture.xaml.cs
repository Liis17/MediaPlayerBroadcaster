using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MediaPlayerBroadcaster.NativeClient.WPF
{
    public partial class ForScreenCapture : Window
    {
        public ForScreenCapture()
        {
            InitializeComponent();
            MouseDown += ForScreenCapture_MouseDown;
        }

        private void ForScreenCapture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
        public void UpdateText(string track, string artist)
        {
            TrackName.Text = track;
            ArtistName.Text = artist;

            if (track.Length < 5)
            {
                TrackName.FontSize = 65;
            }
            else if (track.Length < 20 && track.Length >= 5)
            {
                TrackName.FontSize = 40;
            }
            else if (track.Length >= 20 && track.Length <= 40)
            {
                TrackName.FontSize = 33;
            }
            else if (track.Length >= 41)
            {
                TrackName.FontSize = 25;
            }
        }

        public void UpdateImage(BitmapImage image)
        {
            ImageTrack.ImageSource = image;

        }
        public void UpdateWindow(int corner)
        {
            BackgroundTrack.CornerRadius = new CornerRadius(corner);
            BackgroundTrackShadow.CornerRadius = new CornerRadius(corner);
            UpdateLayout();
        }
        public void UpdateBackground(BitmapImage image, bool blur = false, int blurRadius = 10)
        {
            WriteableBitmap writeableBitmap = new WriteableBitmap(image);


            if (blur)
            {
                for (int i = 0; i < blurRadius; i++)
                {
                    writeableBitmap = writeableBitmap.Convolute(WriteableBitmapExtensions.KernelGaussianBlur5x5);
                }

            }
            BackgroundTrackImage.ImageSource = writeableBitmap;


        }
        public void UpdateBackground(System.Drawing.Color color)
        {

        }
    }
}
