using System;
using System.Windows.Forms;

namespace BezierCurveEditor
{
	public class EditorMode
	{
		public string StatusBarText { get; private set; }
		public Keys ModeKey { get; private set; }

		public Action ModeActivated { get; private set; }
		public Action ModeDeactivated { get; private set; }

		public EditorMode(Keys key, string text, Action modeActivated, Action modeDeactivated)
		{
			ModeKey = key;
			StatusBarText = text;
			ModeActivated = modeActivated;
			ModeDeactivated = modeDeactivated;
		}
	}
}
