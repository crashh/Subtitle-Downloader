using System.Windows;
using System.Windows.Input;
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

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Maximize_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                return;
            }

            this.WindowState = WindowState.Maximized;
        }

        private void Minimize_OnClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void WindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Maximize_OnClick(sender, e);
                return;
            }

            DragMove();
        }
    }
}
