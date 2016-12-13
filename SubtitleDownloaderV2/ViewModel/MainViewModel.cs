using System.Deployment.Application;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SubtitleDownloader.ViewModel.SubtitleSearch.Search;
using SubtitleDownloader.ViewModel.SubtitleSearch.Settings;

namespace SubtitleDownloader.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        public ICommand ListSearchCommand { get; set; }
        public ICommand InputSearchCommand { get; set; }
        public ICommand SettingsCommand { get; set; }

        private readonly SearchViewModel _searchViewModel;
        private readonly ManualViewModel _manualViewModel;
        private readonly SettingsViewModel _settingsViewModel;

        #region Observables

        private int width;
        public int Width
        {
            get { return width; }
            set
            {
                this.Set(() => this.Width, ref this.width, value);
                _settingsViewModel.Width = this.width;
            }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set
            {
                this.Set(() => this.Height, ref this.height, value);
                _settingsViewModel.Height = this.height;
            }
        }

        private string version;
        public string Version
        {
            get { return this.version; }
            set { this.Set(() => this.Version, ref this.version, value); }
        }

        private bool _isListSearchSelected;
        public bool IsListSearchSelected
        {
            get { return _isListSearchSelected; }
            set { this.Set(() => this.IsListSearchSelected, ref this._isListSearchSelected, value); }
        }

        private bool _isSettingsSelected;
        public bool IsSettingsSelected
        {
            get { return _isSettingsSelected; }
            set { this.Set(() => this.IsSettingsSelected, ref this._isSettingsSelected, value); }
        }

        private object selectedViewModel;
        public object SelectedViewModel
        {
            get { return selectedViewModel; }
            set { this.Set(() => this.SelectedViewModel, ref this.selectedViewModel, value); }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(
            SearchViewModel searchViewModel, 
            ManualViewModel manualViewModel,
            SettingsViewModel settingsViewModel)
        {
            this.Version = ApplicationDeployment.IsNetworkDeployed 
                ? $"ver. {ApplicationDeployment.CurrentDeployment.CurrentVersion}" 
                : "ver. DEBUG";

            this._searchViewModel = searchViewModel;
            this._manualViewModel = manualViewModel;
            this._settingsViewModel = settingsViewModel;

            this.Width = settingsViewModel.Width;
            this.Height = settingsViewModel.Height;

            ListSearchCommand = new RelayCommand(OpenListSearch);
            InputSearchCommand = new RelayCommand(OpenInputSearch);
            SettingsCommand = new RelayCommand(OpenSettings);
            OpenListSearch(); //Default open window
        }

        #region Methods

        /// <summary>
        /// Displays the Search View.
        /// </summary>
        private void OpenListSearch()
        {
            IsListSearchSelected = true;
            IsSettingsSelected = false;
            SelectedViewModel = _searchViewModel;
            _searchViewModel.OnPresented();
        }

        /// <summary>
        /// Displays the Search View.
        /// </summary>
        private void OpenInputSearch()
        {
            IsListSearchSelected = true;
            IsSettingsSelected = false;
            SelectedViewModel = _manualViewModel;
            _manualViewModel.OnPresented();
        }

        /// <summary>
        /// Displays the Settings View.
        /// </summary>
        private void OpenSettings()
        {
            IsListSearchSelected = false;
            IsSettingsSelected = true;
            SelectedViewModel = _settingsViewModel;
            _settingsViewModel.OnPresented();
        }

        #endregion


    }
}