﻿namespace BezierCurveEditor.Controls
{
	partial class Canvas
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
			// Canvas
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Name = "Canvas";
			this.Size = new System.Drawing.Size(1187, 530);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Canvas_Paint);
			this.DoubleClick += new System.EventHandler(this.Canvas_DoubleClick);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Canvas_MouseUp);
			this.ResumeLayout(false);

		}

		#endregion
	}
}
