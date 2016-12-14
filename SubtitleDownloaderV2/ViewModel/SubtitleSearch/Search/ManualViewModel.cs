using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SubtitleDownloaderV2.Model;
using SubtitleDownloaderV2.Services;

namespace SubtitleDownloader.ViewModel.SubtitleSearch.Search
{
    public class ManualViewModel : ViewModelBase
    {
        public ICommand SearchCommand { get; set; }
        public ICommand OpenBrowserCommand { get; set; }
        public ICommand BrowseCommand { get; set; }

        private readonly SearchViewModel searchViewModel;

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
                return !string.IsNullOrEmpty(this.customEntry?.Url);
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
        public ManualViewModel(SearchViewModel searchViewModel)
        {
            this.searchViewModel = searchViewModel;

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
            FileEntry selectedEntry = searchViewModel.SelectedEntry;

            if (selectedEntry != null)
            {
                this.customEntry = new FileEntry(selectedEntry.GetFullPath(), selectedEntry.Title, selectedEntry.Release, selectedEntry.Episode);
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
            try
            {
                Process.Start(customEntry.Url);
            }
            catch (Exception)
            {
                //ignored
            }
        }

        /// <summary>
        /// Perform the search for selected entry.
        /// </summary>
        private void DoSearch()
        {
            Progress = string.Empty;

            var subscene = new SubsceneService(customEntry)
            {
                WriteProgress = WriteToProgressWindow
            };

            Task.Run(() => subscene.Search());
        }

        public void OpenFileDialogBrowser()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                CustomEntry = new FileEntry(dialog.SelectedPath, customEntry.Title, customEntry.Release, customEntry.Episode);
            }
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
