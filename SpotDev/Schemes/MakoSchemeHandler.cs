using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace SpotDev
{
    public class MakoSchemeHandler : CefSharp.ISchemeHandler
    {

        bool CefSharp.ISchemeHandler.ProcessRequest(CefSharp.IRequest request, ref string mimeType, ref System.IO.Stream stream)
        {
            if (request.Url.StartsWith("mako://", StringComparison.OrdinalIgnoreCase) && request.Url.EndsWith(".html"))
            {
                WebClient wc = new WebClient();

                String data = wc.DownloadString(request.Url.ToString().Replace("mako://","http://"));

                Program.me.Preprocess(data, "", false, request.Url.ToString());

                stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Program.me.Output));
                mimeType = "text/html";
                return true;
            }
            return false;
        }
    }
}
