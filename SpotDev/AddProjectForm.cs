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
    public partial class AddProjectForm : Form
    {
        public AddProjectForm()
        {
            InitializeComponent();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = listView1.SelectedItems.Count == 1;
        }
    }
}
