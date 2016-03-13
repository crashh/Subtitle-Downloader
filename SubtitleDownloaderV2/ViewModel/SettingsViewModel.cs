using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SubtitleDownloaderV2.Util;

namespace SubtitleDownloaderV2.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public ICommand SaveCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand BrowseCommand { get; set; }

        #region Observables

        /// <summary>
        /// The path to the directory we want to scan.
        /// </summary>
        public string workingFolderPath;
        public string WorkingFolderPath
        {
            get { return workingFolderPath; }
            set { this.Set(() => this.WorkingFolderPath, ref this.workingFolderPath, value); }
        }

        /// <summary>
        /// Should we ignore folders, which already contains subtitle files.
        /// </summary>
        private bool ignoreAlreadySubbedFolders;
        public bool IgnoreAlreadySubbedFolders
        {
            get { return ignoreAlreadySubbedFolders; }
            set { this.Set(() => this.IgnoreAlreadySubbedFolders, ref this.ignoreAlreadySubbedFolders, value); }
        }

        /// <summary>
        /// Result of the action to save or reset.
        /// </summary>
        private string result;
        public string Result
        {
            get { return result; }
            set { this.Set(() => this.Result, ref this.result, value); }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsViewModel()
        {
            this.SaveCommand = new RelayCommand(SaveCurrentSettings);
            this.ResetCommand = new RelayCommand(LoadSettingsFile);
            this.BrowseCommand = new RelayCommand(OpenFileDialogBrowser);

            Settings.applicationPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SubtitleDownloader\Settings";

            OnPresented(); // Need this information immediately
        }

        /// <summary>
        /// Called everytime this view is displayed.
        /// </summary>
        public void OnPresented()
        {
            // Get _settings file, or create new one:
            if (File.Exists(Settings.applicationPath))
            {
                try
                {
                    LoadSettingsFile();
                }
                catch (Exception e)
                {
                    GenerateSettingsFile();
                    LoadSettingsFile();
                }
            }
            else
            {
                GenerateSettingsFile();
                LoadSettingsFile();
            }

            this.Result = string.Empty;
        }

        #region Methods

        /// <summary>
        /// Generates a new settingsfile, with some preset values.
        /// Either called because non existed, or that the format was unexpected.
        /// </summary>
        private void GenerateSettingsFile()
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SubtitleDownloader\");
            TextWriter settingsFile = new StreamWriter(Settings.applicationPath, false);

            settingsFile.WriteLine("No directory path set");
            settingsFile.WriteLine("true");
            settingsFile.Close();
        }

        /// <summary>
        /// Reads _settings and stores them into public variables.
        /// Note: These variables are read by the program to handle several requests.
        /// </summary>
        private void LoadSettingsFile()
        {
            string[] settings = File.ReadAllLines(Settings.applicationPath);
            WorkingFolderPath = Settings.directoryPath = settings[0];
            IgnoreAlreadySubbedFolders = Settings.ignoreAlreadySubbedFolders = Boolean.Parse(settings[1]);
            Result = "Settings restored!";
        }

        /// <summary>
        /// Save current set settings to file.
        /// </summary>
        public void SaveCurrentSettings()
        {
            string[] settings = new string[2];
            settings[0] = Settings.directoryPath = WorkingFolderPath;
            settings[1] = IgnoreAlreadySubbedFolders.ToString();
            Settings.ignoreAlreadySubbedFolders = IgnoreAlreadySubbedFolders;
            File.WriteAllLines(Settings.applicationPath, settings);
            Result = "Settings saved!";
        }

        public void OpenFileDialogBrowser()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                WorkingFolderPath = dialog.SelectedPath;
            }
        }

        #endregion
    }
}
