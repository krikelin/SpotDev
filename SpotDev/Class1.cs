using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SpotDev
{
    class DefaultSkin
    {
        public Color BackgroundColor = Color.FromArgb(55, 55, 55);
        public Color ForegroundColor = Color.FromArgb(221, 221, 221);
        public Color SelectionColor = Color.FromArgb(172, 220, 254);


        public Color SelectedForeColor = Color.FromArgb(0, 24, 80);
        public Color ListBackgroundColor = Color.FromArgb(71, 71, 71);
        public Color TabBarBackgroundColor = Color.FromArgb(166, 166, 166);
        public Color TabBarBackgroundColor2 = Color.FromArgb(135, 135, 135);
        public LinearGradientBrush TabBarBackgroundGradient; 
        public LinearGradientBrush TabBarActiveBackgroundGradient;
        public DefaultSkin() 
        {
            TabBarActiveBackgroundGradient = new LinearGradientBrush(new Point(0, 0), new Point(0, 24), Color.FromArgb( 44, 44, 44), this.BackgroundColor);
            TabBarBackgroundGradient=  new LinearGradientBrush(new Point(0, 0), new Point(0, 28), this.TabBarBackgroundColor, this.TabBarBackgroundColor2);

        }
    }
}
