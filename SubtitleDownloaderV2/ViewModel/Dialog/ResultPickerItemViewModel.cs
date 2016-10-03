using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace SubtitleDownloader.ViewModel.Dialog
{
    public class ResultPickerItemViewModel : ViewModelBase
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public ResultPickerItemViewModel(string name, string address)
        {
            this.Name = name;
            this.Address = address;
        }
    }
}
