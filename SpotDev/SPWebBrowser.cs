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
    public partial class SPWebBrowser : UserControl
    {
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
    }
}
