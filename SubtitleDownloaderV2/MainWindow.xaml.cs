using System.Windows;
using Microsoft.Practices.ServiceLocation;
using SubtitleDownloaderV2.ViewModel;

namespace SubtitleDownloaderV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModelLocator vml = new ViewModelLocator();
            this.DataContext = ServiceLocator.Current.GetInstance<MainViewModel>();
            AppContext.MainWindow = this;
        }
    }
}
