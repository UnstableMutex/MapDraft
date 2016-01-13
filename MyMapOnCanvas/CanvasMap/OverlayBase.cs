using System.Windows;

namespace CanvasMap
{
    internal abstract class OverlayBase : IZoomItems
    {
        protected readonly MapMiddle _map;

        public OverlayBase(MapMiddle map)
        {
            _map = map;
        }

        public abstract void Click(Point mouse);

        public abstract void OnViewPortChange(Rect oldvp, Rect newvp, byte currentZoom, byte newZoom, Point mouse);
    }
}