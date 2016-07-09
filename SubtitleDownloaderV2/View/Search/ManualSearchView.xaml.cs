using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Practices.ServiceLocation;
using SubtitleDownloaderV2.ViewModel;

namespace SubtitleDownloaderV2.View
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class ManualSearchView : UserControl
    {
        public ManualSearchView()
        {
            InitializeComponent();
        }

        private void ButtonExpandProgress_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            if (button == null) return;

            button.Content = (bool)button.IsChecked ? ">>" : "<<";
        }
    }
}
