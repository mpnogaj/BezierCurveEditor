using System;
using System.Drawing;
using System.Windows.Forms;

namespace BezierCurveEditor.Controls
{
	public sealed partial class DraggablePoint : UserControl
	{
		private Color _inactiveColor = Color.Red;
		private Color _activeColor = Color.Blue;

		private bool _pointSelected = false;

		public bool PointSelected
		{
			get => _pointSelected;
			set
			{
				if (value == _pointSelected) return;
				_pointSelected = value;
				this.Curve.Selected = value;

				this.Invalidate();
			}
		}

		public BezierCurve Curve { get;}

		public DraggablePoint(BezierCurve curve)
		{
			InitializeComponent();

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.UserPaint, true);

			this.BackColor = Color.Transparent;
			Curve = curve;
			PointSelected = false;
		}

		private void DraggablePoint_Paint(object sender, PaintEventArgs e)
		{
			if (Curve.Selected)
			{
				var color = PointSelected ? _activeColor : _inactiveColor;
				var pen = new Pen(new SolidBrush(color), 2f);

				var rect = ((Control)sender).ClientRectangle;
				
				e.Graphics.DrawRectangle(pen, ((Control)sender).ClientRectangle);
			}
		}

		public void RemovePoint()
		{
			this.Parent.Controls.Remove(this);
		}

		private const int WM_NCHITTEST = 0x84;
		private const int HTTRANSPARENT = -1;

		protected override void WndProc(ref Message message)
		{
			if (message.Msg == (int)WM_NCHITTEST && !Curve.Selected)
				message.Result = (IntPtr)HTTRANSPARENT;
			else
				base.WndProc(ref message);
		}
	}
}
