using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SpotDev
{
    public partial class MainForm : Form
    {
     
        public Dictionary<String, Control> Windows { get; set; }

        /// <summary>
        /// Loads a project into the tree view
        /// </summary>
        /// <param name="Dir"></param>
        private void LoadProject(DirectoryInfo Dir)
        {
            // Create new listItem
            SPListItem d = AddItem(Dir.Name,new Uri( "spotdev:project:"+Dir.Name+":overview"));
            d.Icon = Resources.spotifyapp;
            SPListItem liManifest = d.AddItem("Manifest", new Uri("spotdev:project:" + Dir.Name + ":manifest"));
            liManifest.Text = "App Manifest";
            liManifest.Icon = Resources.ic_settings;
           
        }
        public void BuildMenu()
        {
            this.spListView1.Items.Clear();
            // Add prexisisting
            AddItem("#Start", new Uri("spotdev:start"));
            AddItem("Start", new Uri("spotdev:start"));
            AddItem("#Projects", new Uri("spotdev:start"));
            SPListItem item = AddItem("New Project", new Uri("spotdev:add"));
            item.Icon = Resources.ic_add;
            item.Color = Color.FromArgb(127, 255, 127);
            String workDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Spotify";

            // Load Dirs
            if (!Directory.Exists(workDirectory))
            {
                Directory.CreateDirectory(workDirectory);
            }
            DirectoryInfo c = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Spotify");
            foreach (DirectoryInfo di in c.GetDirectories())
            {
                LoadProject(di);
            }
        }
        private SPListView spListView1;
        /// <summary>
        /// Navigates through the set of windows
        /// </summary>
        /// <param name="uri"></param>
        public void navigate(Uri uri)
        {
            String url = uri.ToString();
            if (url.StartsWith("spotdev:start"))
            {
                SPWebBrowser browser = new SPWebBrowser();
                contentPanel.Controls.Add(browser);
                browser.Dock = DockStyle.Fill;
              
            }
            if (!Windows.ContainsKey(url))
            {
                if (url.Split(':')[1] == "project")
                {
                    // Create a new window based on query
                    String app = url.Split(':')[2];
                    String action = url.Split(':')[3];
                    switch (action)
                    {
                        case "manifest":
                            // TODO add manifest here
                            MessageBox.Show("Show manifest editor");
                            break;
                    }
                }
            }
            else
            {
                Control c = Windows[url];
                c.BringToFront();
            }
        }
        Panel contentPanel;
        public MainForm()
        {
            InitializeComponent();
            this.spListView1 = new SPListView();
            // Add content panel
            contentPanel = new Panel();
            this.panel3.Controls.Add(contentPanel);
            this.panel3.Controls.Add(spListView1);
            
            contentPanel.Dock = DockStyle.Fill;
            spListView1.Dock = DockStyle.Left;
            spListView1.Width =204;
            spListView1.ItemSelected += new SPListView.SPListItemMouseEventHandler(spListView1_ItemSelected);
            BuildMenu();
            this.Windows = new Dictionary<String, Control>();
                
            
            
        }

        void spListView1_ItemSelected(object Sender, SPListView.SPListItemEventArgs e)
        {
            navigate(e.Item.Uri);
        }
        private SPListItem AddItem(String text, Uri uri)
        {
            return this.spListView1.AddItem(text, uri);
        }
        private SPListItem AddItem(String text, Uri uri, Image icon)
        {


            return this.spListView1.AddItem(text, uri, icon);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void spListView2_Load(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
