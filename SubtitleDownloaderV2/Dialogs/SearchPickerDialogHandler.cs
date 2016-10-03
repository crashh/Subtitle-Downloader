using System;
using System.Runtime.CompilerServices;
using System.Windows;
using SubtitleDownloader.ViewModel.Dialog;
using SubtitleDownloaderV2.Services;
using SubtitleDownloaderV2.View.Dialog;
using SubtitleDownloaderV2.ViewModel.Dialog;

namespace SubtitleDownloaderV2.Dialogs
{
    /// <summary>
    /// Simple dialog handler used to initiate a dialog window.
    /// </summary>
    public class SearchPickerDialogHandler
    {
        private readonly ResultPickerItemViewModel[] choices; 

        public SearchPickerDialogHandler(ResultPickerItemViewModel[] choices)
        {
            this.choices = choices;
        }

        /// <summary>
        /// Starts the dialog window, will list choices given as argument to the ctor.
        /// </summary>
        /// <param name="onSavedAction"></param>
        public void StartDialog(Action<int> onSavedAction = null)
        {
            var view = new ResultPickerView();
            var vm = new ResultPickerViewModel(choices, onSavedAction);

            view.DataContext = vm;
            view.Loaded += (sender, args) => vm.OnPresented();
            view.Owner = AppContext.MainWindow;
            view.Closing += (sender, args) => AppContext.MainWindow?.Activate();
            view.ShowDialog(); // ShowDialog will block remaining windows.
            view.Activate();
        }
    }
}