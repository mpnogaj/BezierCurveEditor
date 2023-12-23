using BezierCurveEditor.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BezierCurveEditor
{
	public class BezierCurve
	{
		/// <summary>
		/// Occurs when curve should be repainted
		/// </summary>
		public event EventHandler<EventArgs> CurveShouldBeRepainted;
		
		private void OnCurveShouldBeRepainted()
		{
			CurveShouldBeRepainted?.Invoke(this, EventArgs.Empty);
		}
		
		/// <summary>
		/// Occurs when curve is removed
		/// </summary>
		public event EventHandler<EventArgs> CurveRemoved;

		private void OnCurveRemoved()
		{
			CurveRemoved?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when new point is added to list, or removed
		/// </summary>
		public event EventHandler<PointAddedRemovedEventArgs> PointAddedRemoved;

		private void OnPointAddedRemoved(Method method, DraggablePoint point, int index)
		{
			PointAddedRemoved?.Invoke(this, new PointAddedRemovedEventArgs(method, point, index));
		}
		

		private bool _selected = true;
		public bool Selected
		{
			get => _selected;
			set
			{
				if (value == _selected) return;
				_selected = value;

				foreach (var draggablePoint in DraggablePoints)
				{
					draggablePoint.Visible = value;
				}

				OnCurveShouldBeRepainted();
			}
		}

		public ObservableCollection<DraggablePoint> DraggablePoints { get; } = new ObservableCollection<DraggablePoint>();
		
		public List<Point> ControlPoints => DraggablePoints.Select(x =>
		{
			var dx = x.Size.Width / 2;
			var dy = x.Size.Height / 2;
			var location = x.Location;
			location.Offset(dx, dy);
			return location;
		}).ToList();

		private readonly Canvas _parentControl;

		public BezierCurve(List<Point> points, Canvas parent)
		{
			_parentControl = parent;
			InitializePoints(points);

			Selected = false;
		}

		private void InitializePoints(List<Point> points)
		{
			foreach (var point in points)
			{
				var draggablePoint = new DraggablePoint(this);
				draggablePoint.LocationChanged += DraggablePoint_LocationChanged;
				draggablePoint.Location = point;
				DraggablePoints.Add(draggablePoint);
			}
			
			_parentControl.Controls.AddRange(DraggablePoints.Cast<Control>().ToArray());
		}


		public void InsertPoint(Point point, int index)
		{
			var draggablePoint = new DraggablePoint(this);
			draggablePoint.LocationChanged += DraggablePoint_LocationChanged;
			draggablePoint.Location = point;
			DraggablePoints.Insert(index, draggablePoint);
			_parentControl.Controls.Add(draggablePoint);

			OnPointAddedRemoved(Method.Added, draggablePoint, index);
		}

		public void DeleteCurve()
		{
			foreach (var curvePoints in DraggablePoints)
			{
				_parentControl.Controls.Remove(curvePoints);
			}
			DraggablePoints.Clear();
			OnCurveRemoved();
		}

		public void DeletePoint(DraggablePoint point)
		{
			point.Parent.Controls.Remove(point);

			var index = DraggablePoints.IndexOf(point);
			DraggablePoints.RemoveAt(index);

			//delete curve
			if (DraggablePoints.Count == 1)
			{
				DeleteCurve();
				return;
			}
			OnPointAddedRemoved(Method.Removed, point, index);
			OnCurveShouldBeRepainted();
		}

		public void DisableDrag()
		{
			foreach (var draggablePoint in DraggablePoints)
			{
				draggablePoint.Draggable(false);
				draggablePoint.Cursor = Cursors.Default;
			}
		}

		public void EnableDrag()
		{
			foreach (var draggablePoint in DraggablePoints)
			{
				draggablePoint.Draggable(true);
				draggablePoint.Cursor = Cursors.SizeAll;
			}
		}

		public void DisableDelete()
		{
			foreach (var draggablePoint in DraggablePoints)
			{
				draggablePoint.Removable = false;
				draggablePoint.Cursor = Cursors.Default;
			}
		}

		public void EnableDelete()
		{
			foreach (var draggablePoint in DraggablePoints)
			{
				draggablePoint.Removable = true;
				draggablePoint.Cursor = Cursors.Cross;
			}
		}

		private void DraggablePoint_LocationChanged(object sender, EventArgs e)
		{
			var senderControl = (Control)sender;
			var location = senderControl.Location;

			var newLocation = new Point(Math.Max(location.X, 0), Math.Max(location.Y, 0));
			senderControl.Location = newLocation;

			OnCurveShouldBeRepainted();
		}

		#region BezierPointsApproximation

		public PointF[] GetBezierApproximation(int outputSegmentCount = 128)
		{
			var controlPoints = this.ControlPoints;

			var points = new PointF[outputSegmentCount + 1];
			for (var i = 0; i <= outputSegmentCount; i++)
			{
				var t = (double)i / outputSegmentCount;
				points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Count);
			}
			return points;
		}

		private PointF GetBezierPoint(double t, IReadOnlyList<Point> controlPoints, int index, int count)
		{
			if (count == 1)
				return controlPoints[index];
			var p0 = GetBezierPoint(t, controlPoints, index, count - 1);
			var p1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
			return new PointF(Convert.ToSingle((1 - t) * p0.X + t * p1.X), Convert.ToSingle((1 - t) * p0.Y + t * p1.Y));
		}

		#endregion
	}

	public class PointAddedRemovedEventArgs
	{
		public PointAddedRemovedEventArgs(Method method, DraggablePoint point, int index)
		{
			Method = method;
			Point = point;
			Index = index;
		}

		public Method Method { get; private set; }
		public DraggablePoint Point { get; private set; }
		public int Index { get; private set; }
	}

	public enum Method
	{
		Added,
		Removed
	}

	internal static class BezierCurveExtensions
	{
		public static void DrawCurve(this BezierCurve curve,
			Graphics graphics,
			Pen mainPen,
			bool drawHelper = false, 
			Pen helperPen = null)

		{
			var curvePoints = curve.ControlPoints;
			var points = curve.GetBezierApproximation(32);

			if (drawHelper)
			{
				if (helperPen == null) throw new InvalidOperationException();
				graphics.DrawLines(helperPen, curvePoints.ToArray());
			}

			graphics.DrawLines(mainPen, points);
		}
	}
}
