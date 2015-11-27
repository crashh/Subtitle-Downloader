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
    public partial class ModifyForm : Form
    {

        public string returnValue1 { get; set; } //dircontent
        public string returnValue2 { get; set; } //title
        public string returnValue3 { get; set; } //release
        public string returnValue4 { get; set; } //episode

        private SettingsForm sf;

        // Variables used to relocate window:
        // source: http://stackoverflow.com/questions/1592876/make-a-borderless-form-movable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public ModifyForm(String dircontent, String title, String release, String episode, SettingsForm sf)
        {
            InitializeComponent();
            textBox1.Text = dircontent;
            textBox2.Text = title;
            textBox3.Text = release;
            textBox4.Text = episode;

            this.sf = sf;
        }

        private void ModifyForm_Load(object sender, EventArgs e)
        {
            addBtn.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.returnValue1 = textBox1.Text;
            this.returnValue2 = textBox2.Text;
            this.returnValue3 = textBox3.Text;
            this.returnValue4 = textBox4.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            addBtn.Visible = true;
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            sf.addExpectedName(textBox3.Text);
        }

        private void ModifyForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);            }

        }
    }
}
