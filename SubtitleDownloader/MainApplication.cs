using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SubtitleDownloader
{
    public partial class MainApplication : Form
    {
        private readonly SettingsForm _settingsForm = new SettingsForm();
        private String[] _allDirectoryContents;
        private String[] _entryTitleName;
        private String[] _entryReleaseGroup;
        private String[] _entryEpisode;
        private int _selectedEntry;
        private String _latestLookUpLink;

        /// <summary>
        /// Constructor, loads in settings, updates arrays, and lists data in window.
        /// </summary>
        public MainApplication()
        {
            InitializeComponent();

            errorLabel.Text = "";
            currentPathLabel.Text = "Current path: " + _settingsForm.torrentDownloadPath;

            listViewDirContents.View = View.Details;
            listViewDirContents.Columns.Add("Content missing subtitles:");
            listViewDirContents.Columns.Add("Est. Title Name");
            listViewDirContents.Columns.Add("Est. Release Name");
            listViewDirContents.Columns.Add("Episode");
        }
        private void MainApplication_Load(object sender, EventArgs e)
        {
            UpdateDirectoryListing();
        }

        /// <summary>
        /// Helper method, updates all data in arrays, and displays the info to the listView.
        /// </summary>
        private void UpdateDirectoryListing()
        {
            listViewDirContents.ForeColor = System.Drawing.Color.Black;
            listViewDirContents.Items.Clear();

            // Retrieve and list all content in folder:
            IOParsing ioParsing = new IOParsing();
            if (Directory.Exists(_settingsForm.torrentDownloadPath))
            {
                _allDirectoryContents = ioParsing.CleanDirectoryListing(
                    Directory.GetFileSystemEntries(_settingsForm.torrentDownloadPath),
                    _settingsForm.ignoreAlreadySubbedFolders);

                _entryTitleName = ioParsing.IsolateTitleName(_allDirectoryContents);
                _entryEpisode = ioParsing.IsolateEpisodeNumber(_allDirectoryContents);
                _entryReleaseGroup = ioParsing.IsolateReleaseName(_allDirectoryContents, _settingsForm.expectedNames);

                for (int i = 0; i < _allDirectoryContents.Length; i++)
                {
                    ListViewItem row = new ListViewItem();
                    row.Text = (Path.GetFileName(_allDirectoryContents[i]));
                    row.SubItems.Add(_entryTitleName[i]);
                    row.SubItems.Add(Path.GetFileName(_entryReleaseGroup[i]));
                    row.SubItems.Add(_entryEpisode[i]);
                    listViewDirContents.Items.Add(row);
                }

                listViewDirContents.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            else
            {
                listViewDirContents.ForeColor = System.Drawing.Color.Red;
                ListViewItem row = new ListViewItem { Text = "Invalid directory, please set a directory in settings." };
                listViewDirContents.Items.Add(row);
                listViewDirContents.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        /// <summary>
        /// Called when clicking the settings option on the menu-bar.
        /// </summary>
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _settingsForm.ShowDialog();
            if (_settingsForm.DialogResult == DialogResult.OK)
            {
                UpdateDirectoryListing();
            }
        }

        /// <summary>
        /// Helper method, ONLY updates display info.
        /// Note: Currently only used after modifying an entry.
        /// </summary>
        private void UpdateDirectoryListingDisplay()
        {
            listViewDirContents.ForeColor = System.Drawing.Color.Black;
            listViewDirContents.Items.Clear();

            if (_allDirectoryContents.Length == 0)
            {
                listViewDirContents.ForeColor = System.Drawing.Color.Red;
                ListViewItem row = new ListViewItem();
                row.Text = "Nothing found. Please set a directory in settings.";
                listViewDirContents.Items.Add(row);
            }

            for (int i = 0; i < _allDirectoryContents.Length; i++)
            {
                ListViewItem row = new ListViewItem();
                row.Text = (Path.GetFileName(_allDirectoryContents[i]));
                row.SubItems.Add(_entryTitleName[i]);
                row.SubItems.Add(Path.GetFileName(_entryReleaseGroup[i]));
                row.SubItems.Add(_entryEpisode[i]);
                listViewDirContents.Items.Add(row);
            }

            listViewDirContents.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        /// <summary>
        /// Called when clicking the button, to initiate download subtitles.
        /// </summary>
        private void btnDownload_Click(object sender, EventArgs e)
        {
            const bool SUCCESS = true;
            const bool FAILURE = false;

            textBoxProgress.ForeColor = System.Drawing.Color.ForestGreen;
            textBoxProgress.Clear();

            WebCrawler webCrawler = new WebCrawler();

            // Search for the selected entry name:
            WriteToProgressWindow("Querying for " + _entryTitleName[_selectedEntry] + "...", SUCCESS);
            webCrawler.RetrieveHtmlAtUrl("http://subscene.com/subtitles/title?q=" + _entryTitleName[_selectedEntry] + "&l=");
            String[] searchResult = webCrawler.PickCorrectSearchEntry();

            // Check if search query was successfull:
            if (searchResult.Length < 1)
            {
                WriteToProgressWindow("FAILURE! Search result gave no hits...", FAILURE);
                return;
            }
            WriteToProgressWindow("Found " + searchResult.Length + " possible results...", SUCCESS);

            // Have user select correct search entry:
            PickEntryForm pickEntryForm = new PickEntryForm(searchResult);
            pickEntryForm.ShowDialog();
            String searchResultPicked = pickEntryForm.ReturnValue1;
            WriteToProgressWindow("User picked " + searchResultPicked + "...", SUCCESS);

            _latestLookUpLink = "http://subscene.com/subtitles/" + searchResultPicked;

            // Find correct subtitle:
            WriteToProgressWindow("Querying for subtitles to " + searchResultPicked + "...", SUCCESS);
            webCrawler.RetrieveHtmlAtUrl("http://subscene.com/subtitles/" + searchResultPicked);
            String correctSub = webCrawler.PickCorrectSubtitle(_entryReleaseGroup[_selectedEntry], _entryEpisode[_selectedEntry]);
            if (correctSub == null)
            {
                WriteToProgressWindow("FAILURE! Could not find any subtitles for this release...", FAILURE);
                btnOpenBrowser.Visible = true;
                return;
            }
            WriteToProgressWindow("Found possible match...", SUCCESS);

            // Initiate the download:
            WriteToProgressWindow("Querying download page...", SUCCESS);
            webCrawler.RetrieveHtmlAtUrl("http://subscene.com/" + correctSub);
            String downloadLink = webCrawler.FindDownloadUrl();
            bool result = webCrawler.InitiateDownload(
                "http://subscene.com" + downloadLink,
                _allDirectoryContents[_selectedEntry],
                _entryTitleName[_selectedEntry]
            );

            if (!result)
            {
                WriteToProgressWindow("FAILURE! Error downloading subtitle!", FAILURE);
                btnOpenBrowser.Visible = true;
                return;

            }
            WriteToProgressWindow("SUCCESS! Subtitle downloaded!", SUCCESS);
            WriteToProgressWindow("Unpacking rar file..", SUCCESS);
            webCrawler.UnrarFile(_allDirectoryContents[_selectedEntry]);
            btnOpenBrowser.Visible = true;
        }

        /// <summary>
        /// Prints the given string to textbox, if parameter false is given color will turn red.
        /// </summary>
        private void WriteToProgressWindow(String message, bool success)
        {
            if (!success)
            {
                textBoxProgress.ForeColor = System.Drawing.Color.Red;
            }
            textBoxProgress.Text += message + "\r\n\r\n";
        }

        /// <summary>
        /// Called when user selects a row in listView.
        /// </summary>
        private void listViewDirContents_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listViewDirContents.SelectedItems.Count > 0)
            {
                btnModify.Visible = true;
                btnDownload.Enabled = true;
                btnOpenFolder.Visible = true;
                _selectedEntry = listViewDirContents.SelectedItems[0].Index;
            }
            else
            {
                btnModify.Visible = false;
                btnDownload.Enabled = false;
                btnOpenFolder.Visible = false;

            }
            btnOpenBrowser.Visible = false;
        }

        /// <summary>
        /// Opens window to modify selected entry.
        /// </summary>
        private void btnModify_Click(object sender, EventArgs e)
        {
            if (_allDirectoryContents == null) return;

            ModifyForm modify = new ModifyForm(
                _allDirectoryContents[_selectedEntry], _entryTitleName[_selectedEntry], _entryReleaseGroup[_selectedEntry], _entryEpisode[_selectedEntry], _settingsForm
            );
            modify.ShowDialog();
            if (modify.DialogResult == DialogResult.OK)
            {
                _allDirectoryContents[_selectedEntry] = modify.returnValue1;
                _entryTitleName[_selectedEntry] = modify.returnValue2;
                _entryReleaseGroup[_selectedEntry] = modify.returnValue3;
                _entryEpisode[_selectedEntry] = modify.returnValue4;
                UpdateDirectoryListingDisplay();
            }
        }

        /// <summary>
        /// Called when clicking info on the menu-bar.
        /// </summary>
        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("INFO:\nThis program will automatically find the subtitles for the movie or tv-show you select. Simply go to " +
                "settings and select the folder you download to. \n\nHOWTO:\n1. Select settings and select folder with movies in it.\n2. " +
                "Select the title and click download.\n3. Subtitle will download to same location as a rar.\n\n" +
                "This program is written by Lars Thomasen. \ncrashh @ Github", "Program information", MessageBoxButtons.OK);
        }

        /// <summary>
        /// Called when clicking exit on the menu-bar.
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Open default internet browser and last failed web lookup.
        /// </summary>
        private void btnOpenBrowser_Click(object sender, EventArgs e)
        {
            Process.Start(_latestLookUpLink);
        }

        /// <summary>
        /// Open explorer window at selected entry path.
        /// </summary>
        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            IOParsing ioParsing = new IOParsing();
            String pathToFolder = ioParsing.GetPath(_allDirectoryContents[_selectedEntry]);
            Process.Start(pathToFolder);
        }
    }
}
