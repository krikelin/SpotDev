using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CefSharp;
using System.IO;

namespace SpotDev
{
    public class SPSchemeHandler : ISchemeHandler
    {
        public String sp_res_folder = "";
        public bool ProcessRequest(IRequest request, ref string mimeType, ref Stream stream)
        {
            if (request.Url.StartsWith("sp://", StringComparison.OrdinalIgnoreCase))
            {
                String path = sp_res_folder + "/" + request.Url.Replace("sp://", "");
                FileInfo fil = new FileInfo(path);
                mimeType = "text/" + fil;
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                return true;
            }

           

            return false;
        }
    }

    public class SPSchemeHandlerFactory : ISchemeHandlerFactory
    {
        public ISchemeHandler Create()
        {
            return new SPSchemeHandler();
        }
    }
}
