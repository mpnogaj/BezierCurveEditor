using System;
using System.Drawing;
using System.Windows.Forms;

namespace BezierCurveEditor
{
	internal class ResizeableControl
	{
		private static bool _resizing;
		private static Point _cursorStartPoint;
		private static Size _currentControlStartSize;
		private static bool MouseIsInRightEdge { get; set; }
		private static bool MouseIsInBottomEdge { get; set; }
		
		internal static void Init(Control control)
		{
			Init(control, control);
		}

		internal static void Init(Control control, Control container)
		{
			_resizing = false;
			_cursorStartPoint = Point.Empty;
			MouseIsInRightEdge = false;
			MouseIsInBottomEdge = false;
			
			control.MouseDown += (sender, e) => StartMovingOrResizing(control, e);
			control.MouseUp += (sender, e) => StopDragOrResizing(control);
			control.MouseMove += (sender, e) => MoveControl(container, e);
			control.Paint += (sender, args) =>
			{
				var borderPen = new Pen(Color.Black, 0.5f);

				args.Graphics.DrawLine(borderPen, control.Width - 1, 0, control.Width - 1, control.Height - 1);
				args.Graphics.DrawLine(borderPen, 0, control.Height - 1, control.Width - 1, control.Height - 1);
			};
		}

		private static void UpdateMouseEdgeProperties(Control control, Point mouseLocationInControl)
		{
			MouseIsInRightEdge = Math.Abs(mouseLocationInControl.X - control.Width) <= 2;
			MouseIsInBottomEdge = Math.Abs(mouseLocationInControl.Y - control.Height) <= 2;
		}

		private static void UpdateMouseCursor(Control control)
		{
			if (MouseIsInRightEdge)
			{
				if (MouseIsInBottomEdge)
				{
					control.Cursor = Cursors.SizeNWSE;
				}
				else
				{
					control.Cursor = Cursors.SizeWE;
				}
			}
			else if (MouseIsInBottomEdge)
			{
				control.Cursor = Cursors.SizeNS;
			}
			else
			{
				control.Cursor = Cursors.Default;
			}
		}

		private static void StartMovingOrResizing(Control control, MouseEventArgs e)
		{
			if (_resizing)
			{
				return;
			}
			if (MouseIsInRightEdge || MouseIsInBottomEdge)
			{
				_resizing = true;
				_currentControlStartSize = control.Size;
			}

			_cursorStartPoint = new Point(e.X, e.Y);
			control.Capture = true;
		}

		private static void MoveControl(Control control, MouseEventArgs e)
		{
			if (!_resizing)
			{
				UpdateMouseEdgeProperties(control, new Point(e.X, e.Y));
				UpdateMouseCursor(control);
			}
			if (_resizing)
			{
				if (MouseIsInRightEdge)
				{
					if (MouseIsInBottomEdge)
					{
						control.Width = (e.X - _cursorStartPoint.X) + _currentControlStartSize.Width;
						control.Height = (e.Y - _cursorStartPoint.Y) + _currentControlStartSize.Height;
					}
					else
					{
						control.Width = (e.X - _cursorStartPoint.X) + _currentControlStartSize.Width;
					}
				}
				else if (MouseIsInBottomEdge)
				{
					control.Height = (e.Y - _cursorStartPoint.Y) + _currentControlStartSize.Height;
				}
				else
				{
					StopDragOrResizing(control);
				}

				control.Invalidate();
			}
		}

		private static void StopDragOrResizing(Control control)
		{
			_resizing = false;
			control.Capture = false;
			UpdateMouseCursor(control);
		}
	}

	internal static class ResizeableControlExtensions
	{
		public static void Resizeable(this Control control)
		{
			ResizeableControl.Init(control);
		}
	}
}
