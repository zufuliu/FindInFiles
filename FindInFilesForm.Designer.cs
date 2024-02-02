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
			components = new System.ComponentModel.Container();
			label2 = new Label();
			label3 = new Label();
			label4 = new Label();
			textBoxContexLine = new TextBox();
			checkBoxMatchCase = new CheckBox();
			buttonFind = new Button();
			richTextBox = new RichTextBox();
			contextMenuStrip = new ContextMenuStrip(components);
			clearAllToolStripMenuItem = new ToolStripMenuItem();
			toolStripMenuItem1 = new ToolStripSeparator();
			selectFontToolStripMenuItem = new ToolStripMenuItem();
			checkBoxInvert = new CheckBox();
			checkBoxMultiline = new CheckBox();
			checkBoxPcre2 = new CheckBox();
			checkBoxRecursive = new CheckBox();
			fontDialog = new FontDialog();
			checkBoxWholeWord = new CheckBox();
			checkBoxRegex = new CheckBox();
			textBoxGlob = new TextBox();
			textBoxEncoding = new TextBox();
			comboBoxSearchPath = new ComboBox();
			comboBoxSearchPattern = new ComboBox();
			saveSearchHistoryToolStripMenuItem = new ToolStripMenuItem();
			clearSearchHistoryToolStripMenuItem = new ToolStripMenuItem();
			toolStripMenuItem3 = new ToolStripSeparator();
			contextMenuStrip.SuspendLayout();
			SuspendLayout();
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(23, 15);
			label2.Name = "label2";
			label2.Size = new Size(79, 17);
			label2.TabIndex = 3;
			label2.Text = "Search Path:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(7, 41);
			label3.Name = "label3";
			label3.Size = new Size(95, 17);
			label3.TabIndex = 5;
			label3.Text = "Search Pattern:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(14, 69);
			label4.Name = "label4";
			label4.Size = new Size(88, 17);
			label4.TabIndex = 7;
			label4.Text = "Context Lines:";
			// 
			// textBoxContexLine
			// 
			textBoxContexLine.Location = new Point(108, 66);
			textBoxContexLine.Name = "textBoxContexLine";
			textBoxContexLine.Size = new Size(50, 23);
			textBoxContexLine.TabIndex = 2;
			textBoxContexLine.Text = "0";
			// 
			// checkBoxMatchCase
			// 
			checkBoxMatchCase.AutoSize = true;
			checkBoxMatchCase.Checked = true;
			checkBoxMatchCase.CheckState = CheckState.Checked;
			checkBoxMatchCase.Location = new Point(233, 68);
			checkBoxMatchCase.Name = "checkBoxMatchCase";
			checkBoxMatchCase.Size = new Size(93, 21);
			checkBoxMatchCase.TabIndex = 4;
			checkBoxMatchCase.Text = "Match case";
			checkBoxMatchCase.UseVisualStyleBackColor = true;
			// 
			// buttonFind
			// 
			buttonFind.Location = new Point(742, 63);
			buttonFind.Name = "buttonFind";
			buttonFind.Size = new Size(75, 30);
			buttonFind.TabIndex = 12;
			buttonFind.Text = "Find All";
			buttonFind.UseVisualStyleBackColor = true;
			buttonFind.Click += buttonStart_Click;
			// 
			// richTextBox
			// 
			richTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			richTextBox.BorderStyle = BorderStyle.FixedSingle;
			richTextBox.ContextMenuStrip = contextMenuStrip;
			richTextBox.DetectUrls = false;
			richTextBox.Font = new Font("Consolas", 10.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
			richTextBox.Location = new Point(7, 95);
			richTextBox.Name = "richTextBox";
			richTextBox.Size = new Size(821, 462);
			richTextBox.TabIndex = 13;
			richTextBox.Text = "";
			richTextBox.WordWrap = false;
			richTextBox.DoubleClick += richTextBox_DoubleClick;
			// 
			// contextMenuStrip
			// 
			contextMenuStrip.Items.AddRange(new ToolStripItem[] { clearAllToolStripMenuItem, toolStripMenuItem1, saveSearchHistoryToolStripMenuItem, clearSearchHistoryToolStripMenuItem, toolStripMenuItem3, selectFontToolStripMenuItem });
			contextMenuStrip.Name = "contextMenuStrip";
			contextMenuStrip.Size = new Size(195, 126);
			// 
			// clearAllToolStripMenuItem
			// 
			clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
			clearAllToolStripMenuItem.Size = new Size(194, 22);
			clearAllToolStripMenuItem.Text = "Clear Find Result";
			clearAllToolStripMenuItem.Click += clearFindResultToolStripMenuItem_Click;
			// 
			// toolStripMenuItem1
			// 
			toolStripMenuItem1.Name = "toolStripMenuItem1";
			toolStripMenuItem1.Size = new Size(191, 6);
			// 
			// selectFontToolStripMenuItem
			// 
			selectFontToolStripMenuItem.Name = "selectFontToolStripMenuItem";
			selectFontToolStripMenuItem.Size = new Size(194, 22);
			selectFontToolStripMenuItem.Text = "Select Font";
			selectFontToolStripMenuItem.Click += selectFontToolStripMenuItem_Click;
			// 
			// checkBoxInvert
			// 
			checkBoxInvert.AutoSize = true;
			checkBoxInvert.Location = new Point(676, 68);
			checkBoxInvert.Name = "checkBoxInvert";
			checkBoxInvert.Size = new Size(60, 21);
			checkBoxInvert.TabIndex = 9;
			checkBoxInvert.Text = "Invert";
			checkBoxInvert.UseVisualStyleBackColor = true;
			// 
			// checkBoxMultiline
			// 
			checkBoxMultiline.AutoSize = true;
			checkBoxMultiline.Location = new Point(436, 68);
			checkBoxMultiline.Name = "checkBoxMultiline";
			checkBoxMultiline.Size = new Size(76, 21);
			checkBoxMultiline.TabIndex = 6;
			checkBoxMultiline.Text = "Multiline";
			checkBoxMultiline.UseVisualStyleBackColor = true;
			// 
			// checkBoxPcre2
			// 
			checkBoxPcre2.AutoSize = true;
			checkBoxPcre2.Location = new Point(518, 68);
			checkBoxPcre2.Name = "checkBoxPcre2";
			checkBoxPcre2.Size = new Size(64, 21);
			checkBoxPcre2.TabIndex = 7;
			checkBoxPcre2.Text = "PCRE2";
			checkBoxPcre2.UseVisualStyleBackColor = true;
			// 
			// checkBoxRecursive
			// 
			checkBoxRecursive.AutoSize = true;
			checkBoxRecursive.Checked = true;
			checkBoxRecursive.CheckState = CheckState.Checked;
			checkBoxRecursive.Location = new Point(588, 68);
			checkBoxRecursive.Name = "checkBoxRecursive";
			checkBoxRecursive.Size = new Size(82, 21);
			checkBoxRecursive.TabIndex = 8;
			checkBoxRecursive.Text = "Recursive";
			checkBoxRecursive.UseVisualStyleBackColor = true;
			// 
			// fontDialog
			// 
			fontDialog.ShowApply = true;
			fontDialog.ShowColor = true;
			fontDialog.Apply += fontDialog_Apply;
			// 
			// checkBoxWholeWord
			// 
			checkBoxWholeWord.AutoSize = true;
			checkBoxWholeWord.Location = new Point(332, 68);
			checkBoxWholeWord.Name = "checkBoxWholeWord";
			checkBoxWholeWord.Size = new Size(98, 21);
			checkBoxWholeWord.TabIndex = 5;
			checkBoxWholeWord.Text = "Whole word";
			checkBoxWholeWord.UseVisualStyleBackColor = true;
			// 
			// checkBoxRegex
			// 
			checkBoxRegex.AutoSize = true;
			checkBoxRegex.Checked = true;
			checkBoxRegex.CheckState = CheckState.Checked;
			checkBoxRegex.Location = new Point(164, 68);
			checkBoxRegex.Name = "checkBoxRegex";
			checkBoxRegex.Size = new Size(63, 21);
			checkBoxRegex.TabIndex = 3;
			checkBoxRegex.Text = "Regex";
			checkBoxRegex.UseVisualStyleBackColor = true;
			// 
			// textBoxGlob
			// 
			textBoxGlob.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			textBoxGlob.Location = new Point(676, 12);
			textBoxGlob.Name = "textBoxGlob";
			textBoxGlob.Size = new Size(151, 23);
			textBoxGlob.TabIndex = 10;
			textBoxGlob.Text = "*.*";
			// 
			// textBoxEncoding
			// 
			textBoxEncoding.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			textBoxEncoding.Location = new Point(676, 38);
			textBoxEncoding.Name = "textBoxEncoding";
			textBoxEncoding.Size = new Size(151, 23);
			textBoxEncoding.TabIndex = 11;
			// 
			// comboBoxSearchPath
			// 
			comboBoxSearchPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			comboBoxSearchPath.FormattingEnabled = true;
			comboBoxSearchPath.Location = new Point(108, 12);
			comboBoxSearchPath.Name = "comboBoxSearchPath";
			comboBoxSearchPath.Size = new Size(562, 25);
			comboBoxSearchPath.TabIndex = 0;
			// 
			// comboBoxSearchPattern
			// 
			comboBoxSearchPattern.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			comboBoxSearchPattern.FormattingEnabled = true;
			comboBoxSearchPattern.Location = new Point(108, 38);
			comboBoxSearchPattern.Name = "comboBoxSearchPattern";
			comboBoxSearchPattern.Size = new Size(562, 25);
			comboBoxSearchPattern.TabIndex = 1;
			// 
			// saveSearchHistoryToolStripMenuItem
			// 
			saveSearchHistoryToolStripMenuItem.Name = "saveSearchHistoryToolStripMenuItem";
			saveSearchHistoryToolStripMenuItem.Size = new Size(194, 22);
			saveSearchHistoryToolStripMenuItem.Text = "Save Search History";
			saveSearchHistoryToolStripMenuItem.Click += saveSearchHistoryToolStripMenuItem_Click;
			// 
			// clearSearchHistoryToolStripMenuItem
			// 
			clearSearchHistoryToolStripMenuItem.Name = "clearSearchHistoryToolStripMenuItem";
			clearSearchHistoryToolStripMenuItem.Size = new Size(194, 22);
			clearSearchHistoryToolStripMenuItem.Text = "Clear Search History";
			clearSearchHistoryToolStripMenuItem.Click += clearSearchHistoryToolStripMenuItem_Click;
			// 
			// toolStripMenuItem3
			// 
			toolStripMenuItem3.Name = "toolStripMenuItem3";
			toolStripMenuItem3.Size = new Size(191, 6);
			// 
			// FindInFilesForm
			// 
			AllowDrop = true;
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(834, 561);
			Controls.Add(comboBoxSearchPattern);
			Controls.Add(comboBoxSearchPath);
			Controls.Add(textBoxEncoding);
			Controls.Add(textBoxGlob);
			Controls.Add(checkBoxRegex);
			Controls.Add(checkBoxWholeWord);
			Controls.Add(checkBoxRecursive);
			Controls.Add(checkBoxPcre2);
			Controls.Add(checkBoxMultiline);
			Controls.Add(checkBoxInvert);
			Controls.Add(richTextBox);
			Controls.Add(buttonFind);
			Controls.Add(checkBoxMatchCase);
			Controls.Add(textBoxContexLine);
			Controls.Add(label4);
			Controls.Add(label3);
			Controls.Add(label2);
			DoubleBuffered = true;
			Name = "FindInFilesForm";
			Text = "Find in Files";
			FormClosing += FindInFilesForm_FormClosing;
			DragDrop += FindInFilesForm_DragDrop;
			DragEnter += FindInFilesForm_DragEnter;
			contextMenuStrip.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private Label label2;
		private Label label3;
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
		private CheckBox checkBoxWholeWord;
		private CheckBox checkBoxRegex;
		private TextBox textBoxGlob;
		private TextBox textBoxEncoding;
		private ComboBox comboBoxSearchPath;
		private ComboBox comboBoxSearchPattern;
		private ToolStripMenuItem saveSearchHistoryToolStripMenuItem;
		private ToolStripMenuItem clearSearchHistoryToolStripMenuItem;
		private ToolStripSeparator toolStripMenuItem3;
	}
}
