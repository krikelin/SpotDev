using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

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
        private void Paint(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Program.Skin.BackgroundColor), 0, 0, this.Width, this.Height);
            int pos = -scrollY;
            if (Items != null)
                foreach (SPListItem Item in Items)
                {
                    Color foreColor = Item.CustomColor ? Item.Color : Program.Skin.ForegroundColor;
                    if (Item.Text.StartsWith("#"))
                    {
                        foreColor = Color.FromArgb(100,100,100);
                        g.DrawString(Item.Text.Replace("#", ""), new Font("MS Sans Serif", 8), new SolidBrush(Color.FromArgb(50,50,50)), new Point(14, pos + 1));
                        g.DrawString(Item.Text.Replace("#", ""), new Font("MS Sans Serif", 8), new SolidBrush(foreColor), new Point(14, pos + 2));
                    }
                    else
                    {
                        if (Item.Selected)
                        {
                            g.DrawImage(Resources.menu_selection, 0, pos, this.Width, Resources.menu_selection.Height);
                            
                            foreColor = Program.Skin.SelectedForeColor;
                        }
                        g.DrawString(Item.Text, new Font("MS Sans Serif", 8), new SolidBrush(foreColor), new Point(38, pos + 2));
                        if (Item.Icon != null)
                        {
                            g.DrawImage(Item.Icon, 16, pos +1, 16, 16);
                        } 
                        // If has subItems create expander
                        if (Item.Children.Count > 0)
                        {
                            Image expander = Item.Expanded ? Resources.ic_expander_open : Resources.ic_expander_closed;
                            g.DrawImage(expander, 1, pos, 16, 16);
                        }
                    } 
                    pos += ItemHeight;
                    // If has subitems draw them
                    if(Item.Expanded)
                        foreach (SPListItem subItem in Item.Children)
                        {
                            foreColor = subItem.CustomColor ? subItem.Color : Program.Skin.ForegroundColor;
                            if (subItem.Selected)
                            {

                                g.DrawImage(Resources.menu_selection, 0, pos, this.Width, Resources.menu_selection.Height);
                            
                                foreColor = Program.Skin.SelectedForeColor;
                            }
                            g.DrawString(subItem.Text, new Font("MS Sans Serif", 8), new SolidBrush(foreColor), new Point(52, pos + 2));
                            if (Item.Icon != null)
                            {
                                g.DrawImage(Item.Icon, 32, pos + 1, 16, 16);
                            }
                            pos += ItemHeight;
                        }

                }
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
        public List<SPListItem> Items { get; set; }
        private void ucMenu_Load(object sender, EventArgs e)
        {

        }

        private void SPListView_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void SPListView_MouseDown(object sender, MouseEventArgs e)
        {
            int pos = -scrollY;
            // Draw all list items
            if(Items != null)
            foreach (SPListItem Item in Items)
            {
                
                if (e.Y > pos && e.Y < pos + ItemHeight)
                {
                    // If clicked on expander
                    if (e.X < 16 && Item.Children.Count > 0)
                    {
                        Item.Expanded = !Item.Expanded;
                        break;
                    }
                    // Deselect all items
                    foreach(SPListItem item in this.Items) 
                    {
                        item.Selected = false;
                        
                        
                        foreach (SPListItem subItem in item.Children)
                        {
                            subItem.Selected = false;
                            
                        }
                    }
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
                        if (e.Y > pos && e.Y < pos + ItemHeight)
                        {
                            
                            // Deselect all items
                            foreach (SPListItem item in  Item.Children)
                            {
                                item.Selected = false;
                           
                            }
                            // Deselect all items
                            foreach (SPListItem item in this.Items)
                            {
                                item.Selected = false;
                                // Deselect all items
                                foreach (SPListItem subItem2 in item.Children)
                                {
                                    subItem2.Selected = false;

                                }
                              

                            }
                            subItem.Selected = true;
                            SPListItemEventArgs args = new SPListItemEventArgs();
                            args.Item = subItem;
                            this.ItemSelected(this, args);
                        }

                        pos += ItemHeight;
                    }
                
            }
            this.Paint(this.CreateGraphics());
        }
    }
    
}
