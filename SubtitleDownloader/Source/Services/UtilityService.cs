using System;
using System.Diagnostics;
using System.IO;

namespace SubtitleDownloader.Services
{
    class UtilityService
    {
        /// <summary>
        /// Checks wether a given path points to a file or a folder. Always returns path to the folder.
        /// </summary>
        public String GetPath(String path)
        {
            if (path.Contains(".mkv") || path.Contains(".mp4") || path.Contains(".avi"))
            {
                // Path points directly to a file:
                return path.Substring(0, (path.Length - Path.GetFileName(path).Length) - 1);
            }
            // Path already points to a folder:
            return path;
        }

        /// <summary>
        /// Unrars the file at specified location.
        /// Note: this does not wait for download to complete, so might not always work.
        /// </summary>
        public void UnrarFile(String location)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            UtilityService utilSerivce = new UtilityService();
            String pathToFolder = utilSerivce.GetPath(location);

            // TODO: This assumes program is located on C:
            if (pathToFolder.Substring(0, 1) != "C")
            {
                cmd.StandardInput.WriteLine(pathToFolder.Substring(0, 1) + ":");
            }

            cmd.StandardInput.WriteLine("cd \"" + pathToFolder + "\"");
            cmd.StandardInput.WriteLine("unzip \"autosub-pull.rar\"");
            cmd.StandardInput.WriteLine("rm \"autosub-pull.rar\"");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Console.WriteLine(cmd.StandardOutput.ReadToEnd());
        }
    }
}
