using System.ComponentModel;
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

        public ICommand SearchCommand { get; set; }
        public ICommand SettingsCommand { get; set; }

        private readonly SearchViewModel searchViewModel;
        private readonly SettingsViewModel settingsViewModel;

        #region Observables

        private bool isSearchNotSelected;
        public bool IsSearchNotSelected
        {
            get { return isSearchNotSelected; }
            set { this.Set(() => this.IsSearchNotSelected, ref this.isSearchNotSelected, value); }
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
        public MainViewModel(SearchViewModel searchViewModel, SettingsViewModel settingsViewModel)
        {
            this.searchViewModel = searchViewModel;
            this.settingsViewModel = settingsViewModel;

            isSearchNotSelected = true;
            isSettingsNotSelected = true;

            SearchCommand = new RelayCommand(OpenSearch);
            SettingsCommand = new RelayCommand(OpenSettings);
            OpenSearch(); //Default open window
        }

        #region Methods

        /// <summary>
        /// Displays the Search View.
        /// </summary>
        private void OpenSearch()
        {
            IsSearchNotSelected = false;
            IsSettingsNotSelected = true;
            SelectedViewModel = searchViewModel;
            searchViewModel.OnPresented();
        }

        /// <summary>
        /// Displays the Settings View.
        /// </summary>
        private void OpenSettings()
        {
            IsSearchNotSelected = true;
            IsSettingsNotSelected = false;
            SelectedViewModel = settingsViewModel;
            settingsViewModel.OnPresented();
        }

        #endregion


    }
}