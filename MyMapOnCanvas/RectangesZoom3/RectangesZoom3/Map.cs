using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RectangesZoom3
{
    class Map:MapBase
    {
        protected override void OnZoom(Point mouse, int currentZoom, int newZoom)
        {
            throw new NotImplementedException();
        }

        protected override void OnDragMap(Vector v)
        {
            throw new NotImplementedException();
        }
    }

    class ZoomItems
    {
        private readonly int _zoom;

        public ZoomItems(int zoom)
        {
            _zoom = zoom;
        }

    }
}
