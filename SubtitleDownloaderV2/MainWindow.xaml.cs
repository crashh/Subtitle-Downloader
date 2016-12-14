﻿using System.Windows;
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

            AppContext.MainWindow = this;
        }
    }
}
