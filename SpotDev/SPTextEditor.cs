using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SpotDev
{
    public partial class SPTextEditor : UserControl, ISPComponent
    {
        /// <summary>
        /// File name
        /// </summary>
        public String fileName = "";
        public bool isSaved = false;
        public SPTextEditor()
        {
            InitializeComponent();
            this.richTextBox1.TextChanged += new EventHandler(richTextBox1_TextChanged);
        }
        public void LoadFile(String fileName)
        {
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    this.fileName = fileName;
                    richTextBox1.Text = sr.ReadToEnd();
                    sr.Close();
                    isSaved = true;
                }
            }
            catch (Exception e)
            {
            }
        }
        void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            isSaved = false;
        }

        private void SPTextEditor_Load(object sender, EventArgs e)
        {
            richTextBox1.BackColor = Program.Skin.BackgroundColor;
            richTextBox1.ForeColor = Program.Skin.ForegroundColor;
           
        }

        public void Save()
        {
            using (StreamWriter sw = new StreamWriter(this.FileName))
            {
                sw.Write(this.richTextBox1.Text);
                sw.Close();
                isSaved = true;
            }
        }

        public void Save(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(fileName);
                sw.Close();
             
                this.fileName = fileName;
                isSaved = true;
            }
        }

        public bool IsSaved
        {
            get
            {
                return this.isSaved;
            }
            set
            {
                this.isSaved = value;
            }
        }

        public string FileName
        {
            get { return this.fileName; }
        }

        private void richTextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }


      
    }
}
