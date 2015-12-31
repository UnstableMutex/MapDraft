using System.Windows;

namespace RectangesZoom3
{
    abstract class OverlayBase:IZoomItems
    {
        protected readonly Map _map;
        public OverlayBase(Map map)
        {
            _map = map;
        }
       
        public abstract void Click(Point mouse);
        public abstract void OnViewPortChange(Rect oldvp, Rect newvp, byte currentZoom, byte newZoom, Point mouse);
    }
}