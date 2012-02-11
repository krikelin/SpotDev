using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Board
{
    public partial class MakoParser : Form
    {
        public MakoParser()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Method to test content, output both preparsed and parsed content
        /// </summary>
        /// <param name="file"></param>
        private void TestContent(String file)
        {
            Engine = new MakoEngine();

            using (StreamReader SR = new StreamReader(file))
            {
                textBox1.Text = SR.ReadToEnd();

                ParseFile(textBox1.Text);
            }

        }

        frmBoard board;
        /// <summary>
        /// Our test engine
        /// </summary>
        MakoEngine Engine;
        private void MakoParser_Load(object sender, EventArgs e)
        {
          
            // Create an new test form
            board = new frmBoard();
            board.Show();
        }

        private string ParseFile(string p)
        {
            MakoParser parser = new MakoParser();

            return textBox2.Text = parser.ParseFile(p);
        }
     

        private void button1_Click(object sender, EventArgs e)
        {
            ParseFile(textBox1.Text);
            
            board.Show();
        
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            board.LoadContent(textBox3.Text);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }
    }
}
