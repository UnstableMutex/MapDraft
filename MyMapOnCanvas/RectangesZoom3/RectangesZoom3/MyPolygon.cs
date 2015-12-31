using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;

namespace RectangesZoom3
{
    class MyPolygon
    {
        private readonly sbyte _mark;

        public MyPolygon(sbyte mark)
        {
            _mark = mark;
            Figures = new LinkedList<FrameworkElement>();
        }

        public Thumb CreateThumb(Point position, Canvas canvas)
        {

            Thumb t = new Thumb();
            t.DragDelta += ThumbDragDelta;
            Canvas.SetLeft(t, position.X - size / 2);
            Canvas.SetTop(t, position.Y - size / 2);

            var res = canvas.Resources[typeof(Thumb)] as Style;
            t.Style = res;
            return t;
        }
        private void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {

            var t = sender as Thumb;
                DragThumb(t, new Point(e.HorizontalChange, e.VerticalChange));
            


        }

        public Thumb AddThumb(Point position, Canvas canvas)
        {
            var t = CreateThumb(position, canvas);

            if (Figures.Any())
            {
                var line = CreateLine(Figures.Last.Value as Thumb, t);
                canvas.Children.Add(line);
                Figures.AddLast(line);
            }
            Figures.AddLast(t);
            return t;


        }

        public void MakePolygon(Canvas canvas)
        {
            var lastPoint = Figures.First.Value as Thumb;
            var firstPoint = Figures.Last.Value as Thumb;

            var lastline = CreateLine(firstPoint, lastPoint);
            Figures.AddLast(lastline);
            canvas.Children.Add(lastline);
            // polygons.Add(polygon);


            Polygon = GetPolyFromLL(Figures, canvas);

            canvas.Children.Add(Polygon);

            // return Figures;
        }
        Polygon GetPolyFromLL(LinkedList<FrameworkElement> polygon, Canvas canvas)
        {
            Polygon p = new Polygon();
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

        const int size = 20;
        public void AddThumbOnCenter(Line line)
        {

            var node = Figures.Find(line);
            var canvas = line.Parent as Canvas;
            // var polygon = node.List;
            //тут где то глюк при добавлении к последнему ребру
            var lastThumb = node.CircledNext().Value as Thumb;
            var thumbList = node.List.OfType<Thumb>().ToList();
            var thumbIndex = thumbList.IndexOf(lastThumb);
            var centerX = (line.X1 + line.X2) / 2;
            var centerY = (line.Y1 + line.Y2) / 2;
            var endX2 = line.X2;
            var endY2 = line.Y2;
            line.X2 = centerX;
            line.Y2 = centerY;
            var newPoint = new Point(centerX, centerY);
            Thumb t = CreateThumb(newPoint, canvas);
            canvas.Children.Add(t);
            var midtnode = Figures.AddAfter(node, t);

            var newLine = CreateLine(t, lastThumb);
            Figures.AddAfter(midtnode, newLine);
            canvas.Children.Add(newLine);
            if (thumbIndex == 0)
            {
                Polygon.Points.Add(newPoint);
            }
            else
            {
                Polygon.Points.Insert(thumbIndex, newPoint);
            }


        }



        private Line CreateLine(Thumb first, Thumb last)
        {
            Line t = new Line();
            var canvas = first.Parent as Canvas;
            t.SetFirstPointAsElement(first);
            t.SetLastPointAsElement(last);
            var res = canvas.Resources[typeof(Line)] as Style;
            t.Style = res;
            return t;


        }
        private void UpdatePolygon(Point old, Point n)
        {

            var pol = this.Polygon;
            var foundPoint = pol.Points.Single(p => Math.Abs(p.X - old.X) < double.Epsilon & Math.Abs(p.Y - old.Y) < double.Epsilon);
            var ind = pol.Points.IndexOf(foundPoint);
            pol.Points.RemoveAt(ind);
            pol.Points.Insert(ind, n);
        }
        public LinkedList<FrameworkElement> Figures { get; private set; }
        public Polygon Polygon { get; set; }
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
                Line start = (Line)tmp.Value;
                start.SetLastPointAsElement(thumb);

            }
            if ((tmp = node.CircledNext()) != null)
            {
                Line start = (Line)tmp.Value;
                start.SetFirstPointAsElement(thumb);
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

        private void RealDeleteThumb(LinkedListNode<FrameworkElement> polygon, LinkedList<FrameworkElement> ll, Thumb thumb)
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

        static void MoveOnCanvas(Thumb thumb, Point offset)
        {
            var cl = overNan(Canvas.GetLeft(thumb));
            var ct = overNan(Canvas.GetTop(thumb));
            Canvas.SetLeft(thumb, cl + offset.X);
            Canvas.SetTop(thumb, ct + offset.Y);
        }
        static double overNan(double d)
        {
            if (double.IsNaN(d))
            { d = 0; }
            return d;
        }

        private LinkedListNode<FrameworkElement> FindPolygon(Thumb thumb)
        {
            return Figures.Find(thumb);
        }

        public void Zoom(double zoomFactor)
        {
            foreach (var fe in Figures)
            {

                if (fe is Line)
                {

                }
                if (fe is Thumb)
                {

                }
            }
        }
    }
}