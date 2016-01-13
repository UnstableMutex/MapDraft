using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace CanvasMap
{
    internal class MyPolygonCollection
    {
        private readonly MapMiddle _map;

        private MyPolygon active;
        private List<MyPolygon> polygons = new List<MyPolygon>();

        public MyPolygonCollection(MapMiddle map)
        {
            _map = map;
        }

        public IEnumerable<LinkedList<Point>> GetPolygonPoints()
        {
            List<LinkedList<Point>> l = new List<LinkedList<Point>>();
            foreach (var poly in polygons)
            {
                var gp = new LinkedList<Point>();
                var points = poly.Figures.OfType<Thumb>();
                foreach (var point in points)
                {
                    var pos = point.GetCenter();
                    gp.AddLast(pos);
                }
                l.Add(gp);
            }
            return l;
        }

        public void Click(Point mouse)
        {
            if (active != null)
            {
                var thumb = active.AddThumb(mouse, _map);
                _map.Children.Add(thumb);
            }
        }

        public void Start(sbyte mark)
        {
            active = new MyPolygon(mark);
        }

        public void End()
        {
            if (active != null)
            {
                active.MakePolygon(_map);
                polygons.Add(active);
                active = null;
            }
        }

        public void Zoom(double scale, Point mouse)
        {
            foreach (var p in polygons)
            {
                p.Zoom(scale, mouse);
            }
            if (active != null)
            {
                active.Zoom(scale, mouse);
            }
        }

        public void Move(Vector v)
        {
            foreach (var p in polygons)
            {
                p.Move(v);
            }
            if (active != null)
            {
                active.Move(v);
            }
        }

        public void Upload(List<LinkedList<Point>> pointPolygons)
        {
            foreach (var poly in pointPolygons)
            {
                MyPolygon p = new MyPolygon(1);
                foreach (var point in poly)
                {
                    var t = p.AddThumb(point, _map);
                    _map.Children.Add(t);
                }
                p.MakePolygon(_map);
                polygons.Add(p);
            }
        }
    }
}