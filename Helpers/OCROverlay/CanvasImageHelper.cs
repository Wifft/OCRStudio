using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using Windows.Storage;

using OCRStudio.Extensions;

namespace OCRStudio.Helpers.OCROverlay
{
    public static class CanvasImageHelper
    {
        public static void GetWindowBoundsImage(Windows.OCROverlay passedWindow)
        {
            int windowWidth = (int) (passedWindow.Bounds.Width);
            int windowHeight = (int) (passedWindow.Bounds.Height);

            Point absPosPoint = passedWindow.GetAbsolutePosition();
            int thisCorrectedLeft = absPosPoint.X;
            int thisCorrectedTop = absPosPoint.Y;

            using Bitmap bitmap = new(windowWidth, windowHeight, PixelFormat.Format32bppArgb);

            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(new Point(thisCorrectedLeft, thisCorrectedTop), new Point(0, 0), bitmap.Size, CopyPixelOperation.SourceCopy);

            bitmap.Save(Path.Join(ApplicationData.Current.TemporaryFolder.Path, $"ocr_overlay_capture_{passedWindow.CurrentScreen.Handle}.bmp"), ImageFormat.Bmp);
            bitmap.Dispose();
        }
    }
}
