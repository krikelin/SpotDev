﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Web.Script.Serialization;

namespace SpotDev
{
    public partial class SPManifestEditor : UserControl, ISPComponent
    {
       
        public SPManifestEditor()
        {
            InitializeComponent();
        }

        private void SPManifestEditor_Load(object sender, EventArgs e)
        {

        }
        private bool isSaved = false;
        private string fileName;
        public void Save(string fileName)
        {
       
            JavaScriptSerializer js = new JavaScriptSerializer();
            String manifest = js.Serialize(this);
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(manifest);
                sw.Close();
            }
         
        }

        bool ISPComponent.IsSaved
        {
            get
            {
                return false;
            }
            set
            {

            }
        }
        private SpotifyManifest Manifest { get; set; }
        public void LoadFile(string fileName)
        {

            if (File.Exists(fileName))
            {
                
                FileInfo file = new FileInfo(fileName);
                // If folder and manifest.json exists deserialize it
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                using (StreamReader sr = new StreamReader(file.FullName))
                {
                    this.Manifest = (SpotifyManifest)serializer.Deserialize<SpotifyManifest>(sr.ReadToEnd());
                    sr.Close();
                }
                this.fileName = fileName;
            }
            propertyGrid1.SelectedObject = this.Manifest;

        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {

        }


        public void Save()
        {
            this.Save(this.fileName);
        }

        void ISPComponent.Save()
        {
            throw new NotImplementedException();
        }

        void ISPComponent.Save(string fileName)
        {
            throw new NotImplementedException();
        }

        void ISPComponent.LoadFile(string fileName)
        {
            throw new NotImplementedException();
        }

        void ISPComponent.Undo()
        {
            throw new NotImplementedException();
        }

        void ISPComponent.Redo()
        {
            throw new NotImplementedException();
        }

        bool ISPComponent.Close()
        {
            throw new NotImplementedException();
        }
    }
}
