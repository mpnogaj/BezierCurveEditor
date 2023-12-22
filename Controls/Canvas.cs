using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace BezierCurveEditor.Controls
{
	public partial class Canvas : UserControl
	{
		public bool UnsavedChanges { get; private set; }

		public event EventHandler<EventArgs> StatusChanged;

		public void OnStatusChanged()
		{
			StatusChanged?.Invoke(this, EventArgs.Empty);
		}

		public event EventHandler<PointsHierarchyChangedEventArgs> PointsHierarchyChanged;

		public void OnPointsHierarchyChanged(BezierCurve curve)
		{
			PointsHierarchyChanged?.Invoke(this, new PointsHierarchyChangedEventArgs(curve));
		}

		public event EventHandler<ModeChangedEventArgs> ModeChanged;

		public void OnModeChanged()
		{
			var modeType = ModeType.Unknown;
			try
			{
				modeType = _modes.First(x => x.Value == CurrentMode).Key;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
			ModeChanged?.Invoke(this, new ModeChangedEventArgs(modeType));
		}

		private string _status = string.Empty;

		public string Status
		{
			get => _status;
			private set
			{
				if (value == Status) return;
				_status = value;
				OnStatusChanged();
			}
		}

		private readonly Dictionary<ModeType, EditorMode> _modes;
		private EditorMode _currentMode;

		public EditorMode CurrentMode
		{
			get => _currentMode;
			private set
			{
				if (value == _currentMode) return;
				var previousMode = _currentMode;
				_currentMode = value;

				previousMode?.ModeDeactivated();
				_currentMode.ModeActivated();

				Status = _currentMode.StatusBarText;
				OnModeChanged();
			}
		}

		private readonly List<Point> _addCurveBuffer = new List<Point>();

		public ObservableCollection<BezierCurve> Curves { get; } = new ObservableCollection<BezierCurve>();

		public Canvas()
		{
			InitializeComponent();

			_modes = new Dictionary<ModeType, EditorMode>
			{
				{ ModeType.Normal, new EditorMode(Keys.Escape, "Normal mode", () => { }, () => { }) },
				{
					ModeType.Insert, new EditorMode(Keys.I, "Insert mode", () => { }, () =>
					{
						_addCurveBuffer.Clear();
						this.Invalidate();
					})
				},
				{
					ModeType.Delete, new EditorMode(Keys.D, "Delete mode", () =>
					{
						foreach (var curve in Curves)
						{
							curve.EnableDelete();
						}
					}, () =>
					{
						foreach (var curve in Curves)
						{
							curve.DisableDelete();
						}
					})
				},
				{
					ModeType.Move, new EditorMode(Keys.M, "Move mode", () =>
					{
						foreach (var curve in Curves)
						{
							curve.EnableDrag();
						}
					}, () =>
					{
						foreach (var curve in Curves)
						{
							curve.DisableDrag();
						}
					})
				}
			};

			CurrentMode = _modes[(int)ModeType.Normal];

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.UserPaint, true);
		}

		private void Canvas_Paint(object sender, PaintEventArgs e)
		{
			var pen = new Pen(Color.Black, 2);
			var helperPen = new Pen(Color.FromArgb(184, 94, 42), 1);
			var graphics = e.Graphics;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			foreach (var bezierCurve in Curves)
			{
				bezierCurve.DrawCurve(graphics, pen, bezierCurve.Selected, helperPen);
			}

			foreach (var point in _addCurveBuffer)
			{
				const int circleRadius = 4;
				e.Graphics.FillEllipse(new SolidBrush(Color.Red), point.X - circleRadius, point.Y - circleRadius,
					2 * circleRadius, 2 * circleRadius);
			}
		}

		private void Canvas_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && _currentMode == _modes[ModeType.Insert])
			{
				_addCurveBuffer.Add(e.Location);
				if (ModifierKeys == Keys.Shift && _addCurveBuffer.Count > 1)
				{
					CreateCurve(_addCurveBuffer);
					_addCurveBuffer.Clear();
				}

				this.Invalidate();
			}
		}

		private void CreateCurve(List<Point> points)
		{
			var curve = new BezierCurve(points, this);
			Curves.Add(curve);
			curve.CurveChanged += Curve_CurveChanged;
			curve.CurveRemoved += Curve_CurveRemoved;
			curve.DraggablePoints.CollectionChanged += (o, args) => { OnPointsHierarchyChanged(curve); };
			
			UnsavedChanges = true;
		}

		private void Curve_CurveChanged(object sender, EventArgs e)
		{
			UnsavedChanges = true;
			this.Invalidate();
		}

		private void Curve_CurveRemoved(object sender, EventArgs e)
		{
			var curve = (BezierCurve)sender;
			curve.CurveRemoved -= Curve_CurveRemoved;
			curve.CurveChanged -= Curve_CurveChanged;
			this.Curves.Remove(curve);

			UnsavedChanges = true;
			this.Invalidate();
		}


		public void HandleKeyPressed(Keys key)
		{
			var mode = _modes.Values.FirstOrDefault(x => x.ModeKey == key);
			if (mode != null) CurrentMode = mode;
		}

		public void ChangeMode(ModeType mode)
		{
			if (!_modes.ContainsKey(mode)) return;
			CurrentMode = _modes[mode];
		}

		public DataModel SaveCurves()
		{
			var curves = Curves.Select(x => x.DraggablePoints.Select(y => new PointModel
			{
				X = y.Location.X,
				Y = y.Location.Y
			}).ToList()).ToList();

			this.UnsavedChanges = false;

			return new DataModel()
			{
				CanvasHeight = this.Height,
				CanvasWidth = this.Width,
				Curves = curves
			};
		}

		public void LoadCurves(DataModel model)
		{
			Clear();

			this.Height = model.CanvasHeight;
			this.Width = model.CanvasWidth;

			foreach (var modelCurve in model.Curves)
			{
				var points = modelCurve.Select(x => new Point(x.X, x.Y)).ToList();
				CreateCurve(points);
			}

			//we just loaded those curves from file
			//clear flag
			UnsavedChanges = false;

			this.Invalidate();
		}

		public void Clear(bool shouldMarkUnsavedChanges = true)
		{
			while(Curves.Count > 0)
			{
				Curves[0].DeleteCurve();
			}

			//sometimes we want to clear all curves set UnsavedChanges flag
			if (!shouldMarkUnsavedChanges)
			{
				UnsavedChanges = false;
			}

			this.Invalidate();
		}
	}

	public class PointsHierarchyChangedEventArgs
	{
		public PointsHierarchyChangedEventArgs(BezierCurve curve)
		{
			Curve = curve;
		}

		public BezierCurve Curve { get; private set; }
	}

	public class ModeChangedEventArgs
	{
		public ModeChangedEventArgs(ModeType mode)
		{
			Mode = mode;
		}

		public ModeType Mode { get; }
	}

	public enum ModeType
	{
		Normal = 0,
		Insert = 1,
		Delete = 2,
		Move = 3,
		Unknown
	}
}