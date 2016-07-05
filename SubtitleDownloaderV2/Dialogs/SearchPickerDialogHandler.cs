using System;
using System.Runtime.CompilerServices;
using System.Windows;
using SubtitleDownloaderV2.Services;
using SubtitleDownloaderV2.View.Dialog;
using SubtitleDownloaderV2.ViewModel.Dialog;

namespace SubtitleDownloaderV2.Dialogs
{
    public class SearchPickerDialogHandler
    {

        private readonly string[] choices; 

        public SearchPickerDialogHandler(string[] choices)
        {
            this.choices = choices;
        }

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