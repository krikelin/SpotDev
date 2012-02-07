/*
* Copyright (C) 2012 Alexander Forselius
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
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Drawing;
namespace SpotDev
{
    [Serializable]
    public class SpotifyPermission
    {
        public String Name { get; set; }
        public override string ToString()
        {
            return this.Name;
        }
    }
    [Serializable]
    public class SpotifyApp
    {
        public void Save()
        {
            this.Manifest.Save();
        }
        public bool Saved { get; set; }
        public SpotifyApp(DirectoryInfo directory)
        {
           
            this.Directory = directory;

            // Create the directory if it didn't exist
            if (!Directory.Exists)
            {
                directory.Create();
            }
            if (File.Exists(directory.FullName + "\\manifest.json"))
            {
                FileInfo file = new FileInfo(directory.FullName + "\\manifest.json");
                // If folder and manifest.json exists deserialize it
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                using (StreamReader sr = new StreamReader(file.FullName))
                {
                    this.Manifest = (SpotifyManifest)serializer.Deserialize<SpotifyManifest>(sr.ReadToEnd());
                }
            }
            this.Manifest = new SpotifyManifest(directory);
        }
        public DirectoryInfo Directory { get; set; }
        public SpotifyManifest Manifest { get; set; }
    }
    [Serializable]
    public class SpotifyTab
    {
        [DefaultValue("")]
        public String name { get; set; }
         [DefaultValue("")]
         public String description { get; set; }
         [DefaultValue("")]
         public String parameters { get; set; }
         public override string ToString()
         {
             return this.name;
         }
    }
    [Serializable]
    public class SpotifyManifest
    {
        private DirectoryInfo directory;
        public SpotifyManifest(DirectoryInfo directory)
        {
            this.directory = directory;
            AppName = new Dictionary<String, String>();

            DefaultTabs = new List<SpotifyTab>();
            RequiredPermissions = new List<String>();
            SupportedLanguages = new List<string>();
        }
        [DefaultValue("")]
        public String AppDescription { get; set; }
        public String AppIcon { get; set; }
        public Dictionary<String, String> AppName { get; set; }

        public Uri AppURL { get; set; }
        [DefaultValue("")]
        public String BundleCopyright { get; set; }
        [DefaultValue("")]
        public String BundleIdentifier { get; set; }
        [DefaultValue("")]
        public String BundleType { get; set; }
        public List<SpotifyTab> DefaultTabs { get; set; }
        public List<String> RequiredPermissions { get; set; }
        public List<String> SupportedLanguages { get; set; }
        [DefaultValue("")]
        public String VendorIdentifier { get; set; }
        /// <summary>
        /// Saves the manifest
        /// </summary>
        public void Save()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            String manifest = js.Serialize(this);
            using (StreamWriter sw = new StreamWriter(directory.FullName + "\\manifest.json"))
            {
                sw.Write(manifest);
                sw.Close();
            }
            
        }

    }
}
