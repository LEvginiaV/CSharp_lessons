using System;

namespace DistanceTask
{
	public static class DistanceTask
	{
		// Расстояние от точки (x, y) до отрезка AB с координатами A(ax, ay), B(bx, by)
		public static double GetDistanceToSegment(double ax, double ay, double bx, double by, double x, double y)
		{
            //вырожденная линия - расстояние от точки до точки по т-ме Пифагора
            if (bx - ax == 0 && by - ay == 0) return DistansePointToPoint(ax, ay, x, y);
            //линия вида  x=const
            else if (bx - ax == 0)
            {
                if (y > Math.Min(ay, by) && y < Math.Max(ay, by)) return Math.Abs(x - ax);
                else return DistansePointToPoint(ax, Math.Min(ay, by), x, y);
            }
            //линия вида  y=const
            else if (by - ay == 0 ) return Math.Abs(y - ay);
            //линия вида  x=kx+b
            else
            {
                double differenceBxAx = bx - ax;
                double differenceByAy = by - ay;
                double xCoorditateIntersected = 
                    (ax*Math.Pow(differenceByAy, 2) - bx*Math.Pow(differenceBxAx, 2) - (ay-y)*differenceBxAx*differenceByAy)/
                    Math.Pow(differenceByAy, 2) + Math.Pow(differenceBxAx, 2);

                if (xCoorditateIntersected > Math.Min(ax, bx) && xCoorditateIntersected < Math.Max(ax, bx))
                {
                    double yCoordinateIntersected = x*differenceByAy/differenceBxAx + ay - ax*differenceByAy/differenceBxAx;
                    return DistansePointToPoint(xCoorditateIntersected, yCoordinateIntersected, x, y);
                }
                else if (xCoorditateIntersected >= Math.Max(ax, bx))
                    return Math.Min(DistansePointToPoint(ax, ay, x, y), DistansePointToPoint(bx, by, x, y));
  
                /*double aKoeficient = 1/(bx - ax);
                double bKoeficient = -1/(by - ay);
                double cKoeficient = ay/(by - ay) - ax/(bx - ax);
                double distance = Math.Abs(aKoeficient*x + bKoeficient*y + cKoeficient)/
                    Math.Sqrt(Math.Pow(aKoeficient, 2)+Math.Pow(bKoeficient, 2));
			    return distance;*/
            }
		}

        public static double DistansePointToPoint (double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
	}
}