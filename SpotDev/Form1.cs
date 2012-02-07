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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SpotDev
{
    public partial class Form1 : Form
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
             InputDialog inputDialog = new InputDialog("New project", "", "Enter the app domain name");
            if (inputDialog.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo dir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Spotify\\"+inputDialog.Value);
           
                SpotifyApp app = new SpotifyApp(dir);
                this.CurrentApp = app;
                propertyGrid1.SelectedObject = app.Manifest;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentApp != null)
            {
                CurrentApp.Save();
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {
                    }
    }
}
