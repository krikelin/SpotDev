using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SpotDev.Schemes
{
    /// <summary>
    /// Clean-room implementation of the sp://(namespace)/ resource token.
    /// </summary>
    class SPResourceSchemeHandler : CefSharp.ISchemeHandler
    {
        /// <summary>
        /// As we aren't allowed to distribute Spotify resources, 
        /// we need to ask the user for the address of the Spotify executable, and
        /// then ask him to unzip the file "resources".zip and point this
        /// property to the folder that hosts the Apps directory 
        /// </summary>
        /// <example>
        /// C:\Users\Alexander\AppData\Roaming\Spotify\Data\resources\apps\import
        /// </example>
        public String ResourceDirectory = @"C:\Users\Alexander\AppData\Roaming\Spotify\Data\resources\apps\";

        public bool ProcessRequest(CefSharp.IRequest request, ref string mimeType, ref System.IO.Stream stream)
        {
            if(request.Url.StartsWith("sp://")) 
            {
                // On windows, we must replace the / with the \
                String URL = request.Url.Replace("sp://", ResourceDirectory).Replace("/", "\\");
                if (URL.EndsWith(".css"))
                {
                    mimeType = "text/css";
                }
                if (URL.EndsWith(".js"))
                {
                    mimeType = "text/javascript";
                }

                String contents = "";
                using (StreamReader sr = new StreamReader(URL))
                {
                    contents = sr.ReadToEnd();
                    
                }
                byte[] bytes = Encoding.UTF8.GetBytes(contents);
                stream = new MemoryStream(bytes);
                return true;
                
            }
            return false;
        }
    }
}
