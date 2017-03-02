using System.Windows.Controls;
using System.Windows.Input;

namespace SubtitleDownloader.View.SubtitleSearch.Search.UserControls
{
    /// <summary>
    /// Interaction logic for ListDetails.xaml
    /// </summary>
    public partial class ListGridDetails : UserControl
    {
        public ListGridDetails()
        {
            InitializeComponent();
        }

        private void UIElement_ClearSelection(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            if (grid?.SelectedItems == null || grid.SelectedItems.Count != 1)
            {
                return;
            }
            var dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
            if (dgr != null && !dgr.IsMouseOver)
            {
                ((DataGridRow) dgr).IsSelected = false;
            }
        }
    }
}
