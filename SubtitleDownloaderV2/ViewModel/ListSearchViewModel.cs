using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SubtitleDownloader.Services;
using SubtitleDownloaderV2.Model;
using SubtitleDownloaderV2.Services;
using SubtitleDownloaderV2.Util;
using SubtitleDownloaderV2.View.Dialog;

namespace SubtitleDownloaderV2.ViewModel
{
    public class ListSearchViewModel : ViewModelBase
    {

        const bool SUCCESS = true;
        const bool FAILURE = false;

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
            get { return $"Current directory: {Settings.directoryPath}"; }
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
            AllEntries.Clear();
            if (Directory.Exists(Settings.directoryPath))
            {
                List<String> ignoredFiles = new List<String> { "desktop.ini", "Thumbs.db", "Movies", "Series" };

                foreach (var entry in Directory.GetFileSystemEntries(Settings.directoryPath))
                {
                    bool subtitleExist = false;
                    if (Settings.ignoreAlreadySubbedFolders && Directory.Exists(entry))
                    {
                        foreach (string dirEntry in Directory.GetFiles(entry))
                        {
                            if (dirEntry.EndsWith(".srt") || dirEntry.EndsWith(".sub") || dirEntry.EndsWith(".src")) //todo
                            {
                                subtitleExist = true;
                                break;
                            }
                        }
                    }

                    String fileName = Path.GetFileName(entry);
                    if (!subtitleExist && !ignoredFiles.Contains(fileName) && !fileName.EndsWith(".srt"))
                    {
                        FileEntry fileEntry = new FileEntry(entry);
                        fileEntry.DefineEntriesFromPath();

                        AllEntries.Add(fileEntry);
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Opens window to modify selected entry.
        /// </summary>
        private void ModifySelectedEntry()
        {

        }
        
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
            //TODO: ugh
            //Will need to redo how we reload information from the disk, so this will be persistent when we change views.
        }

        /// <summary>
        /// Perform the search for selected entry.
        /// </summary>
        private void DoSearch()
        {
            Progress = String.Empty;

            SubsceneParsingService webCrawler = new SubsceneParsingService();

            string[] searchResult;
            if (!SearchForTitle(webCrawler, out searchResult))
            {
                return;
            }

            string searchResultPicked = PickCorrectSearchResult(searchResult);
            if (string.IsNullOrEmpty(searchResultPicked))
            {
                return;
            }

            string correctSub;
            if (!FindMatchingSubtitle(searchResultPicked, webCrawler, out correctSub))
            {
                return;
            }

            if (DownloadSubtitle(webCrawler, correctSub))
            {
                return;
            }

            UnpackSubtitleFile();
        }

        private bool SearchForTitle(SubsceneParsingService webCrawler, out string[] searchResult)
        {
            WriteToProgressWindow("Querying for " + selectedEntry.title + "...", SUCCESS);

            webCrawler.RetrieveHtmlAtUrl("http://subscene.com/subtitles/title?q=" + selectedEntry.title + "&l=");
            searchResult = webCrawler.FindSearchResults();

            if (searchResult.Length < 1)
            {
                WriteToProgressWindow("FAILURE! Search result gave no hits...", FAILURE);
                return false;
            }

            WriteToProgressWindow("Found " + searchResult.Length + " possible results...", SUCCESS);
            return true;
        }

        private string PickCorrectSearchResult(string[] searchResult)
        {
            if (searchResult.Length == 1)
            {
                return searchResult.First();
            }

            ResultPickerView pickEntryForm = new ResultPickerView(searchResult);
            pickEntryForm.ShowDialog();

            if (pickEntryForm.getReturnValue() == -1)
            {
                pickEntryForm.Close();
                return string.Empty;
            }

            String searchResultPicked = searchResult[pickEntryForm.getReturnValue()];
            pickEntryForm.Close();

            WriteToProgressWindow("User picked " + searchResultPicked + "...", SUCCESS);

            selectedEntry.url = "http://subscene.com/subtitles/" + searchResultPicked;
            return searchResultPicked;
        }

        private bool FindMatchingSubtitle(string searchResultPicked, SubsceneParsingService webCrawler, out string correctSub)
        {
            WriteToProgressWindow("Querying for subtitles to " + searchResultPicked + "...", SUCCESS);
            webCrawler.RetrieveHtmlAtUrl("http://subscene.com/subtitles/" + searchResultPicked);
            correctSub = webCrawler.PickCorrectSubtitle(selectedEntry.release, selectedEntry.episode);

            if (correctSub.Length < 1)
            {
                WriteToProgressWindow("FAILURE! Could not find any subtitles for this release...", FAILURE);
                return false;
            }

            WriteToProgressWindow("Found possible match...", SUCCESS);
            return true;
        }

        private bool DownloadSubtitle(SubsceneParsingService webCrawler, string correctSub)
        {
            WriteToProgressWindow("Querying download page...", SUCCESS);
            webCrawler.RetrieveHtmlAtUrl("http://subscene.com/" + correctSub);
            String downloadLink = webCrawler.FindDownloadUrl();

            bool result = webCrawler.InitiateDownload("http://subscene.com" + downloadLink, selectedEntry.GetFullPath()
            );

            if (!result)
            {
                WriteToProgressWindow("FAILURE! Error downloading subtitle!", FAILURE);
                return true;
            }

            WriteToProgressWindow("SUCCESS! Subtitle downloaded!", SUCCESS);
            return false;
        }

        private void UnpackSubtitleFile()
        {
            WriteToProgressWindow("Unpacking rar file..", SUCCESS);
            
            UtilityService.UnrarFile(selectedEntry.GetFullPath());
        }

        private void WriteToProgressWindow(String message, bool success)
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
