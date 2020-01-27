using System;

namespace Rectangles
{
	public static class RectanglesTask
	{
		// Пересекаются ли два прямоугольника (пересечение только по границе также считается пересечением)
		/*public static bool AreIntersectedOldVersion (Rectangle r1, Rectangle r2)
		{
            bool areXIntersect = false;
            bool areYIntersect = false;
			if (r1.Left <= r2.Left)
                areXIntersect = AreIntersectionByOneOrt (r1.Left, r1.Width, r2.Left);
            else areXIntersect = AreIntersectionByOneOrt (r2.Left, r2.Width, r1.Left);
            if (r1.Top <= r2.Top)
                areYIntersect = AreIntersectionByOneOrt (r1.Top, r1.Height, r2.Top);
            else areYIntersect = AreIntersectionByOneOrt (r2.Top, r2.Height, r1.Top);
            return areXIntersect && areYIntersect;
		}

        public static bool AreIntersectionByOneOrt (int left1, int width, int left2)
        {
            if (left2 <= left1 + width) return true;
            else return false;
        }*/

        public static bool AreIntersected(Rectangle r1, Rectangle r2)
		{
			// так можно обратиться к координатам левого верхнего угла первого прямоугольника: r1.Left, r1.Top
            int width = DifferenceBetweenMinUpAndMaxDownCoordinates (r1.Left, r1.Width, r2.Left, r2.Width);
            int height = DifferenceBetweenMinUpAndMaxDownCoordinates (r1.Top, r1.Height, r2.Top, r2.Height);
			if (width < 0 || height < 0)
                return false;
            else return true;
		}

        public static int DifferenceBetweenMinUpAndMaxDownCoordinates (int left1, int width1, int left2, int width2)
        {
            int minRightBorder = Math.Min(left1 + width1, left2 + width2);
            int maxLeftBorder = Math.Max(left1, left2);
            return minRightBorder-maxLeftBorder;
        }

		// Площадь пересечения прямоугольников
		public static int IntersectionSquare(Rectangle r1, Rectangle r2)
		{
            if (AreIntersected(r1, r2))
            {
                int width = DifferenceBetweenMinUpAndMaxDownCoordinates (r1.Left, r1.Width, r2.Left, r2.Width);
                int height = DifferenceBetweenMinUpAndMaxDownCoordinates (r1.Top, r1.Height, r2.Top, r2.Height);
                return width*height;
            }
			else return 0;
		}

		// Если один из прямоугольников целиком находится внутри другого — вернуть номер (с нуля) внутреннего.
		// Иначе вернуть -1
		// Если прямоугольники совпадают, можно вернуть номер любого из них.
		public static int IndexOfInnerRectangle(Rectangle r1, Rectangle r2)
		{
            int width = DifferenceBetweenMinUpAndMaxDownCoordinates (r1.Left, r1.Width, r2.Left, r2.Width);
            int height = DifferenceBetweenMinUpAndMaxDownCoordinates (r1.Top, r1.Height, r2.Top, r2.Height);
            if (width == r1.Width && height == r1.Height) return 0;
            else if (width == r2.Width && height == r2.Height) return 1;
			else return -1;
		}
	}
}