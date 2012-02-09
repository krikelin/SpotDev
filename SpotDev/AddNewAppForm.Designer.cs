namespace SpotDev
{
    partial class AddNewAppForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tbTitle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbNamespace = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.edVendor = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button2.Location = new System.Drawing.Point(291, 157);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button1.Location = new System.Drawing.Point(372, 157);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tbTitle
            // 
            this.tbTitle.Location = new System.Drawing.Point(13, 46);
            this.tbTitle.Name = "tbTitle";
            this.tbTitle.Size = new System.Drawing.Size(207, 20);
            this.tbTitle.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 14);
            this.label1.TabIndex = 5;
            this.label1.Text = "Title (en-US)";
            // 
            // tbNamespace
            // 
            this.tbNamespace.Location = new System.Drawing.Point(13, 95);
            this.tbNamespace.Name = "tbNamespace";
            this.tbNamespace.Size = new System.Drawing.Size(96, 20);
            this.tbNamespace.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 14);
            this.label2.TabIndex = 5;
            this.label2.Text = "Namespace";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(182, 14);
            this.label3.TabIndex = 5;
            this.label3.Text = "The folder and URI will get this name";
            // 
            // edVendor
            // 
            this.edVendor.Location = new System.Drawing.Point(124, 95);
            this.edVendor.Name = "edVendor";
            this.edVendor.Size = new System.Drawing.Size(96, 20);
            this.edVendor.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(121, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 14);
            this.label4.TabIndex = 5;
            this.label4.Text = "Vendor";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(240, 46);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(207, 20);
            this.textBox1.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(237, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 14);
            this.label5.TabIndex = 5;
            this.label5.Text = "URL";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(240, 95);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(207, 20);
            this.textBox2.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(237, 78);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 14);
            this.label6.TabIndex = 5;
            this.label6.Text = "Version";
            // 
            // AddNewAppForm
            // 
            this.AcceptButton = this.button2;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button1;
            this.ClientSize = new System.Drawing.Size(459, 192);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.edVendor);
            this.Controls.Add(this.tbNamespace);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.tbTitle);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "AddNewAppForm";
            this.Text = "Create new App Project";
            this.Load += new System.EventHandler(this.New_App_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbNamespace;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox edVendor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label6;
    }
}