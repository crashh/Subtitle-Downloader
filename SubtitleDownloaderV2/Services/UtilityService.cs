﻿using System;
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
            if (Path.HasExtension(path))
            {
                // Path points directly to a file:
                string newPath = path.Substring(0, (path.Length - Path.GetFileName(path).Length) - 1);
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

            try
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(location + "\\autosub-pull.rar", location);
                System.IO.File.Delete(location + "\\autosub-pull.rar");
            }
            catch (IOException)
            {
                // ignored
                // 1: Subtitle file already exists.
                // 2: Zip file may be in incorrect format, leave the user to unpack it.
            }

        }
    }
}
