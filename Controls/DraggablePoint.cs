using System;
using System.Drawing;
using System.Linq;
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

				var anyDraggableIsSelected = Curve.DraggablePoints.Any(x => x.PointSelected);
				foreach (var draggablePoint in Curve.DraggablePoints)
				{
					draggablePoint.Visible = anyDraggableIsSelected;
				}
				
				this.Curve.OnCurveShouldBeRepainted();
				this.Invalidate();
			}
		}

		private bool _removable = false;

		public bool Removable
		{
			get => _removable;
			set
			{
				if (value == _removable) return;
				_removable = value;
				if (_removable)
				{
					this.Click += DraggablePoint_Remove_Click;
				}
				else
				{
					this.Click -= DraggablePoint_Remove_Click;
				}
			}
		}

		private void DraggablePoint_Remove_Click(object sender, EventArgs e)
		{
			this.Curve.DeletePoint(this);
		}

		public void EnableDrag(bool value)
		{
			this.Draggable(value);
			this.Cursor = value ? Cursors.SizeAll : Cursors.Default;
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
		}

		private void DraggablePoint_Paint(object sender, PaintEventArgs e)
		{
			//do not draw points if curve isn't selected or any of it's points
			if (!(PointSelected || Curve.Selected || Curve.DraggablePoints.Any(x => x.PointSelected)))
				return;

			var color = (Curve.Selected || PointSelected) ? _activeColor : _inactiveColor;
			var pen = new Pen(new SolidBrush(color), 2f);

			e.Graphics.DrawRectangle(pen, ((Control)sender).ClientRectangle);
		}

		//Enable/disable isHitTest
		private const int WM_NCHITTEST = 0x84;
		private const int HTTRANSPARENT = -1;

		protected override void WndProc(ref Message message)
		{
			var shouldBeHitTestVisible = Curve.Selected | PointSelected;

			if (message.Msg == (int)WM_NCHITTEST && !shouldBeHitTestVisible)
				message.Result = (IntPtr)HTTRANSPARENT;
			else
				base.WndProc(ref message);
		}
	}
}
