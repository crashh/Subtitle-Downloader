using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using SubtitleDownloader.ViewModel.SubtitleSearch.Settings;
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
        public ICommand SetDirectoryCommand { get; set; }

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
        ///         private string progress;
        private string getFullPath;
        public string GetFullPath
        {
            get { return getFullPath; }
            set { this.Set(() => this.GetFullPath, ref this.getFullPath, value); }

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

        private bool isPathSet;
        public bool IsPathSet
        {
            get { return isPathSet; }
            set { this.Set(() => this.IsPathSet, ref this.isPathSet, value); }
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
            SetDirectoryCommand = new RelayCommand(DoSetDirectory);
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
                Task.Run(() =>
                {
                    AddDirectoryContent(AllEntries, directoryPath);
                });
            }
            this.SelectedEntry = AllEntries.FirstOrDefault();
            this.IsPathSet = string.IsNullOrWhiteSpace(SubtitleDownloaderV2.Util.Settings.DirectoryPath);
            this.GetFullPath = $"Current directory: {SubtitleDownloaderV2.Util.Settings.DirectoryPath}";
        }

        private void AddDirectoryContent(ICollection<FileEntry> parent, string directory)
        {
            //Somebody should really do something about the method.. but i said somebody...
            var ignoredFiles = new List<String> { "desktop", "thumbs", "movies", "series", "sample" };

            foreach (var entry in Directory.GetFileSystemEntries(directory))
            {
                try
                {
                    var fileName = Path.GetFileName(entry);
                    if (fileName == null) continue;

                    var isDirectory = Directory.Exists(entry);
                    var isDirectoryAndContainsCorrectFileTypes = false;
                    const int fileExtensionLength = 4;
                    var isCorrectFileType = ExpectedNames.FileTypeNames.Contains(fileName.Substring(fileName.Length - fileExtensionLength));

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
                                isDirectoryAndContainsCorrectFileTypes = true;
                            }
                            isCorrectFileType = true;
                        }
                    }


                    if ((SubtitleDownloaderV2.Util.Settings.IgnoreAlreadySubbedFolders && subtitleExist) ||
                        ignoredFiles.Contains(fileName.ToLower().Split('.')[0]) || !isCorrectFileType)
                    {
                        //Dont add this file, so continue with next.
                        continue;
                    }

                    var fileEntry = new FileEntry(entry);
                    fileEntry.DefineEntriesFromPath();

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            parent.Add(fileEntry);
                        });

                    if (isDirectoryAndContainsCorrectFileTypes)
                    {
                        //Add contents within directory.
                        this.AddDirectoryContent(fileEntry.AllEntries, fileEntry.Path);
                    }
                    else
                    {
                        if (AllEntries != parent)
                        {
                            //Set release field to the same as the parent.
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
            }
        }

        /// <summary>
        /// Open explorer window at selected entry path.
        /// </summary>
        private void OpenFolder()
        {
            try
            {
                var path = selectedEntry.GetFullPath();
                Process.Start(path);
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

        private void DoSetDirectory()
        {
            var settings = SimpleIoc.Default.GetInstance<SettingsViewModel>();
            settings.OnPresented();
            settings.OpenFileDialogBrowser();
            settings.SaveCurrentSettings();

            this.OnPresented();
        }

        /// <summary>
        /// Perform the search for selected entry.
        /// </summary>
        private void DoSearch()
        {
            Progress = string.Empty;
            Task.Run(() =>
            {
                new SubsceneService(selectedEntry)
                {
                    WriteProgress = WriteToProgressWindow
                }.Search();

                this.RaisePropertyChanged(nameof(this.SelectedEntry));
            });
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
