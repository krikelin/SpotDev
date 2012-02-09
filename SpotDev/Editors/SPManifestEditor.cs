using System;
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

        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {

        }


        public void Save()
        {
            this.Save(this.fileName);
        }

       

        void ISPComponent.Undo()
        {
            
        }

        void ISPComponent.Redo()
        {
            
        }

        bool ISPComponent.Close()
        {
            return true;
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
