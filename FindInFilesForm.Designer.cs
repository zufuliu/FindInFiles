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
			label2 = new Label();
			textBoxSearchPath = new TextBox();
			label3 = new Label();
			textBoxPattern = new TextBox();
			label4 = new Label();
			textBoxContexLine = new TextBox();
			checkBoxMatchCase = new CheckBox();
			buttonFind = new Button();
			richTextBox = new RichTextBox();
			checkBoxInvert = new CheckBox();
			checkBoxMultiline = new CheckBox();
			checkBoxPcre2 = new CheckBox();
			checkBoxRecursive = new CheckBox();
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
			// textBoxSearchPath
			// 
			textBoxSearchPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			textBoxSearchPath.Location = new Point(108, 12);
			textBoxSearchPath.Name = "textBoxSearchPath";
			textBoxSearchPath.Size = new Size(670, 23);
			textBoxSearchPath.TabIndex = 0;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(7, 45);
			label3.Name = "label3";
			label3.Size = new Size(95, 17);
			label3.TabIndex = 5;
			label3.Text = "Search Pattern:";
			// 
			// textBoxPattern
			// 
			textBoxPattern.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			textBoxPattern.Location = new Point(108, 42);
			textBoxPattern.Name = "textBoxPattern";
			textBoxPattern.Size = new Size(670, 23);
			textBoxPattern.TabIndex = 1;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(14, 79);
			label4.Name = "label4";
			label4.Size = new Size(88, 17);
			label4.TabIndex = 7;
			label4.Text = "Context Lines:";
			// 
			// textBoxContexLine
			// 
			textBoxContexLine.Location = new Point(108, 76);
			textBoxContexLine.Name = "textBoxContexLine";
			textBoxContexLine.Size = new Size(50, 23);
			textBoxContexLine.TabIndex = 2;
			// 
			// checkBoxMatchCase
			// 
			checkBoxMatchCase.AutoSize = true;
			checkBoxMatchCase.Location = new Point(254, 77);
			checkBoxMatchCase.Name = "checkBoxMatchCase";
			checkBoxMatchCase.Size = new Size(95, 21);
			checkBoxMatchCase.TabIndex = 4;
			checkBoxMatchCase.Text = "Match Case";
			checkBoxMatchCase.UseVisualStyleBackColor = true;
			// 
			// buttonFind
			// 
			buttonFind.Location = new Point(580, 72);
			buttonFind.Name = "buttonFind";
			buttonFind.Size = new Size(75, 30);
			buttonFind.TabIndex = 8;
			buttonFind.Text = "Find All";
			buttonFind.UseVisualStyleBackColor = true;
			buttonFind.Click += buttonStart_Click;
			// 
			// richTextBox
			// 
			richTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			richTextBox.BorderStyle = BorderStyle.FixedSingle;
			richTextBox.DetectUrls = false;
			richTextBox.Font = new Font("Consolas", 10.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
			richTextBox.Location = new Point(7, 108);
			richTextBox.Name = "richTextBox";
			richTextBox.Size = new Size(771, 449);
			richTextBox.TabIndex = 11;
			richTextBox.Text = "";
			richTextBox.WordWrap = false;
			richTextBox.DoubleClick += richTextBox_DoubleClick;
			// 
			// checkBoxInvert
			// 
			checkBoxInvert.AutoSize = true;
			checkBoxInvert.Location = new Point(513, 77);
			checkBoxInvert.Name = "checkBoxInvert";
			checkBoxInvert.Size = new Size(60, 21);
			checkBoxInvert.TabIndex = 7;
			checkBoxInvert.Text = "Invert";
			checkBoxInvert.UseVisualStyleBackColor = true;
			// 
			// checkBoxMultiline
			// 
			checkBoxMultiline.AutoSize = true;
			checkBoxMultiline.Location = new Point(357, 77);
			checkBoxMultiline.Name = "checkBoxMultiline";
			checkBoxMultiline.Size = new Size(76, 21);
			checkBoxMultiline.TabIndex = 5;
			checkBoxMultiline.Text = "Multiline";
			checkBoxMultiline.UseVisualStyleBackColor = true;
			// 
			// checkBoxPcre2
			// 
			checkBoxPcre2.AutoSize = true;
			checkBoxPcre2.Location = new Point(441, 77);
			checkBoxPcre2.Name = "checkBoxPcre2";
			checkBoxPcre2.Size = new Size(64, 21);
			checkBoxPcre2.TabIndex = 6;
			checkBoxPcre2.Text = "PCRE2";
			checkBoxPcre2.UseVisualStyleBackColor = true;
			// 
			// checkBoxRecursive
			// 
			checkBoxRecursive.AutoSize = true;
			checkBoxRecursive.Location = new Point(164, 77);
			checkBoxRecursive.Name = "checkBoxRecursive";
			checkBoxRecursive.Size = new Size(82, 21);
			checkBoxRecursive.TabIndex = 3;
			checkBoxRecursive.Text = "Recursive";
			checkBoxRecursive.UseVisualStyleBackColor = true;
			// 
			// FindInFilesForm
			// 
			AllowDrop = true;
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(784, 561);
			Controls.Add(checkBoxRecursive);
			Controls.Add(checkBoxPcre2);
			Controls.Add(checkBoxMultiline);
			Controls.Add(checkBoxInvert);
			Controls.Add(richTextBox);
			Controls.Add(buttonFind);
			Controls.Add(checkBoxMatchCase);
			Controls.Add(textBoxContexLine);
			Controls.Add(label4);
			Controls.Add(textBoxPattern);
			Controls.Add(label3);
			Controls.Add(textBoxSearchPath);
			Controls.Add(label2);
			DoubleBuffered = true;
			Name = "FindInFilesForm";
			Text = "Find in Files";
			DragDrop += FindInFilesForm_DragDrop;
			DragEnter += FindInFilesForm_DragEnter;
			ResumeLayout(false);
			PerformLayout();
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
	}
}
