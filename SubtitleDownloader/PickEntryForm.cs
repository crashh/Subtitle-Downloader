using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SubtitleDownloader
{
    /// <summary>
    /// A pop-up windows which serves for the user to help the program picking the correct search result.
    /// </summary>
    public partial class PickEntryForm : Form
    {
        private String[] entries;
        private int selectedEntry;
        public String ReturnValue1 { get; set; }

        // Variables used to relocate window:
        // source: http://stackoverflow.com/questions/1592876/make-a-borderless-form-movable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        /// <summary>
        /// Constructor, initialises the window.
        /// </summary>
        public PickEntryForm(String[] entries)
        {
            InitializeComponent();
            this.entries = entries;            
        }

        /// <summary>
        /// Occurs before a form is displayed for the first time.
        /// </summary>
        private void PickEntry_Load(object sender, EventArgs e)
        {
            updateListView();
        }

        /// <summary>
        /// Updates the content in listview, so user can select correct search result.
        /// </summary>
        /// <returns>Returns the correct entry picked by user as a String.</returns>
        private void updateListView()
        {
            listView1.Items.Clear();
            listView1.View = View.Details;

            // Retrieve and list all content in folder:
            listView1.Columns.Add("Possible picks");

            for (int i = 0; i < entries.Length; i++)
            {
                ListViewItem row = new ListViewItem();
                String entry = entries[i].Replace('-', ' ');
                row.Text = entry.First().ToString().ToUpper() + entry.Substring(1);
                listView1.Items.Add(row);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        /// <summary>
        /// Sets the entry as return value and terminates the window.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            this.ReturnValue1 = entries[selectedEntry];
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Listener that will be run when the user selects something in the window.
        /// </summary>
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                selectedEntry = listView1.SelectedItems[0].Index;
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
