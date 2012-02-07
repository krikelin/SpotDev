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
        private void LoadProject(DirectoryInfo Dir)
        {
            // Create new listItem
            SPListItem d = AddItem(Dir.Name,new Uri( "spotdev:project:"+Dir.Name));
            d.Icon = Resources.spotifyapp;
            SPListItem liManifest = new SPListItem(this.spListView1);
            liManifest.Text = "App Manifest";
            liManifest.Icon = Resources.ic_settings;
            d.Children.Add(liManifest);
            d.Expanded = true;
        }
        public void BuildMenu()
        {
            this.spListView1.Items.Clear();
            // Add prexisisting
            AddItem("#Start", new Uri("spotdev:start"));
            AddItem("Start", new Uri("spotdev:start"));
            AddItem("#Projects", new Uri("spotdev:start"));

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
        public MainForm()
        {
            InitializeComponent();
            this.spListView1 = new SPListView();
            this.panel3.Controls.Add(spListView1);
            spListView1.Dock = DockStyle.Left;
            spListView1.Width =174;
            spListView1.ItemSelected += new SPListView.SPListItemMouseEventHandler(spListView1_ItemSelected);
            BuildMenu();
                
            
            
        }

        void spListView1_ItemSelected(object Sender, SPListView.SPListItemEventArgs e)
        {
           
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
