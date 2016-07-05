using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SubtitleDownloaderV2.ViewModel.Dialog;

namespace SubtitleDownloaderV2.View.Dialog
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
