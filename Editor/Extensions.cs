using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BezierCurveEditor
{
	public static class PointExtensions
	{
		public static double Distance(this Point p1, Point p2)
		{
			var a = Math.Pow(p1.X - p2.X, 2);
			var b = Math.Pow(p1.Y - p2.Y, 2);

			return Math.Sqrt(a + b);
		}
	}
}
