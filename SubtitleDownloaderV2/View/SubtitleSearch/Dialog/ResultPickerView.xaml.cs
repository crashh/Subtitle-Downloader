using System.Windows;
using System.Windows.Input;

namespace SubtitleDownloader.View.SubtitleSearch.Dialog
{
    /// <summary>
    /// Interaction logic for ResultPickerView.xaml
    /// </summary>
    public partial class ResultPickerView : Window
    {

        public ResultPickerView()
        {
            InitializeComponent();
        }

        private void HideWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }
    }
}
