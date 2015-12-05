using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using SubtitleDownloader.Services;

namespace SubtitleDownloader
{
    public partial class MainApplication : Form
    {
        const bool SUCCESS = true;
        const bool FAILURE = false;

        private readonly SettingsForm settingsForm = new SettingsForm();

        private String[] _allDirectoryContents;
        private String[] _entryTitleName;
        private String[] _entryReleaseGroup;
        private String[] _entryEpisode;

        private int _selectedEntryInListView;
        private String _latestLookUpLink;

        public MainApplication()
        {
            InitializeComponent();

            errorLabel.Text = "";
            currentPathLabel.Text = "Current path: " + settingsForm.WorkingFolderPath;

            listViewDirContents.View = View.Details;
            listViewDirContents.Columns.Add("Content missing subtitles:");
            listViewDirContents.Columns.Add("Est. Title Name");
            listViewDirContents.Columns.Add("Est. Release Name");
            listViewDirContents.Columns.Add("Episode");
        }

        private void MainApplication_Load(object sender, EventArgs e)
        {
            UpdateDirectoryListing(true);
        }

        /// <summary>
        /// Helper method, updates all data in arrays, and displays the info to the listView.
        /// </summary>
        /// <param name="updateFromDisk"> Specify whether to refresh directory contents from disk </param>
        private void UpdateDirectoryListing(bool updateFromDisk)
        {
            String directoryPath = settingsForm.WorkingFolderPath;
            if (Directory.Exists(directoryPath))
            {
                if (updateFromDisk) {
                    RetrieveDirectoryInformation();
                }
                AddRowsToListView();
            }
            else
            {
                AddRowError();
            }
        }

        private void RetrieveDirectoryInformation()
        {
            String[] allFilesAtPath = Directory.GetFileSystemEntries(settingsForm.WorkingFolderPath);
            IOParsingService parsingService = new IOParsingService(allFilesAtPath);

            bool ignoreFolderWithSubtitles = settingsForm.IgnoreAlreadySubbedFolders;
            parsingService.CleanDirectoryListing(ignoreFolderWithSubtitles);

            _allDirectoryContents = parsingService.DirContents;
            _entryTitleName = parsingService.IsolateTitleName();
            _entryEpisode = parsingService.IsolateEpisodeNumber();
            _entryReleaseGroup = parsingService.IsolateReleaseName(settingsForm.ExpectedNames);
        }

        private void AddRowsToListView()
        {
            listViewDirContents.ForeColor = System.Drawing.Color.Black;
            listViewDirContents.Items.Clear();

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

        private void AddRowError()
        {
            listViewDirContents.ForeColor = System.Drawing.Color.Red;
            ListViewItem row = new ListViewItem { Text = "Invalid directory, please set a directory in settings." };
            listViewDirContents.Items.Add(row);
            listViewDirContents.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        /// <summary>
        /// Called when clicking the settings option on the menu-bar.
        /// </summary>
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settingsForm.ShowDialog();
            if (settingsForm.DialogResult == DialogResult.OK)
            {
                UpdateDirectoryListing(true);
            }
        }


        /// <summary>
        /// Called when user selects a row in listView.
        /// </summary>
        private void listViewDirContents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewDirContents.SelectedItems.Count > 0)
            {
                btnModify.Visible = true;
                btnDownload.Enabled = true;
                btnOpenFolder.Visible = true;
                _selectedEntryInListView = listViewDirContents.SelectedItems[0].Index;
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

            ModifyForm modifyForm = new ModifyForm(
                _allDirectoryContents[_selectedEntryInListView], 
                _entryTitleName[_selectedEntryInListView], 
                _entryReleaseGroup[_selectedEntryInListView], 
                _entryEpisode[_selectedEntryInListView], 
                settingsForm
            );
            modifyForm.ShowDialog();

            if (modifyForm.DialogResult == DialogResult.OK)
            {
                _allDirectoryContents[_selectedEntryInListView] = modifyForm.ReturnValue1;
                _entryTitleName[_selectedEntryInListView] = modifyForm.ReturnValue2;
                _entryReleaseGroup[_selectedEntryInListView] = modifyForm.ReturnValue3;
                _entryEpisode[_selectedEntryInListView] = modifyForm.ReturnValue4;
                UpdateDirectoryListing(updateFromDisk: false);
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
            UtilityService utilService = new UtilityService();
            String pathToFolder = utilService.GetPath(_allDirectoryContents[_selectedEntryInListView]);
            Process.Start(fileName: pathToFolder);
        }

        /// <summary>
        /// Called when clicking the button, to initiate download subtitles.
        /// </summary>
        private void btnDownload_Click(object sender, EventArgs e)
        {
            textBoxProgress.ForeColor = System.Drawing.Color.ForestGreen;
            textBoxProgress.Clear();

            WebCrawlerService webCrawler = new WebCrawlerService();

            string[] searchResult;
            if (!SearchForTitle(webCrawler, out searchResult)) { 
                return;
            }

            string searchResultPicked = PickCorrectSearchResult(searchResult);
            
            string correctSub;
            if (FindMatchingSubtitle(searchResultPicked, webCrawler, out correctSub)) {
                return;
            }
            
            if (DownloadSubtitle(webCrawler, correctSub)) {
                return;
            }

            UnpackSubtitleFile();
        }

        private bool SearchForTitle(WebCrawlerService webCrawler, out string[] searchResult)
        {
            WriteToProgressWindow("Querying for " + _entryTitleName[_selectedEntryInListView] + "...", SUCCESS);
            webCrawler.RetrieveHtmlAtUrl("http://subscene.com/subtitles/title?q=" + _entryTitleName[_selectedEntryInListView] + "&l=");
            searchResult = webCrawler.PickCorrectSearchEntry();

            // Check if search query was successfull:
            if (searchResult.Length < 1)
            {
                WriteToProgressWindow("FAILURE! Search result gave no hits...", FAILURE);
                return false;
            }
            WriteToProgressWindow("Found " + searchResult.Length + " possible results...", SUCCESS);
            return true;
        }

        private string PickCorrectSearchResult(string[] searchResult)
        {
            PickEntryForm pickEntryForm = new PickEntryForm(searchResult);
            pickEntryForm.ShowDialog();
            String searchResultPicked = pickEntryForm.ReturnValue1;
            WriteToProgressWindow("User picked " + searchResultPicked + "...", SUCCESS);

            _latestLookUpLink = "http://subscene.com/subtitles/" + searchResultPicked;
            return searchResultPicked;
        }

        private bool FindMatchingSubtitle(string searchResultPicked, WebCrawlerService webCrawler, out string correctSub)
        {
            WriteToProgressWindow("Querying for subtitles to " + searchResultPicked + "...", SUCCESS);
            webCrawler.RetrieveHtmlAtUrl("http://subscene.com/subtitles/" + searchResultPicked);
            correctSub = webCrawler.PickCorrectSubtitle(_entryReleaseGroup[_selectedEntryInListView], _entryEpisode[_selectedEntryInListView]);
            if (correctSub == null)
            {
                WriteToProgressWindow("FAILURE! Could not find any subtitles for this release...", FAILURE);
                btnOpenBrowser.Visible = true;
                return true;
            }
            WriteToProgressWindow("Found possible match...", SUCCESS);
            return false;
        }

        private bool DownloadSubtitle(WebCrawlerService webCrawler, string correctSub)
        {
            WriteToProgressWindow("Querying download page...", SUCCESS);
            webCrawler.RetrieveHtmlAtUrl("http://subscene.com/" + correctSub);
            String downloadLink = webCrawler.FindDownloadUrl();
            bool result = webCrawler.InitiateDownload(
                "http://subscene.com" + downloadLink,
                _allDirectoryContents[_selectedEntryInListView],
                _entryTitleName[_selectedEntryInListView]
                );

            if (!result)
            {
                WriteToProgressWindow("FAILURE! Error downloading subtitle!", FAILURE);
                btnOpenBrowser.Visible = true;
                return true;
            }
            WriteToProgressWindow("SUCCESS! Subtitle downloaded!", SUCCESS);
            return false;
        }

        private void UnpackSubtitleFile()
        {
            WriteToProgressWindow("Unpacking rar file..", SUCCESS);
            UtilityService utilityService = new UtilityService();
            utilityService.UnrarFile(_allDirectoryContents[_selectedEntryInListView]);
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
    }
}
