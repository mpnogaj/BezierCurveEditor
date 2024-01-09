using System.Collections.Generic;

namespace Common
{
	public class DrawingModel
	{
		public int CanvasHeight { get; set; }
		public int CanvasWidth { get; set; }
		public List<List<PointModel>> Curves { get; set; }
	}
}
