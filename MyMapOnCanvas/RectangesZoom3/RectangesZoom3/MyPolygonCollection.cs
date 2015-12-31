using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RectangesZoom3
{
    class MyPolygonCollection
    {
        private readonly Map _map;
        List<MyPolygon> polygons = new List<MyPolygon>();

        public MyPolygonCollection(Map map)
        {
            _map = map;
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
        MyPolygon active;
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
    }
}
