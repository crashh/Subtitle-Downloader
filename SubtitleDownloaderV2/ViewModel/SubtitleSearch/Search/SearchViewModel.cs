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
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using SubtitleDownloader.ViewModel.SubtitleSearch.Settings;
using SubtitleDownloaderV2.Model;
using SubtitleDownloaderV2.Services;
using SubtitleDownloaderV2.Util;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace SubtitleDownloader.ViewModel.SubtitleSearch.Search
{
    public class SearchViewModel : ViewModelBase
    {
        public ICommand OpenFolderCommand { get; set; }
        public ICommand OpenBrowserCommand { get; set; }
        public ICommand ModifyEntryCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand SetDirectoryCommand { get; set; }
        public ICommand ClearSearchCommand { get; set; }

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
        private string getFullPath;
        public string GetFullPath
        {
            get { return getFullPath; }
            set { this.Set(() => this.GetFullPath, ref this.getFullPath, value); }

        }

        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                this.DoSearch(value, (value?.Length ?? 0) > (searchText?.Length ?? 0));
                this.Set(() => this.SearchText, ref this.searchText, value);
            }
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


        private ObservableCollection<FileEntry> TotalEntries;

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
            ClearSearchCommand = new RelayCommand(DoClearSearch);
        }

        /// <summary>
        /// Called everytime the window is displayed.
        /// </summary>
        public void OnPresented()
        {
            this.ShowFirstColumn = Properties.SubSearchSettings.Default.ShowFirstColumn;
            this.GetFullPath = Properties.SubSearchSettings.Default.TargetDirectory;
            this.IsPathSet = string.IsNullOrWhiteSpace(GetFullPath);

            AllEntries.Clear();

            if (Directory.Exists(GetFullPath))
            {
                Task.Run(() =>
                {
                    // Fill grid content async:
                    AddDirectoryContent(AllEntries, GetFullPath);
                });
            }
            this.SelectedEntry = null;
            this.TotalEntries = AllEntries;
        }

        /// <summary>
        /// A recursive method, which traverse parent directory, and any first level sub directories it meets.
        /// </summary>
        private void AddDirectoryContent(ICollection<FileEntry> parent, string directory)
        {
            var ignoredFiles = new List<string> { "desktop", "thumbs", "movies", "series", "sample" };

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
                    
                    if (isDirectory)
                    {
                        //Check if folder already contains a subtitle:
                        if (Properties.SubSearchSettings.Default.IgnoreSubbedFolders)
                        {
                            var subtitleExist = LookForSubtitle(entry);
                            if (subtitleExist)
                            {
                                continue;
                            }
                        }

                        //Check if correct file type is present in first level of dir:
                        var dirEntries = Directory.GetFiles(entry);
                        foreach (var dirEntry in dirEntries)
                        {
                            if (ExpectedNames.FileTypeNames.Contains(Path.GetExtension(dirEntry)) == false)
                            {
                                //Content in directory is not a file type we look for.
                                continue;
                            }
                            if (isCorrectFileType)
                            {
                                isDirectoryAndContainsCorrectFileTypes = true;
                            }
                            isCorrectFileType = true;
                        }
                    }

                    var matchesIgnoredFile = ignoredFiles.Contains(fileName.ToLower().Split('.')[0]);
                    if (matchesIgnoredFile || !isCorrectFileType)
                    {
                        continue;
                    }

                    var fileEntry = new FileEntry(entry);
                    fileEntry.DefineEntriesFromPath();
                    
                    
                    if (isDirectoryAndContainsCorrectFileTypes)
                    {
                        //Add contents within directory (recursive call).
                        this.AddDirectoryContent(fileEntry.AllEntries, fileEntry.Path);
                    }

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        parent.Add(fileEntry);
                    });

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
            return entry != this.GetFullPath && Directory.GetDirectories(entry).Any(LookForSubtitle);
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
            //TODO (do this properly)
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

            var subscene = new SubsceneService(selectedEntry, WriteToProgressWindow);

            Task.Run(() =>
            {
                subscene.Search();
                this.RaisePropertyChanged(() => this.SelectedEntry);
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

        private void DoSearch(string value, bool incrementing)
        {
            if (value == null) return;
            if (value == string.Empty)
            {
                this.AllEntries = TotalEntries;
                return;
            }

            var searchFrom = incrementing ? this.AllEntries : this.TotalEntries;

            if (Properties.SubSearchSettings.Default.ShowFirstColumn)
            {
                var sorted = searchFrom.Where(x => x.Title.ToLower().Contains(value.ToLower()) || x.Filename.ToLower().Contains(value.ToLower()));
                this.AllEntries = new ObservableCollection<FileEntry>(sorted);
            }
            else
            {
                var sorted = this.TotalEntries.Where(x => x.Title.ToLower().Contains(value.ToLower()));
                this.AllEntries = new ObservableCollection<FileEntry>(sorted);
            }
        }

        private void DoClearSearch()
        {
            this.SearchText = string.Empty;
        }
        #endregion
    }
}
