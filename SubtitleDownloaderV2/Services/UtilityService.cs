using System;
using System.Diagnostics;
using System.IO;

namespace SubtitleDownloaderV2.Services
{
    public static class UtilityService
    {
        /// <summary>
        /// Checks wether a given path points to a file or a folder. Always returns path to the folder.
        /// </summary>
        public static String GetPath(String path)
        {
            if (path.Contains(".mkv") || path.Contains(".mp4") || path.Contains(".avi"))
            {
                // Path points directly to a file:
                String newPath = path.Substring(0, (path.Length - Path.GetFileName(path).Length) - 1);
                return newPath + @"\";
            }
            // Path already points to a folder:
            return path;
        }

        /// <summary>
        /// Unrars the file at specified location.
        /// Note: this does not wait for download to complete, so might not always work.
        /// </summary>
        public static void UnrarFile(string location)
        {
            var pathToFolder = GetPath(location);
            
            System.IO.Compression.ZipFile.ExtractToDirectory(pathToFolder+ "\\autosub-pull.rar", pathToFolder);
            System.IO.File.Delete(pathToFolder + "\\autosub-pull.rar");
        }
    }
}
