using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.Storage;

namespace OCRStudio.Helpers
{
    public static partial class TempFolderHelper
    {
        public static async Task ClearTempFolder()
        {
            IReadOnlyList<StorageFile> tempFolder = await ApplicationData.Current.TemporaryFolder.GetFilesAsync();
            foreach (StorageFile file in tempFolder) await file.DeleteAsync();
        }
    }
}
