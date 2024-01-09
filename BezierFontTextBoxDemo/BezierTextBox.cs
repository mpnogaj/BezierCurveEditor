#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Common;
using Newtonsoft.Json;

namespace BezierFontTextBoxDemo
{
	public partial class BezierTextBox : UserControl
	{
		private readonly FontModel _emptyChar = new FontModel
		{
			Curves = new List<List<PointModel>>()
		};

		private readonly List<List<FontModel>> _controlPoints = new();
		private FontPackModel? _fontPack = null;

		private string _text;

		public override string Text
		{
			get => _text;
			set
			{
				if (value == _text) return;
				UpdateText(value);
				_text = value;
			}
		}

		private string? _fontPath;

		public string? FontPath
		{
			get => _fontPath;
			set
			{
				if (value == _fontPath) return;
				_fontPath = value;
				LoadFont();
			}
		}

		public BezierTextBox()
		{
			InitializeComponent();

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.UserPaint, true);

			this.Height = CharacterData.CharacterHeight;
			this.Width = _text?.Length * CharacterData.CharacterWidth ?? 0;
			_text ??= string.Empty;
		}

		private void UpdateText(string newText)
		{
			var newTextSplit = newText.Split(new []{Environment.NewLine}, StringSplitOptions.None);

			for (var i = 0; i < newTextSplit.Length; i++)
			{
				UpdateLine(i, newTextSplit[i]);
			}

			_controlPoints.RemoveRange(newTextSplit.Length, _controlPoints.Count - newTextSplit.Length);

			UpdateSizes();
			this.Invalidate();
		}

		private void UpdateLine(int row, string line)
		{
			Debug.Assert(row <= _controlPoints.Count);

			if (row == _controlPoints.Count)
			{
				_controlPoints.Add(new List<FontModel>());
			}

			for (var column = 0; column < line.Length; column++)
			{
				SetLetter(line[column], row, column);
			}

			_controlPoints[row].RemoveRange(line.Length, _controlPoints[row].Count - line.Length);
		}

		private void LoadFont()
		{
			if (FontPath != null)
			{
				if (Path.GetExtension(FontPath) != FileExtension.FontPackExtension)
				{
					throw new InvalidOperationException("Invalid file extension");
				}

				using var sr = new StreamReader(FontPath);
				var json = sr.ReadToEnd();
				_fontPack = JsonConvert.DeserializeObject<FontPackModel>(json);
			}
			else
			{
				_fontPack = null;
			}

			if (_fontPack == null)
			{
				_controlPoints.Clear();
			}
			else
			{
				//force text update when font is changed
				UpdateText(Text);
			}

			UpdateSizes();
			this.Invalidate();
		}

		private void SetLetter(char letter, int row, int column)
		{
			Debug.Assert(column <= _controlPoints[row].Count);

			if (column == _controlPoints[row].Count)
			{
				_controlPoints[row].Add(_emptyChar);
			}

			if (_fontPack == null)
			{
				_controlPoints[row][column] = _emptyChar;
				return;
			}

			var charMap = _fontPack.CharMap;
			var fallback = _fontPack.FallbackCharacter!;

			var hasKey = charMap.TryGetValue(letter, out var fontModel);
			_controlPoints[row][column] = hasKey ? fontModel! : fallback;
		}

		private void UpdateSizes()
		{
			this.Width = _controlPoints.Max(x => x.Count) * CharacterData.CharacterWidth;
			this.Height = _controlPoints.Count * CharacterData.CharacterHeight;
		}

		private void BezierTextBox_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

			var pen = new Pen(Color.Black, 2.0f);

			for (var i = 0; i < _controlPoints.Count; i++)
			{
				for (var j = 0; j < _controlPoints[i].Count; j++)
				{
					var character = _controlPoints[i][j];

					foreach (var curve in character.Curves)
					{
						var points = curve.Select(x =>
						{
							var point = new Point(x.X, x.Y);
							point.Offset(j * CharacterData.CharacterWidth, i * CharacterData.CharacterHeight);
							return point;
						}).ToList();

						var bezierPoints = Bezier.GetBezierApproximation(points, 32);
						e.Graphics.DrawLines(pen, bezierPoints);
					}
				}
			}

		}
	}
}
