namespace BezierCurveEditor
{
	partial class CharacterBoxPosForm
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
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxX = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxY = new System.Windows.Forms.TextBox();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.okBtn = new System.Windows.Forms.Button();
			this.cancelBtn = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(121, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Char box position X";
			// 
			// textBoxX
			// 
			this.textBoxX.Location = new System.Drawing.Point(8, 39);
			this.textBoxX.Name = "textBoxX";
			this.textBoxX.Size = new System.Drawing.Size(210, 22);
			this.textBoxX.TabIndex = 1;
			this.textBoxX.Validating += new System.ComponentModel.CancelEventHandler(this.IntTextBox_Validating);
			this.textBoxX.Validated += new System.EventHandler(this.IntTextBox_Validated);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(122, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Char box position Y";
			// 
			// textBoxY
			// 
			this.textBoxY.Location = new System.Drawing.Point(8, 83);
			this.textBoxY.Name = "textBoxY";
			this.textBoxY.Size = new System.Drawing.Size(210, 22);
			this.textBoxY.TabIndex = 3;
			this.textBoxY.Validating += new System.ComponentModel.CancelEventHandler(this.IntTextBox_Validating);
			this.textBoxY.Validated += new System.EventHandler(this.IntTextBox_Validated);
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// okBtn
			// 
			this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okBtn.Location = new System.Drawing.Point(179, 111);
			this.okBtn.Name = "okBtn";
			this.okBtn.Size = new System.Drawing.Size(75, 23);
			this.okBtn.TabIndex = 4;
			this.okBtn.Text = "Ok";
			this.okBtn.UseVisualStyleBackColor = true;
			// 
			// cancelBtn
			// 
			this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelBtn.Location = new System.Drawing.Point(98, 111);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.Size = new System.Drawing.Size(75, 23);
			this.cancelBtn.TabIndex = 5;
			this.cancelBtn.Text = "Cancel";
			this.cancelBtn.UseVisualStyleBackColor = true;
			// 
			// CharacterBoxPosForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.CancelButton = this.cancelBtn;
			this.ClientSize = new System.Drawing.Size(310, 163);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.okBtn);
			this.Controls.Add(this.textBoxY);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxX);
			this.Controls.Add(this.label1);
			this.Name = "CharacterBoxPosForm";
			this.Padding = new System.Windows.Forms.Padding(5);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Set char box position";
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxX;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxY;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.Button okBtn;
	}
}