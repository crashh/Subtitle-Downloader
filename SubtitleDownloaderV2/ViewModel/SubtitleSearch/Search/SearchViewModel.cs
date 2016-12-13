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

namespace SubtitleDownloader.ViewModel.SubtitleSearch.Search
{
    public class SearchViewModel : ViewModelBase
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
        public string GetFullPath => $"Current directory: {SubtitleDownloaderV2.Util.Settings.DirectoryPath}";

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
        public SearchViewModel()
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
            this.ShowFirstColumn = SubtitleDownloaderV2.Util.Settings.ShowFirstColumn;

            AllEntries.Clear();

            var directoryPath = SubtitleDownloaderV2.Util.Settings.DirectoryPath;
            if (Directory.Exists(directoryPath))
            {
                AddDirectoryContent(AllEntries, directoryPath);
            }
            this.SelectedEntry = AllEntries.FirstOrDefault();
        }

        private void AddDirectoryContent(ICollection<FileEntry> parent, string directory)
        {
            var ignoredFiles = new List<String> { "desktop", "thumbs", "movies", "series", "sample" };

            foreach (var entry in Directory.GetFileSystemEntries(directory))
            {
                try
                {
                    var fileName = Path.GetFileName(entry);
                    if (fileName == null) continue;

                    var isDirectory = Directory.Exists(entry);
                    var isDirectoryAndContainsSeveralCorrectFileTypes = false;
                    var isCorrectFileType = ExpectedNames.FileTypeNames.Contains(fileName.Substring(fileName.Length - 4));

                    var subtitleExist = false;
                    if (isDirectory)
                    {
                        if (SubtitleDownloaderV2.Util.Settings.IgnoreAlreadySubbedFolders)
                        {
                            subtitleExist = LookForSubtitle(entry);
                        }

                        //Check if correct file type is present in first level of dir:
                        var dirEntries = Directory.GetFiles(entry);
                        foreach (var dirEntry in dirEntries)
                        {
                            if (ExpectedNames.FileTypeNames.Contains(Path.GetExtension(dirEntry)) == false)
                            {
                                continue;
                            }
                            if (isCorrectFileType)
                            {
                                isDirectoryAndContainsSeveralCorrectFileTypes = true;
                            }
                            isCorrectFileType = true;
                        }
                    }


                    if ((SubtitleDownloaderV2.Util.Settings.IgnoreAlreadySubbedFolders && subtitleExist) ||
                        ignoredFiles.Contains(fileName.ToLower().Split('.')[0]) || !isCorrectFileType)
                    {
                        continue;
                    }

                    FileEntry fileEntry = new FileEntry(entry);
                    fileEntry.DefineEntriesFromPath();

                    parent.Add(fileEntry);

                    if (isDirectoryAndContainsSeveralCorrectFileTypes)
                    {
                        this.AddDirectoryContent(fileEntry.AllEntries, fileEntry.Path);
                    }
                    else
                    {
                        if (AllEntries != parent)
                        {
                            var parentEntry = AllEntries.Count > 0 ? AllEntries.Last() : fileEntry;
                            fileEntry.DefineEntriesWithDefault("", parentEntry.Release ?? "", "");
                        }
                    }
                } catch(Exception)
                {
                    //ignored and continue with next (meaning that this entry will not be shown.)
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
            if (entry != SubtitleDownloaderV2.Util.Settings.DirectoryPath)
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
            try
            {
                Process.Start(selectedEntry.Url);
            }
            catch (Exception)
            {
                // ignored
                // todo:  this is sometimes triggered in release versions.
            }
        }

        /// <summary>
        /// Open explorer window at selected entry path.
        /// </summary>
        private void OpenFolder()
        {
            try
            {
                Process.Start(selectedEntry.GetFullPath());
            }
            catch (Exception)
            {
                //ignored
            }
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
                // textBoxProgress.ForeColor = Color.Red;
                // todo: convert to proper mvvm design. (using an observable flag to see from view.)
            }
            Progress += message + "\r\n\r\n";
        }
        #endregion
    }
}
