using System.Windows;

namespace JFKCommonLibrary.ExtensionMethods
{
    public static class PointExtensions
    {
        public static Point Subtract(this Point a, Point b)
        {
            Point ret = new Point();
            ret.X = a.X - b.X;
            ret.Y = a.Y - b.Y;
            return ret;
        }

        public static bool AreVirtuallyEqual(this Point p1, Point p2)
        {
            return p1.X.AreVirtuallyEqual(p2.X) && p1.Y.AreVirtuallyEqual(p2.Y);
        }
    }
}
