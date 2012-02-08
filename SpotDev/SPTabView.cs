using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace SpotDev
{
    /// <summary>
    /// A component like editor, file manager, browser or whatever
    /// </summary>
    public interface ISPComponent 
    {
        void Save();
        void Save(String fileName);
        bool IsSaved {get;set;}
        void LoadFile(String fileName);
    
    }
    /// <summary>
    /// A SPTab
    /// </summary>
    public class SPTab
    {
        public String Title { get; set; }
        public Control Control { get; set; }
        public Uri Uri { get; set; }
    }
    public class SPTabView : Panel
    {
        private SPTab activeTab;
        public SPTab ActiveTab
        {
            get
            {
                return activeTab;
            }
        }
        public Dictionary<String, SPTab> Views { get; set; }
        Panel contentPanel = new Panel();
        private int tabHeight = 24;
        private int scrollX = 0; // ScrollX of tabs
        public void Draw(Graphics g)
        {
            // fill
            g.FillRectangle(new SolidBrush(Program.Skin.TabBarBackgroundColor), new Rectangle(0, 0, this.Width, this.Height));

            // fill tabBar
            g.FillRectangle(Program.Skin.TabBarBackgroundGradient, new Rectangle(0, 0, this.Width, this.tabHeight));
       //     g.DrawLine(new Pen(Color.FromArgb(172, 172, 172), 1), new Point(0, tabHeight - 2), new Point(this.Width, tabHeight - 2));
            
            g.DrawLine(new Pen(Color.FromArgb(42,42,42), 1), new Point(0, tabHeight - 1), new Point(this.Width, tabHeight - 1));
            // Fill views
            int pos = -scrollX;
            foreach (SPTab tab in Views.Values)
            {
                Color foreColor = Color.FromArgb(55, 55, 55);
                Color shadowColor = Color.FromArgb(211, 211, 211);
                if (tab == activeTab)
                {
                    // fill tabBar
                    g.FillRectangle(Program.Skin.TabBarActiveBackgroundGradient, new Rectangle(pos, 0, 120, 28));
            
                    // fill active tab
                     foreColor = Color.FromArgb(211, 211, 211);
                     shadowColor = Color.FromArgb(55, 55, 55);
                     
                     g.DrawImage(Resources.ic_close, new Point(pos + 120 - 24, 5));
                     g.DrawLine(new Pen(Color.FromArgb(122, 122, 122), 1), new Point(pos, 0), new Point(pos, tabHeight - 2));
                }
                bool isSaved = false;
                if(tab.Control is ISPComponent)
                {
                    isSaved = ((ISPComponent)tab.Control).IsSaved;
                }
                // Draw string
                g.DrawString(tab.Title + (isSaved ? "" : "*"), new Font("MS Sans Serif", 8), new SolidBrush(shadowColor), new Point(pos + 20, 3));
                g.DrawString(tab.Title + (isSaved ? "" : "*"), new Font("MS Sans Serif", 8), new SolidBrush(foreColor), new Point(pos + 20, 2));
                
                g.DrawLine(new Pen(Color.FromArgb(172, 172, 172), 1), new Point(pos + 120, 0), new Point(pos + 120, tabHeight - 2));
                pos += 120;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            BufferedGraphicsContext bgc = new BufferedGraphicsContext();
            BufferedGraphics bg = bgc.Allocate(e.Graphics, new Rectangle(0, 0, this.Width, this.Height));
            Graphics g = bg.Graphics;
            Draw(g);
            bg.Render();
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            BufferedGraphicsContext bgc = new BufferedGraphicsContext();
            BufferedGraphics bg = bgc.Allocate(e.Graphics, new Rectangle(0, 0, this.Width, this.Height));
            Graphics g = bg.Graphics;
            Draw(e.Graphics);
            bg.Render();
        }
        /// <summary>
        /// Show a control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="title"></param>
        /// <param name="uri"></param>
        public void ShowControl(Control control, String title, Uri uri)
        {
            String url = uri.ToString();
    
            contentPanel.Controls.Add(control);
            control.Dock = DockStyle.Fill;
            SPTab tab = new SPTab();
            tab.Control = control;
            tab.Title = title;
            tab.Uri = uri;
            this.activeTab = tab;
            Views.Add(url, tab);
            control.BringToFront();
        }
        public SPTabView()
            : base() {
                Views = new Dictionary<String, SPTab>();
                this.BackColor = ( Program.Skin.BackgroundColor);
                // Add contentpanel
                this.Controls.Add(contentPanel);
                contentPanel.Left = 0;
                contentPanel.Top = tabHeight;
                contentPanel.Width = this.Width;
                contentPanel.Height = this.Height - tabHeight;
                contentPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                this.MouseDown += new MouseEventHandler(SPTabView_MouseDown);
                this.TabStop = true;
        }
        
        void SPTabView_MouseDown(object sender, MouseEventArgs e)
        {
            int pos = -scrollX;
            foreach(SPTab tab in this.Views.Values)
            {
                if (e.X >= pos && e.X <= pos + 120)
                {
                    Navigate(tab.Title, tab.Uri);
                    break;
                }
                pos += 120;
            }
        }
        public void Navigate(String title, Uri uri)
        {
            // Add navigation handler here
            String url = uri.ToString();
            
            if (!Views.ContainsKey(url))
            {
                if (url.StartsWith("http://"))
                {
                    ShowControl(new SPWebBrowser(url), "Browser", uri);
                }
                if (url.StartsWith("spotdev:start"))
                {
                    ShowControl(new SPWebBrowser(), "Browser", uri);
                }
                if (url.Split(':')[1] == "project")
                {
                    // Create a new window based on query
                    String app = url.Split(':')[2];
                    String[] tokens = url.Split(':');
                    StringBuilder path = new StringBuilder(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+"\\Spotify");
                    for (int i = 2; i <tokens.Length-1 ; i++)
                    {
                        path.Append("\\" + tokens[i] );
                    }
                    FileInfo file = new FileInfo(path.ToString());
                    if (file.Name == "manifest.json")
                    {
                        SPManifestEditor manifestEditor = new SPManifestEditor();
                        manifestEditor.LoadFile(file.FullName);
                        ShowControl(manifestEditor, file.Name, uri);
                        if (this.TabChanged != null)
                            this.TabChanged(this, new EventArgs());
                    }
                    else
                    {
                        switch (file.Extension)
                        {

                            default:
                                {
                                    SPTextEditor textEditor = new SPTextEditor();
                                    textEditor.LoadFile(file.FullName);
                                    ShowControl(textEditor, file.Name, uri);
                                    if (this.TabChanged != null)
                                        this.TabChanged(this, new EventArgs());

                                }
                                break;
                        }
                    }
                   
                }
            }
            else
            {
                SPTab c = Views[url];
                this.activeTab = c;
                c.Control.BringToFront();
            }
            this.Draw(this.CreateGraphics());
        }
        public event EventHandler TabChanged;
    }
    
}
