using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Board;
using System.IO;

namespace MediaChromeGUI.Board
{
    public class WebView : WebKit.WebKitBrowser

    {
        MakoEngine mMakoEngine;
        public WebView()
        {
            // Add event handler for navigation
            this.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(WebView_Navigating);
            // Initiate parsing
            mMakoEngine = new MakoEngine();
        }
        public String RawCode { get; set; }
        public String Source { get; set; }
        void WebView_Navigating(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e)
        {
            String viewRoot = "views\\";
            String disponent = e.Url.ToString().Split(':')[0];
            String app = e.Url.ToString().Split(':')[1];
            String argument = e.Url.ToString().Replace(disponent+":"+app+":","");
            using(StreamReader SR = new StreamReader(viewRoot+argument+".xml"))
            {

                RawCode = SR.ReadToEnd();
                Source = mMakoEngine.Preprocess(SR.ReadToEnd(), argument, false, disponent + ":" + app + ":" + argument, false);
                
            }
            
        }
        public void Browse(String Uri)
        {
            Navigate(Uri);
        }
    }
}
