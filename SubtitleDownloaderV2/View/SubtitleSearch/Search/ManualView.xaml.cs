using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SubtitleDownloader.View.SubtitleSearch.Search
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class ManualView : UserControl
    {
        public ManualView()
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
