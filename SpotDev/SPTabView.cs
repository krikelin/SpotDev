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
        void Undo();
        void Redo();
        void Cut();
        void Copy();
        void Paste();
        bool Close();
        event EventHandler Changed;
        event EventHandler Saved;
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
            if (control is ISPComponent)
            {
                ISPComponent comp = (ISPComponent)control;
                comp.Changed += new EventHandler(comp_Changed);
                comp.Saved += new EventHandler(comp_Saved);
            }
        }

        void comp_Saved(object sender, EventArgs e)
        {
            Draw(CreateGraphics());
        
        }

        void comp_Changed(object sender, EventArgs e)
        {
            Draw(CreateGraphics());
         
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
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                foreach (SPTab tab in this.Views.Values)
                {
                    if (e.X >= pos && e.X <= pos + 120)
                    {
                        Navigate(tab.Title, tab.Uri);
                        break;
                    }
                    pos += 120;
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                int i = 0;
                foreach (KeyValuePair<String, SPTab> tab in this.Views)
                {
                    if (e.X >= pos && e.X <= pos + 120)
                    {
                        if (tab.Value.Control is ISPComponent)
                        {
                            ISPComponent component = (ISPComponent)tab.Value.Control;
                            if (component.Close())
                            {
                                this.Views.Remove(tab.Key);
                                this.Controls.Remove(tab.Value.Control); // Remove view
                                if (i > 0)
                                {
                                    SPTab prevTab = this.Views.Values.ElementAt(i - 1);
                                    this.Navigate(prevTab.Title, prevTab.Uri);
                                }
                                else
                                {
                                    try
                                    {
                                        SPTab prevTab = this.Views.Values.ElementAt(i + 1);
                                        this.Navigate(prevTab.Title, prevTab.Uri);
                                    }
                                    catch (Exception ex)
                                    {
                                        this.Navigate("Home", new Uri("http://static.cobresia.webfactional.com/spotdev/index.html"));
                                    }
                                }
                                    break;
                            }
                            i++;
                        }
                        break;
                    }
                    pos += 120;
                }
                this.Draw(CreateGraphics());
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
#if(manifest)
                    
                        SPManifestEditor manifestEditor = new SPManifestEditor();
                        manifestEditor.LoadFile(file.FullName);
                        ShowControl(manifestEditor, file.Name, uri);
                        if (this.TabChanged != null)
                            this.TabChanged(this, new EventArgs());
#else
                        SPTextEditor textEditor = new SPTextEditor();
                        textEditor.LoadFile(file.FullName);
                        ShowControl(textEditor, file.Name, uri);
                        if (this.TabChanged != null)
                            this.TabChanged(this, new EventArgs());
#endif
                    }
                    else
                    {
                        switch (file.Extension)
                        {

                            case ".txt":
                            case ".js":
                            case ".json":
                                {
                                    SPTextEditor textEditor = new SPTextEditor();
                                    textEditor.LoadFile(file.FullName);
                                    ShowControl(textEditor, file.Name, uri);
                                    if (this.TabChanged != null)
                                        this.TabChanged(this, new EventArgs());

                                }
                                break;
                            case ".html":
                            case ".xhtml":
                              
                                {
                                    SPHTMLEditor textEditor = new SPHTMLEditor();
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
