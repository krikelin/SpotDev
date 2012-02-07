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
        }
        CefSharp.CefWebBrowser c = new CefSharp.CefWebBrowser();

        private void SPWebBrowser_Load(object sender, EventArgs e)
        {
            this.Controls.Add(c);
            c.Dock = DockStyle.Fill;
            c.Load("welcome.html");
           
        }
    }
}
