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
    public partial class frmBoard : Form
    {
        DrawBoard board1;
        public frmBoard()
        {
            InitializeComponent();
            board1 = new DrawBoard();
            this.Controls.Add(board1);
            board1.Dock = DockStyle.Fill;

            // add an item click event
            board1.LinkClick += new DrawBoard.LinkClicked(board1_LinkClick);
        }

        void board1_LinkClick(object sender, string hRef)
        {
            board1.Navigate(hRef, "segurify", "views");
        }
        /// <summary>
        /// Method to load an example content
        /// </summary>
        /// <param name="address"></param>
        public void LoadContent(String address)
        {
            board1.Navigate(address, "segurify", "views");
        }
        private void frmBoard_Load(object sender, EventArgs e)
        {
            board1.Navigate("segurify:cheatview:c", "segurify", "views");
        }
    }
}
