using System;
using System.Collections.Generic;
using System.Drawing;

namespace Common
{
	public static class Bezier
	{
		public static PointF[] GetBezierApproximation(IReadOnlyList<Point> controlPoints, int outputSegmentCount = 512)
		{
			var points = new PointF[outputSegmentCount + 1];
			for (var i = 0; i <= outputSegmentCount; i++)
			{
				var t = (double)i / outputSegmentCount;
				points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Count);
			}
			return points;
		}

		private static PointF GetBezierPoint(double t, IReadOnlyList<Point> controlPoints, int index, int count)
		{
			if (count == 1)
				return controlPoints[index];
			var p0 = GetBezierPoint(t, controlPoints, index, count - 1);
			var p1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
			return new PointF(Convert.ToSingle((1 - t) * p0.X + t * p1.X), Convert.ToSingle((1 - t) * p0.Y + t * p1.Y));
		}
	}
}
