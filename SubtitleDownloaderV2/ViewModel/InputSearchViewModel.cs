using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
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
    public class InputSearchViewModel : ViewModelBase
    {
        const bool SUCCESS = true;
        const bool FAILURE = false;

        public ICommand SearchCommand { get; set; }
        public ICommand OpenBrowserCommand { get; set; }
        public ICommand BrowseCommand { get; set; }

        private readonly ListSearchViewModel listSearchViewModel;

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

        private bool isURLset;
        public bool IsURLset
        {
            get
            {
                return !string.IsNullOrEmpty(this.customEntry?.url);
            }
            set { this.Set(() => this.IsURLset, ref this.isURLset, value); }
        }


        private FileEntry customEntry;

        public FileEntry CustomEntry
        {
            get
            {
                return customEntry;
            }
            set { this.Set(() => this.CustomEntry, ref this.customEntry, value); }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Constructor
        /// </summary>
        public InputSearchViewModel(ListSearchViewModel listSearchViewModel)
        {
            this.listSearchViewModel = listSearchViewModel;

            this.OpenBrowserCommand = new RelayCommand(OpenBrowser);
            this.SearchCommand = new RelayCommand(DoSearch);
            this.BrowseCommand = new RelayCommand(OpenFileDialogBrowser);

            OnPresented();
        }

        /// <summary>
        /// Called everytime the window is displayed.
        /// </summary>
        public void OnPresented()
        {
            FileEntry selectedEntry = listSearchViewModel.SelectedEntry;

            if (selectedEntry != null)
            {
                this.customEntry = new FileEntry(selectedEntry.GetFullPath(), selectedEntry.title, selectedEntry.release, selectedEntry.episode);
            }
            else
            {
                this.customEntry = new FileEntry(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "", "", "");
            }
        }

        #endregion

        #region Methods
        private void OpenBrowser()
        {
            Process.Start(customEntry.url);
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
            WriteToProgressWindow("Querying for " + customEntry.title + "...", SUCCESS);

            webCrawler.RetrieveHtmlAtUrl("http://subscene.com/subtitles/title?q=" + customEntry.title + "&l=");
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

            customEntry.url = "http://subscene.com/subtitles/" + searchResultPicked;
            return searchResultPicked;
        }

        private bool FindMatchingSubtitle(string searchResultPicked, SubsceneParsingService webCrawler, out string correctSub)
        {
            WriteToProgressWindow("Querying for subtitles to " + searchResultPicked + "...", SUCCESS);
            webCrawler.RetrieveHtmlAtUrl("http://subscene.com/subtitles/" + searchResultPicked);
            correctSub = webCrawler.PickCorrectSubtitle(customEntry.release, customEntry.episode);

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

            bool result = webCrawler.InitiateDownload("http://subscene.com" + downloadLink, customEntry.GetFullPath()
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
            
            UtilityService.UnrarFile(customEntry.GetFullPath());
        }

        private void WriteToProgressWindow(String message, bool success)
        {
            if (!success)
            {
                //textBoxProgress.ForeColor = Color.Red;
            }
            Progress += message + "\r\n\r\n";
        }

        public void OpenFileDialogBrowser()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                CustomEntry = new FileEntry(dialog.SelectedPath, customEntry.title, customEntry.release, customEntry.episode);
            }
        }

        #endregion
    }
}
