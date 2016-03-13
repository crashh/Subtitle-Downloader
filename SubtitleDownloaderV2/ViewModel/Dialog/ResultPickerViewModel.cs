using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SubtitleDownloaderV2.ViewModel.Dialog
{
    class ResultPickerViewModel : ViewModelBase
    {
        public int returnValue;

        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public string[] AllResults { get; set; }

        public int SelectedEntry { get; set; }

        public ResultPickerViewModel(string[] results)
        {
            this.AllResults = results;
            this.SelectedEntry = 0;
            this.returnValue = -1;

            this.OkCommand = new RelayCommand(DoOkCommand);
            this.CancelCommand = new RelayCommand(DoCancelCommand);
        }

        private void DoOkCommand()
        {
            returnValue = SelectedEntry;
        }

        private void DoCancelCommand()
        {
            returnValue = -1;
        }
    }
}
