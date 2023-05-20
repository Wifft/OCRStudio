// Copyright (c) Wifft 2023
// Wifft licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using Windows.Storage;

using OCRStudio.DataModels;

namespace OCRStudio.Helpers
{
    internal static partial class ScreenReadingHelper
    {
        public static List<string> GetScreenshoots(List<CaptureArea> captureAreas)
        {
            List<string> fileNames = new();
            foreach (CaptureArea captureArea in captureAreas) {
                Rectangle rectangle = new (captureArea.Location, captureArea.Size);
                
                string fileName = $"{captureArea.Id}.jpg";
                StorageFolder folder = ApplicationData.Current.TemporaryFolder;

                Bitmap bitmap = new(rectangle.Width, rectangle.Height, PixelFormat.Format32bppArgb);
                
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(new Point(rectangle.Left, rectangle.Top), new Point(0, 0), bitmap.Size, CopyPixelOperation.SourceCopy);

                bitmap.Save(Path.Join(folder.Path, fileName));

                fileNames.Add(fileName);

                bitmap.Dispose();
            }

            return fileNames;
        }
    }
}
