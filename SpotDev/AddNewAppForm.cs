using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpotDev
{
    public partial class AddNewAppForm : Form
    {
        public String Title
        {
            get
            {
                return this.tbTitle.Text;
            }
        }
        public String Namespace
        {
            get
            {
                return this.tbNamespace.Text;
            }
        }
        public AddNewAppForm()
        {
            InitializeComponent();
        }

        private void New_App_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        public string Vendor
        {
            get
            {
                return edVendor.Text;
            }
        }
    }
}
