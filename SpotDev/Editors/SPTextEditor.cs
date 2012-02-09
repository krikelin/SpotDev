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
            this.codeEditor.TextChanged += new EventHandler(richTextBox1_TextChanged);
        }
        public void LoadFile(String fileName)
        {
            codeEditor.Open(fileName);
        }
        void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            isSaved = false;
        }

        private void SPTextEditor_Load(object sender, EventArgs e)
        {
            codeEditor.BackColor = Color.FromArgb(244, 244, 244);
            codeEditor.ForeColor = Program.Skin.ForegroundColor;
            //  codeEditor.ShowLineNumbers = false;
            codeEditor.SelectionBackColor = Program.Skin.SelectionColor;

            codeEditor.SelectionForeColor = Program.Skin.SelectedForeColor;
            codeEditor.GutterMarginColor = Program.Skin.SelectionColor;
            codeEditor.LineNumberBackColor = Program.Skin.SelectionColor;
        }

        public void Save()
        {
            codeEditor.Save();
        }

        public void Save(string fileName)
        {
            codeEditor.Save(fileName);
        }

        public bool IsSaved
        {
            get
            {
                return codeEditor.Saved;
            }
            set
            {
                codeEditor.Saved = value;
            }
        }

        public string FileName
        {
            get { return this.fileName; }
        }

        private void richTextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void codeEditor_Load(object sender, EventArgs e)
        {

        }

        private void codeEditor_Click(object sender, EventArgs e)
        {

        }




    
      
        public void Undo()
        {
            this.codeEditor.Undo();
        }

        public void Redo()
        {
            this.codeEditor.Redo();
        }

        public bool Close()
        {
            if (!isSaved)
            {
                switch (MessageBox.Show("Do you want to save?", "Save?", MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:
                        Save();
                        return true;
                    case DialogResult.No:
                        return true;
                    case DialogResult.Cancel:
                        return false;
                }
            }
            return true;
        }
    }
}
