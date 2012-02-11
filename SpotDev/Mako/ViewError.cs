using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Board
{
    /// <summary>
    /// Form to show about an view error
    /// </summary>
    public partial class ViewError : Form
    {
        /// <summary>
        /// Custom constructor for ViewError, where you specify the
        /// info to the user about the error.
        /// </summary>
        /// <param name="View"></param>
        /// <param name="error"></param>
        public ViewError(String View, String error)
        {
            // Initialize component
            InitializeComponent();

            // Fill the textbox with the error description
            textBox1.Text = error;
        }
        public ViewError()
        {
            InitializeComponent();
        }

        private void ViewError_Load(object sender, EventArgs e)
        {

        }
    }
}
