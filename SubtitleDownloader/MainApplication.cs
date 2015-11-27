using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SubtitleDownloader
{
    public partial class MainApplication : Form
    {

        SettingsForm settingsForm = new SettingsForm();
        String[] directoryContents;
        String[] titleName;
        String[] releaseName;
        String[] episode;
        private int selectedEntry;
        private String failedLookUpLink;

        /// <summary>
        /// Constructor, loads in settings, updates arrays, and lists data in window.
        /// </summary>
        public MainApplication()
        {
            InitializeComponent();

            errorLabel.Text = "";
            currentPathLabel.Text = "Current path: " + settingsForm.torrentDownloadPath;

            listView1.View = View.Details;
            listView1.Columns.Add("Content missing subtitles:");
            listView1.Columns.Add("Est. Title Name");
            listView1.Columns.Add("Est. Release Name");
            listView1.Columns.Add("Episode");            
        }
        private void MainApplication_Load(object sender, EventArgs e)
        {
            updateDirectoryListing();
        }

        /// <summary>
        /// Called when clicking the settings option on the menu-bar.
        /// </summary>
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settingsForm.ShowDialog();
            if (settingsForm.DialogResult == DialogResult.OK)
                updateDirectoryListing();
        }

        /// <summary>
        /// Helper method, updates all data in arrays, and displays the info to the listView.
        /// </summary>
        private void updateDirectoryListing()
        {
            listView1.ForeColor = System.Drawing.Color.Black;
            listView1.Items.Clear();

            // Retrieve and list all content in folder:
            if (settingsForm.torrentDownloadPath != "no directory path set")
            {
                directoryContents = IOParsingHelper.getDirectoryListing(settingsForm);
                titleName = IOParsingHelper.isolateTitleName(directoryContents);
                releaseName = IOParsingHelper.isolateReleaseName(directoryContents, settingsForm);
                episode = IOParsingHelper.isolateEpisodeNumber(directoryContents);                

                for (int i = 0; i < directoryContents.Length; i++)
                {
                    ListViewItem row = new ListViewItem();
                    row.Text = (Path.GetFileName(directoryContents[i]));
                    row.SubItems.Add(titleName[i]);
                    row.SubItems.Add(Path.GetFileName(releaseName[i]));
                    row.SubItems.Add(episode[i]);
                    listView1.Items.Add(row);
                }

                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            else
            {
                listView1.ForeColor = System.Drawing.Color.Red;
                ListViewItem row = new ListViewItem();
                row.Text = "Please set a directory, in settings.";
                listView1.Items.Add(row);
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        /// <summary>
        /// Helper method, ONLY updates display info.
        /// Note: Currently only used after modifying an entry.
        /// </summary>
        private void updateDirectoryListingDisplay()
        {
            listView1.ForeColor = System.Drawing.Color.Black;
            listView1.Items.Clear();

            if (directoryContents.Length == 0)
            {
                listView1.ForeColor = System.Drawing.Color.Red;
                ListViewItem row = new ListViewItem();
                row.Text = "Nothing found. Please set a directory in settings.";
                listView1.Items.Add(row);
            }

            for (int i = 0; i < directoryContents.Length; i++)
            {
                ListViewItem row = new ListViewItem();
                row.Text = (Path.GetFileName(directoryContents[i]));
                row.SubItems.Add(titleName[i]);
                row.SubItems.Add(Path.GetFileName(releaseName[i]));
                row.SubItems.Add(episode[i]);
                listView1.Items.Add(row);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        /// <summary>
        /// Called when clicking the button, to initiate download subtitles.
        /// </summary>
        private void btnDownload_Click(object sender, EventArgs e)
        {
            textBoxProgress.ForeColor = System.Drawing.Color.ForestGreen;
            textBoxProgress.Clear();
            btnOpenBrowser.Visible = false;

            // Search for the given file:
            // And attempt to pick correct entry (if 1 exact, just pick it, otherwise might have to ask user?):
            textBoxProgress.Text += "Querying for " + titleName[selectedEntry] + "...\r\n\r\n";
            String searchQuery = WebFetchHelper.accessWeb("http://subscene.com/subtitles/title?q=" + titleName[selectedEntry] + "&l=");            
            String[] searchResult = WebFetchHelper.findCorrectEntry(searchQuery);
            textBoxProgress.Text += "Found " + searchResult.Length + " possible results...\r\n\r\n";

            String searchResultPicked;
            if (searchResult.Length == 0)
            {
                textBoxProgress.Text += "FAILURE! Search result gave no hits...\r\n\r\n";
                textBoxProgress.ForeColor = System.Drawing.Color.Red;
                return;
            }
            else
            {
                PickEntryForm pickEntryForm = new PickEntryForm(searchResult);
                pickEntryForm.ShowDialog();
                searchResultPicked = pickEntryForm.ReturnValue1;
            }
            textBoxProgress.Text += "User picked " + searchResultPicked + "...\r\n\r\n";

            // Find correct subtitle (look under language and then filename entry):
            textBoxProgress.Text += "Querying for subtitles to " + searchResultPicked + "...\r\n\r\n";
            String findSubQuery = WebFetchHelper.accessWeb("http://subscene.com/subtitles/" + searchResultPicked);            
            String correctSub = WebFetchHelper.findCorrectSub(findSubQuery, releaseName[selectedEntry], episode[selectedEntry]);
            if (correctSub != null)
                textBoxProgress.Text += "Found possible match...\r\n\r\n";
            else
            {
                textBoxProgress.Text += "FAILURE! Could not find any subtitles for this release...\r\n\r\n";
                textBoxProgress.ForeColor = System.Drawing.Color.Red;
                failedLookUpLink = "http://subscene.com/subtitles/" + searchResultPicked;
                btnOpenBrowser.Visible = true;
                return;
            }

            // Initiate the download:
            textBoxProgress.Text += "Querying download page...\r\n\r\n";
            String downloadPageQuery = WebFetchHelper.accessWeb("http://subscene.com/" + correctSub);            
            String downloadLink = WebFetchHelper.findDownloadLink(downloadPageQuery);
            bool result = WebFetchHelper.downloadFile(
                "http://subscene.com" + downloadLink, directoryContents[selectedEntry], titleName[selectedEntry]
            );

            if (result)
            {
                textBoxProgress.Text += "SUCCESS! Subtitle downloaded!\r\n\r\n";
                textBoxProgress.Text += "Unpacking rar file..\r\n\r\n";                
                WebFetchHelper.unrarFile(directoryContents[selectedEntry]);
            }
            else
            {
                textBoxProgress.Text += "FAILURE! Error downloading subtitle!";
                textBoxProgress.ForeColor = System.Drawing.Color.Red;
            }
            
        }

        /// <summary>
        /// Called when user selects a row in listView.
        /// </summary>
        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {                
                btnModify.Visible = true;
                btnDownload.Enabled = true;
                btnOpenFolder.Visible = true;
                selectedEntry = listView1.SelectedItems[0].Index;
            }else
            {
                btnModify.Visible = false;
                btnDownload.Enabled = false;
                btnOpenFolder.Visible = false;

            }
        }

        /// <summary>
        /// Opens window to modify selected entry.
        /// </summary>
        private void btnModify_Click(object sender, EventArgs e)
        {
            if (directoryContents == null) return;

            ModifyForm modify = new ModifyForm(
                directoryContents[selectedEntry], titleName[selectedEntry], releaseName[selectedEntry], episode[selectedEntry], settingsForm
            );
            modify.ShowDialog();
            if (modify.DialogResult == DialogResult.OK)
            {
                directoryContents[selectedEntry] = modify.returnValue1;
                titleName[selectedEntry] = modify.returnValue2;
                releaseName[selectedEntry] = modify.returnValue3;
                episode[selectedEntry] = modify.returnValue4;
                updateDirectoryListingDisplay();
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
            System.Diagnostics.Process.Start(failedLookUpLink);
        }

        /// <summary>
        /// Open explorer window at selected entry path.
        /// </summary>
        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            String pathToFolder = IOParsingHelper.checkFolderOrFile(directoryContents[selectedEntry]);
            Process.Start(pathToFolder);
        }
    }
}
