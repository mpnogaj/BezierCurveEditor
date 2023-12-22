using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BezierCurveEditor
{
	public static class RemovableControl
	{
		private static readonly Dictionary<Control, Action<Control>> RemovableControls =
				   new Dictionary<Control, Action<Control>>();
		

		public static void EnableDeletion(this Control control, Action<Control> callback)
		{
			RemovableControls[control] = callback;
			control.MouseUp += control_MouseUp;
		}

		public static void DisableDeletion(this Control control)
		{
			if (!RemovableControls.ContainsKey(control)) return;
			RemovableControls.Remove(control);
			control.MouseUp -= control_MouseUp;
		}

		static void control_MouseUp(object sender, MouseEventArgs e)
		{
			var senderControl = (Control)sender;

			//double check
			if (!RemovableControls.ContainsKey(senderControl)) return;
			
			var parent = senderControl.Parent;
			parent.Controls.Remove(senderControl);

			RemovableControls[senderControl](senderControl);
		}
	}
}
