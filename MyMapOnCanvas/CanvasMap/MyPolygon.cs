using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CanvasMap
{
    internal class MyPolygon
    {
        private const int size = 20;

        private readonly sbyte _mark;

        internal MyPolygon(sbyte mark)
        {
            _mark = mark;
            Figures = new LinkedList<FrameworkElement>();
        }

        internal LinkedList<FrameworkElement> Figures { get; private set; }
        internal Polygon Polygon { get; set; }

        internal void Move(Vector v)
        {
            foreach (var fe in Figures)
            {
                if (fe is Thumb)
                {
                    var t = fe as Thumb;
                    DragThumb(t, v);
                }
            }
        }

        private Thumb CreateThumb(Point position, Canvas canvas)
        {
            Thumb t = new Thumb();
            //styling    
            t.Width = t.Height = 20;
            Canvas.SetZIndex(t,10);
        
            var ct = new ControlTemplate();
            var ellf = new FrameworkElementFactory(typeof(Ellipse));

            ct.VisualTree = ellf;

            ellf.SetValue(Ellipse.FillProperty, Brushes.Blue);
            ellf.SetValue(Ellipse.StrokeProperty, Brushes.Black);

            ellf.SetValue(Ellipse.StrokeThicknessProperty, 2d);
            ellf.SetValue(Ellipse.WidthProperty, 10d);
            ellf.SetValue(Ellipse.HeightProperty, 10d);

            t.Template = ct;



            t.DragDelta += ThumbDragDelta;
            Canvas.SetLeft(t, position.X - size / 2);
            Canvas.SetTop(t, position.Y - size / 2);
           // var res = canvas.Resources[typeof(Thumb)] as Style;
           // t.Style = res;
            return t;
        }

        private void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var t = sender as Thumb;
            DragThumb(t, new Point(e.HorizontalChange, e.VerticalChange));
        }

        internal Thumb AddThumb(Point position, Canvas canvas)
        {
            var t = CreateThumb(position, canvas);
            if (Figures.Any())
            {
                var line = CreateLine(Figures.Last.Value as Thumb, t, canvas);
                Figures.AddLast(line);
            }
            Figures.AddLast(t);
            return t;
        }

        internal void MakePolygon(Canvas canvas)
        {
            var lastPoint = Figures.First.Value as Thumb;
            var firstPoint = Figures.Last.Value as Thumb;
            var lastline = CreateLine(firstPoint, lastPoint, canvas);
            Figures.AddLast(lastline);
            Polygon = GetPolyFromLL(Figures, canvas);
            canvas.Children.Add(Polygon);
            Panel.SetZIndex(Polygon, 1);
        }

        private Polygon GetPolyFromLL(LinkedList<FrameworkElement> polygon, Canvas canvas)
        {
            Polygon p = CreatePolygon();
            var ths = polygon.ToList().OfType<Thumb>();
            var rname = _mark > 0 ? "pPlus" : "pMinus";
            var res = canvas.Resources[rname] as Style;
            p.Style = res;
            foreach (var thumb in ths)
            {
                p.Points.Add(thumb.GetCenter());
            }
            return p;
        }

        private Polygon CreatePolygon()
        {
            Polygon p = new Polygon();
            Canvas.SetZIndex(p,0);
            p.Fill = Brushes.Blue;
            p.Opacity = .4;
            p.Fill = _mark > 0 ? Brushes.Blue : Constants.NegativeBrush;
            return p;
        }

        internal void AddThumbOnCenter(Line line)
        {
            var node = Figures.Find(line);
            var canvas = line.Parent as Canvas;
            var lastThumb = node.CircledNext().Value as Thumb;
            var thumbList = node.List.OfType<Thumb>().ToList();
            var thumbIndex = thumbList.IndexOf(lastThumb);
            var centerX = (line.X1 + line.X2) / 2;
            var centerY = (line.Y1 + line.Y2) / 2;
            line.X2 = centerX;
            line.Y2 = centerY;
            var newPoint = new Point(centerX, centerY);
            Thumb t = CreateThumb(newPoint, canvas);
            canvas.Children.Add(t);
            var midtnode = Figures.AddAfter(node, t);
            var newLine = CreateLine(t, lastThumb, canvas);
            Figures.AddAfter(midtnode, newLine);
            if (thumbIndex == 0)
            {
                Polygon.Points.Add(newPoint);
            }
            else
            {
                Polygon.Points.Insert(thumbIndex, newPoint);
            }
        }

        private Line CreateLine(Thumb first, Thumb last, Canvas canvas)
        {

            Line t = new Line();
            //styling
            t.Stroke = Brushes.Red;
            t.StrokeThickness = 2;

            t.MouseEnter += t_MouseEnter;
            t.MouseLeave += t_MouseLeave;
            t.MouseRightButtonUp += Line_Click;
            t.SetFirstPointAsElement(first);
            t.SetLastPointAsElement(last);
            canvas.Children.Add(t);
            Canvas.SetZIndex(t, 3);

            var res = canvas.Resources[typeof(Line)] as Style;
            t.Style = res;

            return t;
        }

        private void t_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var l = (Line)sender;
            l.Stroke = Brushes.Red;
        }

        private void t_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var l = (Line)sender;
            l.Stroke = Brushes.Green;
        }

        private void Line_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var l = (Line)sender;
            AddThumbOnCenter(l);
        }

        private void UpdatePolygon(Point old, Point n)
        {
            if (Polygon != null)
            {
                var pol = Polygon;
                Func<Point, bool> filter = p => (p.X == old.X) & p.Y == old.Y;

                var debuginfo = pol.Points.Where(filter);
                var foundPoint = pol.Points.Single(filter);

                var ind = pol.Points.IndexOf(foundPoint);
                pol.Points.RemoveAt(ind);
                pol.Points.Insert(ind, n);
            }
        }

        private void MoveLines(Thumb thumb)
        {
            var node = FindPolygon(thumb);
            if (node == null)
            {
                return;
            }
            LinkedListNode<FrameworkElement> tmp;
            if ((tmp = node.CircledPrevious()) != null)
            {
                var start = tmp.Value as Line;
                if (start != null)
                {
                    start.SetLastPointAsElement(thumb);
                }
            }
            if ((tmp = node.CircledNext()) != null)
            {
                Line end = tmp.Value as Line;
                if (end != null)
                {
                    end.SetFirstPointAsElement(thumb);
                }
            }
        }

        public void DeleteThumb(Thumb thumb)
        {
            var polygon = FindPolygon(thumb);
            if (polygon == null)
            {
                return;
            }
            var ll = polygon.List;
            if (ll.Count % 2 != 0)
            {
                //удаляем только из замкнутых полигонов т.к. незамкнутый можно еще редактировать
                return;
            }
            if (ll.Count < 7)
            {
                //не получица полигон после удаления поэтому не удаляем
                return;
            }

            //все проверки пройдены, удаление:
            RealDeleteThumb(polygon, ll, thumb);
            //удаляем с полигона
            var center = thumb.GetCenter();
            Polygon.Points.Remove(center);
        }

        private void RealDeleteThumb(LinkedListNode<FrameworkElement> polygon, LinkedList<FrameworkElement> ll,
            Thumb thumb)
        {
            var canvas = thumb.Parent as Canvas;
            //надо удалить точку, линию и поставить конец линии в другое место,
            //потому удалить элементы канваса
            var startline = polygon.CircledPrevious();
            var endline = polygon.CircledNext();
            var newEndNode = endline.CircledNext();
            //удаляем последнюю линию
            ll.Remove(endline);
            canvas.Children.Remove(endline.Value);
            //удаляем вершину
            canvas.Children.Remove(thumb);
            ll.Remove(polygon);

            //далее соединяем startLine и newendnode
            var sl = startline.Value as Line;
            sl.SetLastPointAsElement(newEndNode.Value as Thumb);
        }

        public void DragThumb(Thumb thumb, Point offset)
        {
            var old = thumb.GetCenter();
            MoveOnCanvas(thumb, offset);
            MoveLines(thumb);
            var n = thumb.GetCenter();
            UpdatePolygon(old, n);
        }

        private void DragThumb(Thumb thumb, Vector offset)
        {
            DragThumb(thumb, new Point(offset.X, offset.Y));
        }

        private static void MoveOnCanvas(Thumb thumb, Point offset)
        {
            var cl = overNan(Canvas.GetLeft(thumb));
            var ct = overNan(Canvas.GetTop(thumb));
            Canvas.SetLeft(thumb, cl + offset.X);
            Canvas.SetTop(thumb, ct + offset.Y);
        }

        private static double overNan(double d)
        {
            if (double.IsNaN(d))
            {
                d = 0;
            }
            return d;
        }

        private LinkedListNode<FrameworkElement> FindPolygon(Thumb thumb)
        {
            return Figures.Find(thumb);
        }

        public void Zoom(double zoomFactor, Point mouse)
        {
            var pos = mouse;
            foreach (var fe in Figures)
            {
                if (fe is Line)
                {
                }
                if (fe is Thumb)
                {
                    var t = fe as Thumb;
                    var center = t.GetCenter();
                    var x1 = pos.X - zoomFactor * (pos.X - center.X);
                    var y1 = pos.Y - zoomFactor * (pos.Y - center.Y);
                    Vector v = new Point(x1, y1) - center;
                    DragThumb(t, v);
                }
            }
        }
    }
}