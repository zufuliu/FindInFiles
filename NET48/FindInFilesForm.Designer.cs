using System.Windows.Forms;

namespace FindInFiles {
	partial class FindInFilesForm {
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxSearchPath = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxPattern = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxContexLine = new System.Windows.Forms.TextBox();
			this.checkBoxMatchCase = new System.Windows.Forms.CheckBox();
			this.buttonFind = new System.Windows.Forms.Button();
			this.richTextBox = new System.Windows.Forms.RichTextBox();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.clearAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.selectFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.checkBoxInvert = new System.Windows.Forms.CheckBox();
			this.checkBoxMultiline = new System.Windows.Forms.CheckBox();
			this.checkBoxPcre2 = new System.Windows.Forms.CheckBox();
			this.checkBoxRecursive = new System.Windows.Forms.CheckBox();
			this.fontDialog = new System.Windows.Forms.FontDialog();
			this.checkBoxRegex = new System.Windows.Forms.CheckBox();
			this.checkBoxWholeWord = new System.Windows.Forms.CheckBox();
			this.textBoxGlob = new System.Windows.Forms.TextBox();
			this.contextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(23, 12);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(77, 12);
			this.label2.TabIndex = 3;
			this.label2.Text = "Search Path:";
			// 
			// textBoxSearchPath
			// 
			this.textBoxSearchPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxSearchPath.Location = new System.Drawing.Point(103, 8);
			this.textBoxSearchPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.textBoxSearchPath.Name = "textBoxSearchPath";
			this.textBoxSearchPath.Size = new System.Drawing.Size(570, 21);
			this.textBoxSearchPath.TabIndex = 0;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 36);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(95, 12);
			this.label3.TabIndex = 5;
			this.label3.Text = "Search Pattern:";
			// 
			// textBoxPattern
			// 
			this.textBoxPattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxPattern.Location = new System.Drawing.Point(103, 32);
			this.textBoxPattern.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.textBoxPattern.Name = "textBoxPattern";
			this.textBoxPattern.Size = new System.Drawing.Size(677, 21);
			this.textBoxPattern.TabIndex = 1;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 60);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(89, 12);
			this.label4.TabIndex = 7;
			this.label4.Text = "Context Lines:";
			// 
			// textBoxContexLine
			// 
			this.textBoxContexLine.Location = new System.Drawing.Point(103, 56);
			this.textBoxContexLine.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.textBoxContexLine.Name = "textBoxContexLine";
			this.textBoxContexLine.Size = new System.Drawing.Size(43, 21);
			this.textBoxContexLine.TabIndex = 2;
			this.textBoxContexLine.Text = "0";
			// 
			// checkBoxMatchCase
			// 
			this.checkBoxMatchCase.AutoSize = true;
			this.checkBoxMatchCase.Checked = true;
			this.checkBoxMatchCase.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxMatchCase.Location = new System.Drawing.Point(210, 58);
			this.checkBoxMatchCase.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.checkBoxMatchCase.Name = "checkBoxMatchCase";
			this.checkBoxMatchCase.Size = new System.Drawing.Size(84, 16);
			this.checkBoxMatchCase.TabIndex = 4;
			this.checkBoxMatchCase.Text = "Match case";
			this.checkBoxMatchCase.UseVisualStyleBackColor = true;
			// 
			// buttonFind
			// 
			this.buttonFind.Location = new System.Drawing.Point(678, 55);
			this.buttonFind.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.buttonFind.Name = "buttonFind";
			this.buttonFind.Size = new System.Drawing.Size(64, 21);
			this.buttonFind.TabIndex = 11;
			this.buttonFind.Text = "Find All";
			this.buttonFind.UseVisualStyleBackColor = true;
			this.buttonFind.Click += new System.EventHandler(this.buttonStart_Click);
			// 
			// richTextBox
			// 
			this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.richTextBox.ContextMenuStrip = this.contextMenuStrip;
			this.richTextBox.DetectUrls = false;
			this.richTextBox.Font = new System.Drawing.Font("Consolas", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextBox.Location = new System.Drawing.Point(6, 79);
			this.richTextBox.Name = "richTextBox";
			this.richTextBox.Size = new System.Drawing.Size(773, 480);
			this.richTextBox.TabIndex = 12;
			this.richTextBox.Text = "";
			this.richTextBox.WordWrap = false;
			this.richTextBox.DoubleClick += new System.EventHandler(this.richTextBox_DoubleClick);
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearAllToolStripMenuItem,
            this.toolStripMenuItem1,
            this.selectFontToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.Size = new System.Drawing.Size(140, 54);
			// 
			// clearAllToolStripMenuItem
			// 
			this.clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
			this.clearAllToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
			this.clearAllToolStripMenuItem.Text = "Clear All";
			this.clearAllToolStripMenuItem.Click += new System.EventHandler(this.clearAllToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(136, 6);
			// 
			// selectFontToolStripMenuItem
			// 
			this.selectFontToolStripMenuItem.Name = "selectFontToolStripMenuItem";
			this.selectFontToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
			this.selectFontToolStripMenuItem.Text = "Select Font";
			this.selectFontToolStripMenuItem.Click += new System.EventHandler(this.selectFontToolStripMenuItem_Click);
			// 
			// checkBoxInvert
			// 
			this.checkBoxInvert.AutoSize = true;
			this.checkBoxInvert.Location = new System.Drawing.Point(613, 58);
			this.checkBoxInvert.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.checkBoxInvert.Name = "checkBoxInvert";
			this.checkBoxInvert.Size = new System.Drawing.Size(60, 16);
			this.checkBoxInvert.TabIndex = 9;
			this.checkBoxInvert.Text = "Invert";
			this.checkBoxInvert.UseVisualStyleBackColor = true;
			// 
			// checkBoxMultiline
			// 
			this.checkBoxMultiline.AutoSize = true;
			this.checkBoxMultiline.Location = new System.Drawing.Point(388, 58);
			this.checkBoxMultiline.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.checkBoxMultiline.Name = "checkBoxMultiline";
			this.checkBoxMultiline.Size = new System.Drawing.Size(78, 16);
			this.checkBoxMultiline.TabIndex = 6;
			this.checkBoxMultiline.Text = "Multiline";
			this.checkBoxMultiline.UseVisualStyleBackColor = true;
			// 
			// checkBoxPcre2
			// 
			this.checkBoxPcre2.AutoSize = true;
			this.checkBoxPcre2.Location = new System.Drawing.Point(471, 58);
			this.checkBoxPcre2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.checkBoxPcre2.Name = "checkBoxPcre2";
			this.checkBoxPcre2.Size = new System.Drawing.Size(54, 16);
			this.checkBoxPcre2.TabIndex = 7;
			this.checkBoxPcre2.Text = "PCRE2";
			this.checkBoxPcre2.UseVisualStyleBackColor = true;
			// 
			// checkBoxRecursive
			// 
			this.checkBoxRecursive.AutoSize = true;
			this.checkBoxRecursive.Checked = true;
			this.checkBoxRecursive.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxRecursive.Location = new System.Drawing.Point(530, 58);
			this.checkBoxRecursive.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.checkBoxRecursive.Name = "checkBoxRecursive";
			this.checkBoxRecursive.Size = new System.Drawing.Size(78, 16);
			this.checkBoxRecursive.TabIndex = 8;
			this.checkBoxRecursive.Text = "Recursive";
			this.checkBoxRecursive.UseVisualStyleBackColor = true;
			// 
			// fontDialog
			// 
			this.fontDialog.ShowApply = true;
			this.fontDialog.ShowColor = true;
			this.fontDialog.Apply += new System.EventHandler(this.fontDialog_Apply);
			// 
			// checkBoxRegex
			// 
			this.checkBoxRegex.AutoSize = true;
			this.checkBoxRegex.Checked = true;
			this.checkBoxRegex.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxRegex.Location = new System.Drawing.Point(151, 58);
			this.checkBoxRegex.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.checkBoxRegex.Name = "checkBoxRegex";
			this.checkBoxRegex.Size = new System.Drawing.Size(54, 16);
			this.checkBoxRegex.TabIndex = 3;
			this.checkBoxRegex.Text = "Regex";
			this.checkBoxRegex.UseVisualStyleBackColor = true;
			// 
			// checkBoxWholeWord
			// 
			this.checkBoxWholeWord.AutoSize = true;
			this.checkBoxWholeWord.Location = new System.Drawing.Point(299, 58);
			this.checkBoxWholeWord.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.checkBoxWholeWord.Name = "checkBoxWholeWord";
			this.checkBoxWholeWord.Size = new System.Drawing.Size(84, 16);
			this.checkBoxWholeWord.TabIndex = 5;
			this.checkBoxWholeWord.Text = "Whole word";
			this.checkBoxWholeWord.UseVisualStyleBackColor = true;
			// 
			// textBoxGlob
			// 
			this.textBoxGlob.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxGlob.Location = new System.Drawing.Point(679, 8);
			this.textBoxGlob.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.textBoxGlob.Name = "textBoxGlob";
			this.textBoxGlob.Size = new System.Drawing.Size(100, 21);
			this.textBoxGlob.TabIndex = 10;
			this.textBoxGlob.Text = "*.*";
			// 
			// FindInFilesForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 561);
			this.Controls.Add(this.textBoxGlob);
			this.Controls.Add(this.checkBoxWholeWord);
			this.Controls.Add(this.checkBoxRegex);
			this.Controls.Add(this.checkBoxRecursive);
			this.Controls.Add(this.checkBoxPcre2);
			this.Controls.Add(this.checkBoxMultiline);
			this.Controls.Add(this.checkBoxInvert);
			this.Controls.Add(this.richTextBox);
			this.Controls.Add(this.buttonFind);
			this.Controls.Add(this.checkBoxMatchCase);
			this.Controls.Add(this.textBoxContexLine);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textBoxPattern);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBoxSearchPath);
			this.Controls.Add(this.label2);
			this.DoubleBuffered = true;
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "FindInFilesForm";
			this.Text = "Find in Files";
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FindInFilesForm_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FindInFilesForm_DragEnter);
			this.contextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private Label label2;
		private TextBox textBoxSearchPath;
		private Label label3;
		private TextBox textBoxPattern;
		private Label label4;
		private TextBox textBoxContexLine;
		private CheckBox checkBoxMatchCase;
		private Button buttonFind;
		private RichTextBox richTextBox;
		private CheckBox checkBoxInvert;
		private CheckBox checkBoxMultiline;
		private CheckBox checkBoxPcre2;
		private CheckBox checkBoxRecursive;
		private FontDialog fontDialog;
		private ContextMenuStrip contextMenuStrip;
		private ToolStripMenuItem selectFontToolStripMenuItem;
		private ToolStripMenuItem clearAllToolStripMenuItem;
		private ToolStripSeparator toolStripMenuItem1;
		private CheckBox checkBoxRegex;
		private CheckBox checkBoxWholeWord;
		private TextBox textBoxGlob;
	}
}
