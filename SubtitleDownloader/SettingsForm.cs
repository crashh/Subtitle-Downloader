using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SubtitleDownloader
{
    public partial class SettingsForm : Form
    {

        private String[] settings;
        public String thisApplicationPath { get; set; }
        public String torrentDownloadPath { get; set; }
        public List<String> expectedNames { get; set; }
        public bool ignoreAlreadySubbedFolders { get; set; }

        // Variables used to relocate window:
        // source: http://stackoverflow.com/questions/1592876/make-a-borderless-form-movable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        /// <summary>
        /// Opens settings file (or creates a new one if none exists) and sets public variables.
        /// </summary>
        public SettingsForm()
        {
            InitializeComponent();
            thisApplicationPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SubtitleDownloader\Settings";

            // Get settings file, or create new one:
            if (File.Exists(thisApplicationPath))
            {
                settings = System.IO.File.ReadAllLines(thisApplicationPath);
            }
            else
            {
                generateSettingsFile();
            }
            
            // Set loaded settings, or reset them:
            try
            {                
                loadSettingsFile();
            } catch (System.Exception e)
            {
                generateSettingsFile();
                loadSettingsFile();
            }
            
        }

        /// <summary>
        /// Generates a new settingsfile, with some preset values.
        /// Either called because non existed, or that the format was unexpected.
        /// </summary>
        private void generateSettingsFile()
        {
            System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SubtitleDownloader\");
            TextWriter settingsFile = new StreamWriter(thisApplicationPath, false);
            
            settingsFile.WriteLine("No directory path set");
            settingsFile.WriteLine("true");
            expectedNames = new List<String> {
                "KILLERS", "DIMENSION", "SPARKS", "MAJESTiC", "YIFY", "JYK", "Hive", "ROVERS", "LOL", "GHOULS", "EVO", "ETRG"};
            settingsFile.WriteLine(String.Join(",", expectedNames));
            settingsFile.Close();
            settings = System.IO.File.ReadAllLines(thisApplicationPath);
        }

        /// <summary>
        /// Reads settings and stores them into public variables.
        /// Note: These variables are read by the program to handle several requests.
        /// </summary>
        private void loadSettingsFile()
        {
            torrentDownloadPath = settings[0];
            ignoreAlreadySubbedFolders = Boolean.Parse(settings[1]);
            expectedNames = settings[2].Split(',').ToList();
            locationBox.Text = torrentDownloadPath;
            checkBoxIgnoreSubs.Checked = ignoreAlreadySubbedFolders;
        }

        /// <summary>
        /// Adds a name to the list of expected names, if not already present.
        /// </summary>
        public void addExpectedName(String name)
        {
            if (!expectedNames.Contains(name))
            {
                expectedNames.Add(name);
            }
            saveCurrentSettings();
        }

        public void saveCurrentSettings()
        {
            settings[0] = torrentDownloadPath;
            settings[1] = ignoreAlreadySubbedFolders.ToString();
            settings[2] = String.Join(",", expectedNames);
            System.IO.File.WriteAllLines(thisApplicationPath, settings);
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
            saveCurrentSettings(); // save changes to file
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Discards all changes and closes window.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            loadSettingsFile(); // reset changes in memory
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Called when the checkbox is clicked.
        /// </summary>
        private void checkBoxIgnoreSubs_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxIgnoreSubs.Checked)
            {
                ignoreAlreadySubbedFolders = true;
            }
            else
            {
                ignoreAlreadySubbedFolders = false;
            }
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

        /// <summary>
        /// Allows user to move the window, despite borderless.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsForm_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
