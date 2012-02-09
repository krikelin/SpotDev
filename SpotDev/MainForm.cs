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
        
        private void ListDirectory(String project, DirectoryInfo Dir, SPListItem item,ref String path)
        {
            SPListItem add_file = item.AddItem("Add File", new Uri("spotdev:project:" + project + ":" + path + "#AddFile"), Resources.ic_add);
            SPListItem add_folder = item.AddItem("Add Folder", new Uri("spotdev:project:" + project + ":" + path + "#AddFile"), Resources.ic_add);

            // Show all directories
            foreach (DirectoryInfo directory in Dir.GetDirectories())
            {
                String sub_path = path + ":" + directory.Name;
                SPListItem dir = item.AddItem(directory.Name, new Uri("spotdev:project:" + project + ":" + sub_path + ":"), Resources.folder);
                ListDirectory(project, directory, dir, ref sub_path);
            }
            // Show all directories
            foreach (FileInfo file in Dir.GetFiles())
            {
                String sub_path = path + ":" + file.Name;
                SPListItem dir = item.AddItem(file.Name, new Uri("spotdev:project:" + project + ":" + sub_path + ":"), Resources.folder);
                switch (file.Extension)
                {
                    case ".js":
                        
                        dir.Icon = Resources.script;
                        break;
                     case ".json":
                        
                        dir.Icon = Resources.ic_settings;
                        break;
                    default:
                        dir.Icon = Resources.ic_doc_spotify;
                        break;
                }
                
            }
          
        }
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
            String path = "";
            ListDirectory(Dir.Name, Dir, d, ref path);
           

           
            

           
        }
        public void BuildMenu()
        {
            this.spListView1.Items.Clear();
            // Add prexisisting
            AddItem("", new Uri("http://static.cobresia.webfactional.com/spotdev/index.html"));
            
            AddItem("#Start", new Uri("http://static.cobresia.webfactional.com/spotdev/index.html"));
            SPListItem i = AddItem("Start", new Uri("http://static.cobresia.webfactional.com/spotdev/index.html"), Resources.world);
            i.Selected = true;
            AddItem("", new Uri("spotdev:start"));
            AddItem("#Resources", new Uri("spotdev:start"), Resources.world);
            AddItem("App Concept Submission", new Uri("http://developer.spotify.com/en/spotify-apps-api/appconcept/"), Resources.world);
            AddItem("Guidelines", new Uri("http://developer.spotify.com/download/spotify-apps-api/guidelines/"), Resources.world);
            AddItem("Terms of Use", new Uri("spotdev:concept:submit"), Resources.world);
            AddItem("Stack Overflow", new Uri("spotdev:concept:submit"), Resources.world);
            AddItem("", new Uri("spotdev:start"));
            AddItem("#Tech reference", new Uri("spotdev:concept:submit"), Resources.world);
            SPListItem spa_reference = AddItem("Spotify Apps", new Uri("http://developer.spotify.com/download/spotify-apps-api/reference/"), Resources.world);
            spa_reference.AddItem("Preview JS reference", new Uri("http://developer.spotify.com/download/spotify-apps-api/preview/reference/"), Resources.world);
            spa_reference.AddItem("Production JS reference", new Uri("http://developer.spotify.com/download/spotify-apps-api/reference/"), Resources.world);
            AddItem("", new Uri("spotdev:start"));
             
            AddItem("#Social", new Uri("spotdev:concept:submit"));
            
            AddItem("Twitter", new Uri("http://www.twitter.com/SpotifyPlatform"), Resources.world);
            AddItem("IRC (join #Spotify)", new Uri("http://webchat.quakenet.org/"), Resources.world);

            AddItem("", new Uri("spotdev:start"));
            
            AddItem("#Projects", new Uri("spotdev:start"));
            SPListItem item = AddItem("New Project", new Uri("spotdev:create:app"));
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
        public void CreateNewApp(String name, String _namespace, String vendor)
        {
            // Get projects directory
            DirectoryInfo c = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Spotify");
            String path = c.FullName + "\\" + _namespace;
            // Create directory for the app
            Directory.CreateDirectory(path);

            // Create the manifest.json file
            Dictionary<String, String> files = new Dictionary<string,string>();
            files.Add("manifest.json", Properties.Resources.ResourceManager.GetString("manifest_json"));
            files.Add("script.js", Properties.Resources.ResourceManager.GetString("script_js"));
            files.Add("index.html", Properties.Resources.ResourceManager.GetString("index_html"));

            // Customize the files for the particular project
            Dictionary<String, String> processedFiles = new Dictionary<string, string>();
            foreach (KeyValuePair<String, String> file in files)
            {
                processedFiles.Add(file.Key, file.Value.Replace("${App.Title}", name).Replace("${App.BundleIdentifier}", _namespace).Replace("${App.Vendor}", vendor).Replace("${App.Author}", vendor).Replace("${Year}", DateTime.Today.Year.ToString()));
            }

            // Save the files to the directory. For now, we only support 1st level
            foreach (KeyValuePair<String, String> file in processedFiles)
            {
                using (StreamWriter sw = new StreamWriter(path + "\\" + file.Key))
                {
                    sw.Write(file.Value);
                    sw.Close();
                }
            }

            // Create folder for additional resources
            Directory.CreateDirectory(path + "\\css");
            Directory.CreateDirectory(path + "\\js");
            Directory.CreateDirectory(path + "\\img");
        }

        public void CreateProject()
        {
         
            AddNewAppForm addNewAppForm = new AddNewAppForm();
            {
                if (addNewAppForm.ShowDialog() == DialogResult.OK)
                {
                    // Create
                    CreateNewApp(addNewAppForm.Title, addNewAppForm.Namespace, addNewAppForm.Vendor);
                    LoadProject(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Spotify\\" + addNewAppForm.Namespace));

                }
            }
           
        }
        private SPListView spListView1;
        /// <summary>
        /// Navigates through the set of windows
        /// </summary>
        /// <param name="uri"></param>
        public void navigate(Uri uri)
        {
            if (uri.ToString() == "spotdev:create:app")
            {

                CreateProject();
                return;
            }
            
            tabView.Navigate("Test", uri);
        }
        SPTabView tabView;
        Panel contentPanel;
        Panel sideBar;
        SPWebBrowser browser;
        public MainForm()
        {
            InitializeComponent();
            this.spListView1 = new SPListView();
            // Add content panel
            contentPanel = new Panel();
            sideBar = new Panel();
            this.panel3.TabStop = true;

            this.panel3.Controls.Add(contentPanel);
            this.panel3.Controls.Add(spListView1);
            
      //      contentPanel.Controls.Add(sideBar);
            sideBar.Dock = DockStyle.Right;
            sideBar.Width = 320;
            
            contentPanel.TabStop = true;
            contentPanel.Dock = DockStyle.Fill;
            spListView1.Dock = DockStyle.Left;



            spListView1.Width =234;
            spListView1.ItemSelected += new SPListView.SPListItemMouseEventHandler(spListView1_ItemSelected);
            BuildMenu();
            this.Windows = new Dictionary<String, Control>();
            // Add sidebar
           
            browser = new SPWebBrowser("http://webchat.quakenet.org/");
           

            sideBar.Controls.Add(browser);
         
            browser.Dock = DockStyle.Fill;
            
            // Add SPTabView
            tabView = new SPTabView();
            this.contentPanel.Controls.Add(tabView);
            tabView.Dock = DockStyle.Fill;
            tabView.TabChanged += new EventHandler(tabView_TabChanged);
            navigate(new Uri("http://static.cobresia.webfactional.com/spotdev/index.html"));
            
            
        }
       
        void tabView_TabChanged(object sender, EventArgs e)
        {
         
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
            this.menuStrip1.RenderMode = ToolStripRenderMode.Professional;
            this.menuStrip1.Renderer = new SpotifyProfessionalRenderer(new SpotifyStripColorTable());
        }

        private void spListView2_Load(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c = tabView.ActiveTab.Control;
            if (c is ISPComponent )
            {
                ISPComponent component = (ISPComponent)c;
                component.Save();

            }
       }

        private void panel3_Paint_1(object sender, PaintEventArgs e)
        {

        }
    }
}
