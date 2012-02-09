namespace SpotDev
{
    partial class SPTextEditor
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
            AIMS.Libraries.CodeEditor.WinForms.LineMarginRender lineMarginRender1 = new AIMS.Libraries.CodeEditor.WinForms.LineMarginRender();
            this.codeEditor = new AIMS.Libraries.CodeEditor.CodeEditorControl();
            this.syntaxDocument1 = new AIMS.Libraries.CodeEditor.Syntax.SyntaxDocument(this.components);
            this.SuspendLayout();
            // 
            // codeEditor
            // 
            this.codeEditor.ActiveView = AIMS.Libraries.CodeEditor.WinForms.ActiveView.BottomRight;
            this.codeEditor.AutoListPosition = null;
            this.codeEditor.AutoListSelectedText = "";
            this.codeEditor.AutoListVisible = false;
            this.codeEditor.BackColor = System.Drawing.Color.White;
            this.codeEditor.BorderColor = System.Drawing.Color.Transparent;
            this.codeEditor.BreakPointForeColor = System.Drawing.Color.Maroon;
            this.codeEditor.CopyAsRTF = false;
            this.codeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeEditor.Document = this.syntaxDocument1;
            this.codeEditor.FileName = null;
            this.codeEditor.GutterMarginBorderColor = System.Drawing.Color.White;
            this.codeEditor.GutterMarginColor = System.Drawing.Color.White;
            this.codeEditor.GutterMarginWidth = 10;
            this.codeEditor.InfoTipCount = 1;
            this.codeEditor.InfoTipPosition = null;
            this.codeEditor.InfoTipSelectedIndex = 0;
            this.codeEditor.InfoTipVisible = false;
            lineMarginRender1.Bounds = new System.Drawing.Rectangle(10, 0, 19, 16);
            this.codeEditor.LineMarginRender = lineMarginRender1;
            this.codeEditor.LineNumberBackColor = System.Drawing.Color.LightSlateGray;
            this.codeEditor.LineNumberBorderColor = System.Drawing.Color.Tan;
            this.codeEditor.Location = new System.Drawing.Point(0, 0);
            this.codeEditor.LockCursorUpdate = false;
            this.codeEditor.Name = "codeEditor";
            this.codeEditor.Saved = false;
            this.codeEditor.ScopeBackColor = System.Drawing.Color.Gainsboro;
            this.codeEditor.ScopeIndicatorColor = System.Drawing.Color.DarkGray;
            this.codeEditor.ShowScopeIndicator = false;
            this.codeEditor.Size = new System.Drawing.Size(150, 150);
            this.codeEditor.SmoothScroll = false;
            this.codeEditor.SplitviewH = -4;
            this.codeEditor.SplitviewV = -4;
            this.codeEditor.TabGuideColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.codeEditor.TabIndex = 0;
            this.codeEditor.Text = "codeEditorControl1";
            this.codeEditor.WhitespaceColor = System.Drawing.SystemColors.ControlDark;
            this.codeEditor.Click += new System.EventHandler(this.codeEditor_Click);
            // 
            // syntaxDocument1
            // 
            this.syntaxDocument1.Lines = new string[] {
        ""};
            this.syntaxDocument1.MaxUndoBufferSize = 1000;
            this.syntaxDocument1.Modified = false;
            this.syntaxDocument1.UndoStep = 0;
            // 
            // SPTextEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.codeEditor);
            this.Name = "SPTextEditor";
            this.Load += new System.EventHandler(this.SPTextEditor_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private AIMS.Libraries.CodeEditor.CodeEditorControl codeEditor;
        public AIMS.Libraries.CodeEditor.Syntax.SyntaxDocument syntaxDocument1;


    }
}
