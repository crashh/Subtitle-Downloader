using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public ICommand OpenFileCommand { get; set; }

        public ICommand AddReleaseNameCommand { get; set; }
        public ICommand RemoveReleaseNameCommand { get; set; }
        public ICommand AddReleaseNameSecondaryCommand { get; set; }
        public ICommand RemoveReleaseNameSecondaryCommand { get; set; }
        public ICommand AddFileTypeCommand { get; set; }
        public ICommand RemoveFileTypeCommand { get; set; }



        #region Observables
        public List<string> Languages { get; set; }

        public int Width
        {
            get;
            set;
        }
        public int Height { get; set; }

        /// <summary>
        /// Selected language to look for subtitles in.
        /// </summary>
        private string language;
        public string Language
        {
            get { return language; }
            set { this.Set(() => this.Language, ref this.language, value); }
        }

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

        private bool showFirstColumn;
        public bool ShowFirstColumn
        {
            get { return showFirstColumn; }
            set { this.Set(() => this.ShowFirstColumn, ref this.showFirstColumn, value); }
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

        private string selectedReleaseName;
        public string SelectedReleaseName
        {
            get { return selectedReleaseName; }
            set { this.Set(() => this.SelectedReleaseName, ref this.selectedReleaseName, value); }
        }

        private ObservableCollection<string> releaseNames;
        public ObservableCollection<string> ReleaseNames
        {
            get { return releaseNames; }
            set { this.Set(() => this.ReleaseNames, ref this.releaseNames, value); }
        }

        private string selectedReleaseNameSecondary;
        public string SelectedReleaseNameSecondary
        {
            get { return selectedReleaseNameSecondary; }
            set { this.Set(() => this.SelectedReleaseNameSecondary, ref this.selectedReleaseNameSecondary, value); }
        }

        private ObservableCollection<string> releaseNamesSecondary;
        public ObservableCollection<string> ReleaseNamesSecondary
        {
            get { return releaseNamesSecondary; }
            set { this.Set(() => this.ReleaseNamesSecondary, ref this.releaseNamesSecondary, value); }
        }

        private string selectedFileType;
        public string SelectedFileType
        {
            get { return selectedFileType; }
            set { this.Set(() => this.SelectedFileType, ref this.selectedFileType, value); }
        }

        private ObservableCollection<string> fileTypes;
        public ObservableCollection<string> FileTypes
        {
            get { return fileTypes; }
            set { this.Set(() => this.FileTypes, ref this.fileTypes, value); }
        }
        
        public bool ChangePerformed => 
            Settings.IgnoreAlreadySubbedFolders != this.IgnoreAlreadySubbedFolders ||
            Settings.ShowFirstColumn != this.ShowFirstColumn ||
            string.CompareOrdinal(Settings.DirectoryPath, this.WorkingFolderPath) != 0 ||
            string.CompareOrdinal(Settings.Language, this.Language) != 0 ||
            ExpectedNames.ReleaseNames.Except(this.ReleaseNames).Any() ||
            this.ReleaseNames.Except(ExpectedNames.ReleaseNames).Any();
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsViewModel()
        {
            this.SaveCommand = new RelayCommand(SaveCurrentSettings);
            this.ResetCommand = new RelayCommand(LoadSettingsFile);
            this.BrowseCommand = new RelayCommand(OpenFileDialogBrowser);
            this.OpenFileCommand = new RelayCommand(OpenSettingsFile);

            this.AddReleaseNameCommand = new RelayCommand(DoAddReleaseName);
            this.RemoveReleaseNameCommand = new RelayCommand(DoRemoveReleaseName);
            this.AddReleaseNameSecondaryCommand = new RelayCommand(DoAddReleaseNameSecondary);
            this.RemoveReleaseNameSecondaryCommand = new RelayCommand(DoRemoveReleaseNameSecondary);
            this.AddFileTypeCommand = new RelayCommand(DoAddFileType);
            this.RemoveFileTypeCommand = new RelayCommand(DoRemoveFileType);

            this.Languages = new List<string>
            {
                "Arabic", "Brazilian", "Brazillian Portuguese", "Croatian", "Dutch", "Danish", "English", "Farsi/Persian", "Finnish",
                "French", "Finnish", "Greek", "Italian", "Norwegian", "Indonesian", "Italian", "Norwegian", "Japanese", "Romanian", "Russian", "Romanian",
                "Spanish", "Swedish", "Turkish", "Vietnamese"
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
                    this.Result = "Settings loaded from file.";
                }
                catch (Exception)
                {
                    GenerateSettingsFile();
                    LoadSettingsFile();
                    this.Result = "Failed to retrieve settings, all settings reset to default.";
                }
            }
            else
            {
                GenerateSettingsFile();
                LoadSettingsFile();
                this.Result = "No settings file found, all settings reset to default.";
            }
        }

        #region Methods

        /// <summary>
        /// Generates a new settingsfile, with some preset values.
        /// Either called because non existed, or that the format was unexpected.
        /// </summary>
        private void GenerateSettingsFile()
        {
            //TODO: Find out how not to clear previous settings.
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SubtitleDownloader\");
            TextWriter settingsFile = new StreamWriter(Settings.ApplicationPath, false);

            settingsFile.WriteLine("No directory path set");
            settingsFile.WriteLine("true");
            settingsFile.WriteLine("true");
            settingsFile.WriteLine("English");
            settingsFile.WriteLine(string.Join(",", ExpectedNames.ReleaseNames));
            settingsFile.WriteLine(string.Join(",", ExpectedNames.ReleaseNamesSecondary));
            settingsFile.WriteLine(string.Join(",", ExpectedNames.FileTypeNames));
            settingsFile.WriteLine("1000");
            settingsFile.WriteLine("700");
            settingsFile.WriteLine(string.Join(",", ExpectedNames.FileTypeNames));
            settingsFile.Close();
        }

        /// <summary>
        /// Reads _settings and stores them into public variables.
        /// Note: These variables are read by the program to handle several requests.
        /// </summary>
        private void LoadSettingsFile()
        {
            string[] settings = File.ReadAllLines(Settings.ApplicationPath);

            this.WorkingFolderPath          = Settings.DirectoryPath              = settings[0];
            this.IgnoreAlreadySubbedFolders = Settings.IgnoreAlreadySubbedFolders = bool.Parse(settings[1]);
            this.ShowFirstColumn            = Settings.ShowFirstColumn            = bool.Parse(settings[2]);
            this.Language                   = Settings.Language                   = settings[3];
            ExpectedNames.ReleaseNames          = settings[4].Split(',').ToList();
            ExpectedNames.ReleaseNamesSecondary = settings[5].Split(',').ToList();
            ExpectedNames.FileTypeNames         = settings[6].Split(',').ToList();
            this.Width = int.Parse(settings[7]);
            this.Height = int.Parse(settings[8]);

            this.ReleaseNames = new ObservableCollection<string>(ExpectedNames.ReleaseNames);
            this.ReleaseNamesSecondary = new ObservableCollection<string>(ExpectedNames.ReleaseNamesSecondary);
            this.FileTypes = new ObservableCollection<string>(ExpectedNames.FileTypeNames);

            Result = "Settings restored!";
        }

        /// <summary>
        /// Save current set settings to file.
        /// </summary>
        public void SaveCurrentSettings()
        {
            string[] settings = new string[9];

            settings[0] = this.WorkingFolderPath;
            settings[1] = this.IgnoreAlreadySubbedFolders.ToString();
            settings[2] = this.ShowFirstColumn.ToString();
            settings[3] = this.Language;
            settings[4] = string.Join(",", this.ReleaseNames);
            settings[5] = string.Join(",", this.ReleaseNamesSecondary);
            settings[6] = string.Join(",", this.FileTypes);
            settings[7] = this.Width.ToString();
            settings[8] = this.Height.ToString();

            File.WriteAllLines(Settings.ApplicationPath, settings);
            Result = "Settings saved!";

            // Load back in settings, in case user manually edited file too.
            settings = File.ReadAllLines(Settings.ApplicationPath);
            Settings.DirectoryPath              = settings[0];
            Settings.IgnoreAlreadySubbedFolders = bool.Parse(settings[1]);
            Settings.ShowFirstColumn            = bool.Parse(settings[2]);
            Settings.Language                   = settings[3];
            ExpectedNames.ReleaseNames          = settings[4].Split(',').ToList();
            ExpectedNames.ReleaseNamesSecondary = settings[5].Split(',').ToList();
            ExpectedNames.FileTypeNames         = settings[6].Split(',').ToList();
        }

        #endregion

        #region Commands

        public void OpenFileDialogBrowser()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                WorkingFolderPath = dialog.SelectedPath;
            }
        }

        public void OpenSettingsFile()
        {
            Process.Start(Settings.ApplicationPath);
        }

        public void DoAddReleaseName()
        {
            this.ReleaseNames.Add(this.SelectedReleaseName);
            this.SelectedReleaseName = null;
        }

        public void DoRemoveReleaseName()
        {
            this.ReleaseNames.Remove(this.SelectedReleaseName);
        }


        public void DoAddReleaseNameSecondary()
        {
            this.ReleaseNamesSecondary.Add(this.SelectedReleaseName);
            this.SelectedReleaseNameSecondary = null;
        }

        public void DoRemoveReleaseNameSecondary()
        {
            this.ReleaseNamesSecondary.Remove(this.SelectedReleaseName);
        }

        public void DoAddFileType()
        {
            this.FileTypes.Add(this.SelectedFileType);
            this.SelectedFileType = null;
        }

        public void DoRemoveFileType()
        {
            this.FileTypes.Remove(this.SelectedFileType);
        }

        #endregion
    }
}
