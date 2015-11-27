using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubtitleDownloader
{
    public partial class Settings : Form
    {

        private String[] settings;
        public String thisApplicationPath { get; set; }
        public String torrentDownloadPath { get; set; }

        /// <summary>
        /// Opens settings file (or creates a new one if none exists) and sets public variables.
        /// </summary>
        public Settings()
        {
            InitializeComponent();
            thisApplicationPath = AppDomain.CurrentDomain.BaseDirectory;

            // Get settings file, or create new one:
            if (File.Exists(thisApplicationPath + "settings"))
            {
                settings = System.IO.File.ReadAllLines(thisApplicationPath + "settings");
            }
            else
            {
                TextWriter settingsFile = new StreamWriter(thisApplicationPath + "settings", true);
                settingsFile.Write("no directory path set");
                settingsFile.Close();
                settings = System.IO.File.ReadAllLines(thisApplicationPath + "settings");
            }
            
            torrentDownloadPath = settings[0];
            locationBox.Text = torrentDownloadPath;
        }

        /// <summary>
        /// Opens a Folder Browser Dialog, if new path selected update it, otherwise do nothing,
        /// </summary>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFD.ShowDialog() == DialogResult.OK)
            {
                torrentDownloadPath = openFD.SelectedPath;
                locationBox.Text = torrentDownloadPath;
            }
        }

        /// <summary>
        /// Saves all settings and closes window.
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            settings[0] = torrentDownloadPath;
            System.IO.File.WriteAllLines(thisApplicationPath + "settings", settings);
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Closes window.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void locationBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Settings_Load(object sender, EventArgs e)
        {

        }
    }
}
