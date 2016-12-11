using GalaSoft.MvvmLight;

namespace SubtitleDownloader.ViewModel.SubtitleSearch.Dialog
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
