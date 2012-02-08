/*
* Copyright (C) 2011 Alexander Forselius
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SpotDev
{
    public partial class Form1 : UserControl, ISPComponent
    {
        public SpotifyApp CurrentApp { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {

        }

        void ISPEditor.Save()
        {
            if (CurrentApp != null)
            {
                CurrentApp.Save();
            }
        }

        void ISPEditor.SaveAs(string fileName)
        {
            
            this.CurrentApp.Save();
        }

        bool ISPEditor.IsSaved
        {
            get
            {
                return this.CurrentApp.Saved;
            }
            set
            {
                this.CurrentApp.Saved = value;
            }
        }

        string ISPEditor.Text
        {
            get
            {
                return "";
            }
            set
            {
           
            }
        }

        string ISPEditor.FileName
        {
            get { return this.CurrentApp.Directory.FullName; }
        }
    }
}
