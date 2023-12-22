using System.Windows.Forms;

namespace BezierCurveEditor
{
	//https://nickstips.wordpress.com/2010/03/03/c-panel-resets-scroll-position-after-focus-is-lost-and-regained/
	public class CustomPanel : Panel
	{
		protected override System.Drawing.Point ScrollToControl(System.Windows.Forms.Control activeControl)
		{
			// Returning the current location prevents the panel from
			// scrolling to the active control when the panel loses and regains focus
			return this.DisplayRectangle.Location;
		}
	}
}
