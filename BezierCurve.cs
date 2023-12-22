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
		public event EventHandler<EventArgs> CurveChanged;
		public event EventHandler<EventArgs> CurveRemoved;

		public void OnCurveChanged()
		{
			CurveChanged?.Invoke(this, EventArgs.Empty);
		}

		public void OnCurveRemoved()
		{
			CurveRemoved?.Invoke(this, EventArgs.Empty);
		}

		private bool _selected = false;

		public bool Selected
		{
			get => _selected;
			set
			{
				if (value == _selected) return;
				_selected = value;
				OnCurveChanged();
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
				draggablePoint.DisableDeletion();
				draggablePoint.Cursor = Cursors.Default;
			}
		}

		public void EnableDelete()
		{
			foreach (var draggablePoint in DraggablePoints)
			{
				draggablePoint.EnableDeletion((control) =>
				{
					var point = (DraggablePoint)control;
					DeletePoint(point);
				});
				draggablePoint.Cursor = Cursors.Cross;
			}
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
			point.RemovePoint();
			DraggablePoints.Remove(point);

			//delete curve
			if (DraggablePoints.Count == 1)
			{
				DeleteCurve();
				return;
			}
			OnCurveChanged();
		}

		private void DraggablePoint_LocationChanged(object sender, EventArgs e)
		{
			var senderControl = (Control)sender;
			var location = senderControl.Location;

			var newLocation = new Point(Math.Max(location.X, 0), Math.Max(location.Y, 0));
			senderControl.Location = newLocation;

			OnCurveChanged();
		}

		public PointF[] GetBezierApproximation(int outputSegmentCount)
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
			var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
			var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
			return new PointF(Convert.ToSingle((1 - t) * P0.X + t * P1.X), Convert.ToSingle((1 - t) * P0.Y + t * P1.Y));
		}
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
