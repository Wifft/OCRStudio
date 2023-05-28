using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

using OCRStudio.Extensions;

using WinUIEx;

namespace OCRStudio.Helpers.OCROverlay
{
    public static class CanvasImageHelper
    {
        public static ImageSource GetWindowBoundsImage(WindowEx passedWindow)
        {
            int windowWidth = (int) (passedWindow.Bounds.Width);
            int windowHeight = (int) (passedWindow.Bounds.Height);

            Point absPosPoint = passedWindow.GetAbsolutePosition();
            int thisCorrectedLeft = absPosPoint.X;
            int thisCorrectedTop = absPosPoint.Y;

            using Bitmap bitmap = new(windowWidth, windowHeight, PixelFormat.Format32bppArgb);

            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(new Point(thisCorrectedLeft, thisCorrectedTop), new Point(0, 0), bitmap.Size, CopyPixelOperation.SourceCopy);

            return BitmapToImageSource(bitmap);
        }

        private static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new();

            using (MemoryStream stream = new()) {
                bitmap.Save(stream, ImageFormat.Bmp);
                stream.Position = 0;

                bitmapImage.SetSource(stream.AsRandomAccessStream());
            }

            return bitmapImage;
        }
    }
}
