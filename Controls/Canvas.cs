using System;
using System.Collections.Generic;
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

		#region Events

		#region StatusChangedEvent

		public event EventHandler<EventArgs> StatusChanged;

		private void OnStatusChanged()
		{
			StatusChanged?.Invoke(this, EventArgs.Empty);
		}

		#endregion

		#region StatusChangedEvent

		public event EventHandler<EventArgs> ErrorChanged;

		private void OnErrorChanged()
		{
			ErrorChanged?.Invoke(this, EventArgs.Empty);
		}

		#endregion

		#region ModeChangedEvent

		public event EventHandler<ModeChangedEventArgs> ModeChanged;

		private void OnModeChanged()
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

		#endregion

		/// <summary>
		/// Occurs when new point is added to list, or removed
		/// </summary>
		public event EventHandler<PointAddedRemovedEventArgs> PointAddedRemoved;

		private void OnPointAddedRemoved(Method method, DraggablePoint point, int index)
		{
			PointAddedRemoved?.Invoke(this, new PointAddedRemovedEventArgs(method, point, index));
		}

		#region CurvesUpdateEvents

		/// <summary>
		/// Occurs when curve is added to the list
		/// </summary>
		public event EventHandler<CurvesUpdatedEventArgs> CurveAdded;

		private void OnCurveAdded(BezierCurve curve, int index)
		{
			CurveAdded?.Invoke(this, new CurvesUpdatedEventArgs(curve, index));
		}

		/// <summary>
		/// Occurs when curve is removed from the list
		/// </summary>
		public event EventHandler<CurvesUpdatedEventArgs> CurveRemoved;

		private void OnCurveRemoved(BezierCurve curve, int index)
		{
			CurveRemoved?.Invoke(this, new CurvesUpdatedEventArgs(curve, index));
		}

		/// <summary>
		/// Occurs when curve list is cleared
		/// </summary>
		public event EventHandler<EventArgs> CurvesCleared;
		
		private void OnCurvesCleared()
		{
			CurvesCleared?.Invoke(this, EventArgs.Empty);
		}

		#endregion

		public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

		private void OnSelectedItemChanged(object item)
		{
			var type = item?.GetType();
			SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(type, item));
		}

		#endregion

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

		private string _error = string.Empty;

		public string Error
		{
			get => _error;
			private set
			{
				if (value == Error) return;
				_error = value;
				OnErrorChanged();
			}
		}

		private object _selectedItem = null;

		public object SelectedItem
		{
			get => _selectedItem;
			set
			{
				if(value == _selectedItem) return;

				var previousValue = _selectedItem;

				switch (previousValue)
				{
					case null:
						switch (value)
						{
							case BezierCurve curve:
								curve.Selected = true;
								break;
							case DraggablePoint point:
								point.PointSelected = true;
								point.Curve.Selected = true;
								break;
						}

						break;
					case BezierCurve previousCurve:
						previousCurve.Selected = false;
						switch (value)
						{
							case BezierCurve curve:
								curve.Selected = true;
								break;
							case DraggablePoint point:
								point.PointSelected = true;
								point.Curve.Selected = true;
								break;
						}
						break;
					case DraggablePoint previousPoint:
						previousPoint.PointSelected = false;
						previousPoint.Curve.Selected = false;
						switch (value)
						{
							case BezierCurve curve:
								curve.Selected = true;
								break;
							case DraggablePoint point:
								point.PointSelected = true;
								point.Curve.Selected = true;
								break;
						}
						break;
				}

				_selectedItem = value;

				OnSelectedItemChanged(_selectedItem);
			}
		}

		private readonly Dictionary<ModeType, EditorMode> _modes;
		private EditorMode _currentMode;

		private readonly List<Point> _addCurveBuffer = new List<Point>();
		public List<BezierCurve> Curves { get; } = new List<BezierCurve>();


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

		#region Ctor

		public Canvas()
		{
			InitializeComponent();

			_modes = new Dictionary<ModeType, EditorMode>
			{
				{ ModeType.Normal, new EditorMode(Keys.Escape, "Normal mode", () =>
				{
					SelectedItem = null;
				}, () => { }) },
				{
					ModeType.Insert, new EditorMode(Keys.I, "Insert mode", () =>
					{
						SelectedItem = null;
					}, () =>
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
				},
				{
					ModeType.Append, new EditorMode(Keys.A, "Append mode", () => { }, () => { })
				},
				{
					ModeType.Prepend, new EditorMode(Keys.P, "Prepend mode", () => {}, () => {})
				}
			};

			CurrentMode = _modes[(int)ModeType.Normal];

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.UserPaint, true);
		}

		#endregion

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
			if (e.Button == MouseButtons.Left)
			{
				if (_currentMode == _modes[ModeType.Insert])
				{
					_addCurveBuffer.Add(e.Location);
					if (ModifierKeys == Keys.Shift && _addCurveBuffer.Count > 1)
					{
						CreateCurve(_addCurveBuffer);
						_addCurveBuffer.Clear();
					}

					this.Invalidate();
				}
				else if (_currentMode == _modes[ModeType.Append] || _currentMode == _modes[ModeType.Prepend])
				{
					var selectedCurve = Curves.FirstOrDefault(x => x.Selected);
					if (selectedCurve != null)
					{
						Error = string.Empty;
						var newPointIndex = selectedCurve.DraggablePoints.TakeWhile(x => !x.PointSelected).Count();
						if (_currentMode == _modes[ModeType.Append])
						{
							//one of the points is selected
							if (newPointIndex != selectedCurve.DraggablePoints.Count)
								newPointIndex++;
						}
						else
						{
							//we don't have selected point
							if (newPointIndex == selectedCurve.DraggablePoints.Count)
								newPointIndex = 0;
						}

						selectedCurve.InsertPoint(e.Location, newPointIndex);
					}
					else
					{
						Error = "You must select curve or point to use this mode";
					}
				}
			}
		}

		private void CreateCurve(List<Point> points)
		{
			var curve = new BezierCurve(points, this);
			Curves.Add(curve);
			curve.CurveShouldBeRepainted += CurveCurveShouldBeRepainted;
			curve.CurveRemoved += Curve_CurveRemoved;
			curve.PointAddedRemoved += CurveOnPointAddedRemoved;

			OnCurveAdded(curve, Curves.Count - 1);

			UnsavedChanges = true;
		}


		#region CurveEventHandlers 

		private void CurveCurveShouldBeRepainted(object sender, EventArgs e)
		{
			UnsavedChanges = true;
			this.Invalidate();
		}

		private void Curve_CurveRemoved(object sender, EventArgs e)
		{
			var curve = (BezierCurve)sender;

			var index = this.Curves.IndexOf(curve);
			if (index < 0)
			{
				throw new InvalidOperationException("Curve wasn't in the list!");
			}

			this.Curves.RemoveAt(index);
			curve.CurveRemoved -= Curve_CurveRemoved;
			curve.CurveShouldBeRepainted -= CurveCurveShouldBeRepainted;
			curve.PointAddedRemoved -= CurveOnPointAddedRemoved;
			
			OnCurveRemoved(curve, index);

			UnsavedChanges = true;
			this.Invalidate();
		}

		private void CurveOnPointAddedRemoved(object sender, PointAddedRemovedEventArgs e)
		{
			if (e.Method == Method.Removed)
			{
				SelectedItem = e.Point.Curve;
			}

			OnPointAddedRemoved(e.Method, e.Point, e.Index);
			UnsavedChanges = true;
		}

		#endregion

		public bool HandleKeyPressed(Keys key)
		{
			var mode = _modes.Values.FirstOrDefault(x => x.ModeKey == key);
			if (mode == null) return false;
			CurrentMode = mode;
			return true;
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
			while (Curves.Count > 0)
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

	public class SelectedItemChangedEventArgs
	{
		public SelectedItemChangedEventArgs(Type objectType, object o)
		{
			ObjectType = objectType;
			Object = o;
		}

		public Type ObjectType { get; private set; }
		public object Object { get; private set; }
	}

	public class PointsHierarchyChangedEventArgs
	{
		public PointsHierarchyChangedEventArgs(BezierCurve curve)
		{
			Curve = curve;
		}

		public BezierCurve Curve { get; private set; }
	}

	public class CurvesUpdatedEventArgs
	{
		public CurvesUpdatedEventArgs(BezierCurve curve, int index)
		{
			Curve = curve;
			Index = index;
		}

		public BezierCurve Curve { get; private set; }
		public int Index { get; private set; }
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
		Append = 4,
		Prepend = 5,
		Unknown
	}
}