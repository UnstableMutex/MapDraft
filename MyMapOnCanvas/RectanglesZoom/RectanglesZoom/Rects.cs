using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RectanglesZoom
{
    class Rects
    {
        private readonly int _zoom;
        public ClipRectIndex IndexRect;
        public Rect Position;

        public Rects(int zoom)
        {
            _zoom = zoom;
        }

        public int Zoom
        {
            get { return _zoom; }
        }

        public double TileSize;
    }
}
