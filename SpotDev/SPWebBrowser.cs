using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpotDev
{
    public partial class SPWebBrowser : UserControl, ISPComponent
    {
        public CefSharp.CefWebBrowser Browser
        {
            get
            {
                return this.c;
            }
        }
        public SPWebBrowser()
        {
            InitializeComponent();
            this.Controls.Add(c);
            c.Dock = DockStyle.Fill;
        }
        public SPWebBrowser(String url)
        {
            InitializeComponent();
            this.Controls.Add(c);
            c.Dock = DockStyle.Fill;
            c.CreateControl();
            c.Load(url);
        }
        CefSharp.CefWebBrowser c = new CefSharp.CefWebBrowser();

        private void SPWebBrowser_Load(object sender, EventArgs e)
        {
            
           
        }

        public void Save()
        {
            
        }

        public void Save(string fileName)
        {
            
        }

        public bool IsSaved
        {
            get
            {
                return true;
            }
            set
            {
                
            }
        }

        public void LoadFile(string fileName)
        {
            
        }

        public void Undo()
        {
            
        }

        public void Redo()
        {
            
        }

        public bool Close()
        {
            return true;
        }
    }
}
