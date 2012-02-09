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
using System.Windows.Forms;
using System.Drawing;

namespace SpotDev
{
    internal class SpotifyProfessionalRenderer : ToolStripProfessionalRenderer
    {
        public SpotifyProfessionalRenderer(ProfessionalColorTable table) : base(table)
        {
            
        }
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if(e.Item.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(Program.Skin.SelectionColor), e.Item.Bounds);
            }
            base.OnRenderMenuItemBackground(e);
        
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            
            e.TextColor = Color.White;
            
            base.OnRenderItemText(e);
        }
    }
   
    /// <summary>
    /// Orginally from http://www.codeproject.com/Articles/29497/Custom-Rendering-for-the-ToolStrip-MenuStrip-and-S
    /// </summary>
    internal class SpotifyStripColorTable :   ProfessionalColorTable
    {
        public override Color MenuStripGradientBegin
        {
            get
            {
                return Program.Skin.TabBarBackgroundColor;
            }
        }
        public override Color MenuStripGradientEnd
        {
            get
            {
                return Program.Skin.TabBarBackgroundColor;
            }
        }
        public override Color RaftingContainerGradientBegin
        {
            get
            {
                return Program.Skin.ListBackgroundColor;
            }
        }
        public override Color ImageMarginGradientBegin
        {
            get
            {
                return Program.Skin.TabBarBackgroundColor;
            }
        }
        public override Color ImageMarginGradientMiddle
        {
            get
            {
                return Program.Skin.TabBarBackgroundColor2;
            }

        }
        public override Color MenuItemSelected
        {
            get
            {
                return Program.Skin.SelectionColor;
            }
        }
        public override Color  ButtonPressedHighlight
        {
	        get 
	        {
                return Program.Skin.SelectionColor;
	        }
        }
        public override Color  ButtonPressedHighlightBorder
        {
	        get 
	        { 
		        return Program.Skin.SelectionColor;
            }
        }

        public override Color ImageMarginGradientEnd
        {
            get
            {
                return Program.Skin.TabBarBackgroundColor;
            }
        }
        public override Color RaftingContainerGradientEnd
        {
            get
            {
                return Program.Skin.ListBackgroundColor;
            }
        }
        /// <summary>
        /// Gets the border color of the System.Windows.Forms.ToolStrip control.
        /// </summary>

        public override Color ToolStripBorder
        {
            get
            {
                return Color.Black;
            }
        }

        /// <summary>
        /// Gets the starting color of the content panel gradient
        /// on System.Windows.Forms.ToolStrip control.
        /// </summary>

        public override Color ToolStripContentPanelGradientBegin
        {
            get
            {
                return Program.Skin.TabBarBackgroundColor;
            }
        }

        /// <summary>
        /// Gets the ending color of the content panel gradient
        /// on System.Windows.Forms.ToolStrip control.
        /// </summary>

        public override Color ToolStripContentPanelGradientEnd
        {
            get
            {
                return Program.Skin.TabBarBackgroundColor2;
            }
        }

        /// <summary>
        /// Gets the background color of the drop down
        /// on the System.Windows.Forms.ToolStrip control.
        /// </summary>

        public override Color ToolStripDropDownBackground
        {
            get
            {
                return Program.Skin.ListBackgroundColor;
            }
        }

        /// <summary>
        /// Gets the starting color of the gradient
        /// on the System.Windows.Forms.ToolStrip control.
        /// </summary>

        public override Color ToolStripGradientBegin
        {
            get
            {
                return Program.Skin.BackgroundColor;
            }
        }

        /// <summary>
        /// Gets the middle color of the gradient
        /// on the System.Windows.Forms.ToolStrip control.
        /// </summary>

        public override Color ToolStripGradientMiddle
        {
            get
            {
                return Program.Skin.BackgroundColor;
               
            }
        }

        /// <summary>
        /// Gets the ending color of the gradient
        /// on the System.Windows.Forms.ToolStrip control.
        /// </summary>

        public override Color ToolStripGradientEnd
        {
            get
            {
                return Program.Skin.ListBackgroundColor;
            }
        }
    }
    static class Program
    {
        public static DefaultSkin Skin = new DefaultSkin();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CefSharp.CEF.Initialize(new CefSharp.Settings(), new CefSharp.BrowserSettings());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
