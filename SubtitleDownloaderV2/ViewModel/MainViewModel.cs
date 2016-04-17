using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;

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
        private readonly InputSearchViewModel InputSearchViewModel;
        private readonly SettingsViewModel SettingsViewModel;

        #region Observables

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
            InputSearchViewModel inputSearchViewModel,
            SettingsViewModel settingsViewModel)
        {
            this.version = "ver. " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            this.ListSearchViewModel = listSearchViewModel;
            this.InputSearchViewModel = inputSearchViewModel;
            this.SettingsViewModel = settingsViewModel;

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
            SelectedViewModel = InputSearchViewModel;
            InputSearchViewModel.OnPresented();
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