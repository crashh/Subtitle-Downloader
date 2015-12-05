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

        private String[] _settings;
        public String ThisApplicationPath { get; set; }
        public String WorkingFolderPath { get; set; }
        public List<String> ExpectedNames { get; set; }
        public bool IgnoreAlreadySubbedFolders { get; set; }

        // Variables used to relocate window:
        // source: http://stackoverflow.com/questions/1592876/make-a-borderless-form-movable
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        /// <summary>
        /// Opens _settings file (or creates a new one if none exists) and sets public variables.
        /// </summary>
        public SettingsForm()
        {
            InitializeComponent();
            ThisApplicationPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SubtitleDownloader\Settings";

            // Get _settings file, or create new one:
            if (File.Exists(ThisApplicationPath))
            {
                _settings = File.ReadAllLines(ThisApplicationPath);
            }
            else
            {
                GenerateSettingsFile();
            }
            
            // Set loaded _settings, or reset them:
            try
            {                
                LoadSettingsFile();
            } catch (Exception e)
            {
                GenerateSettingsFile();
                LoadSettingsFile();
            }
        }

        private void Settings_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Generates a new settingsfile, with some preset values.
        /// Either called because non existed, or that the format was unexpected.
        /// </summary>
        private void GenerateSettingsFile()
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\SubtitleDownloader\");
            TextWriter settingsFile = new StreamWriter(ThisApplicationPath, false);
            
            settingsFile.WriteLine("No directory path set");
            settingsFile.WriteLine("true");
            ExpectedNames = new List<String> {
                "KILLERS", "DIMENSION", "SPARKS", "MAJESTiC", "YIFY", "JYK", "Hive", "ROVERS", "LOL", "GHOULS", "EVO", "ETRG"};
            settingsFile.WriteLine(String.Join(",", ExpectedNames));
            settingsFile.Close();
            _settings = File.ReadAllLines(ThisApplicationPath);
        }

        /// <summary>
        /// Reads _settings and stores them into public variables.
        /// Note: These variables are read by the program to handle several requests.
        /// </summary>
        private void LoadSettingsFile()
        {
            WorkingFolderPath = _settings[0];
            IgnoreAlreadySubbedFolders = Boolean.Parse(_settings[1]);
            ExpectedNames = _settings[2].Split(',').ToList();
            locationBox.Text = WorkingFolderPath;
            checkBoxIgnoreSubs.Checked = IgnoreAlreadySubbedFolders;
        }

        /// <summary>
        /// Adds a name to the list of expected names, if not already present.
        /// </summary>
        public void AddExpectedName(String name)
        {
            if (!ExpectedNames.Contains(name))
            {
                ExpectedNames.Add(name);
            }
            SaveCurrentSettings();
        }

        public void SaveCurrentSettings()
        {
            _settings[0] = WorkingFolderPath;
            _settings[1] = IgnoreAlreadySubbedFolders.ToString();
            _settings[2] = String.Join(",", ExpectedNames);
            File.WriteAllLines(ThisApplicationPath, _settings);
        }

        /// <summary>
        /// Opens a Folder Browser Dialog, if new path selected update it, otherwise do nothing,
        /// </summary>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFD.ShowDialog() == DialogResult.OK)
            {
                WorkingFolderPath = openFD.SelectedPath;
                locationBox.Text = WorkingFolderPath;
            }
        }

        /// <summary>
        /// Saves all _settings and closes window.
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveCurrentSettings(); // save changes to file
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Discards all changes and closes window.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            LoadSettingsFile(); // reset changes in memory
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Called when the checkbox is clicked.
        /// </summary>
        private void checkBoxIgnoreSubs_CheckedChanged(object sender, EventArgs e)
        {
            IgnoreAlreadySubbedFolders = checkBoxIgnoreSubs.Checked;
        }

        /// <summary>
        /// Allows user to move the window, despite borderless.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsForm_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
    }
}
