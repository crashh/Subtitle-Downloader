using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubtitleDownloader
{
    public partial class PickEntry : Form
    {

        String[] entries;
        int selectedEntry;
        public String ReturnValue1 { get; set; }

        /// <summary>
        /// Constructor, initialises the window.
        /// </summary>
        public PickEntry(String[] entries)
        {
            InitializeComponent();
            this.entries = entries;
            updateListView();
        }

        /// <summary>
        /// Updates the content in listview, so user can select correct search result.
        /// </summary>
        /// <returns>Returns the correct entry picked by user as a String.</returns>
        private String[] updateListView()
        {
            listView1.Items.Clear();
            listView1.View = View.Details;

            // Retrieve and list all content in folder:
            listView1.Columns.Add("Possible picks");

            for (int i = 0; i < entries.Length; i++)
            {
                ListViewItem row = new ListViewItem();
                row.Text = entries[i];
                listView1.Items.Add(row);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            return entries;
        }

        private void PickEntry_Load(object sender, EventArgs e)
        {

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
    }
}
