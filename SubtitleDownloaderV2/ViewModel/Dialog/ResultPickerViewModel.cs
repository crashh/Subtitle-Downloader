using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SubtitleDownloader.ViewModel.Dialog;

namespace SubtitleDownloaderV2.ViewModel.Dialog
{
    class ResultPickerViewModel : ViewModelBase
    {
        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public ResultPickerItemViewModel[] AllResults { get; set; }

        public int SelectedEntry { get; set; }

        private readonly Action<int> onSavedAction;

        public ResultPickerViewModel(ResultPickerItemViewModel[] results, Action<int> onSavedAction)
        {
            this.AllResults = results;
            this.SelectedEntry = 0;

            this.onSavedAction = onSavedAction;

            this.OkCommand = new RelayCommand(DoOkCommand);
            this.CancelCommand = new RelayCommand(DoCancelCommand);
        }

        private void DoOkCommand()
        {
            this.onSavedAction.Invoke(SelectedEntry);
        }

        private void DoCancelCommand()
        {
            this.onSavedAction.Invoke(-1);
        }

        public void OnPresented()
        {
            
        }
    }
}
