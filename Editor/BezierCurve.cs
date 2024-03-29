﻿using BezierCurveEditor.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Common;

namespace BezierCurveEditor
{
	public class BezierCurve
	{
		/// <summary>
		/// Occurs when curve should be repainted
		/// </summary>
		public event EventHandler<EventArgs> CurveShouldBeRepainted;
		
		public void OnCurveShouldBeRepainted()
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
		public Canvas Canvas => _parentControl;

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
				var halfH = draggablePoint.Size.Height / 2;
				var halfW = draggablePoint.Size.Width / 2;
				draggablePoint.Location = new Point(point.X - halfW, point.Y - halfH);
				DraggablePoints.Add(draggablePoint);
			}
			
			_parentControl.Controls.AddRange(DraggablePoints.Cast<Control>().ToArray());
		}


		public void InsertPoint(Point point, int index)
		{
			var draggablePoint = new DraggablePoint(this);
			draggablePoint.LocationChanged += DraggablePoint_LocationChanged;
			var halfH = draggablePoint.Size.Height / 2;
			var halfW = draggablePoint.Size.Width / 2;
			draggablePoint.Location = new Point(point.X - halfW, point.Y - halfH);
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
				draggablePoint.Draggable = false;
			}
		}

		public void EnableDrag()
		{
			foreach (var draggablePoint in DraggablePoints)
			{
				draggablePoint.Draggable = true;
			}
		}

		public void DisableDelete()
		{
			foreach (var draggablePoint in DraggablePoints)
			{
				draggablePoint.Removable = false;
			}
		}

		public void EnableDelete()
		{
			foreach (var draggablePoint in DraggablePoints)
			{
				draggablePoint.Removable = true;
			}
		}

		private void DraggablePoint_LocationChanged(object sender, EventArgs e)
		{
			OnCurveShouldBeRepainted();
		}

		public void WriteSvgPath(StreamWriter outputStream)
		{
			var nfi = new NumberFormatInfo
			{
				NumberDecimalSeparator = "."
			};

			var points = Bezier.GetBezierApproximation(ControlPoints);

			outputStream.Write($"<path fill-opacity=\"0\" stroke=\"black\" stroke-width=\"1\" d=\"M {points[0].X.ToString(nfi)},{points[0].Y.ToString(nfi)}");

			for (var i = 1; i < points.Length; i++)
			{
				outputStream.Write($" L {points[i].X.ToString(nfi)},{points[i].Y.ToString(nfi)}");
			}

			outputStream.WriteLine("\"/>");
			outputStream.Flush();
		}
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
			Pen? helperPen = null)
		{
			var curvePoints = curve.ControlPoints;
			var points = Bezier.GetBezierApproximation(curvePoints, 32);

			if (drawHelper)
			{
				if (helperPen == null) throw new InvalidOperationException();
				graphics.DrawLines(helperPen, curvePoints.ToArray());
			}

			graphics.DrawLines(mainPen, points);
		}
	}
}
