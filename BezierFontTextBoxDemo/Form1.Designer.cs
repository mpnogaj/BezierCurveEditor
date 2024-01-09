namespace BezierFontTextBoxDemo
{
	partial class Form1
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.bezierTextBox = new BezierFontTextBoxDemo.BezierTextBox();
			this.loadFontBtn = new System.Windows.Forms.Button();
			this.textBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoScroll = true;
			this.panel1.BackColor = System.Drawing.SystemColors.Highlight;
			this.panel1.Controls.Add(this.bezierTextBox);
			this.panel1.Location = new System.Drawing.Point(12, 179);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1343, 627);
			this.panel1.TabIndex = 1;
			// 
			// bezierTextBox
			// 
			this.bezierTextBox.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.bezierTextBox.FontPath = null;
			this.bezierTextBox.Location = new System.Drawing.Point(0, 0);
			this.bezierTextBox.Name = "bezierTextBox";
			this.bezierTextBox.Size = new System.Drawing.Size(1337, 627);
			this.bezierTextBox.TabIndex = 0;
			// 
			// loadFontBtn
			// 
			this.loadFontBtn.Location = new System.Drawing.Point(12, 12);
			this.loadFontBtn.Name = "loadFontBtn";
			this.loadFontBtn.Size = new System.Drawing.Size(75, 23);
			this.loadFontBtn.TabIndex = 2;
			this.loadFontBtn.Text = "Load font";
			this.loadFontBtn.UseVisualStyleBackColor = true;
			this.loadFontBtn.Click += new System.EventHandler(this.loadFontBtn_Click);
			// 
			// textBox
			// 
			this.textBox.AcceptsReturn = true;
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox.Location = new System.Drawing.Point(12, 57);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(1343, 116);
			this.textBox.TabIndex = 3;
			this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 38);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(81, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "Text to show";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1367, 818);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox);
			this.Controls.Add(this.loadFontBtn);
			this.Controls.Add(this.panel1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private BezierTextBox bezierTextBox;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button loadFontBtn;
		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.Label label1;
	}
}

