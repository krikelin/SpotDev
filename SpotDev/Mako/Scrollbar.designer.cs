﻿namespace Board
{
    partial class Scrollbar
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Scrollbar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "Scrollbar";
            this.Size = new System.Drawing.Size(18, 565);
            this.Load += new System.EventHandler(this.Scrollbar_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Scrollbar_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Scrollbar_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Scrollbar_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Scrollbar_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
    }
}
