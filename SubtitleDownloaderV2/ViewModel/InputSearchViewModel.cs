using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SubtitleDownloaderV2.Model;
using SubtitleDownloaderV2.Services;

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
            Progress = string.Empty;

            var subscene = new SubsceneService(customEntry);
            subscene.WriteProgress = WriteToProgressWindow;
            subscene.Search();
        }

        public void OpenFileDialogBrowser()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                CustomEntry = new FileEntry(dialog.SelectedPath, customEntry.title, customEntry.release, customEntry.episode);
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
