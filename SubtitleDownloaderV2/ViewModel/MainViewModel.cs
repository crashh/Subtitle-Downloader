using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using System.Deployment.Application;

namespace SubtitleDownloaderV2.ViewModel
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

        private readonly ListSearchViewModel ListSearchViewModel;
        private readonly ManualSearchViewModel _manualSearchViewModel;
        private readonly SettingsViewModel SettingsViewModel;

        #region Observables

        private int width;
        public int Width
        {
            get { return width; }
            set
            {
                this.Set(() => this.Width, ref this.width, value);
                SettingsViewModel.Width = this.width;
            }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set
            {
                this.Set(() => this.Height, ref this.height, value);
                SettingsViewModel.Height = this.height;
            }
        }

        private string version;
        public string Version
        {
            get { return this.version; }
            set { this.Set(() => this.Version, ref this.version, value); }
        }

        private bool isListSearchNotSelected;
        public bool IsListSearchNotSelected
        {
            get { return isListSearchNotSelected; }
            set { this.Set(() => this.IsListSearchNotSelected, ref this.isListSearchNotSelected, value); }
        }

        private bool isInputSearchNotSelected;
        public bool IsInputSearchNotSelected
        {
            get { return isInputSearchNotSelected; }
            set { this.Set(() => this.IsInputSearchNotSelected, ref this.isInputSearchNotSelected, value); }
        }

        private bool isSettingsNotSelected;
        public bool IsSettingsNotSelected
        {
            get { return isSettingsNotSelected; }
            set { this.Set(() => this.IsSettingsNotSelected, ref this.isSettingsNotSelected, value); }
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
            ListSearchViewModel listSearchViewModel, 
            ManualSearchViewModel manualSearchViewModel,
            SettingsViewModel settingsViewModel)
        {
            this.Version = ApplicationDeployment.IsNetworkDeployed 
                ? $"ver. {ApplicationDeployment.CurrentDeployment.CurrentVersion}" 
                : "ver. DEBUG";

            this.ListSearchViewModel = listSearchViewModel;
            this._manualSearchViewModel = manualSearchViewModel;
            this.SettingsViewModel = settingsViewModel;

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
            IsListSearchNotSelected = false;
            IsInputSearchNotSelected = true;
            IsSettingsNotSelected = true;
            SelectedViewModel = ListSearchViewModel;
            ListSearchViewModel.OnPresented();
        }

        /// <summary>
        /// Displays the Search View.
        /// </summary>
        private void OpenInputSearch()
        {
            IsListSearchNotSelected = true;
            IsInputSearchNotSelected = false;
            IsSettingsNotSelected = true;
            SelectedViewModel = _manualSearchViewModel;
            _manualSearchViewModel.OnPresented();
        }

        /// <summary>
        /// Displays the Settings View.
        /// </summary>
        private void OpenSettings()
        {
            IsListSearchNotSelected = true;
            IsInputSearchNotSelected = true;
            IsSettingsNotSelected = false;
            SelectedViewModel = SettingsViewModel;
            SettingsViewModel.OnPresented();
        }

        #endregion


    }
}