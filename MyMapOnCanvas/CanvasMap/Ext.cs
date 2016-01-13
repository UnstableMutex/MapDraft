using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;

namespace CanvasMap
{
    internal static class Ext
    {
        public static Point GetCenter(this Thumb t)
        {
            var x = GetCenterX(t);
            var y = GetCenterY(t);
            return new Point(x, y);
        }

        public static LinkedListNode<T> CircledPrevious<T>(this LinkedListNode<T> l)
        {
            return l.Previous ?? l.List.Last;
        }

        public static LinkedListNode<T> CircledNext<T>(this LinkedListNode<T> l)
        {
            return l.Next ?? l.List.First;
        }

        public static void SetFirstPointAsElement(this Line l, Control c)
        {
            var x1=GetCenterX(c);
            l.X1 = x1;
            l.Y1 = GetCenterY(c);
        }

        private static double GetCenterX(Control c)
        {
            return OverNan(Canvas.GetLeft(c)) + c.Width / 2;
        }

        private static double GetCenterY(Control c)
        {
            return OverNan(Canvas.GetTop(c)) + c.Height / 2;
        }

        public static void SetLastPointAsElement(this Line l, Control c)
        {
            l.X2 = GetCenterX(c);
            l.Y2 = GetCenterY(c);
        }

        private static double OverNan(double d)
        {
            if (double.IsNaN(d))
            {
                d = 0;
            }
            return d;
        }
    }
}