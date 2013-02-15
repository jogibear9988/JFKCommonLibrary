using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace JFKCommonLibrary.ExtensionMethods
{
    public static class DoubleExtensions
    {
        public static bool AreVirtuallyEqual(this double d1, double d2)
        {
            if (double.IsPositiveInfinity(d1))
                return double.IsPositiveInfinity(d2);

            if (double.IsNegativeInfinity(d1))
                return double.IsNegativeInfinity(d2);

            if (double.IsNaN(d1))
                return double.IsNaN(d2);

            double n = d1 - d2;
            double d = (Math.Abs(d1) + Math.Abs(d2) + 10) * 1.0e-15;
            return (-d < n) && (d > n);
        }

        public static bool AreVirtuallyEqual(this Size s1, Size s2)
        {
            return (AreVirtuallyEqual(s1.Width, s2.Width)
                && AreVirtuallyEqual(s1.Height, s2.Height));
        }

        public static bool AreVirtuallyEqual(this Rect r1, Rect r2)
        {
            return r1.TopLeft.AreVirtuallyEqual(r2.TopLeft) && r1.BottomRight.AreVirtuallyEqual(r2.BottomRight);
        }

        public static bool AreVirtuallyEqual(this Vector v1, Vector v2)
        {
            return (AreVirtuallyEqual(v1.X, v2.X)
                && AreVirtuallyEqual(v1.Y, v2.Y));
        }

        public static bool GreaterThanOrVirtuallyEqual(this double d1, double d2)
        {
            return (d1 > d2 || AreVirtuallyEqual(d1, d2));
        }

        public static bool LessThanOrVirtuallyEqual(this double d1, double d2)
        {
            return (d1 < d2 || AreVirtuallyEqual(d1, d2));
        }

        public static bool StrictlyLessThan(this double d1, double d2)
        {
            return (d1 < d2 && !AreVirtuallyEqual(d1, d2));
        }

        public static bool StrictlyGreaterThan(this double d1, double d2)
        {
            return (d1 > d2 && !AreVirtuallyEqual(d1, d2));
        }

    }
}
