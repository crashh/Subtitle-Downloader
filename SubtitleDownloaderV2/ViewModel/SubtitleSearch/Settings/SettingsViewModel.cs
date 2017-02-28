using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SubtitleDownloaderV2.Util;

namespace SubtitleDownloader.ViewModel.SubtitleSearch.Settings
{
    public class SettingsViewModel : ViewModelBase
    {
        public ICommand SaveCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand BrowseCommand { get; set; }

        public ICommand AddReleaseNameCommand { get; set; }
        public ICommand RemoveReleaseNameCommand { get; set; }
        public ICommand AddReleaseNameSecondaryCommand { get; set; }
        public ICommand RemoveReleaseNameSecondaryCommand { get; set; }
        public ICommand AddFileTypeCommand { get; set; }
        public ICommand RemoveFileTypeCommand { get; set; }
        
        #region Observables
        public List<string> Languages { get; set; }
        

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
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsViewModel()
        {
            this.SaveCommand = new RelayCommand(SaveCurrentSettings);
            this.ResetCommand = new RelayCommand(LoadSettingsFile);
            this.BrowseCommand = new RelayCommand(OpenFileDialogBrowser);

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
        }

        /// <summary>
        /// Called everytime this view is displayed.
        /// </summary>
        public void OnPresented()
        {
            LoadSettingsFile();
        }

        #region Methods
        /// <summary>
        /// Reads _settings and stores them into public variables.
        /// Note: These variables are read by the program to handle several requests.
        /// </summary>
        private void LoadSettingsFile()
        {
            this.WorkingFolderPath          = Properties.SubSearchSettings.Default.TargetDirectory;
            this.IgnoreAlreadySubbedFolders = Properties.SubSearchSettings.Default.IgnoreSubbedFolders;
            this.ShowFirstColumn            = Properties.SubSearchSettings.Default.ShowFirstColumn;
            this.Language                   = Properties.SubSearchSettings.Default.SelectedLanguage;
            ExpectedNames.ReleaseNames          = Properties.SubSearchSettings.Default.ReleaseNames.Split(',').ToList();
            ExpectedNames.ReleaseNamesSecondary = Properties.SubSearchSettings.Default.ReleaseNamesSecondary.Split(',').ToList();
            ExpectedNames.FileTypeNames         = Properties.SubSearchSettings.Default.FileTypes.Split(',').ToList();

            this.ReleaseNames = new ObservableCollection<string>(ExpectedNames.ReleaseNames);
            this.ReleaseNamesSecondary = new ObservableCollection<string>(ExpectedNames.ReleaseNamesSecondary);
            this.FileTypes = new ObservableCollection<string>(ExpectedNames.FileTypeNames);

            Result = "Settings loaded!";
        }

        /// <summary>
        /// Save current set settings to file.
        /// </summary>
        public void SaveCurrentSettings()
        {
            Properties.SubSearchSettings.Default.TargetDirectory = this.WorkingFolderPath;
            Properties.SubSearchSettings.Default.IgnoreSubbedFolders = this.IgnoreAlreadySubbedFolders;
            Properties.SubSearchSettings.Default.ShowFirstColumn = this.ShowFirstColumn;
            Properties.SubSearchSettings.Default.SelectedLanguage = this.Language;
            Properties.SubSearchSettings.Default.ReleaseNames = string.Join(",", this.ReleaseNames);
            Properties.SubSearchSettings.Default.ReleaseNamesSecondary = string.Join(",", this.ReleaseNamesSecondary);
            Properties.SubSearchSettings.Default.FileTypes = string.Join(",", this.FileTypes);
            Properties.SubSearchSettings.Default.Save();
            Result = "Settings saved!";
        }
        #endregion

        #region Commands

        public void OpenFileDialogBrowser()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                WorkingFolderPath = dialog.SelectedPath;
            }
        }

        //public void OpenSettingsFile()
        //{
        //    Process.Start(SubtitleDownloaderV2.Util.Settings.ApplicationPath);
        //}

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
