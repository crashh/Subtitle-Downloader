using System;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SubtitleDownloader.Services
{
    /// <summary>
    /// A pop-up windows which serves for the user to help the program picking the correct search result.
    /// </summary>
    public partial class PickEntryForm : Form
    {
        private readonly String[] _entries;
        private int _selectedEntry;
        public String ReturnValue1 { get; set; }

        // Variables used to relocate window:
        // source: http://stackoverflow.com/questions/1592876/make-a-borderless-form-movable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        /// <summary>
        /// Constructor, initialises the window.
        /// </summary>
        public PickEntryForm(String[] entries)
        {
            InitializeComponent();
            this._entries = entries;            
        }

        /// <summary>
        /// Occurs before a form is displayed for the first time.
        /// </summary>
        private void PickEntry_Load(object sender, EventArgs e)
        {
            UpdateListView();
        }

        /// <summary>
        /// Updates the content in listview, so user can select correct search result.
        /// </summary>
        /// <returns>Returns the correct entry picked by user as a String.</returns>
        private void UpdateListView()
        {
            listViewEntryListing.Items.Clear();
            listViewEntryListing.View = View.Details;

            // Retrieve and list all content in folder:
            listViewEntryListing.Columns.Add("Possible picks");

            foreach (string elem in _entries)
            {
                ListViewItem row = new ListViewItem();
                String entry = elem.Replace('-', ' ');
                row.Text = entry.First().ToString().ToUpper() + entry.Substring(1);
                listViewEntryListing.Items.Add(row);
            }

            listViewEntryListing.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        /// <summary>
        /// Sets the entry as return value and terminates the window.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            this.ReturnValue1 = _entries[_selectedEntry];
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Listener that will be run when the user selects something in the window.
        /// </summary>
        private void listViewEntryListing_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewEntryListing.SelectedItems.Count > 0)
            {
                _selectedEntry = listViewEntryListing.SelectedItems[0].Index;
                btnOK.Enabled = true;
            }
            else
            {
                btnOK.Enabled = false;
            }
        }

        private void PickEntryForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
