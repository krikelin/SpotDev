﻿namespace SpotDev
{
    partial class SPListView
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
            this.SuspendLayout();
            // 
            // SPListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "SPListView";
            this.Load += new System.EventHandler(this.ucMenu_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SPListView_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SPListView_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
