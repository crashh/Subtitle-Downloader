using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using SubtitleDownloader.ViewModel;

namespace SubtitleDownloaderV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Application startup point.
            InitializeComponent();

            ViewModelLocator vml = new ViewModelLocator();
            this.DataContext = ServiceLocator.Current.GetInstance<MainViewModel>();
            
            DispatcherHelper.Initialize();

            AppContext.MainWindow = this;
        }
    }
}
