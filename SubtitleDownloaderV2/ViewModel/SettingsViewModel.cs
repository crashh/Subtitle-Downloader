using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Shapes;
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

        private string language;

        public string Language
        {
            get { return language; }
            set { this.Set(() => this.Language, ref this.language, value); }
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

        public List<string> Languages { get; set; } 

        public Rectangle Dimensions { get; set; }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsViewModel()
        {
            this.SaveCommand = new RelayCommand(SaveCurrentSettings);
            this.ResetCommand = new RelayCommand(LoadSettingsFile);
            this.BrowseCommand = new RelayCommand(OpenFileDialogBrowser);
            this.Languages = new List<string>
            {
                "Arabic", "Brazilian", "Croatian", "Dutch", "Danish", "English", "Farsi/Persian", "Finnish",
                "French", "Greek", "Italian", "Norwegian", "Indonesian", "Japanese", "Romanian", "Russian",
                "Spanish", "Turkish", "Vietnamese"
            };

            Settings.ApplicationPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SubtitleDownloader\Settings";

            OnPresented(); // Need this information immediately
        }

        /// <summary>
        /// Called everytime this view is displayed.
        /// </summary>
        public void OnPresented()
        {
            // Get _settings file, or create new one:
            if (File.Exists(Settings.ApplicationPath))
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
            TextWriter settingsFile = new StreamWriter(Settings.ApplicationPath, false);

            settingsFile.WriteLine("No directory path set");
            settingsFile.WriteLine("true");
            settingsFile.WriteLine("English");
            settingsFile.WriteLine(System.Windows.SystemParameters.PrimaryScreenHeight);
            settingsFile.WriteLine(System.Windows.SystemParameters.PrimaryScreenWidth);
            settingsFile.Close();
        }

        /// <summary>
        /// Reads _settings and stores them into public variables.
        /// Note: These variables are read by the program to handle several requests.
        /// </summary>
        private void LoadSettingsFile()
        {
            string[] settings = File.ReadAllLines(Settings.ApplicationPath);
            WorkingFolderPath = Settings.DirectoryPath = settings[0];
            IgnoreAlreadySubbedFolders = Settings.IgnoreAlreadySubbedFolders = bool.Parse(settings[1]);
            Language = Settings.Language = settings[2];
            Result = "Settings restored!";
            Dimensions = new Rectangle() {Height = double.Parse(settings[3]), Width = double.Parse(settings[4])};
        }

        /// <summary>
        /// Save current set settings to file.
        /// </summary>
        public void SaveCurrentSettings()
        {
            string[] settings = new string[3];
            settings[0] = Settings.DirectoryPath = WorkingFolderPath;
            settings[1] = IgnoreAlreadySubbedFolders.ToString();
            settings[2] = Settings.Language = Language;
            Settings.IgnoreAlreadySubbedFolders = IgnoreAlreadySubbedFolders;
            File.WriteAllLines(Settings.ApplicationPath, settings);
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
