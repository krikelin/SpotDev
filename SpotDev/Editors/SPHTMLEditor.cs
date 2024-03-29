﻿using System;
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
    public partial class SPHTMLEditor : UserControl, ISPComponent
    {   
        public event EventHandler Changed;
        public event EventHandler Saved;
        /// <summary>
        /// File name
        /// </summary>
        public String fileName = "";
        public bool isSaved = false;
        public SPHTMLEditor()
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
            if (Changed != null)
            {
                this.Changed(this, new EventArgs());
            }
        }

        private void SPTextEditor_Load(object sender, EventArgs e)
        {
     
            //  codeEditor.ShowLineNumbers = false;
            codeEditor.SelectionBackColor = Program.Skin.SelectionColor;

            codeEditor.SelectionForeColor = Program.Skin.SelectedForeColor;
            codeEditor.GutterMarginColor = Program.Skin.SelectionColor;
            codeEditor.LineNumberBackColor = Program.Skin.SelectionColor;
            
        }

        public void Save()
        {
            codeEditor.Save();
            if (Saved != null)
                Saved(this, new EventArgs());
        }

        public void Save(string fileName)
        {
            codeEditor.Save(fileName);
            if (Saved != null)
                Saved(this, new EventArgs());
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





        public void  Undo()
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

        private void codeEditor_Click_1(object sender, EventArgs e)
        {

        }
        
        public void Cut()
        {
            codeEditor.Cut();
        }
        public void Copy()
        {
            codeEditor.Copy();
        }
        public void Paste()
        {
            codeEditor.Paste();
        }
    }
}
