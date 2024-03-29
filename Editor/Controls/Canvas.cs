﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using BezierCurveEditor.Properties;
using Common;

namespace BezierCurveEditor.Controls
{
	public partial class Canvas : UserControl
	{
		//public const int CharacterXOffset = 200;
		//public const int CharacterYOffset = 100;

		public bool UnsavedChanges { get; private set; }


		private bool _drawBorderPen = false;

		public bool DrawFontBorder
		{
			get => _drawBorderPen;
			set
			{
				if (value == _drawBorderPen) return;
				_drawBorderPen = value;
				//force canvas to redraw
				this.Invalidate();
			}
		}

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

		private object? _selectedItem = null;

		public object? SelectedItem
		{
			get => _selectedItem;
			set
			{
				if(value == _selectedItem) return;

				var previousValue = _selectedItem;

				switch (previousValue)
				{
					case BezierCurve previousCurve:
						previousCurve.Selected = false;
						break;
					case DraggablePoint previousPoint:
						previousPoint.PointSelected = false;
						break;
				}

				switch (value)
				{
					case BezierCurve curve:
						curve.Selected = true;
						break;
					case DraggablePoint point:
						point.PointSelected = true;
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
				{ 
					ModeType.Normal, new EditorMode(Keys.Escape, "Normal mode", () =>
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
			if (DrawFontBorder)
			{
				var borderPen = new Pen(new SolidBrush(Color.Green), 2.0f);
				e.Graphics.DrawRectangle(borderPen, new Rectangle(Settings.Default.CharBoxPosX, Settings.Default.CharBoxPosY, CharacterData.CharacterWidth, CharacterData.CharacterHeight));
			}


			var pen = new Pen(Color.Black, 2);
			var helperPen = new Pen(Color.FromArgb(184, 94, 42), 1);
			var graphics = e.Graphics;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			foreach (var bezierCurve in Curves)
			{
				var drawHelper = bezierCurve.Selected || bezierCurve.DraggablePoints.Any(x => x.PointSelected);

				bezierCurve.DrawCurve(graphics, pen, drawHelper, helperPen);
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
					if (_addCurveBuffer.Count >= 2)
					{
						if (ModifierKeys == Keys.Control)
						{
							CreateCurve(_addCurveBuffer);
							_addCurveBuffer.Clear();
							_addCurveBuffer.Add(e.Location);
						}
						else if (ModifierKeys == Keys.Shift)
						{
							CreateCurve(_addCurveBuffer);
							_addCurveBuffer.Clear();
						}
					}

					this.Invalidate();
				}
				else if (_currentMode == _modes[ModeType.Append] || _currentMode == _modes[ModeType.Prepend])
				{
					if (SelectedItem != null)
					{
						Error = string.Empty;
						BezierCurve curve;
						DraggablePoint? point;

						switch (SelectedItem)
						{
							case BezierCurve selectedCurve:
								curve = selectedCurve;
								point = null;
								break;
							case DraggablePoint selectedPoint:
								curve = selectedPoint.Curve;
								point = selectedPoint;
								break;
							default:
								return;
						}

						var newPointIndex = point == null ? -1 : curve.DraggablePoints.IndexOf(point);
						if (newPointIndex == -1)
						{
							if (CurrentMode == _modes[ModeType.Append])
							{
								newPointIndex = curve.DraggablePoints.Count;
							}
							else if (CurrentMode == _modes[ModeType.Prepend])
							{
								newPointIndex = 0;
							}
						}
						else if (CurrentMode == _modes[ModeType.Append])
						{
							newPointIndex++;
						}

						curve.InsertPoint(e.Location, newPointIndex);
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
			if (mode != null)
			{
				CurrentMode = mode;
				return true;
			}

			if (key == Keys.Enter && CurrentMode == _modes[ModeType.Insert] && _addCurveBuffer.Count >= 2)
			{
				CreateCurve(_addCurveBuffer);
				_addCurveBuffer.Clear();
				this.Invalidate();
			}

			return false;
		}

		public void ChangeMode(ModeType mode)
		{
			if (!_modes.ContainsKey(mode)) return;
			CurrentMode = _modes[mode];
		}

		public DrawingModel SaveCurves()
		{
			var curves = Curves.Select(x => x.ControlPoints.Select(p => new PointModel
			{
				X = p.X,
				Y = p.Y
			}).ToList()).ToList();

			this.UnsavedChanges = false;

			return new DrawingModel()
			{
				CanvasHeight = this.Height,
				CanvasWidth = this.Width,
				Curves = curves
			};
		}

		public void LoadCurves(DrawingModel model)
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

			CurrentMode.ModeActivated();

			this.Invalidate();
		}

		public void Clear(bool shouldMarkUnsavedChanges = true)
		{
			CurrentMode.ModeDeactivated();

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

		private void Canvas_DoubleClick(object sender, EventArgs e)
		{
			if (CurrentMode == _modes[ModeType.Move] && SelectedItem is DraggablePoint point)
			{
				var args = (e as MouseEventArgs)!;
				
				point.Location = new Point(args.X, args.Y);
			}
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