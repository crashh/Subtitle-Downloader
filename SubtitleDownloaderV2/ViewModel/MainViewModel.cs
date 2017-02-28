using System.Deployment.Application;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SubtitleDownloader.ViewModel.ShowTracker;
using SubtitleDownloader.ViewModel.SubtitleSearch.Search;
using SubtitleDownloader.ViewModel.SubtitleSearch.Settings;

namespace SubtitleDownloader.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public ICommand ListSearchCommand { get; set; }
        public ICommand InputSearchCommand { get; set; }
        public ICommand SettingsCommand { get; set; }
        public ICommand ShowTrackerCommand { get; set; }

        public SearchViewModel SearchViewModel { get; }
        public ManualViewModel ManualViewModel { get; }
        public SettingsViewModel SettingsViewModel { get; }
        public ShowTrackerViewModel ShowTrackerViewModel { get; }

        #region Observables

        private int _width;
        public int Width
        {
            get { return _width; }
            set
            {
                this.Set(() => this.Width, ref this._width, value);
                Properties.Settings.Default.WindowWidth = this._width;
                Properties.Settings.Default.Save();
            }
        }

        private int _height;
        public int Height
        {
            get { return _height; }
            set
            {
                this.Set(() => this.Height, ref this._height, value);
                Properties.Settings.Default.WindowHeight = this._height;
                Properties.Settings.Default.Save();
            }
        }

        private string _version;
        public string Version
        {
            get { return this._version; }
            set { this.Set(() => this.Version, ref this._version, value); }
        }

        private bool _isListSearchSelected;
        public bool IsListSearchSelected
        {
            get { return _isListSearchSelected; }
            set { this.Set(() => this.IsListSearchSelected, ref this._isListSearchSelected, value); }
        }

        private bool _isManualSearchSelected;
        public bool IsManualSearchSelected
        {
            get { return _isManualSearchSelected; }
            set { this.Set(() => this.IsManualSearchSelected, ref this._isManualSearchSelected, value); }
        }

        private bool _isSettingsSelected;
        public bool IsSettingsSelected
        {
            get { return _isSettingsSelected; }
            set { this.Set(() => this.IsSettingsSelected, ref this._isSettingsSelected, value); }
        }

        private bool _isShowTrackerSelected;
        public bool IsShowTrackerSelected
        {
            get { return _isShowTrackerSelected; }
            set { this.Set(() => this.IsShowTrackerSelected, ref this._isShowTrackerSelected, value); }
        }


        private object _selectedViewModel;
        public object SelectedViewModel
        {
            get { return _selectedViewModel; }
            set { this.Set(() => this.SelectedViewModel, ref this._selectedViewModel, value); }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(
            SearchViewModel searchViewModel, 
            ManualViewModel manualViewModel,
            SettingsViewModel settingsViewModel,
            ShowTrackerViewModel showTrackerViewModel)
        {
            this.Version = ApplicationDeployment.IsNetworkDeployed 
                ? $"ver. {ApplicationDeployment.CurrentDeployment.CurrentVersion}" 
                : "ver. DEBUG";

            this.SearchViewModel = searchViewModel;
            this.ManualViewModel = manualViewModel;
            this.SettingsViewModel = settingsViewModel;
            this.ShowTrackerViewModel = showTrackerViewModel;

            this.Width = Properties.Settings.Default.WindowWidth;
            this.Height = Properties.Settings.Default.WindowHeight;

            ListSearchCommand = new RelayCommand(OpenListSearch);
            InputSearchCommand = new RelayCommand(OpenInputSearch);
            SettingsCommand = new RelayCommand(OpenSettings);
            ShowTrackerCommand = new RelayCommand(OpenShowTracker);

            OpenListSearch(); //Default open window
        }

        #region Methods

        /// <summary>
        /// Displays the Search View.
        /// </summary>
        private void OpenListSearch()
        {
            IsListSearchSelected = true;
            IsManualSearchSelected = false;
            IsSettingsSelected = false;
            IsShowTrackerSelected = false;
            SelectedViewModel = SearchViewModel;
            SearchViewModel.OnPresented();
        }

        /// <summary>
        /// Displays the Search View.
        /// </summary>
        private void OpenInputSearch()
        {
            IsListSearchSelected = false;
            IsManualSearchSelected = true;
            IsSettingsSelected = false;
            IsShowTrackerSelected = false;
            SelectedViewModel = ManualViewModel;
            ManualViewModel.OnPresented();
        }

        /// <summary>
        /// Displays the Settings View.
        /// </summary>
        private void OpenSettings()
        {
            IsListSearchSelected = false;
            IsManualSearchSelected = false;
            IsSettingsSelected = true;
            IsShowTrackerSelected = false;
            SelectedViewModel = SettingsViewModel;
            SettingsViewModel.OnPresented();
        }

        /// <summary>
        /// Displays the Show Tracker View.
        /// </summary>
        private void OpenShowTracker()
        {
            IsListSearchSelected = false;
            IsManualSearchSelected = false;
            IsSettingsSelected = false;
            IsShowTrackerSelected = true;
            SelectedViewModel = ShowTrackerViewModel;
            ShowTrackerViewModel.OnPresented();
        }

        #endregion


    }
}