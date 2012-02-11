using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Board
{
    public partial class Scrollbar : UserControl
    {
        /// <summary>
        /// The drawboard the scrollbar belongs to
        /// </summary>
        public Board.DrawBoard Host { get; set; }
        /// <summary>
        /// Class for scrolling event args
        /// </summary>
        public class ScrollEventArgs
        {
           
            /// <summary>
            /// The position of the scroll
            /// </summary>
            public float Position { get; set; }
        }
        /// <summary>
        /// Delagate for handling scrolling events
        /// </summary>
        /// <param name="Sender">the object which raised the delegate's event</param>
        /// <param name="e">Arguments provided with the event</param>
        public delegate void ScrollEventHandler(object Sender, ScrollEventArgs e);

        /// <summary>
        /// Raises when moving the scrollbar
        /// </summary>
        public event ScrollEventHandler Scrolling;
        public enum ScrollbarOrientation
        {
            Vertical, Horizontal
        }
        public ScrollbarOrientation Orientation { get; set; }
        public Scrollbar()
        {
            InitializeComponent();
        }
        #region Private Properties
        private float position;
        private float thumbHeight;

        #endregion

        /// <summary>
        /// Gets the free space of the thumbs space
        /// </summary>
        public float FreeSpace
        {
            get
            {
                return SpaceHeight - (thumbHeight*(float)SpaceHeight);
            }
        }

        /// <summary>
        /// Gets scale of the scrollbar
        /// </summary>
        public int ScrollSize
        {
            get
            {
                return this.Width;
            }
        }

      
        /// <summary>
        /// Gets or set the size of thumb - below 100
        /// </summary>
        public float ThumbHeight
        {
           get
            {
                return thumbHeight;
            }
            set
            {
                if (value <= 1 && value >= 0)
                  thumbHeight = value;
                else
                    thumbHeight =thumbHeight > 0.5f ? 1 : 0;
            }
        }
        
        /// <summary>
        /// Gets the height of the space
        /// </summary>
        private int SpaceHeight
        {
            get
            {
                return this.Height - ScrollSize * 2;
            }
        }
    /// <summary>
    /// Gets or sets the position of the scrollbar
    /// </summary>
        public float Position
        {
            get
            {
                return position;
            }
            set
            {
                if (value <= 1 && value >= 0)
                    position = value;
                else
                    position = position > 0.5f ? 100 : 0;
            }
        }
        /// <summary>
        /// Gets the Y-positiono of the thumb
        /// </summary>
        public float ThumbPosition
        {
            get
            {
                return ScrollSize + Position * FreeSpace;
            }
        }
        /// <summary>
        /// Gets or sets if the scrollbar is dragging
        /// </summary>
        public bool Dragging { get; set; }
        /// <summary>
        /// Draws the current state of the scrollbar
        /// </summary>
        /// <param name="g">Graphics engine to draw on</param>
        private void Draw(Graphics f)
        {
            // Create graphics buffer
            BufferedGraphicsContext ER = new BufferedGraphicsContext();
            BufferedGraphics CT = ER.Allocate(f, new Rectangle(0, 0, this.Width, this.Height));
            Graphics g = CT.Graphics;
            int thumbSplit = 15;
            // Perform drawing operation
            if (Orientation == ScrollbarOrientation.Vertical)
            {
                g.DrawImage(Resource1.scrollbar_space, 0, 0, this.Width, this.Height);
                // Draw the up button
                g.DrawImage(Resource1.scrollbar_up, new Rectangle(0, 0, ScrollSize, ScrollSize));

                // Draw the down button
                g.DrawImage(Resource1.scrollbar_down,new Rectangle(0,this.Height-ScrollSize,ScrollSize,ScrollSize));
                
                // thumb heigtn
                int t_height = (int)((float)ThumbHeight*(float)SpaceHeight);
                // draw the thumb
                // Bounds of the thumn
                Rectangle t_bounds = new Rectangle(0, (int)ThumbPosition, ScrollSize, t_height);
                //g.DrawImage(Resource1.scrollbar_thumb2, new Rectangle(0, (int)ThumbPosition, ScrollSize,t_height ));
                g.FillRectangle(new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, 0), new Point(this.Width,0), Color.FromArgb(255, 255, 255), Color.FromArgb(211, 211, 211)),t_bounds );
                g.DrawRectangle(new Pen(Color.FromArgb(188, 188, 188)),t_bounds);


#if (nobug)
                g.DrawImage(Resource1.scrollbar_thumb, new Rectangle(0, (int)ThumbPosition, ScrollSize, ScrollSize),new Rectangle(0,0,ScrollSize,thumbSplit),GraphicsUnit.Pixel);
                g.DrawImage(Resource1.scrollbar_thumb, new Rectangle(0, (int)ThumbPosition+t_height-ScrollSize, ScrollSize, ScrollSize), new Rectangle(0, thumbSplit, ScrollSize, thumbSplit), GraphicsUnit.Pixel);
#endif
            }

            // Result
            CT.Render();
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Draw(e.Graphics);
        }
        private void Scrollbar_Paint(object sender, PaintEventArgs e)
        {
            Draw(e.Graphics);
        }
        public enum ElementOver
        {
            Space,Start, Thumb, End
        }
        /// <summary>
        /// Space between the top of the thumb and the mouse cursor in pixels
        /// </summary>
        int frontY;
        // Decides what mouse is over
        public ElementOver MousePosition { get; set; }
        private void Scrollbar_MouseMove(object sender, MouseEventArgs e)
        {
            MousePosition = ElementOver.Space;
            mouseX = e.X;
            mouseY = e.Y;
            if(mouseY >= 0 && mouseY <= ScrollSize)
            MousePosition = ElementOver.Space;
            if (mouseY >= this.Height - this.ScrollSize && mouseY <= this.Height)
                MousePosition = ElementOver.End;
             
            
                /**
                 * Handle vertical dragging
                 * */
                if (Orientation == ScrollbarOrientation.Vertical)
                {
                    
                    /**
                     * If mouse cursor is witin the thumb's bounds, 
                     * assert the cursor:
                     * */
                    if (e.Y >= ThumbPosition && e.Y >= ThumbHeight + ThumbPosition)
                    {
                        MousePosition = ElementOver.Thumb;
                    }
                    if (e.Y >= ScrollSize)
                    {

                        int mY = (e.Y - ScrollSize) -frontY;// -(int)Math.Round((float)e.Y - ThumbPosition);

                        // Get scroll position
                        if (mY <= FreeSpace)
                        {
                            if (this.Dragging)
                            {
                                MousePosition = ElementOver.Thumb;
                               // float pos = ((float)mY - ((float)Math.Round((float)e.Y - mY)));
                                Position = mY / (float)FreeSpace;
                                /**
                                 * If scroll event is set, raise it
                                 * */
                                if (this.Scrolling != null)
                                {
                                    ScrollEventArgs ex = new ScrollEventArgs();
                                    ex.Position = position;
                                    Scrolling(this, ex);
                                }
                                /**
                                 * If an drawboard is set set it's scrollY
                                 * */
                                if (Host != null)
                                {
                                    Host.scrollY = (int)((float)position * (float)Host.ItemOffset);
                                }
                            }
                        }
                    }
                }
            
        }

        // Mouse coordinates
        private int mouseX, mouseY;
        private void Scrollbar_MouseDown(object sender, MouseEventArgs e)
        {
            switch (MousePosition)
            {
                case ElementOver.Start:
                    break;
                case ElementOver.Thumb:
                    Dragging = true;
                    frontY = (int)(e.Y - ThumbPosition);
                    break;
                case ElementOver.End:
                    break;
            }
         


        }

        private void Scrollbar_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Draw(this.CreateGraphics());
        }

        private void Scrollbar_MouseUp(object sender, MouseEventArgs e)
        {
            // Disable dragging mode
            this.Dragging = false;
        }
    }
}
