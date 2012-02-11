using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CefSharp;
using System.IO;

namespace SpotDev.Schemes
{
    public class SPDevSchemeHandler : ISchemeHandler
    {
        public bool ProcessRequest(IRequest request, ref string mimeType, ref Stream stream)
        {
            if (request.Url.StartsWith("res://", StringComparison.OrdinalIgnoreCase))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(Properties.Resources.ResourceManager.GetString(request.Url.Replace("res://","").Replace(".","_")).Replace("/","-"));
                stream = new MemoryStream(bytes);
                if(request.Url.EndsWith(".html"))
                {
                    mimeType = "text/html";
                }
                else if(request.Url.EndsWith(".png")) 
                {
                    mimeType = "text/png";
                }
                return true;
            }


            return false;
        }
    }
}
