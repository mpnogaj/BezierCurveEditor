using System.Collections.Generic;

namespace BezierCurveEditor
{
	public class DataModel
	{
		public int CanvasHeight { get; set; }
		public int CanvasWidth { get; set; }
		public List<List<PointModel>> Curves { get; set; }
	}

	public class PointModel
	{
		public int X { get; set; }
		public int Y { get; set; }
	}
}
