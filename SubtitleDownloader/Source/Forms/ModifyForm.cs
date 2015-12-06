using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SubtitleDownloader.Services
{
    public partial class ModifyForm : Form
    {

        public string ReturnValue1 { get; set; } //dircontent
        public string ReturnValue2 { get; set; } //title
        public string ReturnValue3 { get; set; } //release
        public string ReturnValue4 { get; set; } //episode

        private readonly SettingsForm _sf;

        // Variables used to relocate window:
        // source: http://stackoverflow.com/questions/1592876/make-a-borderless-form-movable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public ModifyForm(String dircontent, String title, String release, String episode, SettingsForm sf)
        {
            InitializeComponent();
            textBox1.Text = dircontent;
            textBox2.Text = title;
            textBox3.Text = release;
            textBox4.Text = episode;

            this._sf = sf;
        }

        private void ModifyForm_Load(object sender, EventArgs e)
        {
            addBtn.Visible = false;
        }

        /// <summary>
        /// OK Button
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.ReturnValue1 = textBox1.Text;
            this.ReturnValue2 = textBox2.Text;
            this.ReturnValue3 = textBox3.Text;
            this.ReturnValue4 = textBox4.Text;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Cancel Button
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Add new release name inpit field
        /// </summary>
        private void textBoxReleaseName_TextChanged(object sender, EventArgs e)
        {
            addBtn.Visible = true;
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            _sf.AddExpectedName(textBox3.Text);
        }

        /// <summary>
        /// Allows window to be moved.
        /// </summary>
        private void ModifyForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);            }

        }
    }
}
