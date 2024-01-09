namespace BezierCurveEditor.Controls
{
	sealed partial class DraggablePoint
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
			// DraggablePoint
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Cursor = System.Windows.Forms.Cursors.Default;
			this.Name = "DraggablePoint";
			this.Size = new System.Drawing.Size(10, 10);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.DraggablePoint_Paint);
			this.ResumeLayout(false);

		}

		#endregion
	}
}
