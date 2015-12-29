using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RectanglesZoom
{
    class GridSize
    {
        public GridSize(FrameworkElement control)
        {
            HCount = (int) Math.Ceiling(control.ActualWidth/Constants.TileSize);
            VCount = (int) Math.Ceiling(control.ActualHeight/Constants.TileSize);
        }

        public int HCount { get; private set; }
        public int VCount { get; private set; }
    }

    class ClipRectIndex
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;

    }
}