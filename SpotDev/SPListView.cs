using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace SpotDev
{
    [Serializable]
    public partial class SPListView : UserControl
    {
        private int scrollY = 0;
        /// <summary>
        /// Event args
        /// </summary>
        public class SPListItemEventArgs
        {
            public SPListItem Item { get; set; }
        }
        public delegate void SPListItemMouseEventHandler(object Sender, SPListItemEventArgs e);
        public event SPListItemMouseEventHandler ItemSelected;


        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            this.Paint(e.Graphics);
        }
        /// <summary>
        /// Draw item and sub-Items
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="pos"></param>
        /// <param name="level"></param>
        private void drawItem(Graphics g, SPListItem Item, ref int pos, ref int level)
        {
            Color foreColor = Item.CustomColor ? Item.Color : Program.Skin.ForegroundColor;
            if (Item.Text.StartsWith("#"))
            {
                foreColor = Color.FromArgb(150,150,150);
                g.DrawString(Item.Text.ToUpper().Replace("#", ""), new Font("MS Sans Serif", 8), new SolidBrush(Color.FromArgb(50,50,50)), new Point(4, pos + 0));
                g.DrawString(Item.Text.ToUpper().Replace("#", ""), new Font("MS Sans Serif", 8), new SolidBrush(foreColor), new Point(4, pos + 1));
            }
            else
            {
                
                if (Item.Selected)
                {

                    g.DrawImage(Resources.menu_selection, 0, pos, this.Width * 500, Resources.menu_selection.Height);

                    foreColor = Program.Skin.SelectedForeColor;
                }
                else if (Item.Touched)
                {
                    g.FillRectangle(new SolidBrush(Color.Gray), 0, pos, this.Width, ItemHeight);
                }
                else
                {
                    g.DrawString(Item.Text, new Font("MS Sans Serif", 8), new SolidBrush(Color.FromArgb(10, 10, 10)), new Point(level + 32, pos + 3));
                }
                g.DrawString(Item.Text, new Font("MS Sans Serif", 8), new SolidBrush(foreColor), new Point(level+32, pos + 2));
                if (Item.Icon != null)
                {
                    g.DrawImage(Item.Icon, level+16, pos +1, 16, 16);
                } 
                // If has subItems create expander
                if (Item.Children.Count > 0)
                {
                    Image expander = Item.Expanded ? Resources.ic_expander_open : Resources.ic_expander_closed;
                    g.DrawImage(expander, level, pos, 16, 16);
                }
            } 
            pos += ItemHeight;
            // If has subitems draw them
            if(Item.Expanded)
                foreach (SPListItem subItem in Item.Children)
                {
                    level += 16;
                    drawItem(g, subItem, ref pos, ref level);
                    level -= 16;
                }
        
        }
        private void Paint(Graphics gr)
        {
            BufferedGraphicsContext c = new BufferedGraphicsContext();
            BufferedGraphics bg = c.Allocate(gr, new Rectangle(0, 0, this.Width, this.Height));
            Graphics g = bg.Graphics;
            g.FillRectangle(new SolidBrush(Program.Skin.ListBackgroundColor), 0, 0, this.Width, this.Height);
            int pos = -scrollY;
            int level = 0;
            if (Items != null)
                foreach (SPListItem Item in Items)
                {
                    drawItem(g, Item, ref pos, ref level);

                }
            g.DrawLine(new Pen(Color.Black), new Point(this.Width  -1, 0), new Point(this.Width -1, this.Height));
            bg.Render();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            int pos = -scrollY;
            // Draw all list items
            
        }
        public int ItemHeight = 20;
        public SPListView()
        {
           
            InitializeComponent();
            this.Items = new List<SPListItem>();
            this.BackColor = Program.Skin.BackgroundColor;
            this.ForeColor = Program.Skin.ForegroundColor;
            
        }
        public SPListItem AddItem(String text, Uri uri)
        {
            SPListItem c = new SPListItem(this);
            c.Text = text;
            c.Uri = uri;
          
            this.Items.Add(c);
            return c;
        }
        public SPListItem AddItem(String text, Uri uri, Image icon)
        {
            SPListItem c = new SPListItem(this);
            c.Text = text;
            c.Uri = uri;
            c.Icon = icon;
            this.Items.Add(c);
            return c;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Paint(CreateGraphics());
        }
        public List<SPListItem> Items { get; set; }
        private void ucMenu_Load(object sender, EventArgs e)
        {

        }

        private void SPListView_MouseMove(object sender, MouseEventArgs e)
        {
            
        }
        private void deselectItem(SPListItem item)
        {
            item.Selected = false;
            foreach (SPListItem subItem in item.Children)
            {
                deselectItem(subItem);    
            }
        }
        private void checkItem(SPListItem Item, MouseEventArgs e, ref int level, ref int pos)
        {
            
            if (e.Y > pos && e.Y < pos + ItemHeight)
            {
                
               


                Item.Selected = true;
                SPListItemEventArgs args = new SPListItemEventArgs();
                args.Item = Item;
                this.ItemSelected(this, args);
            }
            pos += ItemHeight;
            // If has subitems draw them
            if (Item.Expanded)
                foreach (SPListItem subItem in Item.Children)
                {
                    level += 16;
                    checkItem(subItem, e, ref level, ref pos);
                    level -= 16;
                }
        }
        private void SPListView_MouseDown(object sender, MouseEventArgs e)
        {

            int pos = -scrollY;
            int level = 0;
            // Draw all list items

            foreach (SPListItem Item in Items)
            {
                deselectTouchItem(Item);
            }
            if (Items != null)

                foreach (SPListItem Item in Items)
                {

                    touchItem(Item, e, ref level, ref pos);

                }


            this.Paint(this.CreateGraphics());
        }
        private bool expanding = false;
        private void touchItem(SPListItem Item, MouseEventArgs e, ref int level, ref int pos)
        {
            if (e.Y > pos && e.Y < pos + ItemHeight)
            {

                // If clicked on expander
                if (e.X < level + 17 && Item.Children.Count > 0)
                {
                    Item.Expanded = !Item.Expanded;
                    pos += ItemHeight;
                    expanding = true;
                    return;
                }



                Item.Touched = true;
            }
            pos += ItemHeight;
            // If has subitems draw them
            if (Item.Expanded)
                foreach (SPListItem subItem in Item.Children)
                {
                    level += 16;
                    touchItem(subItem, e, ref level, ref pos);
                    level -= 16;
                }
        }

        private void deselectTouchItem(SPListItem item)
        {
            item.Touched = false;
            foreach (SPListItem subItem in item.Children)
            {
                deselectTouchItem(subItem);
            }
        }

        private void SPListView_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                int pos = -scrollY;
                int level = 0;
                // Draw all list items
                if (!expanding)
                {
                    foreach (SPListItem Item in Items)
                    {
                        deselectItem(Item);
                    }
                    if (Items != null)

                        foreach (SPListItem Item in Items)
                        {

                            checkItem(Item, e, ref level, ref pos);

                        }
                }
                expanding = false;
                
            }
            catch (Exception ex)
            {
            }
            this.Paint(this.CreateGraphics());
        }
    }
    
}
