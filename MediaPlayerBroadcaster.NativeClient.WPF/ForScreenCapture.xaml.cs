using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        }

        public void UpdateImage(BitmapImage image)
        {
            ImageTrack.ImageSource = image;
        }
        public void UpdateBackground(BitmapImage image, bool blur = false, int blurRadius = 10)
        {
            WriteableBitmap writeableBitmap = new WriteableBitmap(image);

            
            if (blur)
            {
                writeableBitmap = writeableBitmap.Convolute(WriteableBitmapExtensions.KernelGaussianBlur5x5);
                writeableBitmap = writeableBitmap.Convolute(WriteableBitmapExtensions.KernelGaussianBlur5x5);
            }
            BackgroundTrackImage.ImageSource = writeableBitmap;
            

        }
        public void UpdateBackground(System.Drawing.Color color)
        {

        }
    }
}
