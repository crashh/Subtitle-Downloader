using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using SubtitleDownloaderV2.Model;
using SubtitleDownloaderV2.Services;
using SubtitleDownloaderV2.Util;
using SubtitleDownloaderV2.View.Dialog;

namespace SubtitleDownloaderV2.ViewModel
{
    public class ListSearchViewModel : ViewModelBase
    {
        public ICommand OpenFolderCommand { get; set; }
        public ICommand OpenBrowserCommand { get; set; }
        public ICommand ModifyEntryCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        #region Observables


        /// <summary>
        /// Displays how the search went.
        /// </summary>
        private string progress;
        public string Progress
        {
            get { return progress; }
            set { this.Set(() => this.Progress, ref this.progress, value); }

        }

        /// <summary>
        /// Returns the full path to the current directory.
        /// </summary>
        public string GetFullPath
        {
            get { return $"Current directory: {Settings.DirectoryPath}"; }
        }

        /// <summary>
        /// The last selected item in the datagrid.
        /// </summary>
        private FileEntry selectedEntry;
        public FileEntry SelectedEntry
        {
            get { return selectedEntry; }
            set
            {
                this.Set(() => this.SelectedEntry, ref this.selectedEntry, value);
                IsURLset = !string.IsNullOrEmpty(this.SelectedEntry?.url);
            }

        }

        /// <summary>
        /// All entries to display in datagrid.
        /// </summary>
        private ObservableCollection<FileEntry> allEntries;
        public ObservableCollection<FileEntry> AllEntries
        {
            get { return allEntries; }
            set { this.Set(() => this.AllEntries, ref this.allEntries, value); }
        }

        private bool isURLset;
        public bool IsURLset
        {
            get
            {
                return !string.IsNullOrEmpty(this.SelectedEntry?.url);
            }
            set { this.Set(() => this.IsURLset, ref this.isURLset, value); }
        }

        private bool showFirstColumn;
        public bool ShowFirstColumn
        {
            get { return showFirstColumn; }
            set { this.Set(() => this.ShowFirstColumn, ref this.showFirstColumn, value); }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Constructor
        /// </summary>
        public ListSearchViewModel()
        {
            this.AllEntries = new ObservableCollection<FileEntry>();

            OpenFolderCommand = new RelayCommand(OpenFolder);
            OpenBrowserCommand = new RelayCommand(OpenBrowser);
            SearchCommand = new RelayCommand(DoSearch);
            ModifyEntryCommand = new RelayCommand(DoModifyEntry);

            OnPresented();
        }

        /// <summary>
        /// Called everytime the window is displayed.
        /// </summary>
        public void OnPresented()
        {
            this.ShowFirstColumn = Settings.ShowFirstColumn;
            AllEntries.Clear();

            if (Directory.Exists(Settings.DirectoryPath))
            {
                List<String> ignoredFiles = new List<String> { "desktop.ini", "Thumbs.db", "Movies", "Series" };

                foreach (var entry in Directory.GetFileSystemEntries(Settings.DirectoryPath))
                {
                    bool subtitleExist = false;
                    if (Settings.IgnoreAlreadySubbedFolders && Directory.Exists(entry))
                    {
                        subtitleExist = LookForSubtitle(entry);
                    }


                    String fileName = Path.GetFileName(entry);
                    if (fileName == null) continue;

                    bool isDirectory = fileName.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) != fileName.Length - 4;
                    bool isCorrectFileType = ExpectedNames.FileTypeNames.Contains(fileName.Substring(fileName.Length - 4));

                    if ((!Settings.IgnoreAlreadySubbedFolders || !subtitleExist)
                        && !ignoredFiles.Contains(fileName) && (isDirectory || isCorrectFileType))
                    {
                        FileEntry fileEntry = new FileEntry(entry);
                        fileEntry.DefineEntriesFromPath();

                        AllEntries.Add(fileEntry);
                    }
                }
            }
        }

        private bool LookForSubtitle(string entry)
        {
            foreach (string dirEntry in Directory.GetFiles(entry))
            {
                if (dirEntry.EndsWith(".srt") || dirEntry.EndsWith(".sub") || dirEntry.EndsWith(".src"))
                {
                    return true;
                }
            }
            if (entry != Settings.DirectoryPath)
            {
                return Directory.GetDirectories(entry).Any(LookForSubtitle);
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Open default internet browser and last failed web lookup.
        /// </summary>
        private void OpenBrowser()
        {
            Process.Start(selectedEntry.url);
        }

        /// <summary>
        /// Open explorer window at selected entry path.
        /// </summary>
        private void OpenFolder()
        {
            Process.Start(selectedEntry.GetFullPath());
        }

        /// <summary>
        /// Open view to modify an entry.
        /// </summary>
        private void DoModifyEntry()
        {
            var main = SimpleIoc.Default.GetInstance<MainViewModel>();
            main.InputSearchCommand.Execute(null);
        }

        /// <summary>
        /// Perform the search for selected entry.
        /// </summary>
        private void DoSearch()
        {
            Progress = string.Empty;

            var subscene = new SubsceneService(selectedEntry) {WriteProgress = WriteToProgressWindow};
            Thread thread = new Thread(subscene.Search);
            thread.Start();
        }

        private void WriteToProgressWindow(string message, bool success)
        {
            if (!success)
            {
                //textBoxProgress.ForeColor = Color.Red;
            }
            Progress += message + "\r\n\r\n";
        }
        #endregion
    }
}
